namespace quiz_app.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int QuizId { get; set; }  // Klucz obcy do tabeli quizów
        public Quiz Quiz { get; set; }  // Obiekt reprezentujący relację
        public List<Answer>? Answers { get; set; }
    }
}
