using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
class Quiz
{
    public Dictionary<string, Question> questions = new();
    public string description = "";
    public string id = "";
    public string owner_id = "";
    public string title = "";

    public Quiz(){}

    public Quiz(string id, string owner_id, string title, Dictionary<string, Question> questions, string description)
    {
        this.id = id;
        this.owner_id = owner_id;
        this.title = title;
        this.description = description;
        this.questions.AddRange(questions);
    }
}