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
    }
}
