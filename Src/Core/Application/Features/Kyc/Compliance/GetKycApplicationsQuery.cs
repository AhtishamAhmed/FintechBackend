using Application.Features.Kyc.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kyc.Compliance
{
    public class GetKycApplicationsQuery : IRequest<ApiResponse<PagedResult<KycDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public KycStatus? Status { get; set; }
    }

    public class GetKycApplicationsQueryHandler : IRequestHandler<GetKycApplicationsQuery, ApiResponse<PagedResult<KycDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetKycApplicationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResult<KycDto>>> Handle(GetKycApplicationsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.KycApplications.AsQueryable();
            if (request.Status.HasValue) query = query.Where(k => k.Status == request.Status);
            query = query.OrderByDescending(k => k.SubmittedAtUtc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(k => new KycDto
                {
                    Id = k.Id,
                    UserId = k.UserId,
                    FullName = k.FullName,
                    DocumentType = k.DocumentType,
                    DocumentNumber = k.DocumentNumber,
                    Status = k.Status.ToString(),
                    RejectionReason = k.RejectionReason,
                    SubmittedAtUtc = k.SubmittedAtUtc,
                    ReviewedAtUtc = k.ReviewedAtUtc
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<PagedResult<KycDto>>(new PagedResult<KycDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
