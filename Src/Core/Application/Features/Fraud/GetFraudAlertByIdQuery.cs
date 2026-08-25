using Application.Exceptions;
using Application.Features.Fraud.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Fraud
{
    public class GetFraudAlertByIdQuery : IRequest<ApiResponse<FraudAlertDto>>
    {
        public Guid AlertId { get; set; }
    }

    public class GetFraudAlertByIdQueryHandler : IRequestHandler<GetFraudAlertByIdQuery, ApiResponse<FraudAlertDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetFraudAlertByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<FraudAlertDto>> Handle(GetFraudAlertByIdQuery request, CancellationToken cancellationToken)
        {
            var f = await _context.FraudAlerts.FirstOrDefaultAsync(x => x.Id == request.AlertId, cancellationToken)
                ?? throw new ApiException("Fraud alert not found.");

            return new ApiResponse<FraudAlertDto>(new FraudAlertDto
            {
                Id = f.Id,
                TransactionId = f.TransactionId,
                Reason = f.Reason,
                Status = f.Status.ToString(),
                ResolutionNotes = f.ResolutionNotes,
                CreatedAtUtc = f.CreatedAtUtc,
                ReviewedAtUtc = f.ReviewedAtUtc
            });
        }
    }
}
