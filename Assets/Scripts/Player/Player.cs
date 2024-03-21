using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject piecePrefab;
    public int points;
    public List<Piece> pieces;
    public GameColor color;

    private void Start()
    {
        InstantiatePieces();
    }

    private void InstantiatePieces()
    {
        for (int i = 0; i < 4; i++)
        {
            var piece = Instantiate(piecePrefab).GetComponent<Piece>();
            var visual = piece.GetComponent<PieceVisual>();
            piece.name = "Piece" + i.ToString();
            piece.transform.parent = transform;
            Vector2 homePosition = Board.I.GetHome(color).transform.position;
            Vector2 offset = Board.I.GetHomeOffset(i);
            visual.HomePosition = homePosition + offset;
            piece.color = color;
            piece.player = this;
            visual.spriteResolver.SetCategoryAndLabel("Body", color.ToString());
        }
    }
}