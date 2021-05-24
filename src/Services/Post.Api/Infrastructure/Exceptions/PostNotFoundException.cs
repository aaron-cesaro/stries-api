using System;

namespace Post.Api.Infrastructure.Exceptions
{
    public class PostNotFoundException : Exception
    {
        public PostNotFoundException()
        {
        }

        public PostNotFoundException(string message)
            : base(message)
        {
        }

        public PostNotFoundException(Exception inner, string message)
            : base(message, inner)
        {
        }
    }
}
