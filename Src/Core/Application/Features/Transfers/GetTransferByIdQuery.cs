using Application.Exceptions;
using Application.Features.Transactions.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Transfers
{
    public class GetTransferByIdQuery : IRequest<ApiResponse<TransactionDto>>
    {
        public Guid TransferId { get; set; }
    }

    public class GetTransferByIdQueryHandler : IRequestHandler<GetTransferByIdQuery, ApiResponse<TransactionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetTransferByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<TransactionDto>> Handle(GetTransferByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == request.TransferId && t.Type == TransactionType.Transfer, cancellationToken)
                ?? throw new ApiException("Transfer not found.");

            var myWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == _currentUserService.UserId, cancellationToken);
            var isParticipant = myWallet != null && (transaction.WalletId == myWallet.Id || transaction.RecipientWalletId == myWallet.Id);
            if (!isParticipant)
            {
                throw new ApiException("Transfer not found.");
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
