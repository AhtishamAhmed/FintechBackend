namespace Application.Features.Transactions.Common
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid WalletId { get; set; }
        public Guid? RecipientWalletId { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
    }
}
