using System;

namespace Jobbie.Domain.Commands
{
    public interface IJobDeleter
    {
        void Delete(Guid jobId);
    }
}