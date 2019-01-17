using System.IO;
using System.Windows;
using Cardbooru.Application;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Helpers;
using Cardbooru.Application.Interfaces;
using Cardbooru.Application.Services;
using Cardbooru.BrowseImages;
using MvvmCross.Plugins.Messenger;
using Newtonsoft.Json;
using Ninject;

namespace Cardbooru
{
    public partial class App : System.Windows.Application
    {
        private IKernel _iocContainer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _iocContainer = new StandardKernel();
            LoadConfiguration();

            ConfigureIocContainer();

            Current.MainWindow = _iocContainer.Get<MainWindowView>();
            Current.MainWindow.Show();
        }

        

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            Cardbooru.Properties.Settings.Default.Save();
        }

        private void LoadConfiguration()
        {
            var configuration = JsonConvert
                .DeserializeObject<RootConfiguration>(
                    File.ReadAllText(Path.Combine(
                        Directory.GetCurrentDirectory(), 
                        "AppSettings.json")));
            _iocContainer.Bind<RootConfiguration>().ToConstant(configuration);

            EnsureCacheDirectoryCreated(configuration.CachePath);

        }

        private void EnsureCacheDirectoryCreated(string cachePathConf)
        {
            var cache = Path.Combine(Directory.GetCurrentDirectory(), cachePathConf);
            if (Directory.Exists(cache))
                return;
            Directory.CreateDirectory(cache);
        }

        private void ConfigureIocContainer()
        {
            _iocContainer.Bind<IMvxMessenger>()
                .To<MvxMessengerHub>()
                .InSingletonScope();

            _iocContainer.Bind<IBooruHttpClient>()
                .To<SystemHttpClient>()
                .InTransientScope();

            _iocContainer.Bind<PostFetcherServiceHelper>()
                .ToSelf()
                .InSingletonScope();

            _iocContainer.Bind<IBitmapImageCreatorService>()
                .To<BitmapImageCreatorService>()
                .InSingletonScope();

            _iocContainer.Bind<IImageCachingService>()
                .To<ImageCachingService>()
                .InSingletonScope();

            _iocContainer.Bind<IImageFetcherService>()
                .To<ImageFetcherService>()
                .InSingletonScope();

            _iocContainer.Bind<BrowseImagesViewModel>()
                .ToSelf();

            _iocContainer.Bind<IPostFetcherService>()
                .To<PostFetcherService>()
                .InSingletonScope();

            _iocContainer.Bind<IPostCollectionManager>()
                .To<BooruCollectionManager>()
                .InSingletonScope();
        }
    }
}
