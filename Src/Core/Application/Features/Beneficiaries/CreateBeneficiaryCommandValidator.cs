using FluentValidation;

namespace Application.Features.Beneficiaries
{
    public class CreateBeneficiaryCommandValidator : AbstractValidator<CreateBeneficiaryCommand>
    {
        public CreateBeneficiaryCommandValidator()
        {
            RuleFor(x => x.Nickname).NotEmpty().MaximumLength(100);
            RuleFor(x => x.BeneficiaryEmail).NotEmpty().EmailAddress();
        }
    }
}
