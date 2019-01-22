using System.Windows.Media.Imaging;

namespace Cardbooru.Application.Entities
{
    public class BooruImageWrapper
    {
        public string Hash { get; set; }
        public BitmapImage Image { get; set; }
    }
}