using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Cardbooru 
{
    public class BooruImage:INotifyPropertyChanged {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("preview_file_url")]
        public string PreviewUrl { get; set; }

        [JsonProperty("large_file_url")]
        public string FullUrl { get; set; }

        [JsonProperty("md5")]
        public string Hash {
            get => hash;
            set {
                hash = value;
                OnPropertyChanged("Hash");
            }
        }

        private string hash;

        public bool IsHasBadPrewImage { get; set; }

        public Image PreviewImage {
            get => previewImage;
            set {
                previewImage = value;
                OnPropertyChanged("PreviewImage");
                OnPropertyChanged("PreviewImageSource");      
            } }

        private Image previewImage;

        public ImageSource PreviewImageSource {
            get => PreviewImage.Source;
            set {
                if (value == null) throw new ArgumentNullException(nameof(value));
                PreviewImageSource = value;
                OnPropertyChanged("PreviewImageSource");
            }
        }

        public Image FullImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
