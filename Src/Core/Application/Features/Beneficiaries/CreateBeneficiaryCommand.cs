using Application.Features.Beneficiaries.Common;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Beneficiaries
{
    public class CreateBeneficiaryCommand : IRequest<ApiResponse<BeneficiaryDto>>
    {
        public string Nickname { get; set; } = string.Empty;
        public string BeneficiaryEmail { get; set; } = string.Empty;
    }
}
