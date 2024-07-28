using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using LudoPlayer;
using Managers;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static GameManager;
using Player = LudoPlayer.Player;

public class NetworkEventManager : MonoBehaviour
{
    public static NetworkEventManager Instance;

    public const byte QuestionAnsweredEventCode = 1;
    public const byte QuizSelectedEventCode = 2;
    public const byte DiceRolled = 3;
    public const byte SettingsDefined = 4;
    public const byte PlayerJoinedRoom = 5;
    public const byte PlayerRejoinedRoom = 6;
    public List<EventData> events = new();

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

    private void Start()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            QuizManager.Instance.OnAnswered += SendQuestionAnsweredEvent;
            QuizManager.Instance.OnSelectedQuiz += SendQuizSelectedEvent;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnNetworkEvent;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnNetworkEvent;
    }

    private void OnNetworkEvent(EventData eventData)
    {
        HandleEvent(eventData, true);
    }

    private void HandleEvent(EventData eventData, bool addToList)
    {
        if (addToList)
            events.Add(eventData);
        byte eventCode = eventData.Code;
        if (eventCode == QuestionAnsweredEventCode)
        {
            SimpleAnswerData simpleAnswerData = JsonConvert.DeserializeObject<SimpleAnswerData>(eventData.CustomData.ToString());
            ExtraData extraData = new()
            {
                diceValue = simpleAnswerData.diceValue,
                player = Resources.FindObjectsOfTypeAll<Player>().Where((p) => p.name == simpleAnswerData.playerName).Single(),
                selectedPiece = Resources.FindObjectsOfTypeAll<Piece>().Where((p) => p.name == simpleAnswerData.selectedPieceName).Single(),
            };
            AnswerData answerData = new(simpleAnswerData.question, simpleAnswerData.selectedAnswer, simpleAnswerData.elapsedTime, extraData);
            var whoAnsweredIsNotThis = extraData.player.color != (GameColor)PhotonNetwork.LocalPlayer.CustomProperties["Color"];
            QuizManager.Instance.SendAnswerEvent(eventData.Sender, answerData);
        }
        if (eventCode == QuizSelectedEventCode)
        {
            QuizzesListUI.Instance.SetQuiz(eventData.CustomData.ToString());
        }
        if (eventCode == DiceRolled)
        {
            int diceNum = (int)eventData.CustomData;
            Dice.Instance.RollDiceToNum(diceNum);
        }
        if (eventCode == SettingsDefined)
        {
            Settings settings = JsonConvert.DeserializeObject<Settings>(eventData.CustomData.ToString());
            GameSettings.Instance.settings.ChangeSettings(settings, false);
        }
    }

    private void SendQuestionAnsweredEvent(object sender, AnswerData answerData)
    {
        ExtraData extraData = (ExtraData)answerData.extraData;
        var whoAnsweredIsThis = extraData.player.color == (GameColor)PhotonNetwork.LocalPlayer.CustomProperties["Color"];
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        if (whoAnsweredIsThis)
        {
            SimpleAnswerData simpleAnswerData = new()
            {
                elapsedTime = answerData.elapsedTime,
                question = answerData.question,
                selectedAnswer = answerData.selectedAnswer,
                diceValue = extraData.diceValue,
                playerName = extraData.player.name,
                selectedPieceName = extraData.selectedPiece.name,
            };
            var data = JsonConvert.SerializeObject(simpleAnswerData);
            PhotonNetwork.RaiseEvent(QuestionAnsweredEventCode, data, options, SendOptions.SendReliable);
        }
    }

    private void SendQuizSelectedEvent(object sender, Quiz quiz)
    {
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RaiseEvent(QuizSelectedEventCode, quiz.id, options, SendOptions.SendReliable);
        }
    }

    public void Replay(List<EventData> otherGameEvents)
    {
        events.Skip(otherGameEvents.Count).ToList().ForEach((@event) => HandleEvent(@event, false));
    }
}