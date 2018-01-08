﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace Cardbooru {
    internal enum ImageSizeType {
        Preview,
        Full
    }

    public class Model {
        private const int DefaultLimitForRequest = 5;
        private const string Danbooru = "https://danbooru.donmai.us";
        private HttpClient _client;

        private ObservableCollection<BooruImage> booruImagesList;

        public ObservableCollection<BooruImage> BooruImagesList => booruImagesList ??
                                                                   (booruImagesList =
                                                                       new ObservableCollection<BooruImage>());

        public async Task<string> GetImages(int pageNum)
        {

            var posts = await GetClient()
                .GetStringAsync(Danbooru + $"/posts.json?limit={DefaultLimitForRequest}&page={pageNum}");

            var collection = JsonConvert.DeserializeObject<ObservableCollection<BooruImage>>(posts);
            await LoadPreviewImages(collection);

            foreach (var booruImage in collection) {
                BooruImagesList.Add(booruImage);
                await Task.Delay(1000);
            }
            

            return "okay";
        }

        private async Task LoadPreviewImages(ObservableCollection<BooruImage> list)
        {
            foreach (BooruImage booruImage in list)
            {
                booruImage.PreviewImage = new Image();
                booruImage.PreviewImage.Source = await GetPreviewImage(booruImage);
            }
        }

        private HttpClient GetClient() {
            if (_client == null) return new HttpClient();
            return _client;
        }

        public Task<ImageSource> GetPreviewImage(BooruImage imageClass) {
            //Check if image has been cached
            if (IsHaveCache(imageClass.Hash))
                return GetImageFromCache(imageClass.Hash, ImageSizeType.Preview);
            //Caching image and save it
            return CacheAndReturnImage(imageClass.PreviewUrl, imageClass.Hash, ImageSizeType.Preview);
        }

        private bool IsHaveCache(string path) {
            return File.Exists(GetImageCacheDir() + path);
        }

        private async Task<ImageSource> CacheAndReturnImage(string url, string inputPath, ImageSizeType type) {
            var properPath = GetProperPath(inputPath, type);
            var bytesImage = await GetImageBytes(url);
            BitmapFrame bitmap;
            using (var mStream = new MemoryStream(bytesImage)) {
                bitmap = BitmapFrame.Create(mStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

            File.WriteAllBytes($"{GetImageCacheDir()}{properPath}", bytesImage);


            return bitmap;
        }

        private async Task<ImageSource> GetImageFromCache(string inputPath, ImageSizeType type) {
            byte[] buff;
            var properPath = GetProperPath(inputPath, type);

            using (var file = new FileStream(properPath, FileMode.Open, FileAccess.Read, FileShare.Read,
                4096, true)) {
                buff = new byte[file.Length];
                await file.ReadAsync(buff, 0, (int) file.Length);
            }

            BitmapFrame bitmap;
            using (var mStream = new MemoryStream(buff)) {
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
    }
}
