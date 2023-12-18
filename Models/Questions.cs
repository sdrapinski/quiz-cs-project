namespace quiz_app.Models
{
    public class Questions
    {
       public int Id { get; set; }
       public string Description { get; set; }
       public List<int> Answers_Id { get; set; }
    }
}
