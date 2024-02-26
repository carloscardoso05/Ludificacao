using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class QuizManager : MonoBehaviour
{
    private Quiz Quiz;
    private bool answering = false;
    public bool selectingQuiz = false;
    public event EventHandler<Question> OnChoseQuestion;
    private List<string>[] availableQuestions = new List<string>[3];
    public static QuizManager I;

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
        QuestionUI.I.OnAnswerSelected += HandleAnswer;
    }

    private void HandleAnswer(object sender, bool correct)
    {
        answering = false;
        Debug.Log(correct ? "Parabéns, você acertou" : "Você errou");
    }

    public void SelectQuiz()
    {
        if (answering)
        {
            throw new Exception("Não pode selecionar um quiz enquanto responde a uma questão");
        }
        QuizzesListUI.I.Show();
        QuizzesListUI.I.ListView.onItemsChosen += (_) => selectingQuiz = false;
        selectingQuiz = true;
    }

    public void SelectQuestion(int difficulty)
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

        OnChoseQuestion?.Invoke(this, question);
        answering = true;
    }

}
