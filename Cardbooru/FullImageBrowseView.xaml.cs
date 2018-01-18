using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cardbooru
{
    /// <summary>
    /// Interaction logic for FullImageBrowseView.xaml
    /// </summary>
    public partial class FullImageBrowseView : UserControl
    {
        public FullImageBrowseView()
        {
            InitializeComponent();
        }

        private void FullImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var contex = DataContext as FullImageBrowsingViewModel;
            contex.CloseImageCommand.Execute(sender);
        }
    }
}
