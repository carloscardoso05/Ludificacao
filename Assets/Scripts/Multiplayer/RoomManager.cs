using Photon.Pun;
using Photon.Realtime;

public class RoomManager : SingletonMonoBehaviour<MonoBehaviourPunCallbacks>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public Room Room
    {
        get => PhotonNetwork.CurrentRoom;
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new()
        {
            MaxPlayers = 4
        };
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
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
        PhotonNetwork.LoadLevel("GameScene");
    }
}
