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
        QuizManager.Instance.OnAnswered += AddPoints;
    }


    private void AddPoints(object sender, AnswerData answerData)
    {
        if (((GameManager.ExtraData)answerData.extraData).player.name == name)
        {
            var questionPoints = new int[] { 100, 200, 300 };
            points += questionPoints[answerData.question.difficulty];
            UiManager.I.UpdatePoints(this);
        }
    }

    private void InstantiatePieces()
    {
        for (int i = 0; i < 4; i++)
        {
            var piece = Instantiate(piecePrefab).GetComponent<Piece>();
            var visual = piece.GetComponent<PieceVisual>();
            piece.name = color.ToString() + "Piece" + i.ToString();
            piece.transform.parent = transform;
            Vector2 homePosition = GameManager.Instance.board.GetHome(color).transform.position;
            Vector2 offset = GameManager.Instance.board.GetHomeOffset(i);
            visual.HomePosition = homePosition + offset;
            piece.color = color;
            piece.player = this;
            visual.spriteResolver.SetCategoryAndLabel("Body", color.ToString());
        }
    }
}