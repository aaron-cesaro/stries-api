using System.Collections.Generic;

namespace Post.Infrastructure.MessageBroker.Helpers
{
    public static class MessageBrokerHelpers
    {
        public static Dictionary<string, object> SetMessageRoute(string key, string value)
        {
            return
                new Dictionary<string, object>
                {
                    { key, value}
                };
        }

    }
}
