using System.Windows;
using MvvmCross.Plugins.Messenger;
using Ninject;

namespace Cardbooru
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            IKernel kernel = new StandardKernel();
            kernel.Bind<IMvxMessenger>().To<MvxMessengerHub>();

            MainWindowView mainWindow = new MainWindowView();
            MainWindowModelView context = kernel.Get<MainWindowModelView>();
            mainWindow.DataContext = context;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            Cardbooru.Properties.Settings.Default.Save();
        }
    }
}
