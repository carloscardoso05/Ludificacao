using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceVisual))]
public class Piece : MonoBehaviour
{
    public PieceVisual visual;
    public GameColor color;
    public Player player;
    public bool inHome = true;
    public NavigationList<Tile> Path;
    public event EventHandler<PieceMovedArgs> PieceMoved;
    public class PieceMovedArgs : EventArgs
    {
        public Piece piece;
        public bool wasInHome;
        public Tile prevTile;
        public Tile currTile;
        public int tilesMoved;
    }

    private void Start()
    {
        PieceMoved += SendOthersToHome;
        QuizManager.Instance.OnAnswered += MovePiece;
        MoveToHome();
    }

    private void MovePiece(object sender, AnswerData answerData)
    {
        var extraData = (GameManager.ExtraData) answerData.extraData;
        var isNotThisPiece = extraData.selectedPiece.name != name;
        var isNotCorrectAnswer = !answerData.selectedAnswer.correct;
        if (isNotThisPiece) return;
        if (isNotCorrectAnswer && !GameManager.Instance.alwaysAnswerRight) return;
        var times = extraData.diceValue + Settings.I.GetDifficultyBonus(answerData.question.difficulty);
        var prevTile = Path.Current;
        Path.Current.pieces.Remove(this);
        Path.CurrentIndex += inHome ? times - 1 : times;
        Path.Current.pieces.Add(this);
        PieceMovedArgs args = new()
        {
            piece = this,
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
            Piece[] copyPieces = new Piece[Path.Current.pieces.Count];
            Path.Current.pieces.CopyTo(copyPieces);
            foreach (Piece p in copyPieces)
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