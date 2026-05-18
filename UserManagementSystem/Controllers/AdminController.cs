using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Data;

namespace UserManagementSystem.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }   
        public IActionResult Index()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            if (isAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            var users = _context.Users.ToList();
            return View("Index" , users);
        }

        public IActionResult DeleteUser(int id)
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            if (isAdmin != "True")
            {
                return RedirectToAction("Index", "Home");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
