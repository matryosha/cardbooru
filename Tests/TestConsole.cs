using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Cardbooru;

namespace Tests
{
    class TestConsole
    {
        static void Main(string[] args) {
            Model model = new Model();
            Task.WaitAll(model.GetImages(2));
            Console.WriteLine(model.BooruImagesList.Count);

            foreach (var booruImage in model.BooruImagesList) {
                Console.WriteLine($@"Id={booruImage.Id}");
                Console.WriteLine($@"Hash={booruImage.Hash}");
                Console.WriteLine($@"UrlPreview={booruImage.PreviewUrl}");
                Console.WriteLine("========================");
            }


            Console.ReadKey();
        }

    }
}
