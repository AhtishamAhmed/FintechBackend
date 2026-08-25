using Application.Features.Wallets.CreateWallet;
using Application.Features.Wallets.GetMyBalance;
using Application.Features.Wallets.GetMyWallet;
using Application.Features.Wallets.GetMyWalletStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/wallets")]
    public class WalletsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalletsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet(CreateWalletCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyWallet()
        {
            var result = await _mediator.Send(new GetMyWalletQuery());
            return Ok(result);
        }

        [HttpGet("me/balance")]
        public async Task<IActionResult> GetMyBalance()
        {
            var result = await _mediator.Send(new GetMyBalanceQuery());
            return Ok(result);
        }

        [HttpGet("me/status")]
        public async Task<IActionResult> GetMyWalletStatus()
        {
            var result = await _mediator.Send(new GetMyWalletStatusQuery());
            return Ok(result);
        }
    }
}
