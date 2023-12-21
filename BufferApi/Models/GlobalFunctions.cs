using BufferApi.Buffer;
using BufferApi.DB.RKNET;

namespace BufferApi.Models
{
    public static class GlobalFunctions
    {

        public static bool Like(RepositoryItem<CalculatorLogsTest> repositoryItem1, RepositoryItem<CalculatorLogsTest> repositoryItem2)
        {
            if (repositoryItem1.Item.ItemCode == repositoryItem2.Item.ItemCode && repositoryItem1.Item.TTCode == repositoryItem2.Item.TTCode && repositoryItem1.Item.SessionId == repositoryItem2.Item.SessionId)
            {
                return true;
            }
            return false;
        }

        public static RepositoryItem<CalculatorLogsTest> Older(RepositoryItem<CalculatorLogsTest> repositoryItem1, RepositoryItem<CalculatorLogsTest> repositoryItem2)
        {
            if (repositoryItem1.Item.Date >= repositoryItem2.Item.Date)
            {
                return repositoryItem2;
            }
            return repositoryItem1;
        }

        public static RepositoryItem<CalculatorLogsTest> Older(List<RepositoryItem<CalculatorLogsTest>> repositoryItems)
        {
            RepositoryItem<CalculatorLogsTest>  returnRepositoryItem = repositoryItems.FirstOrDefault();
            foreach (var item in repositoryItems)
            {
                returnRepositoryItem = Older(returnRepositoryItem, item);
            }
            return returnRepositoryItem;
        }




        public static RepositoryItem<CalculatorLogsTest> Younger(RepositoryItem<CalculatorLogsTest> repositoryItem1, RepositoryItem<CalculatorLogsTest> repositoryItem2)
        {
            if (repositoryItem1.Item.Date >= repositoryItem2.Item.Date)
            {
                return repositoryItem1;
            }
            return repositoryItem2;
        }

        public static RepositoryItem<CalculatorLogsTest> Younger(List<RepositoryItem<CalculatorLogsTest>> repositoryItems)
        {
            RepositoryItem<CalculatorLogsTest> returnRepositoryItem = repositoryItems.FirstOrDefault();
            foreach (var item in repositoryItems)
            {
                returnRepositoryItem = Younger(returnRepositoryItem, item);
            }
            return returnRepositoryItem;
        }


        public static void WriteToFile(string filePath, string text)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(text);
            }
        }

    }
}
