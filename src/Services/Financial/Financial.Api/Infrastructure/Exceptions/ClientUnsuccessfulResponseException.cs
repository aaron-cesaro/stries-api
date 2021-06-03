using System;

namespace Financial.Api.Infrastructure.Exceptions
{
    public class ClientUnsuccessfulResponseException : Exception
    {
        public ClientUnsuccessfulResponseException()
        {
        }

        public ClientUnsuccessfulResponseException(string message)
            : base(message)
        {
        }

        public ClientUnsuccessfulResponseException(Exception inner, string message)
            : base(message, inner)
        {
        }
    }
}
