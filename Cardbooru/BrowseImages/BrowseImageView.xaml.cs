using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cardbooru.Models.Base;

namespace Cardbooru.BrowseImages
{

    public partial class BrowseImageView : UserControl {

        public BrowseImageView()
        {
            InitializeComponent();      
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e) {
            var contex = DataContext as BrowseImagesViewModel;
            var listItem = sender as ListBoxItem;
            contex.SaveStateCommand.Execute(listItem.Content);
            contex.OpenFullCommnad.Execute(listItem.Content);
        }

        private void BrowseImageView_OnLoaded(object sender, RoutedEventArgs e) {
            if(mainListBox.Items.Count == 0) return;
            var contex = DataContext as BrowseImagesViewModel;
            var item = contex.CurrentOpenedItemState as BooruImageModelBase;
            mainListBox.ScrollIntoView(item);
        }
    }
}
