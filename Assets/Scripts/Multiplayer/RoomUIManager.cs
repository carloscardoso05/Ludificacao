using EnumExtension;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer {
	/// <summary>
	/// Classe responsável por gerenciar a interface da sala.
	/// </summary>
	public class RoomUIManager : MonoBehaviourPunCallbacks {
		[SerializeField] private GameObject settingsUIManager;

		/// <summary>
		///     Canvas da tela da sala.
		/// </summary>
		[SerializeField] private Canvas roomCanvas;

		[SerializeField] private RoomManager roomManager;

		/// <summary>
		///     Botão para sair da sala.
		/// </summary>
		[SerializeField] private Button exitRoom;

		/// <summary>
		///     Botão para abrir a tela das opções.
		/// </summary>
		[SerializeField] private Button settings;

		/// <summary>
		///     Botão para iniciar a partida.
		/// </summary>
		[SerializeField] private Button startGame;

		/// <summary>
		///     Botão para fechar a tela das opções.
		/// </summary>
		[SerializeField] private Button settingsClose;

		/// <summary>
		///     Campo de exibição do nome da sala
		/// </summary>
		[SerializeField] private TextMeshProUGUI roomName;

		/// <summary>
		///     Tempo em segundos para atualizar a lista de jogadores.
		/// </summary>
		/// <seealso cref="HandleUpdateRoom" />
		private float _roomUpdateTimer = 1.5f;

		private void Start() {
			exitRoom.onClick.AddListener(RoomManager.LeaveRoom);

			startGame.onClick.AddListener(RoomManager.StartGame);

			settings.onClick.AddListener(() =>
				RoomManager.SetSettingsVisible(settingsUIManager, roomCanvas.gameObject, true));

			settingsClose.onClick.AddListener(() =>
				RoomManager.SetSettingsVisible(settingsUIManager, roomCanvas.gameObject, false));
		}

		private void Update() {
			HandleUpdateRoom();
		}

		/// <summary>
		///     Executa <see cref="UpdateRoomPlayersList" /> quando o temporizador chega a 0.
		/// </summary>
		private void HandleUpdateRoom() {
			if (_roomUpdateTimer <= 0) {
				_roomUpdateTimer = 1.5f;
				UpdateRoomPlayersList();
			}

			_roomUpdateTimer -= Time.deltaTime;
		}

		/// <summary>
		///     Atualiza a lista dos jogadores na sala.
		/// </summary>
		private void UpdateRoomPlayersList() {
			if (!PhotonNetwork.InRoom) return;
			var players = PhotonNetwork.PlayerList;
			for (var i = 0; i < 4; i++) {
				var color = ColorsManager.GetColorsByPlayersQty(4)[i];
				var playerText = transform.Find("Players").Find($"Player{i + 1}");
				if (i >= players.Length) {
					playerText.GetComponent<TextMeshProUGUI>().text = "";
					continue;
				}

				var nickName = players[i].NickName;
				playerText.GetComponent<TextMeshProUGUI>().text = $"{nickName} - {color.ToStringPtBr()}";
			}
		}

		public override void OnJoinedRoom() {
			UpdateRoomPlayersList();
			roomName.text = PhotonNetwork.CurrentRoom.Name;
			roomCanvas.gameObject.SetActive(true);
		}

		public override void OnConnectedToMaster() {
			roomCanvas.gameObject.SetActive(false);
		}
	}
}