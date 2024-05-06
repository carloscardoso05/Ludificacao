using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class QuizManager : MonoBehaviour
{
    [SerializeField] private CanvasRenderer questionPanel;
    private Quiz Quiz;
    private bool answering = false;
    public bool selectingQuiz = false;
    public event EventHandler<QuestionData> OnChoseQuestion;
    private List<string> availableQuestions = new();
    public event EventHandler<AnswerData> OnAnswered;
    public event EventHandler<Quiz> OnSelectedQuiz;
    public static QuizManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        QuizzesListUI.Instance.OnQuizSelected += (sender, quiz) =>
        {
            Quiz = quiz;
            availableQuestions.AddRange(Quiz.questions.Keys);
        };
        QuizzesListUI.Instance.OnQuizSelected += SendSelectedQuizEvent;
        QuestionUI.Instance.OnAnswerSelected += SendAnswerEvent;
    }

    public void SendAnswerEvent(object sender, AnswerData answerData)
    {
        OnAnswered?.Invoke(sender, answerData);
    }

    public void SendSelectedQuizEvent(object sender, Quiz quiz)
    {
        OnSelectedQuiz?.Invoke(sender, quiz);
    }

    public void SelectQuiz()
    {
        if (answering)
        {
            throw new Exception("Não pode selecionar um quiz enquanto responde a uma questão");
        }
        QuizzesListUI.Instance.Show();
        QuizzesListUI.Instance.ListView.itemsChosen += (_) => selectingQuiz = false;
        selectingQuiz = true;
    }

    public void ShowRandomQuestion(object extraData, GameColor color)
    {
        if (selectingQuiz) throw new Exception("Não pode responder uma questão enquanto seleciona um quiz");
        if (Quiz is null) throw new Exception("Nenhum Quiz foi selecionado ainda");

        if (availableQuestions.Count == 0)
        {
            availableQuestions.AddRange(Quiz.questions.Keys);
        }

        int index = UnityEngine.Random.Range(0, availableQuestions.Count);
        string questionId = availableQuestions.ElementAt(index);
        Question question = Quiz.questions[questionId];
        availableQuestions.RemoveAt(index);

        QuestionData questionData = new()
        {
            question = question,
            extraData = extraData,
            color = color
        };
        OnChoseQuestion?.Invoke(this, questionData);
        answering = true;
    }
}
