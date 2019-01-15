using System.Windows;
using MvvmCross.Plugins.Messenger;
using Ninject;

namespace Cardbooru
{
    public partial class App : Application
    {
        private IKernel _iocContainer;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            _iocContainer = new StandardKernel();
            _iocContainer.Bind<IMvxMessenger>()
                .To<MvxMessengerHub>()
                .InSingletonScope();

            Current.MainWindow = _iocContainer.Get<MainWindowView>();
            Current.MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            Cardbooru.Properties.Settings.Default.Save();
        }
    }
}
