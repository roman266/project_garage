using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace project_garage.Interfaces.IService
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
