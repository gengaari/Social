using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMvc.Data;
using SocialMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace SocialMvc.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Requests()
        {
            var user = await _userManager.GetUserAsync(User);
            var requests = await _db.FriendRequests
                .Include(f => f.Sender)
                .Where(f => f.ReceiverId == user.Id && f.Status == FriendRequestStatus.Pending)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> List()
        {
            var user = await _userManager.GetUserAsync(User);
            var friends = await _db.FriendRequests
                .Include(f => f.Sender)
                .Include(f => f.Receiver)
                .Where(f => (f.SenderId == user.Id || f.ReceiverId == user.Id) && f.Status == FriendRequestStatus.Accepted)
                .ToListAsync();
            return View(friends);
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest(string receiverId)
        {
            var user = await _userManager.GetUserAsync(User);
            var request = new FriendRequest { SenderId = user.Id, ReceiverId = receiverId, Status = FriendRequestStatus.Pending };
            _db.FriendRequests.Add(request);
            await _db.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Respond(int id, bool accept)
        {
            var request = await _db.FriendRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = accept ? FriendRequestStatus.Accepted : FriendRequestStatus.Rejected;
            await _db.SaveChangesAsync();
            return RedirectToAction("Requests");
        }
    }
}
