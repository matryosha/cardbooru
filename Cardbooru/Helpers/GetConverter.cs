using System;

namespace Cardbooru.Helpers
{
    //Uses for add to string parameters 
    public static class GetConverter
    {
        public static string GetPosts(string booruPostsString, int limit, int page)
        {
            string result = String.Empty;
            var postsStringParts = booruPostsString.Split('*');

            return result+=postsStringParts[0]+limit+postsStringParts[1]+page+postsStringParts[2];
        }
    }
}
