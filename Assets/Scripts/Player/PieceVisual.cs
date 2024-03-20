using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(Piece))]
public class PieceVisual : MonoBehaviour
{
    [SerializeField] private Piece piece;
    public SpriteResolver spriteResolver;
    public SpriteRenderer _renderer;
    public Vector2 HomePosition;
    private Vector2 tilePosition;
    private Vector2 cursorPosition;
    public event EventHandler OnAnimationEnded;
    private bool animationEnded = false;

    private void Start()
    {
        piece.OnPieceChangedTileEvent += UpdateTileAndCursorPosition;
        OnAnimationEnded += Reorganize;
    }

    private void Update()
    {
        if (!animationEnded)
        {
            bool stepEnded = MovePlayer();
            if (stepEnded)
            {
                if (cursorPosition != tilePosition)
                {
                    piece.Path.CurrentIndex++;
                    cursorPosition = piece.Path.Current.transform.position;
                }
                else
                {
                    animationEnded = true;
                    OnAnimationEnded?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    private bool MovePlayer()
    {
        if ((Vector2)piece.transform.position != cursorPosition)
        {
            piece.transform.position = Vector3.Lerp(transform.position, cursorPosition, Time.deltaTime * 30);
            return false;
        }
        return true;
    }

    public void SendHome()
    {
        tilePosition = HomePosition;
        cursorPosition = HomePosition;
    }

    public void UpdateTileAndCursorPosition(object sender, Piece.OnPieceChangedTileEventArgs args)
    {
        tilePosition = args.currTile.transform.position;
        piece.Path.CurrentIndex = piece.Path.CurrentIndex - args.tilesMoved + 1;
        cursorPosition = piece.Path.Current.transform.position;
        animationEnded = false;
    }

    private List<Vector3> GetOffsetedPositions(Tile tile)
    {
        // var bounds = tile.gameObject.GetComponent<Renderer>().bounds;
        var sizeX = 1;
        var position = tile.gameObject.transform.position;
        var start = position.x - sizeX / 2;
        List<Vector3> offsetedPositions = new();
        for (int i = 1; i < tile.pieces.Count + 1; i++)
        {
            var x = start + sizeX * i / (tile.pieces.Count + 1);
            Vector3 pos = new(x, tile.transform.position.y, tile.transform.position.z);
            offsetedPositions.Add(pos);
        }
        return offsetedPositions;
    }

    private void Reorganize(object sender, EventArgs args)
    {
        if (piece.inHome) return;
        var positions = GetOffsetedPositions(piece.Path.Current);
        for (int i = 0; i < positions.Count; i++)
        {
            piece.Path.Current.pieces[i].transform.position = positions[i];
        }
    }
}