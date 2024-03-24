public class AnswerData
{
    public Question question;
    public Answer selectedAnswer;
    public float elapsedTime;
    public object extraData;

    public AnswerData(Question question, Answer selectedAnswer, float elapsedTime, object extraData)
    {
        this.question = question;
        this.selectedAnswer = selectedAnswer;
        this.elapsedTime = elapsedTime;
        this.extraData = extraData;
    }
}