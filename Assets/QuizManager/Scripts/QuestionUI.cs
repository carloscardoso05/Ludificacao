using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

class QuestionUI : MonoBehaviour
{
    public event EventHandler<AnswerData> OnAnswerSelected;
    private bool QuestionRuning = false;
    private float maxTime;
    private float elapsedTime;
    private Question currentQuestion;
    private readonly string[] difficultiesPtBr = { "Fácil", "Média", "Difícil" };
    private float[] timesForDifficulties;
    private readonly Color[] difficultiesBGColors = { Color.green, Color.yellow, Color.red };
    public static QuestionUI I;

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        Hide();
        QuizManager.I.OnChoseQuestion += (sender, question) => Render(question);
    }

    private void Update()
    {
        if (QuestionRuning) UpdateTimer(Time.deltaTime);
    }

    private void Render(Question question)
    {
        timesForDifficulties = new float[] {
            Settings.I.GetDifficultyTimer(0),
            Settings.I.GetDifficultyTimer(1),
            Settings.I.GetDifficultyTimer(2),
        };
        currentQuestion = question;
        PrepareQuestion(question.question);
        PrepareAnswers(question.answers);
        PrepareDifficulty(question.difficulty);
        PrepareTimer(question.difficulty);
        Show();
    }

    private void PrepareQuestion(string text)
    {
        var questionText = transform.Find("Header").Find("QuestionText").GetComponent<TextMeshProUGUI>();
        questionText.text = text;
    }

    private void PrepareAnswers(List<Answer> answers)
    {
        foreach ((string answerNumber, Answer answer) in answers.Select((answer, index) => ((index + 1).ToString(), answer)))
        {
            var answerElement = transform.Find("Answers").Find($"AnswerButton{answerNumber}");
            var buttonText = answerElement.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = answer.text;
            answerElement.GetComponent<Button>().onClick.RemoveAllListeners();
            answerElement.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnAnswerSelected?.Invoke(this, new AnswerData(currentQuestion, answer, elapsedTime));
                    Hide();
                }
            );
        }
    }

    private void PrepareDifficulty(int difficulty)
    {
        var questionDifficulty = transform.Find("Header").Find("Difficulty");
        var difficultyText = questionDifficulty.GetComponentInChildren<TextMeshProUGUI>();
        difficultyText.text = difficultiesPtBr[difficulty];
        var difficultyImage = questionDifficulty.GetComponentInChildren<Image>();
        difficultyImage.color = difficultiesBGColors[difficulty];
    }

    private void PrepareTimer(int difficulty)
    {
        maxTime = timesForDifficulties[difficulty];
        elapsedTime = 0;
        var progress = transform.Find("Header").Find("Timer");
        var progressText = progress.GetComponent<TextMeshProUGUI>();
        string timeString = Math.Floor(maxTime).ToString();
        progressText.text = $"Tempo restante: {timeString}";
    }

    private void UpdateTimer(float timeElapsedInSecconds)
    {
        var progress = transform.Find("Header").Find("Timer");
        var progressText = progress.GetComponent<TextMeshProUGUI>();
        elapsedTime += timeElapsedInSecconds;
        string timeString = Math.Floor(maxTime - elapsedTime).ToString();
        progressText.text = $"Tempo restante: {timeString}";
        if (elapsedTime > maxTime)
        {
            Debug.Log("Tempo excedido");
            OnAnswerSelected?.Invoke(this, new AnswerData(currentQuestion, null, maxTime));
            Hide();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        QuestionRuning = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        QuestionRuning = false;
    }
}