using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class SystemLog
    {
        public int Id { get; private set; }
        public string AdminId { get; private set; }
        public string Action { get; private set; } 
        public string EntityId { get; private set; } 
        public string Note { get; private set; } 
        public DateTime Timestamp { get; private set; }

        public SystemLog(string adminId, string action, string entityId, string note)
        {
            AdminId = adminId;
            Action = action;
            EntityId = entityId;
            Note = note;
            Timestamp = DateTime.UtcNow;
        }
    }
}
