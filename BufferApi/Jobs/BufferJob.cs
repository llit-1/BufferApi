using BufferApi.Buffer;
using BufferApi.DB.RKNET;
using BufferApi.Models;
using Microsoft.Data.SqlClient;
using Quartz;

namespace BufferApi.Jobs
{
    public class BufferJob : IJob
    {
        public CalculatorLogRepository CalculatorLogRepository { get; set; }
        public string LogPath { get; set; } = "BufferJobError";
        public BufferJob(IServiceProvider serviceProvider)
        {
            CalculatorLogRepository = serviceProvider.GetRequiredService<CalculatorLogRepository>();
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            DeleteSimilar();
            TryToAdd();
            CalculatorLogRepository.Licvidation();
            return Task.CompletedTask;
        }

        private void TryToAdd()
        {
            string connectionString = "Data Source=RKSQL.shzhleb.ru\\SQL2019; Initial Catalog=RKNET; User ID=rk7; Password=wZSbs6NKl2SF; TrustServerCertificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new();
                    command.CommandText = CreateCommandString();
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    connection.Close();
                    for (int i = 0; i < CalculatorLogRepository.RepositoryItems.Count; i++)
                    {
                        CalculatorLogRepository.RepositoryItems[i].LicvidationDate = DateTime.Now;
                    }

                }
                catch (Exception ex)
                {
                    connection.Close();
                    GlobalFunctions.WriteToFile(LogPath, DateTime.Now + " " + ex.Message);
                }
            }

        }

        private void DeleteSimilar()
        {

            for (int i = 0; i < CalculatorLogRepository.RepositoryItems.Count; i++)
            {
                if (CalculatorLogRepository.RepositoryItems[i].LicvidationDate < DateTime.Now)
                {
                    continue;
                }
                List<RepositoryItem<CalculatorLogsTest>> deleteList = CalculatorLogRepository.RepositoryItems.Where(c => GlobalFunctions.Like(CalculatorLogRepository.RepositoryItems[i], c)).ToList();
                deleteList.Remove(GlobalFunctions.Younger(deleteList));
                for (int j = 0; j < deleteList.Count; j++)
                {
                    deleteList[j].LicvidationDate = DateTime.Now;
                }
            }
            CalculatorLogRepository.Licvidation();
        }

        private string CreateCommandString()
        {
            string commandString = "USE RKNET\r\nInsert into CalculatorLogsTest Values\r\n";
            for (int i = 0; i < CalculatorLogRepository.RepositoryItems.Count; i++)
            {
                string datetime = CalculatorLogRepository.RepositoryItems[i].Item.Date.ToString("yyyy-dd-MM HH:mm:ss.fff");
                string rest = "null";
                string fact = "null";
                if (CalculatorLogRepository.RepositoryItems[i].Item.Rest != null)
                {
                    rest = CalculatorLogRepository.RepositoryItems[i].Item.Rest.ToString();
                }
                if (CalculatorLogRepository.RepositoryItems[i].Item.Fact != null)
                {
                    fact = CalculatorLogRepository.RepositoryItems[i].Item.Fact.ToString();
                }
                commandString += $"('{CalculatorLogRepository.RepositoryItems[i].Item.UserName}', {CalculatorLogRepository.RepositoryItems[i].Item.ItemCode}, '{CalculatorLogRepository.RepositoryItems[i].Item.ItemName}', {CalculatorLogRepository.RepositoryItems[i].Item.TTCode}, '{CalculatorLogRepository.RepositoryItems[i].Item.TTName}', {rest}, {CalculatorLogRepository.RepositoryItems[i].Item.Result}, {fact}, '{datetime}', '{CalculatorLogRepository.RepositoryItems[i].Item.SessionId}')";
                if (i < CalculatorLogRepository.RepositoryItems.Count - 1)
                {
                    commandString += ",\r\n";
                }
            }
            return commandString;
        }
        
    }
}

