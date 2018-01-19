using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cardbooru
{

    public partial class BrowseImageView : UserControl {

        public BrowseImageView()
        {
            InitializeComponent();      
        }

        private void Item_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            
            
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e) {
            var contex = DataContext as BrowseImagesViewModel;
            var listItem = sender as ListBoxItem;
            contex.SaveStateCommand.Execute(listItem.Content);
            contex.OpenFullCommnad.Execute(listItem.Content);
        }


        private void BrowseImageView_OnUnloaded(object sender, RoutedEventArgs e) {

        }


        private void BrowseImageView_OnLoaded(object sender, RoutedEventArgs e) {
            if(mainListBox.Items.Count == 0) return;
            var contex = DataContext as BrowseImagesViewModel;
            var item = contex.CurrentOpenedItemState as BooruImageModel;
            mainListBox.ScrollIntoView(item);
        }
    }
}
