using System;

namespace Cardbooru.Application.Exceptions
{
    public class QueryPageException : Exception
    {
        public QueryPageException(string message)
        :base(message)
        {
            
        }
    }
}