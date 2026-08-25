using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Support
{
    public class CloseSupportTicketCommand : IRequest<ApiResponse<string>>
    {
        public Guid TicketId { get; set; }
    }

    public class CloseSupportTicketCommandHandler : IRequestHandler<CloseSupportTicketCommand, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;

        public CloseSupportTicketCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> Handle(CloseSupportTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
                ?? throw new ApiException("Support ticket not found.");

            ticket.Status = SupportTicketStatus.Closed;
            ticket.ClosedAtUtc = DateTime.UtcNow;
            ticket.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>(null!, "Support ticket closed.");
        }
    }
}
