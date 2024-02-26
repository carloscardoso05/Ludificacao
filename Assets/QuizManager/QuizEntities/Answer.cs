class Answer
{
    public string id = "";
    public string text = "";
    public bool correct = false;

    public Answer(string id, string text, bool correct)
    {
        this.text = text;
        this.correct = correct;
    }
}