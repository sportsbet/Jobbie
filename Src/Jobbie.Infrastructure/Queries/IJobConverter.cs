using Jobbie.Domain.Models;
using Quartz;

namespace Jobbie.Infrastructure.Queries
{
    public interface IJobConverter
    {
        Job For(IJobDetail job);
    }
}