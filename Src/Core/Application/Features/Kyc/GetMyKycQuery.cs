using Application.Exceptions;
using Application.Features.Kyc.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kyc
{
    public class GetMyKycQuery : IRequest<ApiResponse<KycDto>>
    {
    }

    public class GetMyKycQueryHandler : IRequestHandler<GetMyKycQuery, ApiResponse<KycDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetMyKycQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<KycDto>> Handle(GetMyKycQuery request, CancellationToken cancellationToken)
        {
            var kyc = await _context.KycApplications.FirstOrDefaultAsync(k => k.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("You have not submitted KYC yet.");

            return new ApiResponse<KycDto>(new KycDto
            {
                Id = kyc.Id,
                UserId = kyc.UserId,
                FullName = kyc.FullName,
                DocumentType = kyc.DocumentType,
                DocumentNumber = kyc.DocumentNumber,
                Status = kyc.Status.ToString(),
                RejectionReason = kyc.RejectionReason,
                SubmittedAtUtc = kyc.SubmittedAtUtc,
                ReviewedAtUtc = kyc.ReviewedAtUtc
            });
        }
    }
}
