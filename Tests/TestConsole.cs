using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Cardbooru;
using Cardbooru.Helpers;

namespace Tests
{
    class TestConsole
    {
        static void Main(string[] args) {
            BooruWorker _booruWorker = new BooruWorker();
            Task.WaitAll(_booruWorker.FillBooruImages(2));
            Console.WriteLine(_booruWorker.BooruImages.Count);

            foreach (var booruImage in _booruWorker.BooruImages) {
                Console.WriteLine($@"Id={booruImage.Id}");
                Console.WriteLine($@"Hash={booruImage.Hash}");
                Console.WriteLine($@"UrlPreview={booruImage.PreviewUrl}");
                Console.WriteLine("========================");
            }


            Console.ReadKey();
        }

    }
}
