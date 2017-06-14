using Jobbie.Domain.Models;
using Quartz;

namespace Jobbie.Scheduler.Queries
{
    public interface IScheduleConverter
    {
        Schedule For(ITrigger trigger);
    }
}