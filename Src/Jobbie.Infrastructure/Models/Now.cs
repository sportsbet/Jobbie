using System;

namespace Jobbie.Infrastructure.Models
{
    public sealed class Now : INow
    {
        private DateTime? _now;

        public DateTime Utc
        {
            get
            {
                if (!_now.HasValue)
                    _now = DateTime.UtcNow;
                return _now.Value;
            }
        }
    }
}