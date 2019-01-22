using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Infrastructure;
using Cardbooru.Domain.Entities;

namespace Cardbooru.Gui.Wpf.Helpers
{
    //[Obsolete]
    //public static class BooruWorker {
    //    private static int NumberOfPicPerRequest = Properties.Settings.Default.NumberOfPicPerRequest;
    //    private static BooruPost _baseModel;
    //    private static HttpClient _client;
    //    private static BitmapFrame _defaultImage;
    //    //private static int _countOfAddedPicsPerRequest;

    //    public static async Task FillBooruImages(PageNumberKeeper pageKeeper, 
    //        ObservableCollection<BooruPost> realBooruImages, 
    //        BooruSiteType booruSiteType, 
    //        CancellationToken cancellationToken)
    //    {
    //        QueryPageCheck(pageKeeper.NextQueriedPage, booruSiteType);
    //        if (cancellationToken.IsCancellationRequested)
    //        {
    //            cancellationToken.ThrowIfCancellationRequested();
    //            GetClient().CancelPendingRequests();
    //        }

    //        _baseModel = null;
    //        //_countOfAddedPicsPerRequest = 0;

    //        switch (booruSiteType)
    //        {
    //            case BooruSiteType.Danbooru:
    //                _baseModel = new DanbooruImageModel();
    //                break;
    //            case BooruSiteType.SafeBooru:
    //                _baseModel = new SafebooruImageModel();
    //                break;
    //            case BooruSiteType.Gelbooru:
    //                _baseModel = new GelbooruImageModel();
    //                break;
    //        }

    //        //Get json file with posts
    //        string posts = String.Empty;
    //        posts = await GetClient()
    //            .GetStringAsync(_baseModel.GetSiteUrl() +
    //                            GetConverter.GetPosts(_baseModel.GetPostsUrl(), NumberOfPicPerRequest, pageKeeper.NextQueriedPage));

    //        //Create metadata collection 
    //        var collection = DeserializePostsToCollection(booruSiteType, posts);

    //        collection = FillTagsList(collection, cancellationToken);
    //        //collection = SafeSearch(collection);

            
    //        //Download preview image and add with all metadata to realBooruImage collection
    //        await LoadPreviewImages(collection, realBooruImages, cancellationToken,pageKeeper);

    //        pageKeeper.QueriedPagesCount++;
    //        pageKeeper.NextQueriedPage++;
    //    }

    //    private static ObservableCollection<BooruPost> SafeSearch(ObservableCollection<BooruPost> collection)
    //    {
    //        var outCollection = new ObservableCollection<BooruPost>();
    //        List<String> pattern = new List<string>
    //        {
    //            "s",
    //            "e",
    //            "q",
    //            "u"
    //        };
    //        if (Properties.Settings.Default.SafeCheck)
    //            pattern.Remove("s");
    //        if (Properties.Settings.Default.ExplicitCheck)
    //            pattern.Remove("e");
    //        if (Properties.Settings.Default.QuestionableCheck)
    //            pattern.Remove("q");
    //        if (Properties.Settings.Default.UndefinedCheck)
    //            pattern.Remove("u");

    //        foreach (BooruPost modelBase in collection)
    //        {
    //            var patternFailed = false;
    //            foreach (string s in pattern)
    //            {
    //                if (modelBase.Rating == s)
    //                {
    //                    patternFailed = true;
    //                    break;
    //                }
    //            }
    //            if (patternFailed) continue;
    //            outCollection.Add(modelBase);
    //            }

    //        return outCollection;
    //    }
    //    private static ObservableCollection<BooruPost> FillTagsList(ObservableCollection<BooruPost> collection, CancellationToken cancellationToken)
    //    {
    //        StringCollection filteredTags = Properties.Settings.Default.BlackListTags;
    //        ObservableCollection<BooruPost> outCollection = new ObservableCollection<BooruPost>();
    //        string[] desiredTags = {};

    //        foreach (var booruImageModelBase in collection)
    //        {
    //            if (cancellationToken.IsCancellationRequested)
    //            {
    //                cancellationToken.ThrowIfCancellationRequested();
    //                GetClient().CancelPendingRequests();
    //            }

    //            var tagsArr = booruImageModelBase.TagsString.Split(' ');
    //            booruImageModelBase.TagsString = null;
    //            bool filterTagSpotted = false;


    //            int counterOfDesiredTags = 0;

