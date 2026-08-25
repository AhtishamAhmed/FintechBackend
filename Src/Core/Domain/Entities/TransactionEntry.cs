using Domain.Enums;

namespace Domain.Entities
{
    public class TransactionEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TransactionId { get; set; }
        public Guid WalletId { get; set; }
        public EntryDirection Direction { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Transaction Transaction { get; set; } = null!;
        public Wallet Wallet { get; set; } = null!;
    }
}
