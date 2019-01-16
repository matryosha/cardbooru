using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Interfaces;
using Cardbooru.Helpers.Base;

namespace Cardbooru.Application.Services
{
    public class BitmapImageCreatorService : IBitmapImageCreatorService
    {
        public Task<BitmapImage> CreateImageAsync(byte[] bytes)
        {
            return new Task<BitmapImage>(() =>
            {
                BitmapImage image;
                try
                {
                    // early I created BitmapFrame but it appers to consuming REALLY a lot of memory (about 1gig after loading 400 images)
                    // So it sucks
                    //   bitmap = BitmapFrame.Create(wpapper, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                    using (var wpapper = new WrappingStream(new MemoryStream(bytes)))
                    {
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = wpapper;
                        image.EndInit();
                        image.Freeze();
                    }
                }
                catch (Exception e)
                {
                    //Todo add logging
                    image = null;
                }

                return image;
            });
        }
    }
}