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
    private List<Quiz> Quizzes = new();
    public event EventHandler<Quiz> OnQuizSelected;
    public static QuizzesListUI I;

    private void Awake() {
        I = this;
    }

    private void Start()
    {
        QuizProvider.OnGetQuizzes += LoadQuizzes;
        ListView.selectionType = SelectionType.Single;
        ListView.onItemsChosen += OnQuizChosen;
        TextField.RegisterCallback<ChangeEvent<string>>(searchChanged);
    }

    private void searchChanged(ChangeEvent<string> changeEvent) {
        string search = changeEvent.newValue.ToLower().Trim();
        ListView.itemsSource = Quizzes.Select((quiz) => quiz.title).Where((title) => title.ToLower().Contains(search)).ToList();
    }

    private void OnQuizChosen(IEnumerable<object> objects)
    {
        int i = ListView.selectedIndex;
        Quiz quiz = Quizzes[i];
        OnQuizSelected?.Invoke(this, quiz);
        Hide();
    }

    void LoadQuizzes(object sender, Dictionary<string, Quiz> quizzes)
    {
        Quizzes = quizzes.Values.ToList();
        ListView.itemsSource = Quizzes.Select((quiz) => quiz.title).ToList();
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