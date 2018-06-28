using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru.Helpers.Base;
using Cardbooru.Models;
using Cardbooru.Models.Base;
using Newtonsoft.Json;

namespace Cardbooru.Helpers
{
    internal enum ImageSizeType {
        Preview,
        Full
    }

    public enum BooruType
    {
        Danbooru,
        SafeBooru,
        Gelbooru
    }


    public static class BooruWorker {
        private static int NumberOfPicPerRequest = Properties.Settings.Default.NumberOfPicPerRequest;
        private static BooruImageModelBase _baseModel;
        private static HttpClient _client;
        private static BitmapFrame _defaultImage;
        private static int _countOfAddedPicsPerRequest;

        public static async Task<int> FillBooruImages(int pageNum, ObservableCollection<BooruImageModelBase> realBooruImages, BooruType booruType)
        {
            _baseModel = null;
            _countOfAddedPicsPerRequest = 0;

            switch (booruType)
            {
                case BooruType.Danbooru:
                    _baseModel = new DanbooruImageModel();
                    break;
                case BooruType.SafeBooru:
                    _baseModel = new SafebooruImageModel();
                    break;
                case BooruType.Gelbooru:
                    _baseModel = new GelbooruImageModel();
                    break;
            }

            //Get json file with posts
            string posts = String.Empty;
            posts = await GetClient()
                .GetStringAsync(_baseModel.GetSiteUrl() +
                                GetConverter.GetPosts(_baseModel.GetPostsUrl(), NumberOfPicPerRequest, pageNum));


            //Create metadata collection 
            var collection = DeserializePostsToCollection(booruType, posts);

            collection = FillTagsList(collection);
            //collection = SafeSearch(collection);

            _countOfAddedPicsPerRequest = collection.Count;
            //Download preview image and add with all metadata to realBooruImage collection
            await LoadPreviewImages(collection, realBooruImages);

            return _countOfAddedPicsPerRequest;
        }

        private static ObservableCollection<BooruImageModelBase> SafeSearch(ObservableCollection<BooruImageModelBase> collection)
        {
            var outCollection = new ObservableCollection<BooruImageModelBase>();
            List<String> pattern = new List<string>
            {
                "s",
                "e",
                "q",
                "u"
            };
            if (Properties.Settings.Default.SafeCheck)
                pattern.Remove("s");
            if (Properties.Settings.Default.ExplicitCheck)
                pattern.Remove("e");
            if (Properties.Settings.Default.QuestionableCheck)
                pattern.Remove("q");
            if (Properties.Settings.Default.UndefinedCheck)
                pattern.Remove("u");

            foreach (BooruImageModelBase modelBase in collection)
            {
                var patternFailed = false;
                foreach (string s in pattern)
                {
                    if (modelBase.Rating == s)
                    {
                        patternFailed = true;
                        break;
                    }
                }
                if (patternFailed) continue;
                outCollection.Add(modelBase);
                }

            return outCollection;
        }
        private static ObservableCollection<BooruImageModelBase> FillTagsList(ObservableCollection<BooruImageModelBase> collection)
        {
            StringCollection filteredTags = Properties.Settings.Default.BlackListTags;
            ObservableCollection<BooruImageModelBase> outCollection = new ObservableCollection<BooruImageModelBase>();
            string[] desiredTags = {};

            foreach (var booruImageModelBase in collection)
            {
                var tagsArr = booruImageModelBase.TagsString.Split(' ');
                booruImageModelBase.TagsString = null;
                bool filterTagSpotted = false;


                int counterOfDesiredTags = 0;

                foreach (var tag in desiredTags)
                {
                    if (tagsArr.Contains(tag))
                        counterOfDesiredTags++;
                }
                if(counterOfDesiredTags != desiredTags.Length) continue;
                foreach (string s in tagsArr)
                {
                    //images will be removed if blacklisted tag spotted
                    foreach (string blackTag in filteredTags)
                    {
                        if (filteredTags.Count == 0) break;
                        if (s == blackTag)
                        {
                            filterTagSpotted = true;
                            break;
                        }
                    }



                    if(filterTagSpotted) break;

                    booruImageModelBase.TagsList.Add(s);
                }
                if(filterTagSpotted) continue;
                outCollection.Add(booruImageModelBase);
                }

            return outCollection;
        }

        public static async Task LoadFullImage(BooruImageModelBase booruImageModel) {
            if(booruImageModel == null)
                throw new Exception("no boouru image");
            if(booruImageModel.FullImage!=null)
                return;

            ImageSource image;
            //Check if image has been cached
            if (IsHasCache(booruImageModel.Hash, ImageSizeType.Full)) {
                image = await GetImageFromCache(booruImageModel.Hash, ImageSizeType.Full);
            }
            else {
                //Caching image and save it
                image = await CacheAndReturnImage(booruImageModel.FullImageUrl, booruImageModel.Hash, ImageSizeType.Full);
            }

            booruImageModel.FullImage = new Image();
            booruImageModel.FullImage.Source = image;
            booruImageModel.IsFullImageLoaded = true;
        }

