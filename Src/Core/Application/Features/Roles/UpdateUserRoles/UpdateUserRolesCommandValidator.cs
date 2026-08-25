using FluentValidation;

namespace Application.Features.Roles.UpdateUserRoles
{
    public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
    {
        private static readonly string[] ValidRoles = { "Customer", "Admin", "SupportAgent", "ComplianceOfficer" };

        public UpdateUserRolesCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Roles).NotEmpty();
            RuleForEach(x => x.Roles).Must(role => ValidRoles.Contains(role))
                .WithMessage($"Role must be one of: {string.Join(", ", ValidRoles)}.");
        }
    }
}
