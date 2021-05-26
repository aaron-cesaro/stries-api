using System.Collections.Generic;

namespace User.Api.Infrastructure.MessageBroker.Helpers
{
    public static class MessageBrokerHeaders
    {
        public static Dictionary<string, object> SetMessageHeader(string key, object value)
        {
            return new Dictionary<string, object>
            {
                { key, value }
            };
        }
    }
}
