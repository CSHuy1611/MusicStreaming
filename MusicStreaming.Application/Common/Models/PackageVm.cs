using MusicStreaming.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Models
{
    public class PackageVm
    {
        public List<PackageDto> Packages { get; set; }
    }
}
