using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public EndGame EndGameScreen;
    public CanvasRenderer Settings;
    public CanvasRenderer MainMenu;
    public static UiManager I;

    private void Awake()
    {
        I = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.I.OnGameEnded += EndGame;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void EndGame(object sender, Piece winner)
    {
        Dictionary<GameColor, string> colorsPtBr = new() {
            {GameColor.Blue, "Azul"},
            {GameColor.Red, "Vermelho"},
            {GameColor.Green, "Verde"},
            {GameColor.Yellow, "Amarelo"},
        };
        EndGameScreen.transform.Find("Nome").GetComponent<TextMeshProUGUI>().text = $"Jogador {colorsPtBr[winner.color]} ganhou !";
        ShowEndGame(winner.color);
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
}
