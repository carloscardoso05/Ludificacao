using EnumExtension;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject SettingsUIManager;
    [SerializeField] private Canvas roomCanvas;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private Button exitRoom;
    [SerializeField] private Button settings;
    [SerializeField] private Button startGame;
    [SerializeField] private Button settingsClose;
    [SerializeField] private TextMeshProUGUI roomName;
    private float roomUpdateTimer = 1.5f;

    private void Start()
    {
        exitRoom.onClick.AddListener(() => roomManager.LeaveRoom());

        startGame.onClick.AddListener(() => roomManager.StartGame());

        settings.onClick.AddListener(() => roomManager.OpenSettings(SettingsUIManager, roomCanvas.gameObject));

        settingsClose.onClick.AddListener(() => roomManager.CloseSettings(SettingsUIManager, roomCanvas.gameObject));
    }

    private void Update()
    {
        HandleUpdateRoom();
    }

    private void HandleUpdateRoom()
    {
        if (roomUpdateTimer <= 0)
        {
            roomUpdateTimer = 1.5f;
            UpdateRoom();
        }
        roomUpdateTimer -= Time.deltaTime;
    }

    private void UpdateRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            var players = PhotonNetwork.PlayerList;
            for (int i = 0; i < 4; i++)
            {
                var color = ColorsManager.GetColorsByPlayersQty(4)[i];
                var playerText = transform.Find("Players").Find($"Player{i + 1}");
                if (i >= players.Length)
                {
                    playerText.GetComponent<TextMeshProUGUI>().text = "";
                    continue;
                }
                var nickName = players[i].NickName;
                playerText.GetComponent<TextMeshProUGUI>().text = $"{nickName} - {color.ToStringPtBr()}";
            }
        }
    }

    public override void OnJoinedRoom()
    {
        UpdateRoom();
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        roomCanvas.gameObject.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        roomCanvas.gameObject.SetActive(false);
    }
}