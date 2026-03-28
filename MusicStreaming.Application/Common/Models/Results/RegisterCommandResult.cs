using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Models.Results
{
    public class RegisterCommandResult
    {
        public bool IsSuccess { get; set; }
        public string UserId { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
