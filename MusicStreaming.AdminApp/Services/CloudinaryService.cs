using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MusicStreaming.AdminApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.AdminApp.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(string cloudName, string apiKey, string apiSecret)
        {
            var acc = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadFileAsync(byte[] fileBytes, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            using var stream = new MemoryStream(fileBytes);

            bool isAudio = fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                           fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase);

            if (isAudio)
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = folder
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Cloudinary audio upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            else
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = folder
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Cloudinary image upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<bool> DeleteFileAsync(string publicId, CancellationToken cancellationToken = default)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }
    }
}
