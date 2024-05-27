using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nickName;
    [SerializeField] private TextMeshProUGUI message;

    public static ConnectionManager Instance;

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

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Application.runInBackground = true;
    }

    public void OnClick_Connect()
    {
        message.text = "";
        nickName.text = nickName.text.Trim();
        if (nickName.text.Length == 0)
        {
            message.text = "Insira o nome do jogador";
            return;
        }
        PhotonNetwork.NickName = nickName.text;
        PhotonNetwork.ConnectUsingSettings();
        message.text = "Carregando...";
    }

    public void OnClick_ConnectOffline()
    {
        PhotonNetwork.OfflineMode = true;
        message.text = "";
        message.text = "Carregando...";
        SceneManager.LoadScene("OfflineGameScene");
    }

    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.OfflineMode) return;
        if (PhotonNetwork.InLobby) return;
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene("Lobby");
    }

    public override async void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Desconectado. Causa: {cause}");
        Debug.Log("Tentando reentrar em sala");
        while (!PhotonNetwork.ReconnectAndRejoin())
        {
            await Task.Delay(2 * 1000);
            Debug.Log("Tentando reentrar em sala");
        }

        Debug.Log("Reentrou na sala com sucesso");
        // NetworkEventManager.Instance.Replay();
    }

    public override async void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        await Task.Delay(1 * 1000);
        Debug.Log($"Player entered with Id: {newPlayer.UserId}");
        if (!PhotonNetwork.CurrentRoom.IsOpen)
        {
            PlayerRejoinedRoomEventData eventData = new()
            {
                otherGameEvents = NetworkEventManager.Instance.events,
                newPlayerId = newPlayer.UserId
            };
            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(NetworkEventManager.PlayerRejoinedRoom, JsonConvert.SerializeObject(eventData), options, SendOptions.SendReliable);
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"Player left with Id: {otherPlayer.UserId}");
    }

    public class PlayerRejoinedRoomEventData
    {
        public List<EventData> otherGameEvents;
        public string newPlayerId;
    }
}