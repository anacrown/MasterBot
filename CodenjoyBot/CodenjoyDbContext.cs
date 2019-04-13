using CodenjoyBot.Entity;
using Microsoft.EntityFrameworkCore;

namespace CodenjoyBot
{
    public sealed class CodenjoyDbContext : DbContext
    {
        public CodenjoyDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<LaunchModel> LaunchModels { get; set; }
        public DbSet<DataFrameModel> DataFrameModels { get; set; }
        public DbSet<ExceptionModel> ExceptionModels { get; set; }
        public DbSet<LaunchSettingsModel> LaunchSettingsModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = DMPStore.db");
        }
    }
}