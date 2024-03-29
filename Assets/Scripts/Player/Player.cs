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
        QuizManager.Instance.OnAnswered += AddPoints;
        InstantiatePieces();
    }


    private void AddPoints(object sender, AnswerData answerData)
    {
        if (((GameManager.ExtraData)answerData.extraData).player.name == name && answerData.selectedAnswer.correct)
        {
            var questionPoints = new int[] { 100, 200, 300 };
            points += questionPoints[answerData.question.difficulty];
            UiManager.I.UpdatePoints(this);
        }
    }

    private void InstantiatePieces()
    {
        var initialIndex = GameManager.Instance.board.GetInitialIndex(color);
        var whiteTiles = GameManager.Instance.board.GetTiles(GameColor.White);
        var colorTiles = GameManager.Instance.board.GetTiles(color);
        for (int i = 0; i < 4; i++)
        {
            var path = Board.GetPath(whiteTiles, colorTiles, initialIndex);
            var piece = Instantiate(piecePrefab).GetComponent<Piece>();
            var visual = piece.GetComponent<PieceVisual>();
            piece.name = color.ToString() + "Piece" + i.ToString();
            piece.transform.parent = transform;
            Vector2 homePosition = GameManager.Instance.board.GetHome(color).transform.position;
            Vector2 offset = GameManager.Instance.board.GetHomeOffset(i);
            visual.HomePosition = homePosition + offset;
            piece.color = color;
            piece.player = this;
            piece.Path = path;
            visual.spriteResolver.SetCategoryAndLabel("Body", color.ToString());
            pieces.Add(piece);
        }
    }
}