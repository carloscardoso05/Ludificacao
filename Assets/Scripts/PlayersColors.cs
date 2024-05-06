using System;
using System.Linq;

public class ColorsManager
{
    public GameColor[] colors;
    public GameColor currentColor;
    public static ColorsManager I;

    public ColorsManager(int playersQuantity)
    {
        colors = GetColorsByPlayersQty(playersQuantity);
        currentColor = colors.First();
    }

    public static GameColor[] GetColorsByPlayersQty(int playersQuantity)
    {
        return playersQuantity switch
        {
            2 => new GameColor[] { GameColor.Blue, GameColor.Green },
            3 => new GameColor[] { GameColor.Blue, GameColor.Red, GameColor.Green },
            4 => new GameColor[] { GameColor.Blue, GameColor.Red, GameColor.Green, GameColor.Yellow },
            _ => throw new ArgumentOutOfRangeException("Os valores válidos são 2, 3 e 4")
        };
    }

    public void UpdateColor()
    {
        for (int i = 0; i < colors.Length; i++)
            if (currentColor == colors[i])
            {
                currentColor = colors[(i + 1) % colors.Length];
                break;
            }
    }
}
