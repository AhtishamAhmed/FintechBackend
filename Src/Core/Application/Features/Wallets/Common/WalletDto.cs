namespace Application.Features.Wallets.Common
{
    public class WalletDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
