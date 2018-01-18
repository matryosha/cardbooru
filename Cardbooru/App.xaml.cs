using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cardbooru.Bootstrap;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            //Mvx.LazyConstructAndRegisterSingleton<IMvxMessenger, MvxMessengerHub>();
            //var msg = new MvxMessengerHub();

            AppView app = new AppView();
            AppModelView contex = new AppModelView();
            app.DataContext = contex;
            app.Show();
        }
        
        
    }
}
