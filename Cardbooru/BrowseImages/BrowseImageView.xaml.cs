using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cardbooru.Models.Base;

namespace Cardbooru.BrowseImages
{
    public partial class BrowseImageView : UserControl {

        private const double MaxImageWidth = 320;
        private const double MinImageWidth = 250;

        public BrowseImageView()
        {
            InitializeComponent();   
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e) {
            var contex = DataContext as BrowseImagesViewModel;
            var listItem = sender as ListBoxItem;
            var a = listItem.ActualWidth; //Margin 10 : 4 rows = 281; Margin 0 : 4 rows = 261;
            var b = listItem.Width;
            var c = Resources["ImageItemWidth"];//Margin 10 :4 rows = 251; Margin 0 : 4 rows = 251
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


            var widthOfListBox = ListBoxColumn.ActualWidth - 2; // 17 -- width of scrollbar
            var capacityOfMaxImageSize = (int) widthOfListBox / (int) MaxImageWidth;
            if (widthOfListBox / capacityOfMaxImageSize >= MaxImageWidth) ++capacityOfMaxImageSize;

            double newImageSize = widthOfListBox / capacityOfMaxImageSize;

            //while (newImageSize > MaxImageWidth) {
            //    newImageSize -= MinImageWidth;
            //    if (MaxImageWidth >= widthOfListBox ) {
            //        newImageSize = widthOfListBox;
            //        break;
            //    }
            //    if (newImageSize < MinImageWidth) {
            //        newImageSize += ++capacityOfMaxImageSize / 2;
            //    }
            //}
            Resources["ImageItemHeight"] = Resources["ImageItemWidth"] = newImageSize  ;

        }

    }
}
