using Domain.Enums;

namespace Domain.Entities
{
    public class KycApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public KycStatus Status { get; set; } = KycStatus.Pending;
        public string? RejectionReason { get; set; }
        public string? ReviewedByUserId { get; set; }
        public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAtUtc { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
