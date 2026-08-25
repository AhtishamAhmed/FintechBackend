using FluentValidation;

namespace Application.Features.Wallets.CreateWallet
{
    public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
    {
        public CreateWalletCommandValidator()
        {
            RuleFor(x => x.Currency).NotEmpty().Length(3);
        }
    }
}
