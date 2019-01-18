using System.IO;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Interfaces;
using Cardbooru.Application.Services;
using Cardbooru.BrowseImages;
using MvvmCross.Plugins.Messenger;
using Ninject;

namespace Cardbooru.Application.Helpers
{
    public static class Extensions
    {
        public static void ConfigureIoc(this IKernel kernel)
        {
            kernel.Bind<IMvxMessenger>()
                .To<MvxMessengerHub>()
                .InSingletonScope();

            kernel.Bind<IBooruHttpClient>()
                .To<SystemHttpClient>()
                .InTransientScope();

            kernel.Bind<PostFetcherServiceHelper>()
                .ToSelf()
                .InSingletonScope();

            kernel.Bind<IBitmapImageCreatorService>()
                .To<BitmapImageCreatorService>()
                .InSingletonScope();

            kernel.Bind<IImageCachingService>()
                .To<ImageCachingService>()
                .InSingletonScope();

            kernel.Bind<IImageFetcherService>()
                .To<ImageFetcherService>()
                .InSingletonScope();

            kernel.Bind<BrowseImagesViewModel>()
                .ToSelf();

            kernel.Bind<IPostFetcherService>()
                .To<PostFetcherService>()
                .InSingletonScope();

            kernel.Bind<IPostCollectionManager>()
                .To<BooruCollectionManager>()
                .InSingletonScope();

            kernel.Bind<IBooruConfiguration>()
                .To<JsonBooruConfiguration>()
                .InSingletonScope();
        }

        public static void EnsureCacheDirectoryCreated(this IBooruConfiguration configuration)
        {
            var cache = Path.Combine(Directory.GetCurrentDirectory(), configuration.CachePath);
            if (Directory.Exists(cache))
                return;
            Directory.CreateDirectory(cache);
        }
    }
}
