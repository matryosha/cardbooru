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

        public abstract string GetPostsUrl();
        public abstract string GetSiteUrl();
        //public abstract string GetRating();

        public virtual string TagsString { get; set; }
        public virtual string Rating { get; set; }
        public virtual string Id { get; set; }
        public virtual string PreviewImageUrl { get; set; }
        public virtual string FullImageUrl { get; set; }
        public virtual List<string> TagsList { get; set; } = new List<string>();
        
        public virtual Image FullImage { get; set; }
        public ImageSource FullImageSource {
            get => FullImage.Source;
            set {
                FullImageSource = value;
                OnPropertyChanged("FullImageSource");
            }
        }
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
                PreviewImageSource = value ?? throw new ArgumentNullException("Preview Image source is null");
                OnPropertyChanged("PreviewImageSource");
            }
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
