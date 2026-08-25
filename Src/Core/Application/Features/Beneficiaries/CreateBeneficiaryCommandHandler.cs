using Application.Exceptions;
using Application.Features.Beneficiaries.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Beneficiaries
{
    public class CreateBeneficiaryCommandHandler : IRequestHandler<CreateBeneficiaryCommand, ApiResponse<BeneficiaryDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateBeneficiaryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<ApiResponse<BeneficiaryDto>> Handle(CreateBeneficiaryCommand request, CancellationToken cancellationToken)
        {
            var beneficiaryUser = await _userManager.FindByEmailAsync(request.BeneficiaryEmail)
                ?? throw new ApiException("No customer found with that email.");

            var beneficiary = new Beneficiary
            {
                UserId = _currentUserService.UserId!,
                Nickname = request.Nickname,
                BeneficiaryEmail = beneficiaryUser.Email!
            };

            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<BeneficiaryDto>(new BeneficiaryDto
            {
                Id = beneficiary.Id,
                Nickname = beneficiary.Nickname,
                BeneficiaryEmail = beneficiary.BeneficiaryEmail,
                CreatedAtUtc = beneficiary.CreatedAtUtc
            }, "Beneficiary added successfully.");
        }
    }
}
