using Application.Exceptions;
using Application.Features.Beneficiaries.Common;
using Application.Interfaces;
using Application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Beneficiaries
{
    public class UpdateBeneficiaryCommand : IRequest<ApiResponse<BeneficiaryDto>>
    {
        public Guid BeneficiaryId { get; set; }
        public string Nickname { get; set; } = string.Empty;
    }

    public class UpdateBeneficiaryCommandValidator : AbstractValidator<UpdateBeneficiaryCommand>
    {
        public UpdateBeneficiaryCommandValidator()
        {
            RuleFor(x => x.Nickname).NotEmpty().MaximumLength(100);
        }
    }

    public class UpdateBeneficiaryCommandHandler : IRequestHandler<UpdateBeneficiaryCommand, ApiResponse<BeneficiaryDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateBeneficiaryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<BeneficiaryDto>> Handle(UpdateBeneficiaryCommand request, CancellationToken cancellationToken)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.Id == request.BeneficiaryId && b.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("Beneficiary not found.");

            beneficiary.Nickname = request.Nickname;
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<BeneficiaryDto>(new BeneficiaryDto
            {
                Id = beneficiary.Id,
                Nickname = beneficiary.Nickname,
                BeneficiaryEmail = beneficiary.BeneficiaryEmail,
                CreatedAtUtc = beneficiary.CreatedAtUtc
            }, "Beneficiary updated successfully.");
        }
    }
}
