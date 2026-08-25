using Application.Exceptions;
using Application.Features.Kyc.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kyc
{
    public class UpdateMyKycCommand : IRequest<ApiResponse<KycDto>>
    {
        public string FullName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
    }

    public class UpdateMyKycCommandValidator : AbstractValidator<UpdateMyKycCommand>
    {
        public UpdateMyKycCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(50);
        }
    }

    public class UpdateMyKycCommandHandler : IRequestHandler<UpdateMyKycCommand, ApiResponse<KycDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateMyKycCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<KycDto>> Handle(UpdateMyKycCommand request, CancellationToken cancellationToken)
        {
            var kyc = await _context.KycApplications.FirstOrDefaultAsync(k => k.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("You have not submitted KYC yet.");

            if (kyc.Status is KycStatus.UnderReview or KycStatus.Approved)
            {
                throw new ApiException("KYC cannot be edited while under review or already approved.");
            }

            kyc.FullName = request.FullName;
            kyc.DocumentType = request.DocumentType;
            kyc.DocumentNumber = request.DocumentNumber;
            kyc.Status = KycStatus.Pending;
            kyc.RejectionReason = null;

            await _context.SaveChangesAsync(cancellationToken);

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
            }, "KYC updated successfully.");
        }
    }
}
