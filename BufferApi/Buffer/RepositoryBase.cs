using BufferApi.DB;
using Microsoft.EntityFrameworkCore;

namespace BufferApi.Buffer
{
    public abstract class RepositoryBase<T>
    {
        public List<RepositoryItem<T>> RepositoryItems { get; set; } = new List<RepositoryItem<T>>();
        public void Add(T item, DateTime startDate, DateTime licvidationDate)
        {
            RepositoryItems.Add(new RepositoryItem<T>(item, startDate, licvidationDate));
        }

        public void Add(List<T> Items, DateTime startDate, DateTime licvidationDate)
        {
            foreach (var item in Items)
            {
                RepositoryItems.Add(new RepositoryItem<T>(item, startDate, licvidationDate));
            }
        }


        public virtual void TryToAdd()
        {}

        public void Licvidation()
        {
            RepositoryItems.RemoveAll(c => c.LicvidationDate <= DateTime.Now);
        }       
    }

    public class RepositoryItem<T>
    {
        public RepositoryItem(T t, DateTime startDate, DateTime licvidationDate)
        {
            Item = t;
            StartDate = startDate;
            LicvidationDate = licvidationDate;
        }
        public T Item { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LicvidationDate { get; set; }

    }
}
