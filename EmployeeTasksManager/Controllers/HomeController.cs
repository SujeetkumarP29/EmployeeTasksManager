using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EmployeeTasksManager.Models;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace EmployeeTasksManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmployeeTasksManagerContext _context;
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        bool isActive;

        public HomeController(EmployeeTasksManagerContext context, UserManager<Employee> userManager, SignInManager<Employee> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ----------------- Authentication -----------------
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _context.Employees
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
            return RedirectToAction("Index");
        }
        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            var passwordHasher = new PasswordHasher<Employee>();
            var result = passwordHasher.VerifyHashedPassword(null, storedHashedPassword, enteredPassword);
            return result == PasswordVerificationResult.Success;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {

            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (!User.Identity.IsAuthenticated || userIdClaim == null)
            {
                Console.WriteLine("❌ User is not authenticated after redirection.");
                return RedirectToAction("Login");
            }

            var userId = userIdClaim.Value;
            var userRole = roleClaim.Value;

            Console.WriteLine($"🔹 UserId: {userId}");
            Console.WriteLine($"🔹 Role: {userRole}");

            var tasks = userRole == "Admin"
                ? _context.EmployeeTasks.Include(e => e.Employee)
                : _context.EmployeeTasks.Where(t => t.EmployeeId == userId);

            return View(await tasks.ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> Create(EmployeeTask task)
        {
            task.EmployeeId = _context.Employees.FirstOrDefault().Id;
            _context.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.EmployeeTasks.FindAsync(id);
            if (task == null) return NotFound();
            return View(task);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EmployeeTask task)
        {
            if (task.DueDate == null || task.DueDate < new DateTime(1753, 1, 1))
            {
                ModelState.AddModelError("DueDate", "Invalid date. Please select a valid Due Date.");
                return View(task);
            }

            try
            {
                task.TaskCreatedDate = DateTime.Now;
                _context.Update(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving task: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                return View(task);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.EmployeeTasks.FindAsync(id);
            if (task != null)
            {
                _context.EmployeeTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
