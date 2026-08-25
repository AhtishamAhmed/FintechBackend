using Application.Features.Users.Common;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Users.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<ApiResponse<UserProfileDto>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
