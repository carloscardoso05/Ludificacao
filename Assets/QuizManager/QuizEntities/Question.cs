using System.Collections.Generic;

class Question
{
    public string id = "";
    public string question = "";
    public int difficulty = 0;
    public List<Answer> answers = new();

    public Question(string id, string question, int difficulty, List<Answer> answers)
    {
        this.id = id;
        this.question = question;
        this.difficulty = difficulty;
        this.answers.AddRange(answers);
    }
}