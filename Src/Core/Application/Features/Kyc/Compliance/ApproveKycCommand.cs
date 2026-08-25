using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kyc.Compliance
{
    public class ApproveKycCommand : IRequest<ApiResponse<string>>
    {
        public Guid KycId { get; set; }
    }

    public class ApproveKycCommandHandler : IRequestHandler<ApproveKycCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        public ApproveKycCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<string>> Handle(ApproveKycCommand request, CancellationToken cancellationToken)
        {
            var kyc = await _context.KycApplications.FirstOrDefaultAsync(k => k.Id == request.KycId, cancellationToken)
                ?? throw new ApiException("KYC application not found.");

            kyc.Status = KycStatus.Approved;
            kyc.ReviewedByUserId = _currentUserService.UserId;
            kyc.ReviewedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyAsync(kyc.UserId, "KYC approved", "Your KYC application has been approved.", cancellationToken);

            return new ApiResponse<string>(null!, "KYC application approved.");
        }
    }
}
