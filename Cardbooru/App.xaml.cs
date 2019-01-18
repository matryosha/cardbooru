using System.Windows;
using Cardbooru.Application.Helpers;
using Cardbooru.Application.Interfaces;
using Ninject;

namespace Cardbooru
{
    public partial class App : System.Windows.Application
    {
        private IKernel _iocContainer;
        private IBooruConfiguration _configuration;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _iocContainer = new StandardKernel();
            _iocContainer.ConfigureIoc();
            _configuration = _iocContainer.Get<IBooruConfiguration>();
            _configuration.EnsureCacheDirectoryCreated();

            Current.MainWindow = _iocContainer.Get<MainWindowView>();
            Current.MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            Cardbooru.Properties.Settings.Default.Save();
        }
      
    }
}
