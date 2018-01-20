using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Cardbooru.Models.Base;

namespace Cardbooru.BrowseImages
{
    public partial class BrowseImageView : UserControl {

        private static Action EmptyDelegate = delegate () { };
        private const double MaxImageWidth = 320;
        //private const double MinImageWidth = 250;

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


            var widthOfListBox = ListBoxColumn.ActualWidth - 2 - 17; // 17 -- width of scrollbar 
            // 2 -- internal listbox margin that no possible to turn of (sure, manystringcode method exist)
            //https://stackoverflow.com/questions/38289768/how-to-remove-margin-on-listbox-itemscontainer-in-wpf

            var capacityOfMaxImageSize = (int)widthOfListBox / (int)MaxImageWidth;
            if (widthOfListBox / capacityOfMaxImageSize >= MaxImageWidth) ++capacityOfMaxImageSize;

            //double newImageSize = widthOfListBox / capacityOfMaxImageSize; //whitout margin
            widthOfListBox = widthOfListBox - capacityOfMaxImageSize * 40; // 40 -- margin(x2 because both side)
            double newImageSize = widthOfListBox / capacityOfMaxImageSize;

            Resources["ImageItemHeight"] = Resources["ImageItemWidth"] = newImageSize;
            

        }


        private void Toggle_OnChecked(object sender, RoutedEventArgs e) { }

    }
}
