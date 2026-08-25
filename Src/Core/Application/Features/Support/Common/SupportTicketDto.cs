namespace Application.Features.Support.Common
{
    public class SupportTicketDto
    {
        public Guid Id { get; set; }
        public string CustomerUserId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AssignedAgentUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DateTime? ClosedAtUtc { get; set; }
    }
}
