using Idempotency_Key.Model;
using Microsoft.EntityFrameworkCore;

namespace Idempotency_Key.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<IdempotencyRecord> IdempotencyRecords
            => Set<IdempotencyRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdempotencyRecord>()
                .HasIndex(x => x.IdempotencyKey)
                .IsUnique();

            modelBuilder.Entity<IdempotencyRecord>()
                .Property(x => x.IdempotencyKey)
                .HasMaxLength(100);

        }
    }
}
