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
        GameManager.Instance.OnTurnChanged += ChangeIconColor;
    }

    private void ChangeIconColor(object sender, GameColor newColor)
    {
        var icons = new GameObject[] { BlueIcon, RedIcon, GreenIcon, YellowIcon };
        currentIcon = GetIcon(ColorsManager.I.currentColor);
        foreach (GameObject icon in icons)
        {
            icon.transform.localScale = Vector3.one;
            Seta(icon).SetActive(false);
        }
    }
    private void Update()
    {
        if (currentIcon != null)
        {
            if (!Seta(currentIcon).activeSelf) {
                Seta(currentIcon).SetActive(true);
            }
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

    private GameObject Seta(GameObject icon) {
        return icon.transform.parent.Find("Seta").gameObject;
    }
}