using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kyc.Compliance
{
    public class RejectKycCommand : IRequest<ApiResponse<string>>
    {
        public Guid KycId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class RejectKycCommandValidator : AbstractValidator<RejectKycCommand>
    {
        public RejectKycCommandValidator()
        {
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        }
    }

    public class RejectKycCommandHandler : IRequestHandler<RejectKycCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        public RejectKycCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<string>> Handle(RejectKycCommand request, CancellationToken cancellationToken)
        {
            var kyc = await _context.KycApplications.FirstOrDefaultAsync(k => k.Id == request.KycId, cancellationToken)
                ?? throw new ApiException("KYC application not found.");

            kyc.Status = KycStatus.Rejected;
            kyc.RejectionReason = request.Reason;
            kyc.ReviewedByUserId = _currentUserService.UserId;
            kyc.ReviewedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.NotifyAsync(kyc.UserId, "KYC rejected", $"Your KYC application was rejected: {request.Reason}", cancellationToken);

            return new ApiResponse<string>(null!, "KYC application rejected.");
        }
    }
}
