namespace universe_lab.DAL.Models;

public class V1AuditLogOrderDal
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long OrderItemId { get; set; }
    public long CustomerId { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}