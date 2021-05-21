using System;

namespace Post.Infrastructure.Exceptions
{
    public class PostDbOperationNotExecutedException : Exception
    {
        public PostDbOperationNotExecutedException()
        {
        }

        public PostDbOperationNotExecutedException(string message)
            : base(message)
        {
        }

        public PostDbOperationNotExecutedException(Exception inner, string message)
            : base(message, inner)
        {
        }
    }
}