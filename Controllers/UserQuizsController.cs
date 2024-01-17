using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quiz_app.Data;
using quiz_app.Models;

namespace quiz_app.Controllers
{
    public class UserQuizsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserQuizsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserQuizs
        [Authorize]
        public async Task<IActionResult> Index()
        {
            IdentityUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var applicationDbContext = _context.UserQuiz.Where(u=>u.User == user).Include(u => u.Quiz).Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserQuizs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserQuiz == null)
            {
                return NotFound();
            }

            var userQuiz = await _context.UserQuiz
                .Include(u => u.Quiz)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userQuiz == null)
            {
                return NotFound();
            }

            return View(userQuiz);
        }

        // GET: UserQuizs/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["QuizId"] = new SelectList(_context.Quiz, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UserQuizs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,QuizId,DateCompleted")] UserQuiz userQuiz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userQuiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuizId"] = new SelectList(_context.Quiz, "Id", "Id", userQuiz.QuizId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userQuiz.UserId);
            return View(userQuiz);
        }

        // GET: UserQuizs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserQuiz == null)
            {
                return NotFound();
            }

            var userQuiz = await _context.UserQuiz.FindAsync(id);
            if (userQuiz == null)
            {
                return NotFound();
            }
            ViewData["QuizId"] = new SelectList(_context.Quiz, "Id", "Id", userQuiz.QuizId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userQuiz.UserId);
            return View(userQuiz);
        }

        // POST: UserQuizs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,QuizId,DateCompleted")] UserQuiz userQuiz)
        {
            if (id != userQuiz.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userQuiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserQuizExists(userQuiz.Id))
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
            ViewData["QuizId"] = new SelectList(_context.Quiz, "Id", "Id", userQuiz.QuizId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userQuiz.UserId);
            return View(userQuiz);
        }

        // GET: UserQuizs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserQuiz == null)
            {
                return NotFound();
            }

            var userQuiz = await _context.UserQuiz
                .Include(u => u.Quiz)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userQuiz == null)
            {
                return NotFound();
            }

            return View(userQuiz);
        }

        // POST: UserQuizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserQuiz == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserQuiz'  is null.");
            }
            var userQuiz = await _context.UserQuiz.FindAsync(id);
            if (userQuiz != null)
            {
                _context.UserQuiz.Remove(userQuiz);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserQuizExists(int id)
        {
          return (_context.UserQuiz?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
