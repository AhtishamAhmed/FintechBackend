using Application.Exceptions;
using Application.Features.Support.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Support
{
    public class GetSupportTicketByIdQuery : IRequest<ApiResponse<SupportTicketDto>>
    {
        public Guid TicketId { get; set; }
    }

    public class GetSupportTicketByIdQueryHandler : IRequestHandler<GetSupportTicketByIdQuery, ApiResponse<SupportTicketDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetSupportTicketByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<SupportTicketDto>> Handle(GetSupportTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
                ?? throw new ApiException("Support ticket not found.");

            var isStaff = _currentUserService.IsInRole("SupportAgent") || _currentUserService.IsInRole("Admin");
            if (!isStaff && ticket.CustomerUserId != _currentUserService.UserId)
            {
                throw new ApiException("Support ticket not found.");
            }

            return new ApiResponse<SupportTicketDto>(new SupportTicketDto
            {
                Id = ticket.Id,
                CustomerUserId = ticket.CustomerUserId,
                Subject = ticket.Subject,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                AssignedAgentUserId = ticket.AssignedAgentUserId,
                CreatedAtUtc = ticket.CreatedAtUtc,
                UpdatedAtUtc = ticket.UpdatedAtUtc,
                ClosedAtUtc = ticket.ClosedAtUtc
            });
        }
    }
}
