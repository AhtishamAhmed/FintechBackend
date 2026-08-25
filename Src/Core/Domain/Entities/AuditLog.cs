namespace Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string? AdditionalData { get; set; }
        public string? IpAddress { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}
