using System;
using System.Collections.Generic;
using System.Linq;
using LudoPlayer;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using static LudoPlayer.Piece;

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
        GetComponentInChildren<TextMeshPro>().text = (int.Parse(name.Last().ToString()) + 1).ToString();
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
                    if (!piece.inHome)
                    {
                        piece.OnPieceMoved(pieceChangedTileArgs);
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Move a peça do jogador a cada frame quando a animação está acontecendo.
    /// </summary>
    /// <returns>Se a etapa (anda uma casa de cada vez) da animação terminou</returns>
    private bool MovePlayer()
    {
        if ((Vector2)piece.transform.position != cursorPosition)
        {
            piece.transform.position = Vector3.Lerp(transform.position, cursorPosition, Time.deltaTime * 30);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Envia esta peça para sua casa de origem.
    /// </summary>
    public void SendHome()
    {
        tilePosition = HomePosition;
        cursorPosition = HomePosition;
        animationEnded = false;
    }

    /// <summary>
    /// Inicia a animação de movimento da peça.
    /// </summary>
    /// <param name="args">Argumentos para iniciar a animação.</param>
    public void StartAnimation(PieceMovedArgs args)
    {
        tilePosition = args.CurrTile.transform.position;
        piece.Path.CurrentIndex -= args.TilesMoved - 1;
        cursorPosition = piece.Path.Current.transform.position;
        animationEnded = false;
        pieceChangedTileArgs = args;
    }

    /// <summary>
    /// Calcula as posições deslocadas das peças quando hão várias em uma mesma casa.
    /// </summary>
    /// <param name="tile">Casa em que as peças estão.</param>
    /// <returns>As posições com um espaçamento entre si.</returns>
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

    /// <summary>
    /// Reorganiza as peças em suas posições deslocadas quando existem várias peças na mesma casa.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
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