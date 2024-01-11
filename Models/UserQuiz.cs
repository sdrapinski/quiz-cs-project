using Microsoft.AspNetCore.Identity;

namespace quiz_app.Models
{
    public class UserQuiz
    {
      
            public int Id { get; set; }
            public string UserId { get; set; }  // Klucz obcy do tabeli użytkowników
            public IdentityUser User { get; set; }  // Obiekt reprezentujący relację
            public int QuizId { get; set; }  // Klucz obcy do tabeli quizów
            public Quiz Quiz { get; set; }  // Obiekt reprezentujący relację
            public DateTime DateCompleted { get; set; }
            public int Score { get; set; } 
            
        
    }
}
