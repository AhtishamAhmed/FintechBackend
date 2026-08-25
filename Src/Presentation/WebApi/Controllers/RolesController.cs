using Application.Features.Roles.GetRoles;
using Application.Features.Roles.GetUserRoles;
using Application.Features.Roles.UpdateUserRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _mediator.Send(new GetRolesQuery());
            return Ok(result);
        }

        [HttpGet("users/{id}/roles")]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            var result = await _mediator.Send(new GetUserRolesQuery { UserId = id });
            return Ok(result);
        }

        [HttpPut("users/{id}/roles")]
        public async Task<IActionResult> UpdateUserRoles(string id, UpdateUserRolesCommand command)
        {
            command.UserId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
