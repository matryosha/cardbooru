﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cardbooru.Helpers
{
    public class PageNumberKeeper : INotifyPropertyChanged
    {

        public int AddedImagesCount { get; set; }
        public int NextQueriedPage { get; set; }
        public Dictionary<int, int> QuriedPagesAccordance = new Dictionary<int, int>();
        /// <summary>
        /// How many pages were queried to fill current displayed page
        /// </summary>
        public int QueriedPagesCount { get; set; }
        /// <summary>
        /// Current UI page
        /// </summary>
        private int _displayedPage;
        public int DisplayedPage
        {
            get => _displayedPage;


            set
            {
                _displayedPage = value;
                OnPropertyChanged("DisplayedPage");
            }
        }

        public void ResetDisplayedPage()
        {
            DisplayedPage = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
