using System;

namespace Post.Api.Infrastructure.Exceptions
{
    public class PostAlreadyArchivedException : Exception
    {
        public PostAlreadyArchivedException()
        {
        }

        public PostAlreadyArchivedException(string message)
            : base(message)
        {
        }

        public PostAlreadyArchivedException(Exception inner, string message)
            : base(message, inner)
        {
        }
    }
}