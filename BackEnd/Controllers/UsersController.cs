using BackEnd.Models;
using BackEnd.Models.Entity;
using BackEnd.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }
     
        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel user = new RegisterViewModel();
            return View(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                var user = new User();
                user = model.mapRegisterToUser(model);
                user.role_id = 2;
                int kq = await _userService.addUser(user);
                switch (kq)
                {
                    case 1:
                        return RedirectToAction("Login", "Users");
                    case -1:
                        ModelState.AddModelError("msg", "Email đã tồn tại trong hệ thống!");
                        return View(model);
                    case 0:
                        ModelState.AddModelError("msg", "Tài khoản đã tồn tại trong hệ thống!");
                        return View(model);
                    default:
                        break;
                }
                return NotFound();
            }
        }
     
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            User? user = _userService.userLogin(username, password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("id", user.id);
                HttpContext.Session.SetString("fullname", user.fullname);
                HttpContext.Session.SetString("email", user.email);
                HttpContext.Session.SetString("role_name", user.role?.name ?? "USER");
                
                // Nếu là ADMIN thì redirect về trang Admin Dashboard
                if (user.role?.name == "ADMIN")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.message = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }
        }
       
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
