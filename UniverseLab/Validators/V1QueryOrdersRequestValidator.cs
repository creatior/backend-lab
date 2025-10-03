using FluentValidation;
using Models.Dto.V1.Requests;

namespace universe_lab.Validators;

public class V1QueryOrdersRequestValidator: AbstractValidator<V1QueryOrdersRequest>
{
    public V1QueryOrdersRequestValidator()
    {
        RuleForEach(x => x.Ids)
            .GreaterThan(0);
        
        RuleForEach(x => x.CustomerIds)
            .GreaterThan(0);
        
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .When(x => x.Page is not null);
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .When(x => x.PageSize is not null);
    }
}