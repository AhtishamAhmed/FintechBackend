using Application.Features.Users.ChangePassword;
using Application.Features.Users.GetProfile;
using Application.Features.Users.GetUserStatus;
using Application.Features.Users.UpdateProfile;
using Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var result = await _mediator.Send(new GetProfileQuery());
            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(UpdateProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangeMyPassword(ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("me/status")]
        public async Task<IActionResult> GetMyStatus()
        {
            var result = await _mediator.Send(new GetUserStatusQuery());
            return Ok(result);
        }
    }
}
