using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using quiz_app.Models;

namespace quiz_app.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<quiz_app.Models.Answer>? Answer { get; set; }
        public DbSet<quiz_app.Models.Question>? Question { get; set; }
        public DbSet<quiz_app.Models.Quiz>? Quiz { get; set; }
        public DbSet<quiz_app.Models.UserQuiz>? UserQuiz { get; set; }
    }
}