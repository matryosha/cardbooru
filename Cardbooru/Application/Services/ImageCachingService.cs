using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Interfaces;
using Cardbooru.Helpers;
using Cardbooru.Models.Base;

namespace Cardbooru.Application.Services
{
    public class ImageCachingService : IImageCachingService
    {
        private string _cachePath;
        private readonly IBitmapImageCreatorService _imageCreatorService;
        public ImageCachingService(IBooruConfiguration conf,
            IBitmapImageCreatorService imageCreatorService)
        {
            _cachePath = conf.CachePath;
            _imageCreatorService = imageCreatorService;
        }


        public async Task CacheImageAsync(BooruImageModelBase booruImage, 
            ImageSizeType imageType, 
            byte[] bytes, 
            ImageSizeType sizeType,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (FileStream stream = File.Open(GetImagePath(booruImage, imageType), FileMode.OpenOrCreate))
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (IOException e)
            {

            }
        }

        public async Task<BitmapImage> GetImageAsync(
            BooruImageModelBase booruImage, 
            ImageSizeType imageType,
            CancellationToken cancellationToken = default)
        {
            byte[] buff;
            using (var file = new FileStream(GetImagePath(booruImage, imageType), 
                FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                buff = new byte[file.Length];
                await file.ReadAsync(buff, 0, (int)file.Length, cancellationToken).ConfigureAwait(false);
            }

            return await _imageCreatorService.CreateImageAsync(buff).ConfigureAwait(false);
        }

        public bool IsHasCache(BooruImageModelBase booruImage, 
            ImageSizeType imageType)
        {
            return File.Exists(GetImagePath(
                booruImage, imageType));
        }

        /// <summary>
        /// Return full path to image in cache directory depends on imagesizetype
        /// </summary>
        private string GetImagePath(BooruImageModelBase booruImage,ImageSizeType type)
        {
            var workDir = Path.Combine(Directory.GetCurrentDirectory(), _cachePath);
            return type == ImageSizeType.Preview
                ? Path.Combine(
                    workDir, booruImage.Hash + "_preview")
                : Path.Combine(
                    workDir, booruImage.Hash + "_full");
        }
    }
}
