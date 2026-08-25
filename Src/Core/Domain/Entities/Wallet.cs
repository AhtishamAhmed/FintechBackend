using Domain.Enums;

namespace Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string Currency { get; set; } = "PKR";
        public decimal Balance { get; set; }
        public WalletStatus Status { get; set; } = WalletStatus.Active;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; } = null!;
        public ICollection<TransactionEntry> Entries { get; set; } = new List<TransactionEntry>();
    }
}
