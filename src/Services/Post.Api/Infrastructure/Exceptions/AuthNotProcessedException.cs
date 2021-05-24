using System;
using System.Runtime.Serialization;

namespace Post.Api.Infrastructure.Exceptions
{
    [Serializable]
    internal class AuthNotProcessedException : Exception
    {
        private Exception ex;
        private string v;

        public AuthNotProcessedException()
        {
        }

        public AuthNotProcessedException(string message) : base(message)
        {
        }

        public AuthNotProcessedException(Exception ex, string v)
        {
            this.ex = ex;
            this.v = v;
        }

        public AuthNotProcessedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AuthNotProcessedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}