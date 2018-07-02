using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cardbooru.Models.Base
{
    public abstract class BooruImageModelBase :
        INotifyPropertyChanged
    {
        private string _hash;
        private BitmapImage _previewImage;

        public abstract string GetPostsUrl();
        public abstract string GetSiteUrl();
        //public abstract string GetRating();

        public virtual string TagsString { get; set; }
        public virtual string Rating { get; set; }
        public virtual string Id { get; set; }
        public virtual string PreviewImageUrl { get; set; }
        public virtual string FullImageUrl { get; set; }
        public virtual List<string> TagsList { get; set; } = new List<string>();
        
        public virtual BitmapImage FullImage { get; set; }
        private bool _isImageLoaded;

        public bool IsFullImageLoaded {
            get => _isImageLoaded;
            set {
                _isImageLoaded = value;
                OnPropertyChanged("IsFullImageLoaded");
            }
        }

        public virtual string Hash {
            get => _hash;
            set {
                _hash = value;
                OnPropertyChanged("Hash");
            }
        }

        public virtual BitmapImage PreviewImage {
            get => _previewImage;
            set {
                _previewImage = value;
                OnPropertyChanged("PreviewImage");
                OnPropertyChanged("PreviewImageSource");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
