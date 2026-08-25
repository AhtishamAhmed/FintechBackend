using Application.Exceptions;
using Application.Features.Transactions.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Transactions.Admin.GetTransactionById
{
    public class GetAdminTransactionByIdQuery : IRequest<ApiResponse<TransactionDto>>
    {
        public Guid TransactionId { get; set; }
    }

    public class GetAdminTransactionByIdQueryHandler : IRequestHandler<GetAdminTransactionByIdQuery, ApiResponse<TransactionDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAdminTransactionByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<TransactionDto>> Handle(GetAdminTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            var t = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == request.TransactionId, cancellationToken)
                ?? throw new ApiException("Transaction not found.");

            return new ApiResponse<TransactionDto>(new TransactionDto
            {
                Id = t.Id,
                Type = t.Type.ToString(),
                Status = t.Status.ToString(),
                Amount = t.Amount,
                Currency = t.Currency,
                Description = t.Description,
                WalletId = t.WalletId,
                RecipientWalletId = t.RecipientWalletId,
                FailureReason = t.FailureReason,
                CreatedAtUtc = t.CreatedAtUtc,
                CompletedAtUtc = t.CompletedAtUtc
            });
        }
    }
}
