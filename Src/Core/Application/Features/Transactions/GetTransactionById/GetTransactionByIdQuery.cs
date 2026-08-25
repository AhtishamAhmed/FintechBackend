using Application.Exceptions;
using Application.Features.Transactions.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Transactions.GetTransactionById
{
    public class GetTransactionByIdQuery : IRequest<ApiResponse<TransactionDto>>
    {
        public Guid TransactionId { get; set; }
    }

    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, ApiResponse<TransactionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetTransactionByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<TransactionDto>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken)
                ?? throw new ApiException("Transaction not found.");

            var myWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == _currentUserService.UserId, cancellationToken);

            var isOwner = myWallet != null && (transaction.WalletId == myWallet.Id || transaction.RecipientWalletId == myWallet.Id);
            if (!isOwner)
            {
                throw new ApiException("Transaction not found.");
            }

            return new ApiResponse<TransactionDto>(new TransactionDto
            {
                Id = transaction.Id,
                Type = transaction.Type.ToString(),
                Status = transaction.Status.ToString(),
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Description = transaction.Description,
                WalletId = transaction.WalletId,
                RecipientWalletId = transaction.RecipientWalletId,
                FailureReason = transaction.FailureReason,
                CreatedAtUtc = transaction.CreatedAtUtc,
                CompletedAtUtc = transaction.CompletedAtUtc
            });
        }
    }
}
