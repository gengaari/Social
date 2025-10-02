using Microsoft.AspNetCore.Identity;

namespace SocialMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<FriendRequest> Friends { get; set; } = new List<FriendRequest>();
    }
}
