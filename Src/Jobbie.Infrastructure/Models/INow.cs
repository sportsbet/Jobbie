using System;

namespace Jobbie.Infrastructure.Models
{
    public interface INow
    {
        DateTime Utc { get; }
    }
}