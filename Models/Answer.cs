namespace quiz_app.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }  // Klucz obcy do tabeli pytań
        public Question Question { get; set; }  // Obiekt reprezentujący relację
    }
}
