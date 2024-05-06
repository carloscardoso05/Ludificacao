using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Canvas lobby;
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private Button joinRoom;
    [SerializeField] private Button createRoom;
    [SerializeField] private TextMeshProUGUI message;

    public void OnClick_JoinRoom()
    {
        roomName.text = roomName.text.Trim();
        if (RoomNameIsNotEmpty())
        {
            roomManager.JoinRoom(roomName.text);
            message.text = "Entrando na sala...";
        }
    }

    public void OnClick_CreateRoom()
    {
        roomName.text = roomName.text.Trim();
        if (RoomNameIsNotEmpty())
        {
            roomManager.CreateRoom(roomName.text);
            message.text = "Criando sala...";
        }
    }

    private bool RoomNameIsNotEmpty()
    {
        message.text = "";
        if (roomName.text.Length == 0)
        {
            message.text = "Insira o nome da sala";
            return false;
        }
        return true;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        this.message.text = message;
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Saiu");
    }

    public override void OnJoinedRoom()
    {
        lobby.gameObject.SetActive(false);
        message.text = "";
    }

    public override void OnConnectedToMaster()
    {
        lobby.gameObject.SetActive(true);
    }
}