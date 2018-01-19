using System.Windows.Controls;
using System.Windows.Input;

namespace Cardbooru
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
    }
}
