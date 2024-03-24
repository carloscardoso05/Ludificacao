using System;
using System.Linq;
using UnityEngine;
using static Piece;
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private GameObject playerPrefab;
    public GameState state;
    public event EventHandler<GameColor> OnTurnChanged;
    public event EventHandler<Piece> OnGameEnded;
    public Dice dice;
    public Board board;
    public static GameManager Instance;

    #region Tester Tools

    public bool alwaysAnswerRight = false;
    public bool directMove = false;

    #endregion

    public class ExtraData
    {
        public int diceValue;
        public Piece selectedPiece;
        public Player player;
    }

    #region Unity Life Cycle

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetMenuVisibility(false);
        ChangeState(GameState.SelectingQuiz);
        QuizManager.Instance.OnAnswered += ChangeTurn;
        QuizManager.Instance.OnSelectedQuiz += (sender, quiz) =>
        {
            SetMenuVisibility(true);
            ChangeState(GameState.SelectingPlayersQnt);
        };
        QuizManager.Instance.SelectQuiz();
    }

    #endregion

    #region Game Mechanics

    public void CheckPlayerWin(object sender, PieceMovedArgs args)
    {
        if (args.currTile.isFinal && args.currTile.pieces.Count == 2)
        {
            OnGameEnded?.Invoke(this, args.currTile.pieces.First());
        }
    }

    private void ChangeTurn(object sender, AnswerData answerData)
    {
        if (dice.Value != 6 || !answerData.selectedAnswer.correct)
        {
            ColorsManager.I.UpdateColor();
        }
        ChangeState(GameState.RollingDice);
        OnTurnChanged?.Invoke(this, ColorsManager.I.currentColor);
    }

    #endregion

    #region Misc

    private void GeneratePlayers(GameColor[] colors)
    {
        var players = new GameObject("Players");
        foreach (GameColor color in colors)
        {
            var playerGO = Instantiate(playerPrefab);
            playerGO.transform.parent = players.transform;
            playerGO.name = color.ToString() + "Player";
            var player = playerGO.GetComponent<Player>();
            player.color = color;
        }
    }

    public void InitGame(int playersQuantity)
    {
        ColorsManager.I = new ColorsManager(playersQuantity);
        GeneratePlayers(ColorsManager.I.colors);
        SetMenuVisibility(false);
        ChangeState(GameState.RollingDice);
        OnTurnChanged?.Invoke(this, ColorsManager.I.currentColor);
    }

    private void SetMenuVisibility(bool isInMenu)
    {
        UiManager.I.SetActiveMainMenu(isInMenu);
        dice.gameObject.SetActive(!isInMenu);
        board.gameObject.SetActive(!isInMenu);
    }

    public void ChangeState(GameState newState)
    {
        state = newState;
    }

    #endregion
}