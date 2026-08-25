using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Beneficiaries
{
    public class DeleteBeneficiaryCommand : IRequest<ApiResponse<string>>
    {
        public Guid BeneficiaryId { get; set; }
    }

    public class DeleteBeneficiaryCommandHandler : IRequestHandler<DeleteBeneficiaryCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DeleteBeneficiaryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(DeleteBeneficiaryCommand request, CancellationToken cancellationToken)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.Id == request.BeneficiaryId && b.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("Beneficiary not found.");

            _context.Beneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>(null!, "Beneficiary removed.");
        }
    }
}
