using Consumer.Base;
using Consumer.Clients;
using Consumer.Config;
using Microsoft.Extensions.Options;
using Models.Dto.V1.Requests;
using Models.Enums;
using UniverseLab.Messages;

namespace Consumer.Consumers;

public class BatchOmsOrderCreatedConsumer(
    IOptions<RabbitMqSettings> rabbitMqSettings,
    IServiceProvider serviceProvider)
    : BaseBatchMessageConsumer<OrderCreatedMessage>(rabbitMqSettings.Value)
{
    protected override async Task ProcessMessages(OrderCreatedMessage[] messages)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<OmsClient>();

        await client.LogOrder(new V1CreateAuditLogRequest
        {
            Orders = messages.SelectMany(order => order.OrderItems.Select(ol =>
                new V1CreateAuditLogRequest.LogOrder
                {
                    OrderId = order.Id,
                    OrderItemId = ol.Id,
                    CustomerId = order.CustomerId,
                    OrderStatus = nameof(OrderStatus.Created)
                })).ToArray()
        }, CancellationToken.None);
    }
}