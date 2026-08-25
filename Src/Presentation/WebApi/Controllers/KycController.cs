using Application.Features.Kyc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/kyc")]
    public class KycController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KycController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(SubmitKycCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _mediator.Send(new GetMyKycQuery());
            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMine(UpdateMyKycCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
