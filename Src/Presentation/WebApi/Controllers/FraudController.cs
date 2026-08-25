using Application.Features.Fraud;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin,ComplianceOfficer")]
    [ApiController]
    [Route("api/fraud/alerts")]
    public class FraudController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FraudController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetFraudAlertsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetFraudAlertByIdQuery { AlertId = id });
            return Ok(result);
        }

        [Authorize(Roles = "ComplianceOfficer")]
        [HttpPost("{id}/review")]
        public async Task<IActionResult> Review(Guid id)
        {
            var result = await _mediator.Send(new ReviewFraudAlertCommand { AlertId = id });
            return Ok(result);
        }

        [Authorize(Roles = "ComplianceOfficer")]
        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> Resolve(Guid id, ResolveFraudAlertCommand command)
        {
            command.AlertId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = "ComplianceOfficer")]
        [HttpPost("{id}/dismiss")]
        public async Task<IActionResult> Dismiss(Guid id, DismissFraudAlertCommand command)
        {
            command.AlertId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
