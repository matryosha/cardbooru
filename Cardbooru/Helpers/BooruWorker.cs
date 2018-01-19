﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace Cardbooru
{
    internal enum ImageSizeType {
        Preview,
        Full
    }

    public class BooruWorker {
        private const int DefaultLimitForRequest = 100;
        private const string Danbooru = "https://danbooru.donmai.us";
        private HttpClient _client;
        private BitmapFrame _defaultImage;

        public async Task FillBooruImages(int pageNum, ObservableCollection<BooruImageModel> realBooruImages)
        {
            //Get json file with posts 
            var posts = await GetClient()
                .GetStringAsync(Danbooru + $"/posts.json?limit={DefaultLimitForRequest}&page={pageNum}");

            //Convert to collection
            var collection = JsonConvert.DeserializeObject<ObservableCollection<BooruImageModel>>(posts);

            //Load preview image in each boouruImageClass
            await LoadPreviewImages(collection, realBooruImages);
        }


        public async Task LoadFullImage(BooruImageModel booruImageModel) {
            if(booruImageModel == null)
                throw new Exception("no boouru image");

            ImageSource image;
            //Check if image has been cached
            if (IsHaveCache(booruImageModel.Hash, ImageSizeType.Full)) {
                image = await GetImageFromCache(booruImageModel.Hash, ImageSizeType.Full);
            }
            else {
                //Caching image and save it
                image = await CacheAndReturnImage(booruImageModel.FullUrl, booruImageModel.Hash, ImageSizeType.Full);
            }

            booruImageModel.FullImage = new Image();
            booruImageModel.FullImage.Source = image;
        }

        public async Task LoadPreviewImages(ObservableCollection<BooruImageModel> booruImagesMetaData, ObservableCollection<BooruImageModel> realBooruImages)
        {
            foreach (BooruImageModel booruImage in booruImagesMetaData)
            {
                //check for empty booru
                if (string.IsNullOrEmpty(booruImage.Hash)) continue;

                booruImage.PreviewImage = new Image();
                booruImage.PreviewImage.Source = await GetPreviewImage(booruImage);
                realBooruImages.Add(booruImage);
            }

        }

        private Task<ImageSource> GetPreviewImage(BooruImageModel imageModelClass) {
            //Check if image has been cached
            if (IsHaveCache(imageModelClass.Hash, ImageSizeType.Preview))
                return GetImageFromCache(imageModelClass.Hash, ImageSizeType.Preview);
            //Caching image and save it
            return CacheAndReturnImage(imageModelClass.PreviewUrl, imageModelClass.Hash, ImageSizeType.Preview);
        }

        private bool IsHaveCache(string path, ImageSizeType type) {
            if(type == ImageSizeType.Preview)
                return File.Exists(GetImageCacheDir() + path + "_preview");
            return File.Exists(GetImageCacheDir() + path + "_full");
        }

        private async Task<ImageSource> CacheAndReturnImage(string url, string inputPath, ImageSizeType type) {
            var properPath = GetProperPath(inputPath, type);
            var bytesImage = await GetImageBytes(url);
            BitmapFrame bitmap;
            try {
                using (var mStream = new MemoryStream(bytesImage))
                {
                    bitmap = BitmapFrame.Create(mStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                bitmap = null;
            }
            
            File.WriteAllBytes($"{GetImageCacheDir()}{properPath}", bytesImage);
            
            return bitmap;
        }

        private async Task<ImageSource> GetImageFromCache(string inputPath, ImageSizeType type) {
            if (inputPath == null) return null;

            byte[] buff;
            var properPath = GetImageCacheDir() + GetProperPath(inputPath, type);

            using (var file = new FileStream(properPath, FileMode.Open, FileAccess.Read, FileShare.Read,
                4096, true)) {
                buff = new byte[file.Length];
                await file.ReadAsync(buff, 0, (int) file.Length);
            }

            BitmapFrame bitmap;
            using (var mStream = new MemoryStream(buff)) {
                Console.WriteLine(properPath);
                bitmap = BitmapFrame.Create(mStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

            return bitmap;
        }

        private string GetImageCacheDir() {
            var path = "cache/image/";
            if (Directory.Exists(path))
                return path;
            Directory.CreateDirectory(path);
            return path;
        }

        private string GetProperPath(string input, ImageSizeType type) {
            if (type == ImageSizeType.Preview)
                return input + "_preview";
            return input + "_full";
        }

        /// <param name="url">Without danbooru prefix</param>
        private async Task<byte[]> GetImageBytes(string url) {
            var bytes = await GetClient().GetByteArrayAsync(Danbooru + url);
            return bytes;
        }

        private BitmapFrame LoadDefImage() {
            if (_defaultImage == null) {
                using (var fStream = File.OpenRead("res/default.jpg"))// ??????????????????????????????
                {
                    _defaultImage = BitmapFrame.Create(fStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            }
            return _defaultImage;
        }

        private HttpClient GetClient()
        {
            return _client ?? (_client = new HttpClient());
        }

    }
}
