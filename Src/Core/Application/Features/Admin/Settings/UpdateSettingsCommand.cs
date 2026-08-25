using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Settings
{
    public class UpdateSettingsCommand : IRequest<ApiResponse<List<SettingDto>>>
    {
        public List<SettingDto> Settings { get; set; } = new();
    }

    public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, ApiResponse<List<SettingDto>>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateSettingsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<SettingDto>>> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
        {
            foreach (var setting in request.Settings)
            {
                var existing = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == setting.Key, cancellationToken);
                if (existing != null)
                {
                    existing.Value = setting.Value;
                    existing.UpdatedAtUtc = DateTime.UtcNow;
                }
                else
                {
                    _context.SystemSettings.Add(new SystemSetting { Key = setting.Key, Value = setting.Value });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            var updated = await _context.SystemSettings
                .OrderBy(s => s.Key)
                .Select(s => new SettingDto { Key = s.Key, Value = s.Value })
                .ToListAsync(cancellationToken);

            return new ApiResponse<List<SettingDto>>(updated, "Settings updated successfully.");
        }
    }
}
