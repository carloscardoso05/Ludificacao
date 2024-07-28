using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer {
	/// <summary>
	///     Classe responsável por gerenciar a entrada e criação de salas.
	/// </summary>
	public class LobbyUIManager : MonoBehaviourPunCallbacks {
		/// <summary>
		///     Canvas do lobby (entrar/criar sala).
		/// </summary>
		[SerializeField] private Canvas lobby;

		/// <summary>
		///     Input do nome da sala.
		/// </summary>
		[SerializeField] private TMP_InputField roomName;

		/// <summary>
		///     <see cref="" />
		/// </summary>
		[SerializeField] private RoomManager roomManager;

		/// <summary>
		///     Botão para entrar na sala.
		/// </summary>
		[SerializeField] private Button joinRoom;

		/// <summary>
		///     Botão para criar a sala.
		/// </summary>
		[SerializeField] private Button createRoom;

		/// <summary>
		///     Mensagem para ser exibida na tela do lobby.
		/// </summary>
		[SerializeField] private TextMeshProUGUI message;

		/// <summary>
		///     Entra na sala.
		/// </summary>
		public void OnClick_JoinRoom() {
			roomName.text = roomName.text.Trim();
			if (!RoomNameIsValid()) return;
			RoomManager.JoinRoom(roomName.text);
			message.text = "Entrando na sala...";
		}

		/// <summary>
		///     Cria a sala.
		/// </summary>
		public void OnClick_CreateRoom() {
			roomName.text = roomName.text.Trim();
			if (!RoomNameIsValid()) return;
			RoomManager.CreateRoom(roomName.text);
			message.text = "Criando sala...";
		}

		/// <summary>
		///     Retorna se o nome da sala é válido (não vazio). Pode exibir uma mensagem de erro.
		/// </summary>
		/// <seealso cref="message" />
		/// <returns>Se o nome da sala é válido.</returns>
		private bool RoomNameIsValid() {
			message.text = "";
			if (roomName.text.Length != 0) return true;
			message.text = "Insira o nome da sala";
			return false;
		}

		/// <summary>
		///     Exibe mensagem de erro caso haja algum ao entrar na sala.
		/// </summary>
		/// <a href="https://doc-api.photonengine.com/en/pun/v1/class_error_code.html">Códigos de erro.</a>
		/// <param name="returnCode">Código do erro.</param>
		/// <param name="errorMessage">Mensagem do erro.</param>
		public override void OnJoinRoomFailed(short returnCode, string errorMessage) {
			message.text = errorMessage;
		}

		public override void OnJoinedRoom() {
			lobby.gameObject.SetActive(false);
			message.text = "";
		}

		public override void OnConnectedToMaster() {
			lobby.gameObject.SetActive(true);
		}
	}
}