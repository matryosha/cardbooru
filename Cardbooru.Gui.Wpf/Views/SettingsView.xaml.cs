using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cardbooru.Gui.Wpf.ViewModels;
using Ookii.Dialogs.Wpf;

namespace Cardbooru.Gui.Wpf.Views
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

        private void UpdateSizeOfCache(object sender, MouseButtonEventArgs e)
        {
            SettingsViewModel context = DataContext as SettingsViewModel;
            context.UpdateSizeOfCache();
        }

        private void SettingsView_OnLoaded(object sender, RoutedEventArgs e)
        {
            SettingsViewModel context = DataContext as SettingsViewModel;
            context.UpdateValues();
        }
    }
}
