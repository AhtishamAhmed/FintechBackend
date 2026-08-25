using Domain.Enums;

namespace Domain.Entities
{
    public class FraudAlert
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TransactionId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public FraudAlertStatus Status { get; set; } = FraudAlertStatus.Open;
        public string? ReviewedByUserId { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAtUtc { get; set; }

        public Transaction Transaction { get; set; } = null!;
    }
}
