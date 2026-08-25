using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Dashboard
{
    public class DashboardDto
    {
        public int TotalCustomers { get; set; }
        public int TotalWallets { get; set; }
        public decimal TotalWalletBalance { get; set; }
        public int TodaysTransactions { get; set; }
        public int PendingKyc { get; set; }
        public int SuspiciousTransactions { get; set; }
        public int FailedTransactions { get; set; }
    }

    public class GetDashboardQuery : IRequest<ApiResponse<DashboardDto>>
    {
    }

    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, ApiResponse<DashboardDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetDashboardQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApiResponse<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var todayStart = DateTime.UtcNow.Date;

            var dashboard = new DashboardDto
            {
                TotalCustomers = (await _userManager.GetUsersInRoleAsync("Customer")).Count,
                TotalWallets = await _context.Wallets.CountAsync(cancellationToken),
                TotalWalletBalance = await _context.Wallets.SumAsync(w => w.Balance, cancellationToken),
                TodaysTransactions = await _context.Transactions.CountAsync(t => t.CreatedAtUtc >= todayStart, cancellationToken),
                PendingKyc = await _context.KycApplications.CountAsync(k => k.Status == KycStatus.Pending, cancellationToken),
                SuspiciousTransactions = await _context.FraudAlerts.CountAsync(f => f.Status == FraudAlertStatus.Open || f.Status == FraudAlertStatus.UnderReview, cancellationToken),
                FailedTransactions = await _context.Transactions.CountAsync(t => t.Status == TransactionStatus.Failed || t.Status == TransactionStatus.Rejected, cancellationToken)
            };

            return new ApiResponse<DashboardDto>(dashboard);
        }
    }
}
