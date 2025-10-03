using FluentValidation;
using Models.Dto.V1.Requests;

namespace universe_lab.Validators;

public class V1CreateAuditLogRequestValidator : AbstractValidator<V1CreateAuditLogRequest>
{
    public V1CreateAuditLogRequestValidator()
    {
        RuleForEach(x => x.Orders).NotNull();
        RuleForEach(x => x.Orders).ChildRules(order =>
        {
            order.RuleFor(o => o.OrderId).GreaterThan(0);
            order.RuleFor(o => o.OrderItemId).GreaterThan(0);
            order.RuleFor(o => o.CustomerId).GreaterThan(0);
            order.RuleFor(o => o.OrderStatus).NotEmpty();
        });
    }
}