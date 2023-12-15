using BufferApi.Buffer;
using BufferApi.DB;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace BufferApi.Jobs
{
    public class BufferJob : IJob
    {
        public CalculatorLogRepository CalculatorLogRepository { get; set; }

        public int a { get; set; }
        public int b { get; set; }
        public BufferJob(IServiceProvider serviceProvider)
        {
            CalculatorLogRepository = serviceProvider.GetRequiredService<CalculatorLogRepository>();
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            TryToAdd();
            CalculatorLogRepository.Licvidation();
            return Task.CompletedTask;
        }

        private void TryToAdd()
        {

            DbContextOptionsBuilder<RknetContext> dbContextOptionsBuilder = new(); // создаём настройки подключения к бд
            dbContextOptionsBuilder.UseSqlServer("Data Source=RKSQL.shzhleb.ru\\SQL2019; Initial Catalog=RKNET; User ID=rk7; Password=wZSbs6NKl2SF; TrustServerCertificate=True");

            using (RknetContext db = new(dbContextOptionsBuilder.Options)) // подключаемся к бд
            {
                for (int i = 0; i < CalculatorLogRepository.RepositoryItems.Count; i++)
                {
                    try
                    {
                        db.Add(CalculatorLogRepository.RepositoryItems[i].Item);
                        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT CalculatorLogsTest ON");
                        db.SaveChanges();
                        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT CalculatorLogsTest OFF");
                        CalculatorLogRepository.RepositoryItems[i].LicvidationDate = DateTime.Now;
                    }

                    catch (Exception)
                    {
                        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT CalculatorLogsTest OFF");
                        continue;
                    }
                }


                //for (int i = 0; i < CalculatorLogRepository.RepositoryItems.Count; i = i + 100)
                //{
                //    for (int j = i; j < i + 100; j++)
                //    {
                //        db.Add(CalculatorLogRepository.RepositoryItems[i].Item);
                //    }
                //    try
                //    {
                //        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT CalculatorLogsTest ON");
                //        db.SaveChanges();
                //        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT CalculatorLogsTest OFF");

                //        for (int j = i; j < i + 100; j++)
                //        {
                //            CalculatorLogRepository.RepositoryItems[i].LicvidationDate = DateTime.Now;
                //        }
                //    }
                //    catch (Exception)
                //    {
                //        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT CalculatorLogsTest OFF");
                //        continue;
                //    }
                //}



            }

        }
    }
}
