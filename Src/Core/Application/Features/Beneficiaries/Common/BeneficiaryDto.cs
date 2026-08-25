namespace Application.Features.Beneficiaries.Common
{
    public class BeneficiaryDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string BeneficiaryEmail { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
