using System;

namespace Cardbooru.Application.Exceptions
{
    public class AssemblePostUrlException : Exception
    {
        public AssemblePostUrlException() :
            base("Error when trying to assemble url with posts")
        { }
    }
}