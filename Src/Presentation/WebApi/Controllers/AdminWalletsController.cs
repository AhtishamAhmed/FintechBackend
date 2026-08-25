using Application.Features.Wallets.Admin.GetWalletById;
using Application.Features.Wallets.Admin.GetWallets;
using Application.Features.Wallets.Admin.UpdateWalletStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin,SupportAgent,ComplianceOfficer")]
    [ApiController]
    [Route("api/admin/wallets")]
    public class AdminWalletsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminWalletsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetWallets([FromQuery] GetWalletsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalletById(Guid id)
        {
            var result = await _mediator.Send(new GetWalletByIdQuery { WalletId = id });
            return Ok(result);
        }

        [Authorize(Roles = "Admin,ComplianceOfficer")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateWalletStatus(Guid id, UpdateWalletStatusCommand command)
        {
            command.WalletId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