    //            foreach (var tag in desiredTags)
    //            {
    //                if (tagsArr.Contains(tag))
    //                    counterOfDesiredTags++;
    //            }
    //            if(counterOfDesiredTags != desiredTags.Length) continue;
    //            foreach (string s in tagsArr)
    //            {
    //                //images will be removed if blacklisted tag spotted
    //                foreach (string blackTag in filteredTags)
    //                {
    //                    if (filteredTags.Count == 0) break;
    //                    if (s == blackTag)
    //                    {
    //                        filterTagSpotted = true;
    //                        break;
    //                    }
    //                }



    //                if(filterTagSpotted) break;

    //                booruImageModelBase.TagsList.Add(s);
    //            }
    //            if(filterTagSpotted) continue;
    //            outCollection.Add(booruImageModelBase);
    //            }

    //        return outCollection;
    //    }

    //    public static async Task LoadFullImage(BooruPost booruImageModel, CancellationToken cancellationToken) {
    //        if(booruImageModel == null)
    //            throw new Exception("no boouru image when trying to load full image");
    //        if(booruImageModel.FullImage!=null)
    //            return;

    //        ImageSource image;
    //        //Check if image has been cached
    //        if (IsHasCache(booruImageModel.Hash, ImageSizeType.Full)) {
    //            image = await GetImageFromCache(booruImageModel.Hash, ImageSizeType.Full, cancellationToken);
    //        }
    //        else {
    //            //Caching image and save it
    //            image = await CacheAndReturnImage(booruImageModel.FullImageUrl, booruImageModel.Hash, ImageSizeType.Full, cancellationToken);
    //        }

    //        booruImageModel.FullImage = (BitmapImage) image;
    //        booruImageModel.IsFullImageLoaded = true;
    //    }

    //    private static async Task LoadPreviewImages(ObservableCollection<BooruPost> booruImagesMetaData,
    //        ObservableCollection<BooruPost> realBooruImages, CancellationToken cancellationToken, PageNumberKeeper pageKeeper)
    //    {
    //        foreach (BooruPost booruImage in booruImagesMetaData)
    //        {
    //            if (cancellationToken.IsCancellationRequested)
    //            {
    //                cancellationToken.ThrowIfCancellationRequested();
    //                GetClient().CancelPendingRequests();
    //            }

    //            //check for empty booru
    //            if (string.IsNullOrEmpty(booruImage.Hash)) continue;

    //            booruImage.PreviewImage = await DownloadPreviewImage(booruImage, cancellationToken) as BitmapImage;

    //        if (booruImage.PreviewImage == null) {
    //                await LoadFullImage(booruImage, cancellationToken);
    //                if (booruImage.FullImage == null) {
    //                    //booruImage.FullImage = new Image();
    //                    //booruImage.PreviewImage = booruImage.FullImage = LoadDefImage();
    //                }
    //                booruImage.PreviewImage = booruImage.FullImage;
    //            }

    //            realBooruImages.Add(booruImage);
    //        }

    //        pageKeeper.AddedImagesCount = realBooruImages.Count;

    //    }

    //    private static ObservableCollection<BooruPost> DeserializePostsToCollection(BooruSiteType type, string posts)
    //    {
    //        switch (type)
    //        {
    //            case BooruSiteType.Danbooru:
    //                return new ObservableCollection<BooruPost>(JsonConvert.DeserializeObject<ObservableCollection<DanbooruImageModel>>(posts));
    //            case BooruSiteType.SafeBooru:
    //                return new ObservableCollection<BooruPost>(JsonConvert.DeserializeObject<ObservableCollection<SafebooruImageModel>>(posts));
    //            case BooruSiteType.Gelbooru:
    //                return new ObservableCollection<BooruPost>(JsonConvert.DeserializeObject<ObservableCollection<GelbooruImageModel>>(posts));
    //        }

    //        throw new Exception("Unknown booru type for deserialize");
    //    }
    //    private static Task<ImageSource> DownloadPreviewImage(BooruPost imageModelClass, CancellationToken cancellationToken) {
    //        //Check if image has been cached
    //        if (IsHasCache(imageModelClass.Hash, ImageSizeType.Preview))
    //            return GetImageFromCache(imageModelClass.Hash, ImageSizeType.Preview, cancellationToken);
    //        //Caching image and save it
    //        return CacheAndReturnImage(imageModelClass.PreviewImageUrl, imageModelClass.Hash, ImageSizeType.Preview, cancellationToken);
    //    }

    //    private static bool IsHasCache(string path, ImageSizeType type) {
    //        if(type == ImageSizeType.Preview)
    //            return File.Exists(GetImageCacheDir() + path + "_preview");
    //        return File.Exists(GetImageCacheDir() + path + "_full");
    //    }

