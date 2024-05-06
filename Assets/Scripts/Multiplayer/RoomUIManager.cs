using EnumExtension;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Canvas roomCanvas;
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private Button exitRoom;
    [SerializeField] private Button settings;
    [SerializeField] private Button startGame;
    [SerializeField] private TextMeshProUGUI roomName;
    private float roomUpdateTimer = 1.5f;

    private void Start()
    {
        exitRoom.onClick.AddListener(() => roomManager.LeaveRoom());

        startGame.onClick.AddListener(() =>
        {
            if (PhotonNetwork.PlayerList.Length > 1)
            {
                roomManager.StartGame();
            }
        });
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
            for (int i = 0; i < players.Length; i++)
            {
                var nickName = players[i].NickName;
                var color = ColorsManager.GetColorsByPlayersQty(4)[i];
                var playerText = transform.Find("Players").Find($"Player{i + 1}");
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