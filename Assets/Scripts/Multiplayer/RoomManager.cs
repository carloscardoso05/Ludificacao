using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Multiplayer {
	/// <summary>
	///     Classe responsável pela interface da sala.
	/// </summary>
	/// <remarks>
	///     Também interage com as opções da partida (bônus e temporizadores).
	/// </remarks>
	public class RoomManager : MonoBehaviourPunCallbacks {
		/// <summary>
		///     Instância da classe usada para garantir que somente uma existirá durante o jogo.
		///     Se uma já existir e for tentado criar outra, a outra será destruída.
		/// </summary>
		private static RoomManager _instance;

		private void Awake() {
			DontDestroyOnLoad(gameObject);
			if (_instance == null)
				_instance = this;
			else
				Destroy(gameObject);
		}

		/// <summary>
		///     Cria e entra em uma sala com o nome especificado.
		/// </summary>
		/// <param name="roomName">Nome da sala.</param>
		public static void CreateRoom(string roomName) {
			RoomOptions options = new() {
				MaxPlayers = 4,
				PlayerTtl = -1,
				EmptyRoomTtl = 0
			};
			PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
		}

		/// <summary>
		///     Entra na sala.
		/// </summary>
		/// <param name="roomName">Nome da sala</param>
		public static void JoinRoom(string roomName) {
			PhotonNetwork.JoinRoom(roomName);
		}

		/// <summary>
		///     Sai da sala.
		/// </summary>
		public static void LeaveRoom() {
			PhotonNetwork.LeaveRoom();
		}

		/// <summary>
		///     Inicia a partida no modo online.
		/// </summary>
		public static void StartGame() {
			if (PhotonNetwork.PlayerList.Length <= 1 || !PhotonNetwork.IsMasterClient) return;
			PhotonNetwork.LoadLevel("GameScene");
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}

		/// <summary>
		///     Exibe ou esconde as opções da partida (bônus e temporizadores).
		/// </summary>
		/// <param name="settings">Interface das opções.</param>
		/// <param name="room">Interface da sala.</param>
		/// <param name="isVisible">Se as opções devem ser exibidas.</param>
		public static void SetSettingsVisible(GameObject settings, GameObject room, bool isVisible) {
			if (!PhotonNetwork.IsMasterClient) return;
			settings.SetActive(isVisible);
			room.SetActive(!isVisible);
		}
	}
}