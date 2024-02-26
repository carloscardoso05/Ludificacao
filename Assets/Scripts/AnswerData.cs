#nullable enable
class AnswerData {
    public Question question;
    public Answer? selectedAnswer;
    public float elapsedTime;

    public AnswerData(Question question, Answer? selectedAnswer, float elapsedTime)
    {
        this.question = question;
        this.selectedAnswer = selectedAnswer;
        this.elapsedTime = elapsedTime;
    }
}