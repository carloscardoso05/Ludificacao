using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public EndGame EndGameScreen;
    public CanvasRenderer Settings;
    public CanvasRenderer MainMenu;
    public static UiManager I;
    public PlayerIcon playersInfo;
    private TextMeshPro BluePoints;
    private TextMeshPro RedPoints;
    private TextMeshPro GreenPoints;
    private TextMeshPro YellowPoints;
    public class EndGameArgs
    {
        public Player winner;
        public Player[] players;
    }

    private void Awake()
    {
        I = this;
    }

    void Start()
    {
        GameManager.Instance.OnGameEnded += EndGame;
        BluePoints = playersInfo.transform.Find("BlueInfo").GetComponentInChildren<TextMeshPro>();
        RedPoints = playersInfo.transform.Find("RedInfo").GetComponentInChildren<TextMeshPro>();
        GreenPoints = playersInfo.transform.Find("GreenInfo").GetComponentInChildren<TextMeshPro>();
        YellowPoints = playersInfo.transform.Find("YellowInfo").GetComponentInChildren<TextMeshPro>();
    }

    private void EndGame(object sender, EndGameArgs args)
    {
        Dictionary<GameColor, string> colorsPtBr = new() {
            {GameColor.Blue, "Azul"},
            {GameColor.Red, "Vermelho"},
            {GameColor.Green, "Verde"},
            {GameColor.Yellow, "Amarelo"},
        };

        EndGameScreen.transform.Find("Nome").GetComponent<TextMeshProUGUI>().text = $"Jogador {colorsPtBr[args.winner.color]} ganhou !";
        
        var sortedPlayers = args.players.ToList().OrderByDescending((p) => p.points).ToArray();
        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            var player = sortedPlayers[i];
            var rankingText = EndGameScreen.transform.Find("Ranking").Find($"{i + 1}Lugar").GetComponent<TextMeshProUGUI>();
            rankingText.text = $"{i + 1}º Lugar - {colorsPtBr[player.color]} - {player.points} pts";
        }

        ShowEndGame(args.winner.color);
    }

    public void ShowSettings()
    {
        MainMenu.gameObject.SetActive(false);
        Settings.gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        MainMenu.gameObject.SetActive(true);
        Settings.gameObject.SetActive(false);
    }

    public void ShowMainMenu() => MainMenu.gameObject.SetActive(true);
    public void HideMainMenu() => MainMenu.gameObject.SetActive(false);
    public void SetActiveMainMenu(bool active) => MainMenu.gameObject.SetActive(active);

    public void ShowEndGame(GameColor winnerColor) => EndGameScreen.ShowEndGameScreen(winnerColor);
    public void HideEndGame() => EndGameScreen.gameObject.SetActive(false);

    public void UpdatePoints(Player player)
    {
        var points = player.points;
        switch (player.color)
        {
            case GameColor.Blue:
                BluePoints.text = points.ToString(); break;
            case GameColor.Red:
                RedPoints.text = points.ToString(); break;
            case GameColor.Green:
                GreenPoints.text = points.ToString(); break;
            case GameColor.Yellow:
                YellowPoints.text = points.ToString(); break;
            default: throw new ArgumentException("Branco não é válido");
        }
    }
}
