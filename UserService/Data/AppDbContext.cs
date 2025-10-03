using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "User Id=postgres.smjdaavxsnbmrdrvejsu;Password=S3nhaS3gur@:(;Server=aws-1-sa-east-1.pooler.supabase.com;Port=5432;Database=postgres",
                    o => o.CommandTimeout(180) // ⏳ Timeout de 3 minutos
                );
            }
        }
    }
}
