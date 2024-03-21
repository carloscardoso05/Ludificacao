using System;
using UnityEngine;

public enum GameState { SelectPlayersNumber, RollingDice, SelectPiece, End };
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private GameObject playerPrefab;
    public static GameManager I;
    public GameState state = GameState.SelectPlayersNumber;
    public event EventHandler<GameColor> OnTurnChanged;
    public event EventHandler<Piece> OnGameEnded;

    public class ExtraData
    {
        public Piece selectedPiece;
        public Player player;
    }

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
        QuizManager.I.SelectQuestion(rnd, new ExtraData { selectedPiece = piece, player = piece.player });
    }

    private void HandleAnswer(object sender, AnswerData answerData)
    {
        var currentColor = ColorsManager.I.currentColor;
        if (answerData.selectedAnswer.correct)
        {
            ExtraData extraData = (ExtraData)answerData.extraData;
            Piece selectedPiece = extraData.selectedPiece;
            Player player = extraData.player;

            selectedPiece.MoveToNextTile(Dice.I.value + Settings.I.GetDifficultyBonus(answerData.question.difficulty));
            var pieceTile = selectedPiece.Path.Current;
            var questionPoints = new int[] { 100, 200, 300 };

            player.points += questionPoints[answerData.question.difficulty];
            UiManager.I.UpdatePoints(currentColor, player.points);
            if (pieceTile.isFinal && pieceTile.pieces.Count == 2) {
                OnGameEnded?.Invoke(this, selectedPiece);
            }
        }
        ColorsManager.I.UpdateColor();
        OnTurnChanged?.Invoke(this, currentColor);
        Dice.I.wasRolled = false;
    }

    #endregion

    #region Misc

    private void GeneratePlayers()
    {
        var players = new GameObject("Players");
        foreach (GameColor color in ColorsManager.I.colors)
        {
            var playerGO = Instantiate(playerPrefab);
            playerGO.transform.parent = players.transform;
            playerGO.name = color.ToString() + "Player";
            playerGO.GetComponent<Player>().color = color;
        }
    }

    public void InitGame(int playersQuantity)
    {
        ColorsManager.I = new ColorsManager(playersQuantity);
        GeneratePlayers();
        SetMenuVisibility(false);
        OnTurnChanged?.Invoke(this, ColorsManager.I.currentColor);
    }

    private void SetMenuVisibility(bool isInMenu)
    {
        UiManager.I.SetActiveMainMenu(isInMenu);
        Dice.I.gameObject.SetActive(!isInMenu);
        Board.I.gameObject.SetActive(!isInMenu);
    }

    #endregion
}