using System;
using UnityEngine;

[RequireComponent(typeof(PieceVisual))]
public class Piece : MonoBehaviour
{
    public PieceVisual visual;
    public GameColor color;
    public Player player;
    public bool inHome = true;
    private int InitialIndex;
    public NavigationList<Tile> Path;
    public event EventHandler<PieceMovedArgs> PieceMoved;
    public class PieceMovedArgs : EventArgs
    {
        public bool wasInHome;
        public Tile prevTile;
        public Tile currTile;
        public int tilesMoved;
    }

    private void Start()
    {
        PieceMoved += SendOthersToHome;
        QuizManager.Instance.OnAnswered += MovePiece;
        InitialIndex = GameManager.Instance.board.GetInitialIndex(color);
        var whiteTiles = GameManager.Instance.board.GetTiles(GameColor.White);
        var colorTiles = GameManager.Instance.board.GetTiles(color);
        Path = Board.GetPath(whiteTiles, colorTiles, InitialIndex);
        MoveToHome();
    }

    private void MovePiece(object sender, AnswerData answerData)
    {
        var isNotThisPiece = ((GameManager.ExtraData)answerData.extraData).selectedPiece.name != name;
        var isNotCorrectAnswer = !answerData.selectedAnswer.correct;
        if (isNotThisPiece) return;
        if (isNotCorrectAnswer && !GameManager.Instance.alwaysAnswerRight) return;
        var times = ((GameManager.ExtraData)answerData.extraData).diceValue;
        var prevTile = Path.Current;
        Path.Current.pieces.Remove(this);
        Path.CurrentIndex += inHome ? times - 1 : times;
        Path.Current.pieces.Add(this);
        PieceMovedArgs args = new()
        {
            currTile = Path.Current,
            prevTile = prevTile,
            tilesMoved = times,
            wasInHome = inHome
        };
        inHome = false;
        visual.StartAnimation(args);
    }

    private void SendOthersToHome(object sender, EventArgs args)
    {
        if (!Path.Current.isSafe)
        {
            foreach (Piece p in Path.Current.pieces)
            {
                if (p.color != color) p.MoveToHome();
            }
        }
    }

    public void MoveToHome()
    {
        Path.Current.pieces.Remove(this);
        inHome = true;
        Path.CurrentIndex = 0;
        visual.SendHome();
    }

    private static bool CanMove(Piece piece)
    {
        var isThisPlayerTurn = ColorsManager.I.currentColor == piece.color;
        var isSelectingPiece = GameManager.Instance.state == GameState.SelectingPiece;
        var isNotFinalTile = !piece.Path.Current.isFinal;
        return isThisPlayerTurn && isSelectingPiece && isNotFinalTile;
    }

    private void OnMouseUp()
    {
        if (CanMove(this))
        {
            var rnd = UnityEngine.Random.Range(0, 3);
            QuizManager.Instance.ShowQuestion(rnd, new GameManager.ExtraData { selectedPiece = this, player = player, diceValue = GameManager.Instance.dice.Value });

        }
    }

    public void OnPieceMoved(PieceMovedArgs args)
    {
        PieceMoved?.Invoke(this, args);
    }
}