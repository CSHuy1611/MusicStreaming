using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Services
{
    public interface IDashboardNotifier
    {
        Task BroadcastDashboardUpdateAsync();
    }
}
