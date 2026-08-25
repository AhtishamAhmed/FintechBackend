using Application.Features.Transactions.Admin.GetFailedTransactions;
using Application.Features.Transactions.Admin.GetTransactionById;
using Application.Features.Transactions.Admin.GetTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin,SupportAgent,ComplianceOfficer")]
    [ApiController]
    [Route("api/admin/transactions")]
    public class AdminTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminTransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] GetTransactionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("failed")]
        public async Task<IActionResult> GetFailedTransactions([FromQuery] GetFailedTransactionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(Guid id)
        {
            var result = await _mediator.Send(new GetAdminTransactionByIdQuery { TransactionId = id });
            return Ok(result);
        }
    }
}
