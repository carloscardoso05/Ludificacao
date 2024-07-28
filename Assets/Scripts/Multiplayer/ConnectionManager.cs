using System.Collections.Generic;
using System.Threading.Tasks;
using LudoPlayer;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer {
	/// <summary>
	///     Classe responsável pela inicialização dos modos de jogo (online e offline) e por sincronizar o estado do jogo com
	///     o estado de jogo da sala (modo online).
	/// </summary>
	public class ConnectionManager : MonoBehaviourPunCallbacks {
		/// <summary>
		///     Instância da classe usada para garantir que somente uma existirá durante o jogo.
		///     Se uma já existir e for tentado criar outra, a outra será destruída.
		/// </summary>
		private static ConnectionManager _instance;

		/// <summary>
		///     Apelido do jogador
		/// </summary>
		[SerializeField] private TMP_InputField nickName;

		/// <summary>
		///     Mensagem para ser exibida na tela de selecionar modo.
		/// </summary>
		[SerializeField] private TextMeshProUGUI message;

		private void Awake() {
			DontDestroyOnLoad(gameObject);
			if (_instance == null)
				_instance = this;
			else
				Destroy(gameObject);
		}

		private void Start() {
			PhotonNetwork.AutomaticallySyncScene = true;
			Application.runInBackground = true;
		}

		/// <summary>
		///     Conecta este cliente ao servidor do jogo.
		/// </summary>
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

		/// <summary>
		///     Inicia o modo offline do jogo e conecta este cliente a uma sala local..
		/// </summary>
		public void OnClick_ConnectOffline() {
			PhotonNetwork.OfflineMode = true;
			message.text = "";
			message.text = "Carregando...";
			PhotonNetwork.CreateRoom("local_room", new RoomOptions {
				PlayerTtl = -1,
				EmptyRoomTtl = -1
			});
		}

		/// <summary>
		///     Carrega a cena do lobby ao conectar ao servidor do jogo.
		/// </summary>
		public override void OnConnectedToMaster() {
			if (PhotonNetwork.OfflineMode) return;
			if (PhotonNetwork.InLobby) return;
			PhotonNetwork.JoinLobby();
			SceneManager.LoadScene("Lobby");
		}

		/// <summary>
		///     Tenta reconectar na sala ao desconectar.
		/// </summary>
		/// <param name="cause">Causa da desconexão.</param>
		public override async void OnDisconnected(DisconnectCause cause) {
			while (!PhotonNetwork.ReconnectAndRejoin())
				await Task.Delay(2 * 1000);
		}

		/// <summary>
		///     Sincroniza o estado do jogo do cliente com o estado do jogo salvo na sala.
		/// </summary>
		/// <remarks>
		///     É executado ao entrar na sala. Ou seja, também é chamado ao reconectar à sala.
		///     Caso esteja no modo offline, carrega a cena de jogo do modo offline.
		/// </remarks>
		public override void OnJoinedRoom() {
			SyncState();
			if (PhotonNetwork.OfflineMode) SceneManager.LoadScene("OfflineGameScene");
		}

		/// <summary>
		///     Sincroniza o estado de jogo deste cliente com o estado de jogo salvo na sala.
		/// </summary>
		/// <remarks>
		///     Sincroniza: <br />
		///     - O estado de jogo - <see cref="GameState" /> <br />
		///     - A posição das peças - (<see cref="LudoPlayer.Player.Piece.Path" />, <see cref="LudoPlayer.Player.Piece.inHome" />) <br />
		///     - O valor do dado - <see cref="Dice.Value" /> <br />
		///     - O jogador da vez - <see cref="ColorsManager.currentColor" /> <br />
		/// </remarks>
		private static void SyncState() {
			var props = PhotonNetwork.CurrentRoom.CustomProperties;
			Dictionary<GameColor, bool[]> piecesInHome = new();
			if (props.TryGetValue("PiecesInHome", out var piecesInHomeObj))
				piecesInHome = JsonConvert.DeserializeObject<Dictionary<GameColor, bool[]>>((string)piecesInHomeObj);


			if (props.TryGetValue("PiecesPositions", out var piecesPositionsObj)) {
				var piecesPositions =
					JsonConvert.DeserializeObject<Dictionary<GameColor, int[]>>((string)piecesPositionsObj);
				foreach (var player in GameManager.Instance.players)
					for (var i = 0; i < 4; i++) {
						var piece = player.pieces[i];
						var inHome = piecesInHome[player.color][i];

						var abs = piecesPositions[piece.color][piece.id];

						if (inHome)
							piece.MoveToHome();

						else if (abs != piece.Path.CurrentIndex + 1)
							piece.Move(abs - piece.Path.CurrentIndex);
					}
			}

			if (props.TryGetValue("DiceValue", out var diceValueObj)) {
				var diceValue = (int)diceValueObj;
				Dice.Instance.RollDiceToNum(diceValue);
			}

			if (props.TryGetValue("GameState", out var gameStateObj)) {
				var gameState = (GameState)gameStateObj;
				GameManager.Instance.ChangeState(gameState);
			}

			if (props.TryGetValue("CurrentPlayer", out var currentPlayerObj)) {
				var currentPlayer = (GameColor)currentPlayerObj;
				ColorsManager.I.currentColor = currentPlayer;
				GameManager.Instance.SendOnTurnChanged(null, currentPlayer);
			}
		}
	}
}