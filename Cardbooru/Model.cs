using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace Cardbooru
{
     public class Model {
         private const int DefaultLimitForRequest = 10;
         private const string Danbooru = "https://danbooru.donmai.us";
         private HttpClient _client;

         public List<BooruImage> BooruImagesList { get; set; }

         public async Task<string> GetImages(int pageNum) {
             if (BooruImagesList == null)
                 BooruImagesList = new List<BooruImage>();

             var posts = await GetClient()
                 .GetStringAsync(Danbooru + $"/posts.json?limit={DefaultLimitForRequest}&page={pageNum}");

             BooruImagesList = JsonConvert.DeserializeObject<List<BooruImage>>(posts);

             return "okay";
         }

         private HttpClient GetClient() {
             if (_client == null) return new HttpClient();
             return _client;
         }

         public Task<ImageSource> GetPreviewImage(BooruImage imageClass) {
             //Check if image has been cached
             if (isHaveCache(imageClass.Hash))
                 return GetImageFromCache(imageClass.Hash + "_preview");
             //Caching image and save it
             return CacheAndReturnPreviewImage(imageClass.PreviewUrl, imageClass.Hash);
         }

         private bool isHaveCache(string hash) {
             return false;
         }

         private async Task<ImageSource> CacheAndReturnPreviewImage(string url, string name) {
             var bytesImage = await GetImageBytes(url);
             BitmapFrame bitmap;
             using (var mStream = new MemoryStream(bytesImage)) {
                 bitmap = BitmapFrame.Create(mStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
             }

             File.WriteAllBytes($"{GetImageCacheDir()}{name}" + "_preview", bytesImage);


             return bitmap;
         }

         private async Task<ImageSource> GetImageFromCache(string path) {
             byte[] buff;
             using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) {
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

         /// <param name="url">Without danbooru prefix</param>
         private async Task<byte[]> GetImageBytes(string url) {
             var bytes = await GetClient().GetByteArrayAsync(Danbooru + url);
             return bytes;
         }
     }
}
