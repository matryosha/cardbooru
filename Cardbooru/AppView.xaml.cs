using System.Windows;
using System.Windows.Input;

namespace Cardbooru
{

    public partial class AppView : Window
    {
        public AppView()
        {
            InitializeComponent(); 
        }

        private void CloseDrawerAfterClick(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }
    }
}
