using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Room Room
    {
        get => PhotonNetwork.CurrentRoom;
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new()
        {
            MaxPlayers = 4,
            PlayerTtl = 60 * 1000,
        };
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        Debug.Log("Master client aqui");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        Debug.Log($"IsMasterClient: {PhotonNetwork.IsMasterClient}");
        if (PhotonNetwork.PlayerList.Length > 1 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public void OpenSettings(GameObject settings, GameObject room)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        settings.SetActive(true);
        room.SetActive(false);
    }

    public void CloseSettings(GameObject settings, GameObject room)
    {
        settings.SetActive(false);
        room.SetActive(true);
    }
}
