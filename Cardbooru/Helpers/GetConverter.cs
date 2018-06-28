using System;

namespace Cardbooru.Helpers
{
    //Uses for add to string parameters 
    public static class GetConverter
    {
        private static bool _isSafeEnable;
        private static bool _isExplicitEnable;
        private static bool _isQuestionableEnable;
        private static int _countOfEnabledRatingTags;

        public static string GetPosts(string booruPostsString, int limit, int page)
        {
            string result = String.Empty;
            var postsStringParts = booruPostsString.Split('*');

            string ratingTagsString = "$tags=";
            if (_countOfEnabledRatingTags > 0)
            {
                ratingTagsString = GetRatingTagsQuery();
            }

            result += postsStringParts[0] + limit + postsStringParts[1] + page + postsStringParts[2] + ratingTagsString;

            return result;
        }

        private static string GetRatingTagsQuery()
        {
            switch (_countOfEnabledRatingTags)
            {
                case 1:
                {
                    if (_isSafeEnable)
                        return "&tags=rating%3As";
                    if (_isExplicitEnable)
                    {
                        if(Properties.Settings.Default.CurrentSite == "SafeBooru") 
                            return "$tags=";
                        return "&tags=rating%3Ae";
                    }
                        return "&tags=rating%3Aq";
                }
                case 2:
                {
                    if (!_isSafeEnable)
                        return "&tags=-rating%3As";
                    if (!_isExplicitEnable)
                        return "&tags=-rating%3Ae";
                    return "&tags=-rating%3Aq";
                }
                default: return "$tags=";
            }
        }

        public static void UpdateRatingTags()
        {
            _countOfEnabledRatingTags = 0;
            _isSafeEnable = Properties.Settings.Default.SafeCheck;
            if (_isSafeEnable) _countOfEnabledRatingTags++;
            _isExplicitEnable = Properties.Settings.Default.ExplicitCheck;
            if (_isExplicitEnable) _countOfEnabledRatingTags++;
            _isQuestionableEnable = Properties.Settings.Default.QuestionableCheck;
            if (_isQuestionableEnable) _countOfEnabledRatingTags++;

        }
    }
}
