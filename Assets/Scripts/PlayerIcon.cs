using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    [SerializeField] private GameObject BlueIcon;
    [SerializeField] private GameObject RedIcon;
    [SerializeField] private GameObject GreenIcon;
    [SerializeField] private GameObject YellowIcon;
    private GameObject currentIcon;

    private void Start()
    {
        GameManager.I.OnTurnChanged += ChangeIconColor;
        currentIcon = GetIcon(GameManager.I.colorsManager.currentColor);
    }

    private void ChangeIconColor(object sender, GameColor newColor)
    {
        currentIcon = GetIcon(GameManager.I.colorsManager.currentColor);
        foreach (Transform child in transform)
        {
            child.localScale = Vector3.one;
        }
    }
    private void Update()
    {
        if (currentIcon != null)
        {
            var x = (float)(math.sin(Time.time) + 2) / 3;
            var y = (float)(math.sin(Time.time) + 2) / 3;
            var newScale = new Vector3(x, y, 1);
            currentIcon.transform.localScale = newScale;
        }
    }

    private GameObject GetIcon(GameColor color)
    {
        return color switch
        {
            GameColor.Blue => BlueIcon,
            GameColor.Red => RedIcon,
            GameColor.Green => GreenIcon,
            GameColor.Yellow => YellowIcon,
            _ => throw new ArgumentException("Branco não é válido")
        };
    }
}