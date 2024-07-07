using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviourPunCallbacks {
	[SerializeField] private TMP_InputField nickName;
	[SerializeField] private TextMeshProUGUI message;

	public static ConnectionManager Instance;

	private void Awake() {
		DontDestroyOnLoad(gameObject);
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	private void Start() {
		PhotonNetwork.AutomaticallySyncScene = true;
		Application.runInBackground = true;
	}

	public void OnClick_Connect() {
		message.text = "";
		nickName.text = nickName.text.Trim();
		if (nickName.text.Length == 0) {
			message.text = "Insira o nome do jogador";
			return;
		}

		PhotonNetwork.NickName = nickName.text;
		PhotonNetwork.ConnectUsingSettings();
		message.text = "Carregando...";
	}

	public void OnClick_ConnectOffline() {
		PhotonNetwork.OfflineMode = true;
		message.text = "";
		message.text = "Carregando...";
		SceneManager.LoadScene("OfflineGameScene");
	}

	public override void OnConnectedToMaster() {
		if (PhotonNetwork.OfflineMode) return;
		if (PhotonNetwork.InLobby) return;
		PhotonNetwork.JoinLobby();
		SceneManager.LoadScene("Lobby");
	}

	public override async void OnDisconnected(DisconnectCause cause) {
		Debug.Log($"Desconectado. Causa: {cause}");
		Debug.Log("Tentando reentrar em sala");
		while (!PhotonNetwork.ReconnectAndRejoin()) {
			await Task.Delay(2 * 1000);
			Debug.Log("Tentando reentrar em sala");
		}

		Debug.Log("Reentrou na sala com sucesso");
		// SyncState();
		// NetworkEventManager.Instance.Replay();
	}

	public override void OnJoinedRoom() {
		SyncState();
	}

	private static void SyncState() {
		var props = PhotonNetwork.CurrentRoom.CustomProperties;
		Dictionary<GameColor, int[]> piecesPositions;
		Dictionary<GameColor, bool[]> piecesInHome = new();
		GameState gameState;
		int diceValue;
		GameColor currentPlayer;
		if (props.TryGetValue("PiecesInHome", out var piecesInHomeObj)) {
			piecesInHome = JsonConvert.DeserializeObject<Dictionary<GameColor, bool[]>>((string)piecesInHomeObj);
			Debug.Log("Pieces in home: " + (string)piecesInHomeObj);
		}

		if (props.TryGetValue("PiecesPositions", out var piecesPositionsObj)) {
			piecesPositions = JsonConvert.DeserializeObject<Dictionary<GameColor, int[]>>((string)piecesPositionsObj);
			Debug.Log("Pos");
			Debug.Log((string)piecesPositionsObj);
			foreach (var player in GameManager.Instance.players) {
				for (int i = 0; i < 4; i++) {
					var piece = player.pieces[i];
					var inHome = piecesInHome[player.color][i];

					var abs = piecesPositions[piece.color][piece.id];
					Debug.Log($"CurrIndex is {piece.Path.CurrentIndex}");
					Debug.Log($"abs is {abs}");
					Debug.Log($"Moving {piece.color} {piece.id} to:");
					if (inHome) {
						Debug.Log("Home");
						piece.MoveToHome();
					}
					else if (abs == piece.Path.CurrentIndex + 1) {
						Debug.Log("Nowhere, abs is 0");
					}
					else {
						Debug.Log("Index: " + (abs - piece.Path.CurrentIndex - 1));
						piece.Move(abs - piece.Path.CurrentIndex - 1);
					}

					Debug.Log("\n");
				}
			}
		}

		if (props.TryGetValue("DiceValue", out var diceValueObj)) {
			diceValue = (int)diceValueObj;
			Dice.Instance.RollDiceToNum(diceValue);
			Debug.Log("Dice");
			Debug.Log(diceValue);
		}

		if (props.TryGetValue("GameState", out var gameStateObj)) {
			gameState = (GameState)gameStateObj;
			GameManager.Instance.ChangeState(gameState);
			Debug.Log("Gmstate");
			Debug.Log(gameState);
		}

		if (props.TryGetValue("CurrentPlayer", out var currentPlayerObj)) {
			currentPlayer = (GameColor)currentPlayerObj;
			ColorsManager.I.currentColor = currentPlayer;
			GameManager.Instance.SendOnTurnChanged(null, currentPlayer);
			Debug.Log("Current player");
			Debug.Log(currentPlayer);
		}

		Debug.Log("Room props: " + JsonConvert.SerializeObject(props));
	}

	public override async void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
		await Task.Delay(1 * 1000);
		Debug.Log($"Player entered with Id: {newPlayer.UserId}");
		// if (!PhotonNetwork.CurrentRoom.IsOpen) {
		// 	PlayerRejoinedRoomEventData eventData = new() {
		// 		otherGameEvents = NetworkEventManager.Instance.events,
		// 		newPlayerId = newPlayer.UserId
		// 	};
		// 	RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
		// 	PhotonNetwork.RaiseEvent(NetworkEventManager.PlayerRejoinedRoom, JsonConvert.SerializeObject(eventData),
		// 		options, SendOptions.SendReliable);
		// }
	}

	public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
		Debug.Log($"Player left with Id: {otherPlayer.UserId}");
	}

	public class PlayerRejoinedRoomEventData {
		public List<EventData> otherGameEvents;
		public string newPlayerId;
	}
}