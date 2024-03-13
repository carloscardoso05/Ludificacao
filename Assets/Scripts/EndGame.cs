using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EndGame : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Sprite blueScreen;
    [SerializeField] private Sprite redScreen;
    [SerializeField] private Sprite greenScreen;
    [SerializeField] private Sprite yellowScreen;

    public void ShowEndGameScreen(GameColor color) {
        Sprite sprite = color switch
        {
            GameColor.Blue => blueScreen,
            GameColor.Red => redScreen,
            GameColor.Green => greenScreen,
            GameColor.Yellow => yellowScreen,
            _ => throw new ArgumentException("Cor branca não é válida"),
        };
        background.sprite = sprite;
        gameObject.SetActive(true);
    }
}
