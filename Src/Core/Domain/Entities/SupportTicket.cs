using Domain.Enums;

namespace Domain.Entities
{
    public class SupportTicket
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerUserId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;
        public string? AssignedAgentUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAtUtc { get; set; }

        public ApplicationUser Customer { get; set; } = null!;
    }
}
