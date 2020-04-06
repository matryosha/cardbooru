using Cardbooru.Application.Infrastructure;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Cardbooru.Gui.Wpf.Infrastructure
{
    static class BitmapImageCreator
    {
        public static BitmapImage Create(byte[] data)
        {
            BitmapImage image;
            try
            {
                // early I created BitmapFrame but it appers to consuming REALLY a lot of memory (about 1gig after loading 400 images)
                // So it sucks
                //   bitmap = BitmapFrame.Create(wpapper, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                using (var wpapper = new WrappingStream(new MemoryStream(data)))
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
        }
    }
}
