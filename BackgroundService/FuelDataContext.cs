using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace BackgroundService
{
    public sealed class FuelDbContext : DbContext
    {
        public FuelDbContext() { }

        public FuelDbContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<FuelPrice> Prices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FuelPrice>()
                .HasKey(item => item.Id)
                .HasIndex(item => item.Date).IsUnique();
            modelBuilder.Entity<FuelPrice>()
                .Property(item => item.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
