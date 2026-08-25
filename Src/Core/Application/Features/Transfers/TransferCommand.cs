using Application.Features.Transactions.Common;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Transfers
{
    public class TransferCommand : IRequest<ApiResponse<TransactionDto>>
    {
        public string RecipientEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? IdempotencyKey { get; set; }
    }
}
