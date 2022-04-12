using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using Network.Models;
using Network.ViewModels;

namespace Network.Controllers
{
    public class AccountController : Controller
    {
        private NetworkContext db;

        public AccountController(NetworkContext db) => this.db = db;

        #region [Index]

        [Authorize]
        public IActionResult Index()
        {
            User? user = GetUser(User, db);
            if (user == null) return RedirectToAction("Login", "Account");
            return View(user);
        }

        #endregion

        #region [Register]

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Account");
            if (!ModelState.IsValid) return View(model);
            if (db.UserExists(model.Email, model.Name))
            {
                ModelState.AddModelError("", "A user with this email/name already exists.");
                return View(model);
            }
            User? user = db.CreateUser(model.Email, model.Name, model.Password, db.GetUserRole());
            if (user == null)
            {
                ModelState.AddModelError("", "Failed to register. Try again later.");
                return View(model);
            }
            db.AddUsers(user);
            await LoginUserAsync(user);
            return RedirectToAction("Index", "Account");
        }

        #endregion

        #region [Login]

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Account");
            if (!ModelState.IsValid) return View(model);
            User? user = db.FindUserByEmail(model.Email);
            if (user == null || !PasswordAccepted(user, model.Password)) 
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(model);
            }
            await LoginUserAsync(user);
            return RedirectToAction("Index", "Account");
        }

        private async Task LoginUserAsync(User user)
        {
            await AuthenticateAsync(user.Email, user.Role.Name);
            db.OnLogin(user);
        }

        private async Task AuthenticateAsync(string email, string roleName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, roleName)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        #endregion

        #region [Logout]

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await LogoutUserAsync();
            return RedirectToAction("Login", "Account");
        }

        private async Task LogoutUserAsync()
        {
            await DeauthenticateAsync();
        }

        private async Task DeauthenticateAsync()
        {
            await HttpContext.SignOutAsync();
        }

        #endregion

        #region [Admin Panel]

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult AdminPanel()
        {
            var model = new AdminPanelModel
            {
                Users = db.Users.ToList(),
                Roles = db.Roles.ToList(),
                Messages = db.Messages.ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(IFormCollection formCollection)
        {
            string[] stringIds = formCollection["rowCheckBox"];
            int[] ids = Array.ConvertAll(stringIds, s => int.Parse(s));
            db.DeleteUsers(ids);
            return RedirectToAction("AdminPanel", "Account");
        }

        #endregion

        [HttpGet]
        [Authorize]
        public ActionResult Search(string term)
        {
            var users = (term == null) ? db.Users : db.Users.Where(u => u.Email.Contains(term));
            return Json(users.ToList());
        }

        private bool PasswordAccepted(User user, string password)
        {
            return user.Password == password;
        }

        static public User? GetUser(ClaimsPrincipal? claimsPrincipal, NetworkContext db)
        {
            if (claimsPrincipal == null) return null;
            var userEmail = claimsPrincipal.FindFirstValue(ClaimsIdentity.DefaultNameClaimType);
            User? user = db.FindUserByEmail(userEmail);
            return user;
        }
    }
}
