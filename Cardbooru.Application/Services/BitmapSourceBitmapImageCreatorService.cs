﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Infrastructure;
using Cardbooru.Application.Interfaces;

namespace Cardbooru.Application.Services
{
    public class BitmapImageCreatorService : IBitmapImageCreatorService
    {
        public Task<BitmapImage> CreateImageAsync(byte[] bytes)
        {
            return Task.Run(() => CreateImage(bytes));
        }

        private static BitmapImage CreateImage(byte[] bytes)
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
        }
    }
}