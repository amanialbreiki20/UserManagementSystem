using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Data;
using UserManagementSystem.Models;
using UserManagementSystem.Services;

namespace UserManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(AppDbContext context , EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    CreatedAt = DateTime.Now
                };

                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered !");
                    return View(model);
                }

                _context.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    HttpContext.Session.SetString("UserName", user.FullName);
                    HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());  
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (userName == null)
            {
                return RedirectToAction("Login");
            }

            return View("Profile");

        }

        public IActionResult EditProfile()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (userName == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.FullName == userName);

            var model = new EditProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email
            };

            return View("EditProfile", model);
        }

        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (userName == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.FullName == userName);

                user.FullName = model.FullName;
                user.Email = model.Email;

                _context.SaveChanges();

                HttpContext.Session.SetString("UserName", user.FullName);

                return RedirectToAction("Profile");
            }

            return View("EditProfile", model);
        }

        public IActionResult ChangePassword()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (userName == null)
            {
                return RedirectToAction("Login");
            }

            return View("ChangePassword");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (userName == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.FullName == userName);

                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                    return View("ChangePassword", model);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                _context.SaveChanges();

                return RedirectToAction("Profile");
            }

            return View("ChangePassword", model);
        }
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user != null)
                {
                    var token = Guid.NewGuid().ToString();
                    user.ResetToken = token;
                    user.ResetTokenExpiry = DateTime.Now.AddHours(1);
                    _context.SaveChanges();

                    var resetLink = Url.Action("ResetPassword", "Account",
                        new { token = token }, Request.Scheme);

                    _emailService.SendResetPasswordEmail(user.Email, resetLink);
                }

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View("ForgotPassword", model);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View("ForgotPasswordConfirmation");
        }

        public IActionResult ResetPassword(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetToken == token
                       && u.ResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View("ResetPassword", model);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.ResetToken == model.Token
                           && u.ResetTokenExpiry > DateTime.Now);

                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.ResetToken ="";
                user.ResetTokenExpiry = null;
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View("ResetPassword", model);
        }

    }
}