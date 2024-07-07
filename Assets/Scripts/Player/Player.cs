using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject piecePrefab;
    public int points;
    public List<Piece> pieces;
    public GameColor color;
    public Photon.Realtime.Player photonPlayer;

    private void Start()
    {
        QuizManager.Instance.OnAnswered += AddPoints;
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

    public void InstantiatePieces()
    {
        var initialIndex = GameManager.Instance.board.GetInitialIndex(color);
        var whiteTiles = GameManager.Instance.board.GetTiles(GameColor.White);
        var colorTiles = GameManager.Instance.board.GetTiles(color);
        for (int i = 0; i < 4; i++)
        {
            Vector2 offset = GameManager.Instance.board.GetHomeOffset(i);
            Vector2 homePosition = (Vector2)GameManager.Instance.board.GetHome(color).transform.position + offset;
            var pieceName = color.ToString() + "Piece" + i.ToString();
            var path = Board.GetPath(whiteTiles, colorTiles, initialIndex);
            var piece = Instantiate(piecePrefab, homePosition, Quaternion.identity).GetComponent<Piece>();
            piece.name = pieceName;
            var visual = piece.GetComponent<PieceVisual>();
            visual.HomePosition = homePosition + offset;
            piece.transform.parent = transform;
            piece.color = color;
            piece.id = (byte)i;
            piece.player = this;
            piece.Path = path;
            visual.spriteResolver.SetCategoryAndLabel("Body", color.ToString());
            pieces.Add(piece);
        }
    }
}