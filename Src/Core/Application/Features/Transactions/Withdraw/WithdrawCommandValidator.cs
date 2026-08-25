using FluentValidation;

namespace Application.Features.Transactions.Withdraw
{
    public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
    {
        public WithdrawCommandValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
