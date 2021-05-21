using System;

namespace Post.Infrastructure.Exceptions
{
    public class AuthorNotProcessedException : Exception
    {
        public AuthorNotProcessedException()
        {
        }

        public AuthorNotProcessedException(string message)
            : base(message)
        {
        }

        public AuthorNotProcessedException(Exception inner, string message)
            : base(message, inner)
        {
        }
    }
}