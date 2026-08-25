using Application.Exceptions;
using Application.Features.Kyc.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kyc
{
    public class SubmitKycCommand : IRequest<ApiResponse<KycDto>>
    {
        public string FullName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
    }

    public class SubmitKycCommandValidator : AbstractValidator<SubmitKycCommand>
    {
        public SubmitKycCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(50);
        }
    }

    public class SubmitKycCommandHandler : IRequestHandler<SubmitKycCommand, ApiResponse<KycDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public SubmitKycCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<KycDto>> Handle(SubmitKycCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!;
            var existing = await _context.KycApplications.FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);

            if (existing != null && existing.Status != KycStatus.Rejected)
            {
                throw new ApiException("KYC already submitted.");
            }

            KycApplication kyc;
            if (existing != null)
            {
                existing.FullName = request.FullName;
                existing.DocumentType = request.DocumentType;
                existing.DocumentNumber = request.DocumentNumber;
                existing.Status = KycStatus.Pending;
                existing.RejectionReason = null;
                existing.SubmittedAtUtc = DateTime.UtcNow;
                existing.ReviewedAtUtc = null;
                existing.ReviewedByUserId = null;
                kyc = existing;
            }
            else
            {
                kyc = new KycApplication
                {
                    UserId = userId,
                    FullName = request.FullName,
                    DocumentType = request.DocumentType,
                    DocumentNumber = request.DocumentNumber
                };
                _context.KycApplications.Add(kyc);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<KycDto>(ToDto(kyc), "KYC submitted successfully.");
        }

        private static KycDto ToDto(KycApplication k) => new()
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
        };
    }
}
