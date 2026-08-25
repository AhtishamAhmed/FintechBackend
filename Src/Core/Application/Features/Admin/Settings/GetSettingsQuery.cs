using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Settings
{
    public class SettingDto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class GetSettingsQuery : IRequest<ApiResponse<List<SettingDto>>>
    {
    }

    public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, ApiResponse<List<SettingDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetSettingsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<SettingDto>>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = await _context.SystemSettings
                .OrderBy(s => s.Key)
                .Select(s => new SettingDto { Key = s.Key, Value = s.Value })
                .ToListAsync(cancellationToken);

            return new ApiResponse<List<SettingDto>>(settings);
        }
    }
}
