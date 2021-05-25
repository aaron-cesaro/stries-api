namespace Post.Api.Application.Models
{
    /// <summary>
    /// Enumerate statuses that a Post can assume:
    ///  * draft: the post has been created but is not published yet.
    ///  * published: the post has been published and can be viewed by other users.
    ///  * archived: the post has been archived. This implies that the post is visible just for the author.
    /// </summary>
    public enum PostStatus
    {
        draft,
        published,
        archived
    }
}
