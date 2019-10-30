using ADSBackend.Data;
using ADSBackend.Models.Identity;
using ADSBackend.Models.AdminViewModels;
using ADSBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        public async Task<SelectList> GetSchoolSelectList ()
        {
            var schools = await _context.School.Include(s => s.Season)
                                               .OrderByDescending(s => s.Season.EndDate)
                                               .ThenBy(x => x.Name)
                                               .ToListAsync();

            return new SelectList(schools.Select(a => new SelectListItem
                                                            {
                                                                Text = a.Name + " (" + a.Season.Name + ")",
                                                                Value = "" + a.SchoolId
                                                            }), "Value", "Text");
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Include(x => x.School).OrderBy(x => x.LastName).ToListAsync();

            var viewModel = users.Select(x => new UserViewModel
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = _userManager.GetRolesAsync(x).Result.FirstOrDefault(),
                SchoolId = x.SchoolId,
                School = x.School
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Schools = await GetSchoolSelectList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,FirstName,LastName,Role,SchoolId,Password,ConfirmPassword")] UserViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.Password))
            {
                ModelState.AddModelError("Password", "Password is required when creating a user");
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = viewModel.Email,
                    Email = viewModel.Email,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    SchoolId = viewModel.SchoolId
                };

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);

                // create user
                await _userManager.CreateAsync(user);

                // assign new role
                await _userManager.AddToRoleAsync(user, viewModel.Role);

                // send confirmation email
                var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.EmailConfirmationLink(user.Id, confirmationCode, Request.Scheme);

                //    await _emailSender.SendEmailConfirmationAsync(viewModel.Email, confirmationLink);


                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            var role = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Schools = await GetSchoolSelectList();

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SchoolId = user.SchoolId,
                Role = role.FirstOrDefault()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,FirstName,LastName,Role,SchoolId,Password,ConfirmPassword")] UserViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(id);

                    user.Email = viewModel.Email;
                    user.FirstName = viewModel.FirstName;
                    user.LastName = viewModel.LastName;
                    user.SchoolId = viewModel.SchoolId;

                    if (!string.IsNullOrEmpty(viewModel.Password))
                    {
                        // change the password
                        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);
                    }

                    // upadate user
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // reset user roles
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);

                    // assign new role
                    await _userManager.AddToRoleAsync(user, viewModel.Role);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FindAsync(id);
            var role = await _userManager.GetRolesAsync(user);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role.FirstOrDefault()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            var role = await _userManager.GetRolesAsync(user);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role.FirstOrDefault()
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(x => x.Id == id);
        }
    }
}