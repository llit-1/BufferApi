using BufferApi.DB.RKNET;
using Microsoft.EntityFrameworkCore;

namespace BufferApi.DB
{
    public class RknetContext : DbContext
    {
        public RknetContext(DbContextOptions<RknetContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalculatorLogsTest>().ToTable(tb => tb.HasTrigger("CalculatorLogsTest_UpdatesActualDatas"));
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<CalculatorLogsTest> CalculatorLogsTest { get; set; } // логи калькулятора
    }
}
