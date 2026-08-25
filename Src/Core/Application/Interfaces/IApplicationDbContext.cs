using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Wallet> Wallets { get; }
        DbSet<Transaction> Transactions { get; }
        DbSet<TransactionEntry> TransactionEntries { get; }
        DbSet<Beneficiary> Beneficiaries { get; }
        DbSet<KycApplication> KycApplications { get; }
        DbSet<FraudAlert> FraudAlerts { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<SupportTicket> SupportTickets { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<SystemSetting> SystemSettings { get; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
