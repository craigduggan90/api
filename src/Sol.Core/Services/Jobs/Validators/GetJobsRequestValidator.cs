using FluentValidation;
using Sol.Common.Pagination;
using Sol.Core.Services.Jobs.Requests;
using Sol.Domain.Enums;

namespace Sol.Core.Services.Jobs.Validators;

public class GetJobsRequestValidator : AbstractValidator<GetJobsRequest>
{
    public GetJobsRequestValidator()
    {
        RuleFor(request => request.Cursor)
            .Must(value => value.TryDecodeCursor(out _))
            .When(request => request.Cursor is not null)
            .WithMessage("Invalid cursor value.");

        RuleFor(request => request.Type)
            .Must(value => Enum.TryParse<JobTypeEnum>(value, true, out _))
            .When(request => request.Type is not null)
            .WithMessage("Must contain a valid job type.");

        RuleFor(request => request.Status)
            .Must(value => Enum.TryParse<JobStatusEnum>(value, true, out _))
            .When(request => request.Status is not null)
            .WithMessage("Must contain a valid job status.");

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .LessThan(100)
            .When(request => request.PageSize is not null);
    }
}