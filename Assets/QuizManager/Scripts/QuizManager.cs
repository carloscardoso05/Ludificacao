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
    private List<string>[] availableQuestions = new List<string>[3];
    public static QuizManager I;
    public event EventHandler<AnswerData> OnAnswered;
    public event EventHandler<Quiz> OnSelectedQuiz;

    public class QuestionData : EventArgs
    {
        public Question question;
        public object extraData;
    }

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        QuizzesListUI.I.OnQuizSelected += (sender, quiz) =>
        {
            Quiz = quiz;
            for (int i = 0; i < 3; i++)
            {
                availableQuestions[i] = Quiz.questions.Keys.Where((id) => Quiz.questions[id].difficulty == i).ToList();
            }
        };
        QuizzesListUI.I.OnQuizSelected += HandleSelectedQuiz;
        QuestionUI.I.OnAnswerSelected += PropagateAnswer;
    }

    private void PropagateAnswer(object sender, AnswerData answerData)
    {
        OnAnswered?.Invoke(sender, answerData);
    }

    private void HandleSelectedQuiz(object sender, Quiz quiz)
    {
        OnSelectedQuiz?.Invoke(sender, quiz);
    }

    public void SelectQuiz()
    {
        if (answering)
        {
            throw new Exception("Não pode selecionar um quiz enquanto responde a uma questão");
        }
        QuizzesListUI.I.Show();
        QuizzesListUI.I.ListView.itemsChosen += (_) => selectingQuiz = false;
        selectingQuiz = true;
    }

    public void SelectQuestion(int difficulty, object extraData)
    {
        if (selectingQuiz) throw new Exception("Não pode responder uma questão enquanto seleciona um quiz");
        if (Quiz is null) throw new Exception("Nenhum Quiz foi selecionado ainda");
        List<string> difficultyQuestions = availableQuestions[difficulty];
        if (difficultyQuestions.Count == 0)
        {
            difficultyQuestions = Quiz.questions.Keys.Where((id) => Quiz.questions[id].difficulty == difficulty).ToList();
            Debug.Log("Resetando difficuldade " + difficulty);
        }
        int index = UnityEngine.Random.Range(0, difficultyQuestions.Count);
        string questionId = difficultyQuestions.ElementAt(index);
        Question question = Quiz.questions[questionId];
        difficultyQuestions.RemoveAt(index);

        QuestionData questionData = new()
        {
            question = question,
            extraData = extraData,
        };
        OnChoseQuestion?.Invoke(this, questionData);
        answering = true;
    }

}
