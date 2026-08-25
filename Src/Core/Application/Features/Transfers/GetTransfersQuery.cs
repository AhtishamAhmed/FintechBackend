using Application.Exceptions;
using Application.Features.Transactions.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Transfers
{
    public class GetTransfersQuery : IRequest<ApiResponse<PagedResult<TransactionDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetTransfersQueryHandler : IRequestHandler<GetTransfersQuery, ApiResponse<PagedResult<TransactionDto>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetTransfersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<PagedResult<TransactionDto>>> Handle(GetTransfersQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("You do not have a wallet yet.");

            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var query = _context.Transactions
                .Where(t => t.Type == TransactionType.Transfer && (t.WalletId == wallet.Id || t.RecipientWalletId == wallet.Id))
                .OrderByDescending(t => t.CreatedAtUtc);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionDto
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
                })
                .ToListAsync(cancellationToken);

            return new ApiResponse<PagedResult<TransactionDto>>(new PagedResult<TransactionDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
