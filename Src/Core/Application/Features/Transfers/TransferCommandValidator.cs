using FluentValidation;

namespace Application.Features.Transfers
{
    public class TransferCommandValidator : AbstractValidator<TransferCommand>
    {
        public TransferCommandValidator()
        {
            RuleFor(x => x.RecipientEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
