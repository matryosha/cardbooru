using System.IO;
using System.Threading.Tasks;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;
using Newtonsoft.Json;
using Ninject;

namespace Cardbooru.Application.Configurations
{
    public class JsonBooruConfiguration : IBooruConfiguration
    {
        public FetchConfiguration FetchConfiguration { get; set; }
        public string CachePath { get; set; }
        public BooruSiteType ActiveSite { get; set; }
        public bool ImageCaching { get; set; }

        public JsonBooruConfiguration() { }

        public JsonBooruConfiguration(IKernel ioc)
        {
            var configuration = JsonConvert
                .DeserializeObject<JsonBooruConfiguration>(
                    File.ReadAllText(Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "AppSettings.json")));

            DuckCopyShallow(this, configuration);
        }
        
        public Task SaveConfiguration()
        {
            var settings = JsonConvert.SerializeObject(this, Formatting.Indented);
            System.IO.File.WriteAllText(Path.Combine(
                Directory.GetCurrentDirectory(),
                "AppSettings.json"), settings);
            return Task.CompletedTask;
        }

        private static void DuckCopyShallow(JsonBooruConfiguration dst, JsonBooruConfiguration src)
        {
            var t = typeof(JsonBooruConfiguration);
            foreach (var f in t.GetFields())
            {
                var dstF = t.GetField(f.Name);
                if (dstF == null)
                    continue;
                dstF.SetValue(dst, f.GetValue(src));
            }

            foreach (var f in t.GetProperties())
            {
                var dstF = t.GetProperty(f.Name);
                if (dstF == null)
                    continue;

                dstF.SetValue(dst, f.GetValue(src, null), null);
            }
        }
    }
}