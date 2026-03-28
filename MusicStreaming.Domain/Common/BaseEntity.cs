using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; } 
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; private set; }


        public void Delete()
        {
            IsDeleted = true;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Restore()
        {
            IsDeleted = false;
            UpdatedDate = DateTime.UtcNow;
        }

        protected void UpdateTime()
        {
            UpdatedDate = DateTime.UtcNow;
        }
    }
}
