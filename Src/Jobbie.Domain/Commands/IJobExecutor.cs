using Jobbie.Domain.Models;

namespace Jobbie.Domain.Commands
{
    public interface IJobExecutor
    {
        void Execute(Job job);
    }
}