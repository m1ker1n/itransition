using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using _4th_Task.ViewModels;
using _4th_Task.Models;
using _4th_Task.PrincipalValidator;

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

                    await LoginUser(user);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid credentials");
            }
            return View(model);
        }

        private async Task LoginUser(User user)
        {
            if (user == null) return;
            await Authenticate(user.Id.ToString());
            user.LastLogIn = DateTime.Now;
            await db.SaveChangesAsync();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(ModelState.IsValid)
            {
                User? user = await db.Users.FirstOrDefaultAsync(user => user.Email == model.Email);
                if (user == null)
                {
                    var newUser = db.Users.Add(new User 
                    { 
                        Name = model.Name, 
                        Email = model.Email, 
                        Password = model.Password, 
                        CreatedDate = DateTime.Now, 
                        Banned = false, 
                        LastLogIn = DateTime.Now 
                    });
                    
                    await db.SaveChangesAsync();
                    await Authenticate(newUser.Entity.Id.ToString());
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "This email already registered");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Ban(IFormCollection formCollection)
        {
            return AccountAction(formCollection, BanUsers).Result;
        }

        [HttpPost]
        public async Task<IActionResult> Unban(IFormCollection formCollection)
        {
            return AccountAction(formCollection, UnbanUsers).Result;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(IFormCollection formCollection)
        {
            return AccountAction(formCollection, DeleteUsers).Result;
        }

        private delegate Task ActionDelegate(int[] ids);

        private async Task<IActionResult> AccountAction(IFormCollection formCollection, ActionDelegate action)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var ids = GetIds(formCollection);
            await action(ids);
            return RedirectToAction("Index", "Account");
        }

        private async Task BanUsers(int[] ids)
        {
            await SetBanState(true, ids);
        }

        private async Task UnbanUsers(int[] ids)
        {
            await SetBanState(false, ids);
        }

        private async Task SetBanState(bool state, int[] ids)
        {
            foreach (var id in ids)
            {
                User? user = await db.Users.FirstOrDefaultAsync(user => user.Id == id);
                if (user != null)
                {
                    user.Banned = state;
                    await db.SaveChangesAsync();
                }
            }
        }

        private async Task DeleteUsers(int[] ids)
        {
            foreach (var id in ids)
            {
                User? user = await db.Users.FirstOrDefaultAsync(user => user.Id == id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                }
            }
        }

        private int[] GetIds(IFormCollection formCollection)
        {
            string[] stringIds = formCollection["rowCheckBox"];
            int[] ids = Array.ConvertAll(stringIds, s => int.Parse(s));
            return ids;
        }

        private async Task Authenticate(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId)
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
