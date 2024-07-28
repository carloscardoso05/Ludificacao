using System.Collections.Generic;
using UnityEngine;

namespace LudoPlayer {
	/// <summary>
	///     Jogador do jogo.
	/// </summary>
	public class Player : MonoBehaviour {
		/// <summary>
		///     Prefab da peça.
		/// </summary>
		[SerializeField] private GameObject piecePrefab;

		/// <summary>
		///     Pontuação do jogador.
		/// </summary>
		public int points;

		/// <summary>
		///     Peças do jogador.
		/// </summary>
		public List<Piece> pieces;

		/// <summary>
		///     Cor do jogador.
		/// </summary>
		/// <seealso cref="GameColor" />
		public GameColor color;


		private void Start() {
			QuizManager.Instance.OnAnswered += AddPoints;
		}

		/// <summary>
		///     Incrementa a pontuação do jogador conforme a resposta.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="answerData">Informações da resposta.</param>
		private void AddPoints(object sender, AnswerData answerData) {
			if (((GameManager.ExtraData)answerData.extraData).player.name != name ||
			    !answerData.selectedAnswer.correct) return;
			var questionPoints = new[] { 100, 200, 300 };
			points += questionPoints[answerData.question.difficulty];
			UiManager.I.UpdatePoints(this);
		}

		/// <summary>
		///     Instancia as peças do jogador em suas posições iniciais.
		/// </summary>
		public void InstantiatePieces() {
			var initialIndex = GameManager.Instance.board.GetInitialIndex(color);
			var whiteTiles = GameManager.Instance.board.GetTiles(GameColor.White);
			var colorTiles = GameManager.Instance.board.GetTiles(color);
			for (var i = 0; i < 4; i++) {
				var offset = GameManager.Instance.board.GetHomeOffset(i);
				var homePosition = (Vector2)GameManager.Instance.board.GetHome(color).transform.position + offset;
				var pieceName = color + "Piece" + i;
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
}