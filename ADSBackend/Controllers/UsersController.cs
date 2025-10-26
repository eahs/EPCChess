
using System.Collections.Generic;
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
using ADSBackend.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for managing users.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly DataService _dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        /// <param name="emailSender">The email sender service.</param>
        /// <param name="dataService">The data service.</param>
        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IEmailSender emailSender, DataService dataService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _dataService = dataService;
        }

        /// <summary>
        /// Gets a multi-select list of schools.
        /// </summary>
        /// <param name="schoolIds">The list of currently selected school IDs.</param>
        /// <returns>A <see cref="MultiSelectList"/> of schools.</returns>
        public async Task<MultiSelectList> GetSchoolSelectList (List<int> schoolIds = null)
        {
            var schools = await _context.School.Include(s => s.Season)
                                               .OrderBy(x => x.Name)
                                               .ThenByDescending(s => s.Season.EndDate)
                                               .ToListAsync();

            return new MultiSelectList(schools.Select(a => new SelectListItem
                                                            {
                                                                Text = a.Name + " (" + a.Season.Name + ")",
                                                                Value = "" + a.SchoolId
                                                            }), "Value", "Text", schoolIds ?? new List<int>());
        }

        /// <summary>
        /// Displays a list of all users.
        /// </summary>
        /// <returns>The index view with a list of users.</returns>
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Include(x => x.Schools).ThenInclude(x => x.School).OrderBy(x => x.LastName).ToListAsync();
            var userRoles = await _context.UserRoles.ToListAsync();
            var roleNames = new Dictionary<int, string>();

            var rnames = await _context.Roles.ToListAsync();
            foreach (var rname in rnames)
            {
                roleNames.Add(rname.Id, rname.Name);
            }

            var viewModel = users.Select(x => new UserViewModel
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = roleNames[userRoles.Where(u => u.UserId == x.Id).Select(x => x.RoleId).FirstOrDefault()],
                SchoolIds = x.Schools.Select(x => x.SchoolId).ToList(),
                Schools = x.Schools.Select(x => x.School).ToList()
            }).ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Displays the form for creating a new user.
        /// </summary>
        /// <returns>The create view.</returns>
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Schools = await GetSchoolSelectList();

            return View();
        }

        /// <summary>
        /// Handles the creation of a new user.
        /// </summary>
        /// <param name="viewModel">The view model containing the new user's data.</param>
        /// <returns>A redirect to the index page on success, or the create view with errors on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,FirstName,LastName,Role,SchoolIds,Password,ConfirmPassword")] UserViewModel viewModel)
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
                    LastName = viewModel.LastName
                };

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);

                // create user
                await _userManager.CreateAsync(user);

                foreach (var schoolId in viewModel.SchoolIds)
                {
                    await _dataService.AddUserToSchoolAsync(user, schoolId);
                }

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

        /// <summary>
        /// Displays the form for editing an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to edit.</param>
        /// <returns>The edit view for the user.</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.Include(x => x.Schools).ThenInclude(x => x.School).FirstOrDefaultAsync(x => x.Id == id);
            var role = await _userManager.GetRolesAsync(user);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SchoolIds = user.Schools.Select(x => x.SchoolId).ToList(),
                Schools = user.Schools.Select(x => x.School).ToList(),
                Role = role.FirstOrDefault()
            };

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Schools = await GetSchoolSelectList(viewModel.SchoolIds);

            return View(viewModel);
        }

        /// <summary>
        /// Handles the update of an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to edit.</param>
        /// <param name="viewModel">The view model containing the updated user data.</param>
        /// <returns>A redirect to the index page on success, or the edit view with errors on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,FirstName,LastName,Role,SchoolIds,Password,ConfirmPassword")] UserViewModel viewModel)
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

                    var oldSchools = await _context.UserSchool.Where(x => x.UserId == user.Id).ToListAsync();
                    _context.UserSchool.RemoveRange(oldSchools);
                    await _context.SaveChangesAsync();


                    user.Email = viewModel.Email;
                    user.FirstName = viewModel.FirstName;
                    user.LastName = viewModel.LastName;

                    user.Schools = viewModel.SchoolIds.Select(x => new UserSchool
                    {
                        UserId = user.Id,
                        SchoolId = x
                    }).ToList();

                    if (!string.IsNullOrEmpty(viewModel.Password))
                    {
                        // change the password
                        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);
                    }

                    // update user
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
            ViewBag.Schools = await GetSchoolSelectList(viewModel.SchoolIds);

            return View(viewModel);
        }

        /// <summary>
        /// Displays the details of a specific user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The details view for the user.</returns>
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

        /// <summary>
        /// Displays the confirmation page for deleting a user.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>The delete confirmation view.</returns>
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

        /// <summary>
        /// Handles the deletion of a user.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A redirect to the index page.</returns>
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