    //    private static async Task<ImageSource> CacheAndReturnImage(string url, string inputPath, ImageSizeType type, CancellationToken cancellationToken) {
    //        if (cancellationToken.IsCancellationRequested)
    //        {
    //            cancellationToken.ThrowIfCancellationRequested();
    //        }
    //        var properPath = GetProperPath(inputPath, type);
    //        var bytesImage = await GetImageBytes(url, cancellationToken);
    //        if (bytesImage == null) return null;
    //        BitmapSource bitmap =  await Task.Run(() => CreateBitmapFrame(bytesImage), cancellationToken);
            
    //        using (FileStream stream = File.Open($"{GetImageCacheDir()}{properPath}", FileMode.OpenOrCreate)) {
    //            //stream.Seek(0, SeekOrigin.End);
    //            await stream.WriteAsync(bytesImage, 0, bytesImage.Length, cancellationToken);
    //        }

    //        return bitmap;
    //    }

    //    private static async Task<ImageSource> GetImageFromCache(string inputPath, ImageSizeType type, CancellationToken cancellationToken) {
    //        if (cancellationToken.IsCancellationRequested)
    //        {
    //            cancellationToken.ThrowIfCancellationRequested();
    //        }
    //        if (inputPath == null) return null;

    //        byte[] buff;
    //        var properPath = GetImageCacheDir() + GetProperPath(inputPath, type);
    //        using (var file = new FileStream(properPath, FileMode.Open, FileAccess.Read, FileShare.Read,
    //            4096, true)) {
    //            buff = new byte[file.Length];
    //            await file.ReadAsync(buff, 0, (int) file.Length, cancellationToken);
    //        }

    //        BitmapSource bitmap = await Task.Run((() => CreateBitmapFrame(buff)), cancellationToken);
            

    //        return bitmap;
    //    }

    //    private static string GetImageCacheDir() {
    //        var path = Properties.Settings.Default.PathToCacheFolder;
    //        if (Directory.Exists(path))
    //            return path;
    //        Directory.CreateDirectory(path);
    //        return path;
    //    }

    //    private static string GetProperPath(string input, ImageSizeType type) {
    //        if (type == ImageSizeType.Preview)
    //            return input + "_preview";
    //        return input + "_full";
    //    }

    //    private static async Task<byte[]> GetImageBytes(string url, CancellationToken cancellationToken) {
    //        if (cancellationToken.IsCancellationRequested)
    //        {
    //            cancellationToken.ThrowIfCancellationRequested();
    //            GetClient().CancelPendingRequests();
    //        }
    //        ////////////////////////////////////////
    //        //await Task.Delay(1000);
    //        ////////////////////////////////////////
    //        byte[] bytes;
    //        try
    //        {
    //             bytes = await GetClient().GetByteArrayAsync(url);
    //        }
    //        catch(HttpRequestException e)
    //        {
    //            DownloadImageUrlCheck(url, e);
    //            return null;
    //        }
    //        catch (ArgumentException e)
    //        {
    //            DownloadImageUrlCheck(url, e);
    //            return null;
    //        }
    //        return bytes;
            
    //    }

    //    private static BitmapFrame LoadDefImage() {
    //        if (_defaultImage == null) {
    //            using (var fStream = File.OpenRead("res/default.jpg"))// ??????????????????????????????
    //            {
    //                _defaultImage = BitmapFrame.Create(fStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
    //            }
    //        }
    //        return _defaultImage;
    //    }

    //    private static HttpClient GetClient()
    //    {
    //        return _client ?? (_client = new HttpClient());
    //    }

    //    private static BitmapSource CreateBitmapFrame(byte[] data)
    //    {
    //        BitmapImage image;
    //        try
    //        {
    //            // early I created BitmapFrame but it appers to consuming REALLY a lot of memory (about 1gig after loading 400 images)
    //            // So it sucks
    //            //   bitmap = BitmapFrame.Create(wpapper, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

    //            using (var wpapper = new WrappingStream(new MemoryStream(data)))
    //            {
    //                image = new BitmapImage();
    //                image.BeginInit();
    //                image.CacheOption = BitmapCacheOption.OnLoad;
    //                image.StreamSource = wpapper;
    //                image.EndInit();
    //                image.Freeze();
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            image = null;
    //        }
    //        return image;
    //    }
    //    [Conditional("DEBUG")]
    //    private static void DownloadImageUrlCheck(string url, Exception e)
    //    {
    //        Console.WriteLine($"Image with URL failed to download. {url}");
    //    }

    //    [Conditional("DEBUG")]
    //    private static void QueryPageCheck(int pageNum, BooruSiteType booruSiteType)
    //    {
    //        Console.WriteLine($"Quering {pageNum} page from {booruSiteType}");
    //    }
    //}
}
