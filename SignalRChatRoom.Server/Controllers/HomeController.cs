using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRChatRoom.Server.DbContexts;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ChatDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ChatDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Clients
            .Select(u => new { u.Id, u.PersianName })
            .ToListAsync();

        return Json(users);
    }

    //[HttpGet]
    //public async Task<IActionResult> GetMessages(long userId)
    //{
    //    var currentUserId = long.Parse(_userManager.GetUserId(User));
    //    var messages = await _context.ChatRooms
    //        .Where(c => (c.From == currentUserId && c.To == userId) || (c.From == userId && c.To == currentUserId))
    //        .OrderBy(c => c.CreateDate)
    //        .Select(c => new { c.From, c.Message })
    //        .ToListAsync();

    //    return Json(messages);
    //}
}
