using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMvc.Data;
using SocialMvc.Models;

namespace SocialMvc.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var friendIds = await _db.FriendRequests
                .Where(f => (f.SenderId == user.Id && f.Status == FriendRequestStatus.Accepted) ||
                            (f.ReceiverId == user.Id && f.Status == FriendRequestStatus.Accepted))
                .Select(f => f.SenderId == user.Id ? f.ReceiverId : f.SenderId)
                .ToListAsync();

            var posts = await _db.Posts
                .Where(p => p.Visibility == PostVisibility.Public ||
                            p.AuthorId == user.Id ||
                            (p.Visibility == PostVisibility.FriendsOnly && friendIds.Contains(p.AuthorId)))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string title, string content, PostVisibility visibility)
        {
            var user = await _userManager.GetUserAsync(User);
            var post = new Post { Title = title, Content = content, Visibility = visibility, AuthorId = user.Id };
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
