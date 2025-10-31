using System.Diagnostics;
using System.Text;
using Consumer.Clients;
using Consumer.Config;
using Microsoft.Extensions.Options;
using Models.Dto.V1.Requests;
using Models.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UniverseLab.Common;
using UniverseLab.Messages;

namespace Consumer.Consumers;

public class OmsOrderCreatedConsumer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<RabbitMqSettings> _rabbitMqSettings;
    private readonly ConnectionFactory _factory;
    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;
    
    public OmsOrderCreatedConsumer(IOptions<RabbitMqSettings> rabbitMqSettings, IServiceProvider serviceProvider)
    {
        _rabbitMqSettings = rabbitMqSettings;
        _serviceProvider = serviceProvider;
        _factory = new ConnectionFactory { HostName = rabbitMqSettings.Value.HostName, Port = rabbitMqSettings.Value.Port };
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await _factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(
            queue: _rabbitMqSettings.Value.OrderCreatedQueue, 
            durable: false, 
            exclusive: false,
            autoDelete: false,
            arguments: null, 
            cancellationToken: cancellationToken);
        
        var sw = new Stopwatch();
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);
        _consumer = new AsyncEventingBasicConsumer(_channel);
        
        _consumer.ReceivedAsync += async (sender, args) =>
        {
            sw.Restart();
            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = message.FromJson<OrderCreatedMessage>();

                foreach (var item in order.OrderItems)
                {
                    Console.WriteLine(
                        $"Sending to OMS -> OrderId: {order.Id}, OrderItemId: {item.Id}, CustomerId: {order.CustomerId}, Status: Created");
                }

                using var scope = _serviceProvider.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<OmsClient>();
                await client.LogOrder(new V1CreateAuditLogRequest
                {
                    Orders = order.OrderItems.Select(x =>
                        new V1CreateAuditLogRequest.LogOrder
                        {
                            OrderId = order.Id,
                            OrderItemId = x.Id,
                            CustomerId = order.CustomerId,
                            OrderStatus = nameof(OrderStatus.Created)
                        }).ToArray()
                }, CancellationToken.None);
                await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
                sw.Stop();
                Console.WriteLine($"Order created consumed in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _channel.BasicNackAsync(args.DeliveryTag, false, true, cancellationToken);
            }
        };
        
        await _channel.BasicConsumeAsync(
            queue: _rabbitMqSettings.Value.OrderCreatedQueue, 
            autoAck: true, 
            consumer: _consumer,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        _connection?.Dispose();
        _channel?.Dispose();
    }
}