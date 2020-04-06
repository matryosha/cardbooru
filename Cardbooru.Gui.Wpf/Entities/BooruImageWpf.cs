using Cardbooru.Application.Entities;
using Cardbooru.Gui.Wpf.Infrastructure;
using System;
using System.Windows.Media.Imaging;

namespace Cardbooru.Gui.Wpf.Entities
{
    public class BooruImageWpf : BooruImage
    {
        public BooruImageWpf(BooruImage booruImage)
        {
            Hash = booruImage.Hash;
            Data = booruImage.Data;
        }

        public BitmapImage BitmapImage { get; set; }
        public bool IsBitmapImageInitialized => BitmapImage == null;
        public bool IsSuccessfullyLoaded { get; set; }

        public bool InitializeImage()
        {
            if (IsSuccessfullyLoaded) return true;
            if (Data == null) throw new InvalidOperationException();

            BitmapImage = BitmapImageCreator.Create(Data);

            if (BitmapImage == null) return false;
            IsSuccessfullyLoaded = true;
            return true;
        }
    }
}
