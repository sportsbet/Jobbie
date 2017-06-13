using System;

namespace Jobbie.Domain.Commands
{
    public interface IJobScheduler
    {
        void Create(Guid scheduleId, Guid jobId, string description, DateTime startUtc);

        void Create(Guid scheduleId, Guid jobId, string description, DateTime startUtc, string cron);

        void Create(Guid scheduleId, Guid jobId, string description, DateTime startUtc, string cron, DateTime endUtc);

        void Delete(Guid scheduleId);
    }
}