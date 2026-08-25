using Application.Exceptions;
using Application.Features.AuditLogs.Common;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.AuditLogs
{
    public class GetAuditLogByIdQuery : IRequest<ApiResponse<AuditLogDto>>
    {
        public Guid AuditLogId { get; set; }
    }

    public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, ApiResponse<AuditLogDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAuditLogByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AuditLogDto>> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
        {
            var a = await _context.AuditLogs.FirstOrDefaultAsync(x => x.Id == request.AuditLogId, cancellationToken)
                ?? throw new ApiException("Audit log entry not found.");

            return new ApiResponse<AuditLogDto>(new AuditLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Action = a.Action,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                TimestampUtc = a.TimestampUtc
            });
        }
    }
}