        private static async Task LoadPreviewImages(ObservableCollection<BooruImageModelBase> booruImagesMetaData, ObservableCollection<BooruImageModelBase> realBooruImages)
        {
            foreach (BooruImageModelBase booruImage in booruImagesMetaData)
            {
                //check for empty booru
                if (string.IsNullOrEmpty(booruImage.Hash)) continue;

                booruImage.PreviewImage = new Image();
                booruImage.PreviewImage.Source = await DownloadPreviewImage(booruImage);
                if (booruImage.PreviewImage.Source == null) {
                    await LoadFullImage(booruImage);
                    if (booruImage.FullImage == null) {
                        booruImage.FullImage = new Image();
                        booruImage.PreviewImage.Source = booruImage.FullImage.Source = LoadDefImage();
                    }
                    booruImage.PreviewImage.Source = booruImage.FullImage.Source;
                }
                realBooruImages.Add(booruImage);
            }

        }

        private static ObservableCollection<BooruImageModelBase> DeserializePostsToCollection(BooruType type, string posts)
        {
            switch (type)
            {
                case BooruType.Danbooru:
                    return new ObservableCollection<BooruImageModelBase>(JsonConvert.DeserializeObject<ObservableCollection<DanbooruImageModel>>(posts));
                case BooruType.SafeBooru:
                    return new ObservableCollection<BooruImageModelBase>(JsonConvert.DeserializeObject<ObservableCollection<SafebooruImageModel>>(posts));
                case BooruType.Gelbooru:
                    return new ObservableCollection<BooruImageModelBase>(JsonConvert.DeserializeObject<ObservableCollection<GelbooruImageModel>>(posts));
            }

            throw new Exception("Unknown booru type");
        }
        private static Task<ImageSource> DownloadPreviewImage(BooruImageModelBase imageModelClass) {
            //Check if image has been cached
            if (IsHasCache(imageModelClass.Hash, ImageSizeType.Preview))
                return GetImageFromCache(imageModelClass.Hash, ImageSizeType.Preview);
            //Caching image and save it
            return CacheAndReturnImage(imageModelClass.PreviewImageUrl, imageModelClass.Hash, ImageSizeType.Preview);
        }

        private static bool IsHasCache(string path, ImageSizeType type) {
            if(type == ImageSizeType.Preview)
                return File.Exists(GetImageCacheDir() + path + "_preview");
            return File.Exists(GetImageCacheDir() + path + "_full");
        }

        private static async Task<ImageSource> CacheAndReturnImage(string url, string inputPath, ImageSizeType type) {
            var properPath = GetProperPath(inputPath, type);
            var bytesImage = await GetImageBytes(url);
            if (bytesImage == null) return null;
            BitmapSource bitmap =  await Task.Run(() => CreateBitmapFrame(bytesImage));
            
            using (FileStream stream = File.Open($"{GetImageCacheDir()}{properPath}", FileMode.OpenOrCreate)) {
                //stream.Seek(0, SeekOrigin.End);
                await stream.WriteAsync(bytesImage, 0, bytesImage.Length);
            }

            return bitmap;
        }

        private static async Task<ImageSource> GetImageFromCache(string inputPath, ImageSizeType type) {
            if (inputPath == null) return null;

            byte[] buff;
            var properPath = GetImageCacheDir() + GetProperPath(inputPath, type);

            using (var file = new FileStream(properPath, FileMode.Open, FileAccess.Read, FileShare.Read,
                4096, true)) {
                buff = new byte[file.Length];
                await file.ReadAsync(buff, 0, (int) file.Length);
            }

            BitmapSource bitmap = await Task.Run((() => CreateBitmapFrame(buff)));
            

            return bitmap;
        }

        private static string GetImageCacheDir() {
            var path = Properties.Settings.Default.PathToCacheFolder;
            if (Directory.Exists(path))
                return path;
            Directory.CreateDirectory(path);
            return path;
        }

        private static string GetProperPath(string input, ImageSizeType type) {
            if (type == ImageSizeType.Preview)
                return input + "_preview";
            return input + "_full";
        }

        
        private static async Task<byte[]> GetImageBytes(string url) {
            byte[] bytes;
            try
            {
                 bytes = await GetClient().GetByteArrayAsync(url);
            }
            catch(Exception e)
            {
                DownloadImageUrlCheck(url, e);
                return null;
            }
            return bytes;
            
        }

        private static BitmapFrame LoadDefImage() {
            if (_defaultImage == null) {
                using (var fStream = File.OpenRead("res/default.jpg"))// ??????????????????????????????
                {
                    _defaultImage = BitmapFrame.Create(fStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }
            return _defaultImage;
        }

        private static HttpClient GetClient()
        {
            return _client ?? (_client = new HttpClient());
        }

        private static BitmapSource CreateBitmapFrame(byte[] data)
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
                image = null;
            }
            return image;
        }
        [Conditional("DEBUG")]
        private static void DownloadImageUrlCheck(string url, Exception e)
        {
            Console.WriteLine($"Image with URL failed to download. {url}");
        }
    }
}
