using Application.Features.Users.Common;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Reports
{
    public class GetUsersReportQuery : IRequest<ApiResponse<PagedResult<UserProfileDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetUsersReportQueryHandler : IRequestHandler<GetUsersReportQuery, ApiResponse<PagedResult<UserProfileDto>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUsersReportQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResponse<PagedResult<UserProfileDto>>> Handle(GetUsersReportQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var totalCount = _userManager.Users.Count();
            var users = _userManager.Users
                .OrderBy(u => u.Email)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var items = new List<UserProfileDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                items.Add(new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Status = user.Status.ToString(),
                    Roles = roles.ToList()
                });
            }

            return new ApiResponse<PagedResult<UserProfileDto>>(new PagedResult<UserProfileDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
    }
}
