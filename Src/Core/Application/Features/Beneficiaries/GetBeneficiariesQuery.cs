using Application.Features.Beneficiaries.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Beneficiaries
{
    public class GetBeneficiariesQuery : IRequest<ApiResponse<List<BeneficiaryDto>>>
    {
    }

    public class GetBeneficiariesQueryHandler : IRequestHandler<GetBeneficiariesQuery, ApiResponse<List<BeneficiaryDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetBeneficiariesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<List<BeneficiaryDto>>> Handle(GetBeneficiariesQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.Beneficiaries
                .Where(b => b.UserId == _currentUserService.UserId)
                .OrderBy(b => b.Nickname)
                .Select(b => new BeneficiaryDto
                {
                    Id = b.Id,
                    Nickname = b.Nickname,
                    BeneficiaryEmail = b.BeneficiaryEmail,
                    CreatedAtUtc = b.CreatedAtUtc
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<List<BeneficiaryDto>>(items);
        }
    }
}
