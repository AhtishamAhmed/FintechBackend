using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class FraudDetectionService : IFraudDetectionService
    {
        private const decimal LargeAmountThreshold = 500000m;
        private const int VelocityCount = 5;
        private static readonly TimeSpan VelocityWindow = TimeSpan.FromMinutes(1);

        private readonly IApplicationDbContext _context;

        public FraudDetectionService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string?> EvaluateAsync(string userId, decimal amount, CancellationToken cancellationToken = default)
        {
            if (amount > LargeAmountThreshold)
            {
                return $"Transaction amount {amount:N2} exceeds the large-transaction threshold of {LargeAmountThreshold:N2}.";
            }

            var windowStart = DateTime.UtcNow.Subtract(VelocityWindow);
            var recentCount = await _context.Transactions
                .Where(t => t.InitiatedByUserId == userId && t.CreatedAtUtc >= windowStart)
                .CountAsync(cancellationToken);

            if (recentCount + 1 >= VelocityCount)
            {
                return $"{recentCount + 1} transactions initiated within the last minute, exceeding the velocity limit of {VelocityCount}.";
            }

            return null;
        }
    }
}
