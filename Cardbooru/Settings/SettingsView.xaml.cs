using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;

namespace Cardbooru.Settings
{

    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ChangeCacheDir(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog  dialog = new VistaFolderBrowserDialog();
            dialog.ShowDialog();
            var path = dialog.SelectedPath;
            if(string.IsNullOrEmpty(path)) return;
            SettingsViewModel context  = DataContext as SettingsViewModel;
            context.ChangeCacheDir(path);
        }
    }
}
