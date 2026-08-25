using Application.Features.Reports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin,ComplianceOfficer,SupportAgent")]
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] GetTransactionsReportQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,ComplianceOfficer")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersReportQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,ComplianceOfficer")]
        [HttpGet("wallets")]
        public async Task<IActionResult> GetWallets([FromQuery] GetWalletsReportQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("kyc")]
        public async Task<IActionResult> GetKyc([FromQuery] GetKycReportQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("fraud")]
        public async Task<IActionResult> GetFraud([FromQuery] GetFraudReportQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
