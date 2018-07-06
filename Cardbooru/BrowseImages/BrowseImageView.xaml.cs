using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cardbooru.Models.Base;

namespace Cardbooru.BrowseImages
{
    public partial class BrowseImageView : UserControl {

        private const double MaxImageWidth = 320;
        //private const double MinImageWidth = 250;
        private BrowseImagesViewModel _context;
        public BrowseImageView()
        {
            InitializeComponent(); 
        }   

        private void BrowseImageView_OnLoaded(object sender, RoutedEventArgs e) {
            if (_context == null) _context = DataContext as BrowseImagesViewModel;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            if(System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed) return;
                
            //if (double.IsNaN(ListBoxColumn.ActualWidth)) return;
            //if (Resources["ImageItemHeight"] == null) return;


            var widthOfListBox = ListBoxColumn.ActualWidth - 2 - 17 - 20; // 17 -- width of scrollbar; 20 -- left padding of listbox
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

        //Loading items when scroll at the bottom
        private void MainListBox_OnScrollChanged(object sender, ScrollChangedEventArgs e) {
            /*if (e.VerticalChange > 0) {
                
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    var contex = DataContext as BrowseImagesViewModel;
                    contex.LoadCommand.Execute(new object());
                }
            }*/
            
        }

        private void MainListBox_OnLoaded(object sender, RoutedEventArgs e) {
            var scroll = _context.CurrentScroll as ScrollViewer;
            if(scroll==null) return;
            var a =  FindVisualChild<ScrollViewer>(sender as ListBox);
            a.ScrollToVerticalOffset(scroll.ContentVerticalOffset);
        }

        private void MainListBox_OnUnLoaded(object sender, RoutedEventArgs e) {
            var a =  FindVisualChild<ScrollViewer>(sender as ListBox);
            _context.CurrentScroll = a;
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            // Iterate through all immediate children
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }

        private void NextBtn_Onclick(object sender, RoutedEventArgs e)
        {
            var a = FindVisualChild<ScrollViewer>(mainListBox);
            a.ScrollToTop();
        }
    }
}
