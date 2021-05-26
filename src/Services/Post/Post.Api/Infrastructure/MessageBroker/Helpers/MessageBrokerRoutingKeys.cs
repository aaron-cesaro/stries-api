namespace Post.Api.Infrastructure.MessageBroker.Helpers
{
    public static class MessageBrokerRoutingKeys
    {
        public static string POST_PUBLISHED = "post.published";
        public static string POST_ARCHIVED = "post.archived";
        public static string POST_DELETED = "post.deleted";
    }
}
