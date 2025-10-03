using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using universe_lab.Config;
using UniverseLab.Common;

namespace universe_lab.BLL.Services;

public class RabbitMqService(IOptions<RabbitMqSettings> settings)
{
    private readonly ConnectionFactory _factory = new() { HostName = settings.Value.HostName, Port = settings.Value.Port };
    
    public async Task Publish<T>(IEnumerable<T> enumerable, string queue, CancellationToken token)
    {
        await using var connection = await _factory.CreateConnectionAsync(token);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: token);
        await channel.QueueDeclareAsync(
            queue: queue, 
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: token);

        foreach (var message in enumerable)
        {
            var messageStr = message.ToJson();
            var body = Encoding.UTF8.GetBytes(messageStr);
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue,
                body: body,
                cancellationToken: token);
        }
    }
}