using Application.Features.Kyc.Compliance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "ComplianceOfficer")]
    [ApiController]
    [Route("api/compliance/kyc")]
    public class ComplianceKycController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ComplianceKycController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetKycApplicationsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetKycApplicationByIdQuery { KycId = id });
            return Ok(result);
        }

        [HttpPost("{id}/review")]
        public async Task<IActionResult> Review(Guid id)
        {
            var result = await _mediator.Send(new ReviewKycCommand { KycId = id });
            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _mediator.Send(new ApproveKycCommand { KycId = id });
            return Ok(result);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, RejectKycCommand command)
        {
            command.KycId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
