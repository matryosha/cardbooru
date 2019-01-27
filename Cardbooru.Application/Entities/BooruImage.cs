using System.Windows.Media.Imaging;

namespace Cardbooru.Application.Entities
{
    public class BooruImage
    {
        public string Hash { get; set; }
        public BitmapImage Image { get; set; }
    }
}