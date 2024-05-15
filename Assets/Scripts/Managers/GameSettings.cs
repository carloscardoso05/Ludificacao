using System;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameSettings : MonoBehaviourPunCallbacks
{
    public static GameSettings Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


    #region Bonus
    [SerializeField]
    private Settings _settings = new();
    public Settings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            SendSettingsDefinedEvent(value);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        SendSettingsDefinedEvent(Settings);
    }

    private void Start()
    {
        Settings.SettingsChanged += (settings) => SendSettingsDefinedEvent(settings);
    }

    #endregion

    public int GetDifficultyBonus(int difficulty)
    {
        return difficulty switch
        {
            0 => Settings.EasyBonus,
            1 => Settings.MediumBonus,
            2 => Settings.HardBonus,
            _ => throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}")
        };
    }

    public float GetDifficultyTimer(int difficulty)
    {
        return difficulty switch
        {
            0 => Settings.EasyTimer,
            1 => Settings.MediumTimer,
            2 => Settings.HardTimer,
            _ => throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}")
        };
    }

    public Action<string> SetDifficultyTimer(int difficulty)
    {
        return (newTimer) =>
        {
            switch (difficulty)
            {
                case 0:
                    Settings.EasyTimer = float.Parse(newTimer); break;
                case 1:
                    Settings.MediumTimer = float.Parse(newTimer); break;
                case 2:
                    Settings.HardTimer = float.Parse(newTimer); break;
                default:
                    throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}");
            }
        };
    }
    public Action<string> SetDifficultyBonus(int difficulty)
    {
        return (newBonus) =>
        {
            switch (difficulty)
            {
                case 0:
                    Settings.EasyBonus = int.Parse(newBonus); break;
                case 1:
                    Settings.MediumBonus = int.Parse(newBonus); break;
                case 2:
                    Settings.HardBonus = int.Parse(newBonus); break;
                default:
                    throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}");
            }
        };
    }

    private void SendSettingsDefinedEvent(Settings settings)
    {
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(NetworkEventManager.SettingsDefined, JsonConvert.SerializeObject(settings), options, SendOptions.SendReliable);
        Debug.Log("Configurações enviadas");
        Debug.Log(JsonConvert.SerializeObject(settings));
    }
}