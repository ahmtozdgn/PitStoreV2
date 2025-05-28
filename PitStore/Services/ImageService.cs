using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PitStore.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private const string UploadsFolder = "uploads";
        private const string ThumbnailsFolder = "thumbnails";

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> CreateThumbnailAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            var imagePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));
            var thumbnailPath = Path.Combine(_environment.WebRootPath, ThumbnailsFolder, Path.GetFileName(imageUrl));

            if (!File.Exists(imagePath))
                return string.Empty;

            using (var image = await Image.LoadAsync(imagePath))
            {
                image.Mutate(x => x.Resize(200, 200));
                await image.SaveAsync(thumbnailPath);
            }

            return $"/{ThumbnailsFolder}/{Path.GetFileName(imageUrl)}";
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{UploadsFolder}/{fileName}";
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsPath = Path.Combine(_environment.WebRootPath, folder);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folder}/{fileName}";
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }

            var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        public Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return Task.FromResult(false);

            var imagePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));
            if (!File.Exists(imagePath))
                return Task.FromResult(false);

            try
            {
                File.Delete(imagePath);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            var fileName = Path.GetRandomFileName();
            var extension = Path.GetExtension(file.FileName);
            var newFileName = fileName + extension;

            var filePath = Path.Combine(_environment.WebRootPath, folder, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folder}/{newFileName}";
        }
    }
} 