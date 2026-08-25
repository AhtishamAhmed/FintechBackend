using Domain.Enums;

namespace Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PKR";
        public string? Description { get; set; }

        public string InitiatedByUserId { get; set; } = string.Empty;

        // For Deposit/Withdrawal this is the single wallet involved.
        // For Transfer this is the sender's wallet.
        public Guid WalletId { get; set; }

        // Only set for Transfer.
        public Guid? RecipientWalletId { get; set; }

        public string? IdempotencyKey { get; set; }
        public string? FailureReason { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAtUtc { get; set; }

        public Wallet Wallet { get; set; } = null!;
        public ICollection<TransactionEntry> Entries { get; set; } = new List<TransactionEntry>();
        public FraudAlert? FraudAlert { get; set; }
    }
}
