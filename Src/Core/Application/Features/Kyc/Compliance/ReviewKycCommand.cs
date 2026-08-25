using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kyc.Compliance
{
    public class ReviewKycCommand : IRequest<ApiResponse<string>>
    {
        public Guid KycId { get; set; }
    }

    public class ReviewKycCommandHandler : IRequestHandler<ReviewKycCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ReviewKycCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(ReviewKycCommand request, CancellationToken cancellationToken)
        {
            var kyc = await _context.KycApplications.FirstOrDefaultAsync(k => k.Id == request.KycId, cancellationToken)
                ?? throw new ApiException("KYC application not found.");

            kyc.Status = KycStatus.UnderReview;
            kyc.ReviewedByUserId = _currentUserService.UserId;
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>(null!, "KYC application marked as under review.");
        }
    }
}
