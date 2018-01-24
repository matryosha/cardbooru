using System.Windows;
using System.Windows.Controls;
using Cardbooru.Models.Base;

namespace Cardbooru.BrowseImages
{
    public partial class BrowseImageView : UserControl {

        private const double MaxImageWidth = 320;
        //private const double MinImageWidth = 250;

        public BrowseImageView()
        {
            InitializeComponent();   
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
            if (Resources["ImageItemHeight"] == null) return;


            var widthOfListBox = ListBoxColumn.ActualWidth - 2 - 17; // 17 -- width of scrollbar 
            // 2 -- internal listbox margin that no possible to turn off (sure, manystringscode method exist)
            //https://stackoverflow.com/questions/38289768/how-to-remove-margin-on-listbox-itemscontainer-in-wpf

            var capacityOfMaxImageSize = (int)widthOfListBox / (int)MaxImageWidth;
            if (widthOfListBox / capacityOfMaxImageSize >= MaxImageWidth) ++capacityOfMaxImageSize;

            //double newImageSize = widthOfListBox / capacityOfMaxImageSize; //whitout margin
            widthOfListBox = widthOfListBox - capacityOfMaxImageSize * 40 - capacityOfMaxImageSize * 10; // 40 -- margin between listboxitem(x2 because both side)-
                                                                                                         // and 10 - margin of each image
            double newImageSize = widthOfListBox / capacityOfMaxImageSize;

            Resources["ImageItemHeight"] = Resources["ImageItemWidth"] = newImageSize;
            

        }

        private void MainListBox_OnScrollChanged(object sender, ScrollChangedEventArgs e) {
            if (e.VerticalChange > 0) {
                
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    var contex = DataContext as BrowseImagesViewModel;
                    contex.LoadCommand.Execute(new object());
                }
            }
            
        }
    }
}
