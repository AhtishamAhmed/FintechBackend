using Application.Features.Users.Admin.GetUserById;
using Application.Features.Users.Admin.GetUsers;
using Application.Features.Users.Admin.UpdateUserStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/users")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminUsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { UserId = id });
            return Ok(result);
        }

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetUserProfile(string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { UserId = id });
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(string id, UpdateUserStatusCommand command)
        {
            command.UserId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
