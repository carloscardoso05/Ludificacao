using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

class QuestionUI : MonoBehaviour
{
    public event EventHandler<bool> OnAnswerSelected;
    private bool QuestionRuning = false;
    private float maxTime;
    private float elapsedTime;
    private readonly string[] difficultiesPtBr = { "Fácil", "Média", "Difícil" };
    private readonly int[] timesForDifficulties = { 15, 30, 60 };
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
            answerElement.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnAnswerSelected?.Invoke(this, answer.correct);
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
            Hide();
        }
    }

    public void Show()
    {
        transform.parent.gameObject.SetActive(true);
        QuestionRuning = true;
    }

    public void Hide()
    {
        transform.parent.gameObject.SetActive(false);
        QuestionRuning = false;
    }
}