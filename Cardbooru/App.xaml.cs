using System.Windows;

namespace Cardbooru
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            AppView app = new AppView();
            AppModelView contex = new AppModelView();
            app.DataContext = contex;
            app.Show();
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            Cardbooru.Properties.Settings.Default.Save();
        }
    }
}
