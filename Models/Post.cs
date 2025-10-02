using System.ComponentModel.DataAnnotations;

namespace SocialMvc.Models
{
    public enum PostVisibility
    {
        Public,
        Private,
        FriendsOnly
    }

    public class Post
    {
        public int Id { get; set; }

        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PostVisibility Visibility { get; set; }

        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser Author { get; set; } = null!;
    }
}
