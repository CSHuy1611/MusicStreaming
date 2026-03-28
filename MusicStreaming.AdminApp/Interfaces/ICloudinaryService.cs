using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.AdminApp.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(byte[] fileBytes, string fileName, string folder, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileAsync(string publicId, CancellationToken cancellationToken = default);
    }
}
