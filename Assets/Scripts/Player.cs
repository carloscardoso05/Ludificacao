using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;


public class Piece : MonoBehaviour
{
    public SpriteResolver spriteResolver;
    public SpriteRenderer _renderer;
    public GameColor color;
    public Vector2 HomePosition;
    public bool inHome = true;
    private int InitialIndex;
    public NavigationList<Tile> Path;
    private Vector2 tilePosition;

    private void Start()
    {
        InitialIndex = Board.I.GetInitialIndex(color);
        var whiteTiles = Board.I.GetTiles(GameColor.White);
        var colorTiles = Board.I.GetTiles(color);
        Path = Board.GetPath(whiteTiles, colorTiles, InitialIndex);
        MoveToHome();
    }

    public void MoveToNextTile(int times)
    {
        Path.Current.players.Remove(this);
        Path.CurrentIndex++;
        if (inHome) Path.CurrentIndex = 0;
        Path.Current.players.Add(this);
        tilePosition = Path.Current.transform.position;
        inHome = false;
        if (!Path.Current.isSafe) SendOthersToHome();
        if (times > 1)
        {
            MoveToNextTile(times - 1);
        }
    }

    public void MoveToHome()
    {
        inHome = true;
        Path.CurrentIndex = 0;
        tilePosition = HomePosition;
    }

    private void SendOthersToHome()
    {
        Path.Current.players.ForEach((p) =>
        {
            if (p.color != color) p.MoveToHome();
        });
    }

    private void Update() => MovePlayer();

    private void MovePlayer()
    {
        if ((Vector2)transform.position != tilePosition)
            transform.position = Vector3.Lerp(transform.position, tilePosition, Time.deltaTime * 12);
    }

    private void OnMouseUp()
    {
        if (GameManager.I.colorsManager.currentColor == color) 
        GameManager.I.MovePiece(this);
    }
}