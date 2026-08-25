using Application.Features.Support.Common;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.Support
{
    public class CreateSupportTicketCommand : IRequest<ApiResponse<SupportTicketDto>>
    {
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CreateSupportTicketCommandValidator : AbstractValidator<CreateSupportTicketCommand>
    {
        public CreateSupportTicketCommandValidator()
        {
            RuleFor(x => x.Subject).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        }
    }

    public class CreateSupportTicketCommandHandler : IRequestHandler<CreateSupportTicketCommand, ApiResponse<SupportTicketDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateSupportTicketCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<SupportTicketDto>> Handle(CreateSupportTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = new SupportTicket
            {
                CustomerUserId = _currentUserService.UserId!,
                Subject = request.Subject,
                Description = request.Description
            };

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<SupportTicketDto>(ToDto(ticket), "Support ticket created.");
        }

        private static SupportTicketDto ToDto(SupportTicket t) => new()
        {
            Id = t.Id,
            CustomerUserId = t.CustomerUserId,
            Subject = t.Subject,
            Description = t.Description,
            Status = t.Status.ToString(),
            AssignedAgentUserId = t.AssignedAgentUserId,
            CreatedAtUtc = t.CreatedAtUtc,
            UpdatedAtUtc = t.UpdatedAtUtc,
            ClosedAtUtc = t.ClosedAtUtc
        };
    }
}
