using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using _4th_Task.ViewModels;
using _4th_Task.Models;

namespace _4th_Task.Controllers
{
    public class AccountController : Controller
    {
        private UserContext db;

        public AccountController(UserContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
                return View(db.Users.ToList());

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await db.Users.FirstOrDefaultAsync(user => user.Email == model.Email && user.Password == model.Password);
                if (user != null)
                {
                    if (user.Banned == true)
                    {
                        ModelState.AddModelError("", "You're banned.");
                        return View(model);
                    }
                        
                    await Authenticate(model.Email);
                    user.LastLogIn = DateTime.Now;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid credentials");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(ModelState.IsValid)
            {
                User? user = await db.Users.FirstOrDefaultAsync(user => user.Email == model.Email);
                if (user == null)
                {
                    db.Users.Add(new User { Name = model.Name, Email = model.Email, Password = model.Password, CreatedDate = DateTime.Now, Banned = false, LastLogIn = DateTime.Now });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "This email already registered");
            }
            return View(model);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetBanStates(bool state, params int[] ids)
        {
            foreach(var id in ids)
            {
                User? user = await db.Users.FirstOrDefaultAsync(user => user.Id == id);
                if (user != null)
                {
                    user.Banned = state;
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "Account");
        }

        private async Task Authenticate(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }
}
