using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cardbooru
{

    public partial class BrowseImageView : UserControl
    {

        public BrowseImageView()
        {
            InitializeComponent();      
        }

        private void Item_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            
            
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e) {
            var contex = DataContext as BrowseImagesViewModel;
            var list = sender as ListBoxItem;
            contex.OpenFullCommnad.Execute(list.Content);
        }
    }
}
