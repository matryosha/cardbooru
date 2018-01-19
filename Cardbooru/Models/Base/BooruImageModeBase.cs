using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cardbooru.Models.Base
{
    public abstract class BooruImageModelBase :
        INotifyPropertyChanged
    {
        private string _hash;
        private Image _previewImage;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual string TagsString { get; set; }
        public virtual string Id { get; set; }
        public virtual string PreviewImageUrl { get; set; }
        public virtual string FullImageUrl { get; set; }
        public virtual List<string> TagsList { get; set; } = new List<string>();
        
        public virtual Image FullImage { get; set; }
        public virtual string Hash {
            get => _hash;
            set {
                _hash = value;
                OnPropertyChanged("Hash");
            }
        }

        public virtual Image PreviewImage {
            get => _previewImage;
            set {
                _previewImage = value;
                OnPropertyChanged("PreviewImage");
                OnPropertyChanged("PreviewImageSource");
            }
        }

        public ImageSource PreviewImageSource {
            get => PreviewImage.Source;
            set {
                if (value == null) throw new ArgumentNullException(nameof(value));
                PreviewImageSource = value;
                OnPropertyChanged("PreviewImageSource");
            }
        }



        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
