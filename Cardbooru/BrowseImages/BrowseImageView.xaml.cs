using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cardbooru.Models.Base;

namespace Cardbooru.BrowseImages
{
    public partial class BrowseImageView : UserControl {

        private const double MaxImageWidth = 300;
        private const double MinImageWidth = 250;

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

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            if (double.IsNaN(ListBoxColumn.ActualWidth)) return;
            var widthOfListBox = ListBoxColumn.ActualWidth;
            var capacityOfMaxImageSize = (int) widthOfListBox / (int) MaxImageWidth;

            double newImageSize = widthOfListBox / capacityOfMaxImageSize;
            while (newImageSize > MaxImageWidth)
                newImageSize = widthOfListBox / ++capacityOfMaxImageSize;
            Resources["ImageItemHeight"] = Resources["ImageItemWidth"] = newImageSize - 40;

        }

    }
}
