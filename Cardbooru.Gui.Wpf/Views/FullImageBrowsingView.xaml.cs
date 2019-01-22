using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Cardbooru.Gui.Wpf.ViewModels;

namespace Cardbooru.Gui.Wpf.Views
{
    public partial class FullImageBrowsingView : UserControl
    {
        public FullImageBrowsingView()
        {
            InitializeComponent();
        }

        private void FullImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            CloseWithAnimation();
        }

        private void FullImage_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            if (TagsPanel.IsVisible)
                TagsPanel.Visibility = Visibility.Hidden;
            else {
                TagsPanel.Visibility = Visibility.Visible;
            }
        }

        private void CloseWithAnimation() {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 0.0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(300));


            var myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(OpacityProperty));
            myStoryboard.Completed += MyStoryboard_Completed;
            myStoryboard.Begin(FullImage);
            PrevButGrid.IsEnabled = false;
            NextButGrid.IsEnabled = false;
        }

        private void MyStoryboard_Completed(object sender, EventArgs e)
        {
            var context = DataContext as FullImageBrowsingViewModel;
            context?.CloseImageCommand.Execute(sender);
        }

        private async void FullImageBrowsingView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var context = DataContext as FullImageBrowsingViewModel;
            await context?.ShowFullImage();
        }
    }
}
