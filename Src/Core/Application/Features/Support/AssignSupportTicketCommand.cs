using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Support
{
    public class AssignSupportTicketCommand : IRequest<ApiResponse<string>>
    {
        public Guid TicketId { get; set; }
    }

    public class AssignSupportTicketCommandHandler : IRequestHandler<AssignSupportTicketCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AssignSupportTicketCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(AssignSupportTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
                ?? throw new ApiException("Support ticket not found.");

            ticket.AssignedAgentUserId = _currentUserService.UserId;
            ticket.Status = SupportTicketStatus.InProgress;
            ticket.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>(null!, "Support ticket assigned to you.");
        }
    }
}
