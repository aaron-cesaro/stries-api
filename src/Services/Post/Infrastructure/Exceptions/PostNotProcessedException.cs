﻿using System;

namespace Post.Infrastructure.Exceptions
{
    public class PostNotProcessedException : Exception
    {
        public PostNotProcessedException()
        {
        }

        public PostNotProcessedException(string message)
            : base(message)
        {
        }

        public PostNotProcessedException(Exception inner, string message)
            : base(message, inner)
        {
        }
    }
}
