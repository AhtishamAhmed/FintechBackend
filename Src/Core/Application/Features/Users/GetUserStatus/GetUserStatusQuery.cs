using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.GetUserStatus
{
    public class GetUserStatusQuery : IRequest<ApiResponse<string>>
    {
    }

    public class GetUserStatusQueryHandler : IRequestHandler<GetUserStatusQuery, ApiResponse<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public GetUserStatusQueryHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(GetUserStatusQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId!)
                ?? throw new ApiException("User not found.");

            return new ApiResponse<string>(user.Status.ToString(), null);
        }
    }
}
