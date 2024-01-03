using Microsoft.AspNetCore.Identity;

namespace quiz_app.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? UserId { get; set; }  // Klucz obcy do tabeli użytkowników
        public IdentityUser? Author { get; set; }  // Używając IdentityUser dla relacji z użytkownikiem
        public List<Question>? Questions { get; set; }
        public List<UserQuiz>? UserQuizzes { get; set; }
    }
}
