using Application.Exceptions;
using Application.Features.Support.Common;
using Application.Interfaces;
using Application.Wrappers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Support
{
    public class UpdateSupportTicketCommand : IRequest<ApiResponse<SupportTicketDto>>
    {
        public Guid TicketId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateSupportTicketCommandValidator : AbstractValidator<UpdateSupportTicketCommand>
    {
        public UpdateSupportTicketCommandValidator()
        {
            RuleFor(x => x.Subject).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        }
    }

    public class UpdateSupportTicketCommandHandler : IRequestHandler<UpdateSupportTicketCommand, ApiResponse<SupportTicketDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdateSupportTicketCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<SupportTicketDto>> Handle(UpdateSupportTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.SupportTickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
                ?? throw new ApiException("Support ticket not found.");

            var isStaff = _currentUserService.IsInRole("SupportAgent") || _currentUserService.IsInRole("Admin");
            if (!isStaff && ticket.CustomerUserId != _currentUserService.UserId)
            {
                throw new ApiException("Support ticket not found.");
            }

            ticket.Subject = request.Subject;
            ticket.Description = request.Description;
            ticket.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

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
            }, "Support ticket updated.");
        }
    }
}
