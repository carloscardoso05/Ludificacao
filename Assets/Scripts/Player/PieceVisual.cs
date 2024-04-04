using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.U2D.Animation;
using static Piece;

[RequireComponent(typeof(Piece))]
public class PieceVisual : MonoBehaviour
{
    [SerializeField] private Piece piece;
    public SpriteResolver spriteResolver;
    public SpriteRenderer _renderer;
    public Vector2 HomePosition;
    [SerializeField] private Vector2 tilePosition;
    [SerializeField] private Vector2 cursorPosition;
    [SerializeField] private bool animationEnded = false;
    [SerializeField] private PieceMovedArgs pieceChangedTileArgs;

    private void Start()
    {
        piece.PieceMoved += Reorganize;
        piece.PieceMoved += GameManager.Instance.CheckPlayerWin;
    }

    private void Update()
    {
        if (!animationEnded && PhotonNetwork.IsMasterClient)
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
                    if (!piece.inHome)
                    {
                        piece.OnPieceMoved(pieceChangedTileArgs);
                    }
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
        animationEnded = false;
    }

    public void StartAnimation(PieceMovedArgs args)
    {
        tilePosition = args.currTile.transform.position;
        piece.Path.CurrentIndex -= args.tilesMoved - 1;
        cursorPosition = piece.Path.Current.transform.position;
        animationEnded = false;
        pieceChangedTileArgs = args;
    }

    private List<Vector3> GetOffsetedPositions(Tile tile)
    {
        float spacing = 1.5f;
        float sizeX = spacing * 1 / 3;
        var position = tile.gameObject.transform.position;
        var start = position.x - sizeX / 2;
        List<Vector3> offsetedPositions = new();
        for (float i = 1; i < tile.pieces.Count + 1; i++)
        {
            float x = start + sizeX * i / (tile.pieces.Count + 1);
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
            var currPiece = piece.Path.Current.pieces[i];
            currPiece.transform.position = positions[i];
        }
    }
}