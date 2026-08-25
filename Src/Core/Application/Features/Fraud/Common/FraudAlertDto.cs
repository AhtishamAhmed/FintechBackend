namespace Application.Features.Fraud.Common
{
    public class FraudAlertDto
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ResolutionNotes { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ReviewedAtUtc { get; set; }
    }
}
