using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Fraud
{
    public class ReviewFraudAlertCommand : IRequest<ApiResponse<string>>
    {
        public Guid AlertId { get; set; }
    }

    public class ReviewFraudAlertCommandHandler : IRequestHandler<ReviewFraudAlertCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;

        public ReviewFraudAlertCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(ReviewFraudAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = await _context.FraudAlerts.FirstOrDefaultAsync(f => f.Id == request.AlertId, cancellationToken)
                ?? throw new ApiException("Fraud alert not found.");

            alert.Status = FraudAlertStatus.UnderReview;
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>(null!, "Fraud alert marked as under review.");
        }
    }
}
