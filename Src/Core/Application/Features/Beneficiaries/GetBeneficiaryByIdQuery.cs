using Application.Exceptions;
using Application.Features.Beneficiaries.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Beneficiaries
{
    public class GetBeneficiaryByIdQuery : IRequest<ApiResponse<BeneficiaryDto>>
    {
        public Guid BeneficiaryId { get; set; }
    }

    public class GetBeneficiaryByIdQueryHandler : IRequestHandler<GetBeneficiaryByIdQuery, ApiResponse<BeneficiaryDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetBeneficiaryByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<BeneficiaryDto>> Handle(GetBeneficiaryByIdQuery request, CancellationToken cancellationToken)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.Id == request.BeneficiaryId && b.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("Beneficiary not found.");

            return new ApiResponse<BeneficiaryDto>(new BeneficiaryDto
            {
                Id = beneficiary.Id,
                Nickname = beneficiary.Nickname,
                BeneficiaryEmail = beneficiary.BeneficiaryEmail,
                CreatedAtUtc = beneficiary.CreatedAtUtc
            });
        }
    }
}
