using System;
using System.Collections.Generic;
using Jobbie.Domain.Models;

namespace Jobbie.Domain.Queries
{
    public interface IJobReader
    {
        Job For(Guid jobId);
        IReadOnlyCollection<Job> All();
        IReadOnlyCollection<Job> FilterBy(string description);
    }
}