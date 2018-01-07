using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru;

namespace TestFromWpf
{
    
    public partial class MainWindow : Window
    {


        List<Button> listsButtons = new List<Button>();
        private Model model;
        int index = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void  ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            model = new Model();

            Image image = new Image();
            

            //image.Source = await model.GetPreviewImage(new BooruImage
            //{
            //    Hash = "d34e4cf0a437a5d65f8e82b7bcd02606",
            //    Id = "2",
            //    PreviewUrl = "/data/preview/d34e4cf0a437a5d65f8e82b7bcd02606.jpg"
            //});

            //SaveImageToJPEG(image, "test/new.jpg");
            //MainImage.Source = image.Source;
            await model.GetImages(1);



        }

        private void Action(object sender, RoutedEventArgs e) {
            MainImage.Source = model.BooruImagesList[index].PreviewImage.Source;
            index++;
            //SaveImageToJPEG(model.BooruImagesList[1].PreviewImage, "test/image.jpg");

        }

        private void SaveImageToJPEG(Image ImageToSave, string Location)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ImageToSave.Source.Width,
                (int)ImageToSave.Source.Height,
                100, 100, PixelFormats.Default);
            renderTargetBitmap.Render(ImageToSave);
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (FileStream fileStream = new FileStream(Location, FileMode.Create))
            {
                jpegBitmapEncoder.Save(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
        }


    }
}
