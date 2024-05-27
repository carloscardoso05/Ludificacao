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
        // Application.runInBackground = true;
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Desconectado. Causa: {cause}");
        Debug.Log("Tentando reentrar em sala");
        if (PhotonNetwork.ReconnectAndRejoin())
        {
            Debug.Log("Reentrou na sala com sucesso");
            NetworkEventManager.Instance.Replay();
        }

    }
}