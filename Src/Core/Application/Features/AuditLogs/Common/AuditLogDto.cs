namespace Application.Features.AuditLogs.Common
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
