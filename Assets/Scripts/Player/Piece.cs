using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PieceVisual))]
[RequireComponent(typeof(PhotonView))]
public class Piece : MonoBehaviour {
	[SerializeField] PhotonView view;
	public PieceVisual visual;
	public GameColor color;
	public byte id;
	public Player player;
	public bool inHome = true;
	public NavigationList<Tile> Path;
	public event EventHandler<PieceMovedArgs> PieceMoved;

	public class PieceMovedArgs : EventArgs {
		public Piece piece;
		public bool wasInHome;
		public Tile prevTile;
		public Tile currTile;
		public int tilesMoved;
	}

	private void Start() {
		view = GetComponent<PhotonView>();
		PieceMoved += SendOthersToHome;
		QuizManager.Instance.OnAnswered += MovePieceOnAnswer;
		MoveToHome();
	}

	public void MovePieceOnAnswer(object sender, AnswerData answerData) {
		var extraData = (GameManager.ExtraData)answerData.extraData;
		var isNotThisPiece = extraData.selectedPiece.name != name;
		var isNotCorrectAnswer = !answerData.selectedAnswer.correct && answerData.selectedAnswer.id != "0"; // TODO
		if (isNotThisPiece) return;
		if (isNotCorrectAnswer && !GameManager.Instance.alwaysAnswerRight) return;
		var times = extraData.diceValue + GameSettings.Instance.GetDifficultyBonus(answerData.question.difficulty);
		var args = Move(times);
		var piecesPositions = new Dictionary<GameColor, int[]> {
			{ GameColor.Blue, new[] { 0, 0, 0, 0 } },
			{ GameColor.Red, new[] { 0, 0, 0, 0 } },
			{ GameColor.Green, new[] { 0, 0, 0, 0 } },
			{ GameColor.Yellow, new[] { 0, 0, 0, 0 } },
		};
		var piecesInHome = new Dictionary<GameColor, bool[]> {
			{ GameColor.Blue, new[] { true, true, true, true } },
			{ GameColor.Red, new[] { true, true, true, true } },
			{ GameColor.Green, new[] { true, true, true, true } },
			{ GameColor.Yellow, new[] { true, true, true, true } },
		};
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PiecesInHome", out var piecesInHomeObj)) {
			piecesInHome = JsonConvert.DeserializeObject<Dictionary<GameColor, bool[]>>((string)piecesInHomeObj);
		}

		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PiecesPositions", out var piecesPositionsObj)) {
			piecesPositions = JsonConvert.DeserializeObject<Dictionary<GameColor, int[]>>((string)piecesPositionsObj);
		}

		piecesPositions[color][id] = Path.CurrentIndex + (inHome ? times - 1 : times);
		piecesInHome[color][id] = false;
		Debug.Log("Pieces positions before seting on room");
		Debug.Log(JsonConvert.SerializeObject(piecesPositions));
		Debug.Log("Pieces in home before seting on room");
		Debug.Log(JsonConvert.SerializeObject(piecesInHome));
		Debug.Log("Color: " + color);
		Debug.Log("Id: " + id);
		Debug.Log("index: " + piecesPositions[color][id]);
		PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
			{ "PiecesPositions", JsonConvert.SerializeObject(piecesPositions) },
			{ "PiecesInHome", JsonConvert.SerializeObject(piecesInHome) }
		});
	}

	public PieceMovedArgs Move(int times) {
		var prevTile = Path.Current;
		Path.Current.pieces.Remove(this);
		Path.CurrentIndex += inHome ? times - 1 : times;
		Path.Current.pieces.Add(this);
		PieceMovedArgs args = new() {
			piece = this,
			currTile = Path.Current,
			prevTile = prevTile,
			tilesMoved = times,
			wasInHome = inHome
		};
		inHome = false;
		visual.StartAnimation(args);
		return args;
	}

	private void SendOthersToHome(object sender, EventArgs args) {
		if (!Path.Current.isSafe) {
			Piece[] copyPieces = new Piece[Path.Current.pieces.Count];
			Path.Current.pieces.CopyTo(copyPieces);
			foreach (Piece p in copyPieces) {
				if (p.color != color) p.MoveToHome();
			}
		}
	}

	public void MoveToHome() {
		Path.Current.pieces.Remove(this);
		inHome = true;
		Path.CurrentIndex = 0;
		visual.SendHome();
		var piecesPositions = new Dictionary<GameColor, int[]> {
			{ GameColor.Blue, new[] { 0, 0, 0, 0 } },
			{ GameColor.Red, new[] { 0, 0, 0, 0 } },
			{ GameColor.Green, new[] { 0, 0, 0, 0 } },
			{ GameColor.Yellow, new[] { 0, 0, 0, 0 } },
		};
		var piecesInHome = new Dictionary<GameColor, bool[]> {
			{ GameColor.Blue, new[] { true, true, true, true } },
			{ GameColor.Red, new[] { true, true, true, true } },
			{ GameColor.Green, new[] { true, true, true, true } },
			{ GameColor.Yellow, new[] { true, true, true, true } },
		};
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PiecesInHome", out var piecesInHomeObj)) {
			piecesInHome = JsonConvert.DeserializeObject<Dictionary<GameColor, bool[]>>((string)piecesInHomeObj);
		}

		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PiecesPositions", out var piecesPositionsObj)) {
			piecesPositions = JsonConvert.DeserializeObject<Dictionary<GameColor, int[]>>((string)piecesPositionsObj);
		}

		piecesPositions[color][id] = 0;
		piecesInHome[color][id] = true;
		
		PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
			{ "PiecesPositions", JsonConvert.SerializeObject(piecesPositions) },
			{ "PiecesInHome", JsonConvert.SerializeObject(piecesInHome) }
		});
	}

	private static bool CanMove(Piece piece) {
		var isThisPieceTurn = ColorsManager.I.currentColor == piece.color;
		var isSelectingPiece = GameManager.Instance.state == GameState.SelectingPiece;
		var isNotFinalTile = !piece.Path.Current.isFinal;
		return isThisPieceTurn && isSelectingPiece && isNotFinalTile;
	}

	private void OnMouseUp() {
		if (GameManager.Instance.state != GameState.SelectingPiece) return;
		if (!PhotonNetwork.OfflineMode) {
			var localColor = (GameColor)PhotonNetwork.LocalPlayer.CustomProperties["Color"];
			var isNotThisPlayerTurn = ColorsManager.I.currentColor != localColor;
			if (isNotThisPlayerTurn && !PhotonNetwork.OfflineMode) return;
		}

		if (CanMove(this)) {
			var data = new GameManager.ExtraData
				{ selectedPiece = this, player = player, diceValue = GameManager.Instance.dice.Value };
			QuizManager.Instance.ShowRandomQuestion(data, color);
			return;
		}

		foreach (Piece p in Path.Current.pieces) {
			if (CanMove(p)) {
				var data = new GameManager.ExtraData
					{ selectedPiece = p, player = p.player, diceValue = GameManager.Instance.dice.Value };
				QuizManager.Instance.ShowRandomQuestion(data, p.color);
				break;
			}
		}
	}

	public void OnPieceMoved(PieceMovedArgs args) {
		PieceMoved?.Invoke(this, args);
	}
}