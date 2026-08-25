using Application.Wrappers;
using MediatR;

namespace Application.Features.Auth.CurrentUser
{
    public class CurrentUserQuery : IRequest<ApiResponse<CurrentUserDto>>
    {
    }
}
