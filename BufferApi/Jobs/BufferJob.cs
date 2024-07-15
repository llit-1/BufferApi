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
        public string LogPath { get; set; } = "BufferJobError.txt";
        public BufferJob(IServiceProvider serviceProvider)
        {
            CalculatorLogRepository = serviceProvider.GetRequiredService<CalculatorLogRepository>();
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            if (CalculatorLogRepository.RepositoryItems.Count == 0)
            {
                return Task.CompletedTask;
            }
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
                    var commandStrings = CreateCommandStrings();
                    foreach (var commandString in commandStrings)
                    {
                        command.CommandText = commandString;
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
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

        private List<string> CreateCommandStrings()
        {
            List<string> commandStrings = new List<string>();
            int maxRowsPerInsert = 1000;
            int totalRows = CalculatorLogRepository.RepositoryItems.Count;

            for (int i = 0; i < totalRows; i += maxRowsPerInsert)
            {
                string commandString = "USE RKNET\r\nInsert into CalculatorLogsTest Values\r\n";
                int currentBatchSize = Math.Min(maxRowsPerInsert, totalRows - i);

                for (int j = 0; j < currentBatchSize; j++)
                {
                    int index = i + j;
                    string datetime = CalculatorLogRepository.RepositoryItems[index].Item.Date.ToString("yyyy-dd-MM HH:mm:ss.fff");
                    string rest = CalculatorLogRepository.RepositoryItems[index].Item.Rest != null ? CalculatorLogRepository.RepositoryItems[index].Item.Rest.ToString() : "null";
                    string fact = CalculatorLogRepository.RepositoryItems[index].Item.Fact != null ? CalculatorLogRepository.RepositoryItems[index].Item.Fact.ToString() : "null";

                    commandString += $"('{CalculatorLogRepository.RepositoryItems[index].Item.UserName}', {CalculatorLogRepository.RepositoryItems[index].Item.ItemCode}, '{CalculatorLogRepository.RepositoryItems[index].Item.ItemName}', {CalculatorLogRepository.RepositoryItems[index].Item.TTCode}, '{CalculatorLogRepository.RepositoryItems[index].Item.TTName}', {rest}, {CalculatorLogRepository.RepositoryItems[index].Item.Result}, {fact}, '{datetime}', '{CalculatorLogRepository.RepositoryItems[index].Item.SessionId}')";
                    if (j < currentBatchSize - 1)
                    {
                        commandString += ",\r\n";
                    }
                }

                commandStrings.Add(commandString);
            }

            return commandStrings;
        }
    }
}
