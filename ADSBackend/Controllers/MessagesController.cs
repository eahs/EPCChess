
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADSBackend.Data;
using ADSBackend.Models.MessagesModels;
using Microsoft.AspNetCore.Authorization;

namespace ADSBackend.Controllers
{
    /// <summary>
    /// Controller for managing messages.
    /// </summary>
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all messages.
        /// </summary>
        /// <returns>The index view with a list of messages.</returns>
        // GET: Messages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Message.ToListAsync());
        }

        /// <summary>
        /// Displays the details of a specific message.
        /// </summary>
        /// <param name="id">The ID of the message.</param>
        /// <returns>The details view for the message.</returns>
        // GET: Messages/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        /// <summary>
        /// Displays the form for creating a new message.
        /// </summary>
        /// <returns>The create view.</returns>
        // GET: Messages/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the creation of a new message.
        /// </summary>
        /// <param name="message">The message to create.</param>
        /// <returns>A redirect to the index page on success, or the create view with errors on failure.</returns>
        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Link,PublishDate,Author,Description")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        /// <summary>
        /// Displays the form for editing an existing message.
        /// </summary>
        /// <param name="id">The ID of the message to edit.</param>
        /// <returns>The edit view for the message.</returns>
        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        /// <summary>
        /// Handles the update of an existing message.
        /// </summary>
        /// <param name="id">The ID of the message to edit.</param>
        /// <param name="message">The updated message data.</param>
        /// <returns>A redirect to the index page on success, or the edit view with errors on failure.</returns>
        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Link,PublishDate,Author,Description")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            return View(message);
        }

        /// <summary>
        /// Displays the confirmation page for deleting a message.
        /// </summary>
        /// <param name="id">The ID of the message to delete.</param>
        /// <returns>The delete confirmation view.</returns>
        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        /// <summary>
        /// Handles the deletion of a message.
        /// </summary>
        /// <param name="id">The ID of the message to delete.</param>
        /// <returns>A redirect to the index page.</returns>
        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var message = await _context.Message.FindAsync(id);
            _context.Message.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(string id)
        {
            return _context.Message.Any(e => e.Id == id);
        }
    }
}