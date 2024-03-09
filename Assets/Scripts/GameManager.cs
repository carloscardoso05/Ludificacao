using System;
using UnityEngine;

public enum GameState { SelectPlayersNumber, RollingDice, SelectPiece, End };
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject piecePrefab;
    public static GameManager I;
    public Dice dice;
    public bool diceWasRolled = false;
    public GameState state = GameState.SelectPlayersNumber;
    public ColorsManager colorsManager;
    private Piece currentPiece;
    public event EventHandler<GameColor> OnTurnChanged;
    public event EventHandler<Piece> OnGameEnded;

    #region Unity Life Cycle

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        SetMenuVisibility(false);
        QuizManager.I.OnAnswered += HandleAnswer;
        QuizManager.I.OnSelectedQuiz += (sender, quiz) =>
        {
            SetMenuVisibility(true);
        };
        QuizManager.I.SelectQuiz();
    }

    #endregion

    #region Game Mechanics

    public void MovePiece(Piece piece)
    {
        var rnd = UnityEngine.Random.Range(0, 3);
        currentPiece = piece;
        QuizManager.I.SelectQuestion(rnd);
    }

    private void HandleAnswer(object sender, AnswerData answerData)
    {
        if (answerData.selectedAnswer.correct)
        {
            currentPiece.MoveToNextTile(dice.value + Settings.I.GetDifficultyBonus(answerData.question.difficulty));
            var pieceTile = currentPiece.Path.Current;
            if (pieceTile.isFinal && pieceTile.players.Count == 2)
                OnGameEnded?.Invoke(this, currentPiece);
        }
        colorsManager.UpdateColor();
        OnTurnChanged?.Invoke(this, colorsManager.currentColor);
        diceWasRolled = false;
    }

    #endregion

    #region Misc

    private void GeneratePlayersPieces()
    {
        var players = new GameObject("Players");
        foreach (GameColor color in colorsManager.colors)
        {
            if (color == GameColor.White) continue;
            var colorGO = new GameObject(color.ToString());
            colorGO.transform.parent = players.transform;
            for (int i = 0; i < 4; i++)
            {
                var piece = Instantiate(piecePrefab).GetComponent<Piece>();
                piece.name = "Piece" + i.ToString();
                piece.transform.parent = colorGO.transform;
                Vector2 homePosition = Board.I.GetHome(color).transform.position;
                Vector2 offset = Board.I.GetHomeOffset(i);
                piece.HomePosition = homePosition + offset;
                piece.color = color;
                piece.spriteResolver.SetCategoryAndLabel("Body", color.ToString());
            }
        }
    }

    public void InitGame(int playersQuantity)
    {
        colorsManager = new ColorsManager(playersQuantity);
        GeneratePlayersPieces();
        SetMenuVisibility(false);
        OnTurnChanged?.Invoke(this, colorsManager.currentColor);
    }

    private void SetMenuVisibility(bool isInMenu)
    {
        UiManager.I.SetActiveMainMenu(isInMenu);
        dice.gameObject.SetActive(!isInMenu);
        Board.I.gameObject.SetActive(!isInMenu);
    }

    #endregion
}