﻿using System.Windows;
using System.Windows.Input;

namespace Cardbooru
{

    public partial class MainWindowView : Window
    {
        public MainWindowView(MainWindowViewModel vm)
        {
            DataContext = vm;
            InitializeComponent(); 
        }

        private void CloseDrawerAfterClick(object sender, MouseButtonEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }
    }
}