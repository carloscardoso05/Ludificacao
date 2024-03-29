using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Piece;
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private GameObject playerPrefab;
    public GameState state;
    public event EventHandler<GameColor> OnTurnChanged;
    public event EventHandler<UiManager.EndGameArgs> OnGameEnded;
    public Dice dice;
    public Board board;
    public static GameManager Instance;
    private Player[] players;

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
        ChangeState(GameState.SelectingPlayersQnt);
        QuizManager.Instance.OnAnswered += ChangeTurn;
        QuizManager.Instance.OnSelectedQuiz += (sender, quiz) =>
        {
            ChangeState(GameState.RollingDice);
        };
        SetMenuVisibility(true);
    }

    #endregion

    #region Game Mechanics

    public void CheckPlayerWin(object sender, PieceMovedArgs args)
    {
        if (args.currTile.isFinal && args.currTile.pieces.Count == 2)
        {
            OnGameEnded?.Invoke(this, new()
            {
                players = players,
                winner = args.piece.player,
            });
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
        List<Player> playersList = new();
        var playersGO = new GameObject("Players");
        foreach (GameColor color in colors)
        {
            var playerGO = Instantiate(playerPrefab);
            playerGO.transform.parent = playersGO.transform;
            playerGO.name = color.ToString() + "Player";
            var player = playerGO.GetComponent<Player>();
            player.color = color;
            playersList.Add(player);
        }
        players = playersList.ToArray();
    }

    public void InitGame(int playersQuantity)
    {
        ChangeState(GameState.SelectingQuiz);
        ColorsManager.I = new ColorsManager(playersQuantity);
        GeneratePlayers(ColorsManager.I.colors);
        SetMenuVisibility(false);
        QuizManager.Instance.SelectQuiz();
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