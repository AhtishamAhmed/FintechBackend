using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<TransactionEntry> TransactionEntries => Set<TransactionEntry>();
        public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
        public DbSet<KycApplication> KycApplications => Set<KycApplication>();
        public DbSet<FraudAlert> FraudAlerts => Set<FraudAlert>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.HasIndex(rt => rt.Token).IsUnique();
                entity.HasIndex(rt => rt.UserId);
                entity.HasOne(rt => rt.User)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Wallet>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.HasIndex(w => w.UserId).IsUnique();
                entity.Property(w => w.Balance).HasColumnType("decimal(18,2)");
                entity.HasOne(w => w.User)
                    .WithMany()
                    .HasForeignKey(w => w.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property<uint>("xmin").IsRowVersion();
            });

            builder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.HasIndex(t => t.InitiatedByUserId);
                entity.HasIndex(t => t.CreatedAtUtc);
                entity.HasIndex(t => t.IdempotencyKey).IsUnique();
                entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");
                entity.HasOne(t => t.Wallet)
                    .WithMany()
                    .HasForeignKey(t => t.WalletId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<TransactionEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Transaction)
                    .WithMany(t => t.Entries)
                    .HasForeignKey(e => e.TransactionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Wallet)
                    .WithMany(w => w.Entries)
                    .HasForeignKey(e => e.WalletId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<FraudAlert>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.HasIndex(f => f.Status);
                entity.HasOne(f => f.Transaction)
                    .WithOne(t => t.FraudAlert)
                    .HasForeignKey<FraudAlert>(f => f.TransactionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Beneficiary>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.HasIndex(b => b.UserId);
                entity.HasOne(b => b.User)
                    .WithMany()
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<KycApplication>(entity =>
            {
                entity.HasKey(k => k.Id);
                entity.HasIndex(k => k.UserId).IsUnique();
                entity.HasIndex(k => k.Status);
                entity.HasOne(k => k.User)
                    .WithMany()
                    .HasForeignKey(k => k.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.HasIndex(n => n.UserId);
                entity.HasOne(n => n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<SupportTicket>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasIndex(s => s.CustomerUserId);
                entity.HasIndex(s => s.Status);
                entity.HasOne(s => s.Customer)
                    .WithMany()
                    .HasForeignKey(s => s.CustomerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.HasIndex(a => a.TimestampUtc);
                entity.HasIndex(a => a.UserId);
            });

            builder.Entity<SystemSetting>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasIndex(s => s.Key).IsUnique();
            });
        }
    }
}
