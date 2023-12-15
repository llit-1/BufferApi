using Quartz;
using Quartz.Spi;

namespace BufferApi.Jobs
{
    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public SingletonJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        IJob IJobFactory.NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
       {
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        void IJobFactory.ReturnJob(IJob job) {}
    }
}
