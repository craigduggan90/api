using FluentValidation;
using Sol.Core.Services.Jobs.Requests;
using Sol.Domain.Enums;

namespace Sol.Core.Services.Jobs.Validators;

public class UpdateJobRequestValidator : AbstractValidator<UpdateJobRequest>
{
    public UpdateJobRequestValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty();
        
        RuleFor(request => request.EventTime)
            .NotEqual(default(DateTime));
        
        RuleFor(request => request.Status)
            .Must(value => Enum.TryParse<JobStatusEnum>(value, true, out _))
            .WithMessage("Must contain a valid job status.");
        
        RuleFor(request => request.ErrorCode)
            .NotEmpty()
            .When(request => string.Equals(request.Status, nameof(JobStatusEnum.Failed), StringComparison.OrdinalIgnoreCase))
            .WithMessage("ErrorCode is required when Status is Failed.");

        RuleFor(request => request.ErrorCode)
            .MaximumLength(100);
        
        RuleFor(request => request.ErrorMessage)
            .MaximumLength(255);
    }
}