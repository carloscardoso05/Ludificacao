using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PieceVisual))]
public class Piece : MonoBehaviour
{
    [SerializeField] private PieceVisual visual;
    public GameColor color;
    public bool inHome = true;
    private int InitialIndex;
    public NavigationList<Tile> Path;
    public event EventHandler<OnPieceChangedTileEventArgs> OnPieceChangedTileEvent;
    public class OnPieceChangedTileEventArgs : EventArgs
    {
        public Tile prevTile;
        public Tile currTile;
        public int tilesMoved;
    }

    private void Start()
    {
        visual.OnAnimationEnded += SendOthersToHome;
        InitialIndex = Board.I.GetInitialIndex(color);
        var whiteTiles = Board.I.GetTiles(GameColor.White);
        var colorTiles = Board.I.GetTiles(color);
        Path = Board.GetPath(whiteTiles, colorTiles, InitialIndex);
        MoveToHome();
    }

    public void MoveToNextTile(int times)
    {
        var prevTile = Path.Current;
        Path.Current.pieces.Remove(this);
        Path.CurrentIndex += times;
        if (inHome) Path.CurrentIndex -= 1;
        Path.Current.pieces.Add(this);
        inHome = false;
        OnPieceChangedTileEventArgs args = new()
        {
            currTile = Path.Current,
            prevTile = prevTile,
            tilesMoved = times,
        };
        OnPieceChangedTileEvent?.Invoke(this, args);
    }

    private void SendOthersToHome(object sender, EventArgs args)
    {
        if (Path.Current.isSafe)
        {
            foreach (Piece p in Path.Current.pieces)
            {
                if (p.color != color) p.MoveToHome();
            }
        }
    }

    public void MoveToHome()
    {
        inHome = true;
        Path.CurrentIndex = 0;
        visual.SendHome();
    }

    // private void SendOthersToHome()
    // {
    //     Path.Current.players.ForEach((p) =>
    //     {
    //         if (p.color != color) p.MoveToHome();
    //     });
    // }


    private static bool CanMove(Piece piece)
    {
        return GameManager.I.colorsManager.currentColor == piece.color && GameManager.I.diceWasRolled && piece.Path.Next != piece.Path.Current;
    }

    private void OnMouseUp()
    {
        if (inHome && CanMove(this))
        {
            GameManager.I.MovePiece(this);
            return;
        }

        // Tenta aplicar a ação de mover a cada peça na casa, assim não há problemas com sobreposição
        foreach (Piece piece in Path.Current.pieces)
        {
            if (CanMove(piece))
            {
                GameManager.I.MovePiece(piece);
                break;
            }
        }
    }
}