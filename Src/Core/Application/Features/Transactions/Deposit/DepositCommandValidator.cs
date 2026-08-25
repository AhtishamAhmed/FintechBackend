using FluentValidation;

namespace Application.Features.Transactions.Deposit
{
    public class DepositCommandValidator : AbstractValidator<DepositCommand>
    {
        public DepositCommandValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
