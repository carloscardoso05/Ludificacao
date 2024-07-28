using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour {
	public EndGame EndGameScreen;
	public CanvasRenderer Settings;
	public CanvasRenderer MainMenu;
	public static UiManager I;
	public PlayerIcon playersInfo;
	private TextMeshPro BluePoints;
	private TextMeshPro RedPoints;
	private TextMeshPro GreenPoints;
	private TextMeshPro YellowPoints;

	public class EndGameArgs {
		public Player winner;
		public Player[] players;
	}

	private void Awake() {
		I = this;
	}

	void Start() {
		GameManager.Instance.OnGameEnded += EndGame;
		BluePoints = playersInfo.transform.Find("BlueInfo").GetComponentInChildren<TextMeshPro>();
		RedPoints = playersInfo.transform.Find("RedInfo").GetComponentInChildren<TextMeshPro>();
		GreenPoints = playersInfo.transform.Find("GreenInfo").GetComponentInChildren<TextMeshPro>();
		YellowPoints = playersInfo.transform.Find("YellowInfo").GetComponentInChildren<TextMeshPro>();
		if (PhotonNetwork.OfflineMode) {
			SetActiveMainMenu(true);
		}
	}

	private void EndGame(object sender, EndGameArgs args) {
		Dictionary<GameColor, string> colorsPtBr = new() {
			{ GameColor.Blue, "Azul" },
			{ GameColor.Red, "Vermelho" },
			{ GameColor.Green, "Verde" },
			{ GameColor.Yellow, "Amarelo" },
		};

		EndGameScreen.transform.Find("Nome").GetComponent<TextMeshProUGUI>().text =
			$"Jogador {colorsPtBr[args.winner.color]} ganhou !";

		var sortedPlayers = args.players.ToList().OrderByDescending((p) => p.points).ToArray();
		for (int i = 0; i < sortedPlayers.Length; i++) {
			var player = sortedPlayers[i];
			var rankingText = EndGameScreen.transform.Find("Ranking").Find($"{i + 1}Lugar")
				.GetComponent<TextMeshProUGUI>();
			rankingText.text = $"{i + 1}º Lugar - {colorsPtBr[player.color]} - {player.points} pts";
		}

		ShowEndGame(args.winner.color);
	}

	public void SetActiveSettings(bool active) {
		Debug.Log($"Settings active: {active}");
		MainMenu.gameObject.SetActive(!active);
		Settings.gameObject.SetActive(active);
	}

	public void SetActiveMainMenu(bool active) {
		Debug.Log($"Menu active: {active}");
		MainMenu.gameObject.SetActive(active);
	}

	public void ShowEndGame(GameColor winnerColor) => EndGameScreen.ShowEndGameScreen(winnerColor);

	public void UpdatePoints(Player player) {
		var pointsStr = player.points.ToString();
		switch (player.color) {
			case GameColor.Blue:
				BluePoints.text = pointsStr;
				break;
			case GameColor.Red:
				RedPoints.text = pointsStr;
				break;
			case GameColor.Green:
				GreenPoints.text = pointsStr;
				break;
			case GameColor.Yellow:
				YellowPoints.text = pointsStr;
				break;
			default: throw new ArgumentException("Branco não é válido");
		}
	}
}