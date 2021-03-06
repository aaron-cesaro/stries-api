using System;

namespace Post.Api.Infrastructure.Exceptions
{
    public class AuthorNotFoundException : Exception
    {
        public AuthorNotFoundException()
        {
        }

        public AuthorNotFoundException(string message)
            : base(message)
        {
        }

        public AuthorNotFoundException(Exception inner, string message)
            : base(message, inner)
        {
        }
    }
}