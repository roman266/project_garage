using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using project_garage.Interfaces.IService;
using Microsoft.Extensions.Options;
using project_garage.Models.CloudinarySettings;
using static System.Net.Mime.MediaTypeNames;

namespace project_garage.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly int _maxSizeInMB = 5 * 1024 * 1024;
        private readonly string[] _allowedTypes = { "image/jpeg", "image/png", "image/webp" };

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            ValidateImage(file);
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "profile_pictures"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }

        private void ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            
            if (file.Length > _maxSizeInMB)
                throw new ArgumentException("File weighs more than 5 MB");

            if (!_allowedTypes.Contains(file.ContentType))
                throw new ArgumentException("Дозволені лише файли .jpg, .png, .webp.");
        }
    }
}
