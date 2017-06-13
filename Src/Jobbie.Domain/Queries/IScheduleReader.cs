using System;
using System.Collections.Generic;
using Jobbie.Domain.Models;

namespace Jobbie.Domain.Queries
{
    public interface IScheduleReader
    {
        Schedule For(Guid scheduleId);
        IReadOnlyCollection<Schedule> All();
        IReadOnlyCollection<Schedule> FilterBy(Guid jobId);
        IReadOnlyCollection<Schedule> FilterBy(string description);
    }
}