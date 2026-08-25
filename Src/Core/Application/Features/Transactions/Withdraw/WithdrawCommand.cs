using Application.Features.Transactions.Common;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Transactions.Withdraw
{
    public class WithdrawCommand : IRequest<ApiResponse<TransactionDto>>
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? IdempotencyKey { get; set; }
    }
}
