using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace pbms_be.Data
{
    public class PbmsDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<Auth.Account> Account { get; set; }
        public DbSet<Auth.Role> Role { get; set; }
        public DbSet<WalletF.Currency> Currency { get; set; }
        public DbSet<WalletF.Wallet> Wallet { get; set; }
        public DbSet<Status.ActiveState> ActiveState { get; set; }

        public PbmsDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "";
            if (System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                var instanceConnectionName = System.Environment.GetEnvironmentVariable("INSTANCE_UNIX_SOCKET");
                // throw ecxeption if not set
                if (instanceConnectionName == null)
                {
                    throw new System.Exception("INSTANCE_UNIX_SOCKET not set");
                }
                var databaseName = System.Environment.GetEnvironmentVariable("DB_NAME");
                var userId = System.Environment.GetEnvironmentVariable("DB_USER");
                var password = System.Environment.GetEnvironmentVariable("DB_PASS");
                connectionString = $"Host={instanceConnectionName};User Id={userId};Password={password};Database={databaseName}";
            }
            else
            {
                var env = System.Environment.GetEnvironmentVariable("PBMS_CONNECTION_STRING");
                var env2 = _configuration.GetConnectionString("ConnectionString");
                connectionString = env ?? env2 ?? throw new System.Exception("Connection string not set");
            }
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
