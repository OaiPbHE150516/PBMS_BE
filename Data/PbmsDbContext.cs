using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace pbms_be.Data
{
    public class PbmsDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<Auth.Account> Account { get; set; }
        public DbSet<Auth.Role> Role { get; set; }

        public DbSet<Balance.BalanceHistoryLog> BalanceHistoryLogs { get; set; }

        public DbSet<WalletF.Currency> Currency { get; set; }

        public DbSet<WalletF.Wallet> Wallet { get; set; }

        public DbSet<Budget.Budget> Budget { get; set; }

        public DbSet<Budget.BudgetCategory> BudgetCategory { get; set; }

        public DbSet<Status.ActiveState> ActiveState { get; set; }

        public DbSet<Filter.Category> Category { get; set; }

        public DbSet<Filter.CategoryType> CategoryType { get; set; }

        public DbSet<Filter.DefaultCategory> DefaultCategory { get; set; }

        public DbSet<Trans.Transaction> Transaction { get; set; }

        public DbSet<Invo.Invoice> Invoice { get; set; }

        public DbSet<Invo.ProductInInvoice> ProductInInvoice { get; set; }

        // start of collab fund
        public DbSet<CollabFund.CollabFund> CollabFund { get; set; }

        public DbSet<CollabFund.AccountCollab> AccountCollab { get; set; }

        public DbSet<CollabFund.CollabFundActivity> CollabFundActivity { get; set; }

        public DbSet<CollabFund.CF_DividingMoney> CF_DividingMoney { get; set; }

        public DbSet<CollabFund.CF_DividingMoneyDetail> CF_DividingMoneyDetail { get; set; }


        public DbSet<Custom.DivideMoneyInfo> DivideMoneyInfo { get; set; }

        // TextPrompt
        public DbSet<TextPro.TextPrompt> TextPrompt { get; set; }

        // ScanLog
        public DbSet<LogModel.ScanLog> ScanLogs { get; set; }


        // end of collab fund

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
