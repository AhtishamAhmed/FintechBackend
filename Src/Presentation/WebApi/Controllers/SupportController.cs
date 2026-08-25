using Application.Features.Support;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/support/tickets")]
    public class SupportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSupportTicketCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetSupportTicketsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSupportTicketByIdQuery { TicketId = id });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateSupportTicketCommand command)
        {
            command.TicketId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = "SupportAgent,Admin")]
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> Assign(Guid id)
        {
            var result = await _mediator.Send(new AssignSupportTicketCommand { TicketId = id });
            return Ok(result);
        }

        [Authorize(Roles = "SupportAgent,Admin")]
        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(Guid id)
        {
            var result = await _mediator.Send(new CloseSupportTicketCommand { TicketId = id });
            return Ok(result);
        }
    }
}
