using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PitStore.Services
{
    public interface IImageService
    {
        Task<string> CreateThumbnailAsync(string imageUrl);
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadImageAsync(IFormFile file, string folder);
        Task<bool> DeleteImageAsync(string imageUrl);
        void DeleteImage(string imagePath);
    }
} 