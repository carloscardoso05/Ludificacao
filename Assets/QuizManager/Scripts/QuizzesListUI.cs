using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

class QuizzesListUI : MonoBehaviour
{
    [SerializeField] private UIDocument UIDocument;
    [SerializeField] private QuizProvider QuizProvider;
    private VisualElement Root { get => UIDocument.rootVisualElement; }
    public ListView ListView { get => Root.Q<ListView>("QuizzesList"); }
    private TextField TextField { get => Root.Q<TextField>("SearchQuiz"); }
    [SerializeField] private List<Quiz> Quizzes = new();
    public event EventHandler<Quiz> OnQuizSelected;
    public static QuizzesListUI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Hide();
        QuizProvider.OnGetQuizzes += LoadQuizzes;
        ListView.selectionType = SelectionType.Single;
        ListView.itemsChosen += OnQuizChosen;
        TextField.RegisterCallback<ChangeEvent<string>>(SearchChanged);
    }

    private void SearchChanged(ChangeEvent<string> changeEvent)
    {
        string search = changeEvent.newValue.ToLower().Trim();
        ListView.itemsSource = Quizzes.Select((quiz) => quiz.title).Where((title) => title.ToLower().Contains(search)).ToList();
    }

    private void OnQuizChosen(IEnumerable<object> objects)
    {
        int i = ListView.selectedIndex;
        Quiz quiz = Quizzes.Where((quiz) => quiz.questions.Count > 0).ToArray()[i];
        Debug.Log($"id: {quiz.id} len: {quiz.questions.Count()}");
        OnQuizSelected?.Invoke(this, quiz);
        Hide();
    }

    public void SetQuiz(string quizId)
    {
        Debug.Log($"QuizId: {quizId}");
        Debug.Log($"QuizzesCount: {Quizzes.Count()}");
        Debug.Log($"Filter: {Quizzes.Where((quiz) => quiz.id == quizId).Count()}");
        Quiz quiz = Quizzes.Where((quiz) => quiz.id == quizId).Single();
        Debug.Log($"id: {quiz.id} len: {quiz.questions.Count()}");
        OnQuizSelected?.Invoke(this, quiz);
        Hide();
    }

    void LoadQuizzes(object sender, Dictionary<string, Quiz> quizzes)
    {
        Quizzes = quizzes.Values.ToList();
        ListView.itemsSource = Quizzes.Where((quiz) => quiz.questions.Count > 0).Select((quiz) => quiz.title).ToList();
    }

    public void Show()
    {
        Root.style.display = DisplayStyle.Flex;
    }
    public void Hide()
    {
        Root.style.display = DisplayStyle.None;
        TextField.value = "";
    }
}