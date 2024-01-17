using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quiz_app.Data;
using quiz_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace quiz_app.Controllers
{
    public class QuizsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public QuizsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        // GET: Quizs
        public async Task<IActionResult> Index()
        {
            IdentityUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            return _context.Quiz != null ? 
                          View(await _context.Quiz.Where(m => m.Author == user).Include(m => m.Author).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Quiz'  is null.");
        }
        [Authorize]
        public IActionResult MyQuizzes()
        {
            // Pobierz quizy stworzone przez zalogowanego użytkownika
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var quizzes = _context.Quiz.Where(q => q.UserId == user.Id).ToList();

            var quizCompletionCounts = new Dictionary<int, int>();

            foreach (var quiz in quizzes)
            {
                var completionCount = _context.UserQuiz.Count(uq => uq.QuizId == quiz.Id);
                quizCompletionCounts.Add(quiz.Id, completionCount);
            }

            ViewBag.QuizCompletionCounts = quizCompletionCounts;

            return View(quizzes);
        }


        // Solve
        [Authorize]
        public IActionResult Solve(int id)
        {
            var quiz = _context.Quiz.Include(q => q.Questions).ThenInclude(q => q.Answers).FirstOrDefault(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SubmitAnswers(int quizId, Dictionary<int, int> selectedAnswerIds)
        {
            var quiz = _context.Quiz.Include(q => q.Questions).ThenInclude(q => q.Answers).FirstOrDefault(q => q.Id == quizId);

            if (quiz == null)
            {
                return NotFound();
            }

            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            int score = CalculateScore(quiz, selectedAnswerIds.Values.ToList());

            var userQuiz = new UserQuiz
            {
                UserId = user.Id,
                QuizId = quizId,
                DateCompleted = DateTime.Now,
                Score = score
            };

            _context.UserQuiz.Add(userQuiz);
            _context.SaveChanges();

            return RedirectToAction("Index", "UserQuizs"); // Przykładowe przekierowanie
        }


        private int CalculateScore(Quiz quiz, List<int> selectedAnswerIds)
        {
            int score = 0;

            foreach (var question in quiz.Questions)
            {
                foreach (var selectedAnswerId in selectedAnswerIds)
                {
                    var answer = question.Answers.FirstOrDefault(a => a.Id == selectedAnswerId);

                    if (answer != null && answer.IsCorrect)
                    {
                        // Jeśli odpowiedź jest poprawna, dodaj punkty
                        score++;
                    }
                }
            }

            return score;
        }


        // GET: Quizs/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Quiz == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        // GET: Quizs/Create
        [Authorize]
        public IActionResult Create()
        {
           
           
            return View();
        }

        // POST: Quizs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Description")] Quiz quiz)
        {
            IdentityUser user =await  _userManager.FindByNameAsync(User.Identity.Name);
            quiz.UserId = user.Id;
            quiz.Author = user;
            Console.WriteLine(quiz);
            if (ModelState.IsValid)
            {
                _context.Add(quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }
            return View(quiz);
        }

        // GET: Quizs/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Quiz == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }
            return View(quiz);
        }

        // POST: Quizs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,UserId")] Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.Id))
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
            return View(quiz);
        }

        // GET: Quizs/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Quiz == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quiz
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        // POST: Quizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Quiz == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Quiz'  is null.");
            }
            var quiz = await _context.Quiz.FindAsync(id);
            if (quiz != null)
            {
                _context.Quiz.Remove(quiz);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuizExists(int id)
        {
          return (_context.Quiz?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
