using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cardbooru.FullImageBrowsing
{
    public partial class FullImageBrowsingView : UserControl
    {
        public FullImageBrowsingView()
        {
            InitializeComponent();
        }

        private void FullImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var contex = DataContext as FullImageBrowsingViewModel;
            contex.CloseImageCommand.Execute(sender);
        }

        private void FullImage_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            if (TagsPanel.IsVisible)
                TagsPanel.Visibility = Visibility.Hidden;
            else {
                TagsPanel.Visibility = Visibility.Visible;

            }
        }
    }
}
