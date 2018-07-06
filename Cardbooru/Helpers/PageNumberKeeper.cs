namespace Cardbooru.Helpers
{
    public class PageNumberKeeper
    {
        /// <summary>
        /// How many pages were queried to fill current displayed page
        /// </summary>
        public int CountOfQueriedPages { get; set; }
        /// <summary>
        /// Current UI page
        /// </summary>
        public int NumberOfDisplayedPage { get; set; }
    }
}
