using Application.Exceptions;
using Application.Features.Kyc.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kyc.Compliance
{
    public class GetKycApplicationByIdQuery : IRequest<ApiResponse<KycDto>>
    {
        public Guid KycId { get; set; }
    }

    public class GetKycApplicationByIdQueryHandler : IRequestHandler<GetKycApplicationByIdQuery, ApiResponse<KycDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetKycApplicationByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<KycDto>> Handle(GetKycApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var k = await _context.KycApplications.FirstOrDefaultAsync(x => x.Id == request.KycId, cancellationToken)
                ?? throw new ApiException("KYC application not found.");

            return new ApiResponse<KycDto>(new KycDto
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
            });
        }
    }
}
