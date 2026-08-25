using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Behaviors
{
    public class AuditLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AuditLoggingBehavior(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            var requestName = typeof(TRequest).Name;
            if (requestName.EndsWith("Command"))
            {
                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = _currentUserService.UserId,
                    Action = requestName
                });

                await _context.SaveChangesAsync(cancellationToken);
            }

            return response;
        }
    }
}
