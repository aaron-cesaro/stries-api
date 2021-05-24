using System;

namespace Post.Api.Infrastructure.Exceptions
{
    public class PostAlreadyPublishedException : Exception
    {
        public PostAlreadyPublishedException()
        {
        }

        public PostAlreadyPublishedException(string message)
            : base(message)
        {
        }

        public PostAlreadyPublishedException(Exception inner, string message)
            : base(message, inner)
        {
        }
    }
}