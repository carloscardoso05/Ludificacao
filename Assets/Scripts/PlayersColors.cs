using System;
using System.Linq;

public enum GameColor { White, Blue, Red, Green, Yellow };

public class ColorsManager
{
    public GameColor[] colors;
    public GameColor? currentColor;

    public ColorsManager(int playersQuantity)
    {
        colors = GetColorsByPlayersQty(playersQuantity);
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
        if (currentColor != null)
            for (int i = 0; i < colors.Length; i++)
            {
                if (currentColor == colors[i])
                {
                    currentColor = colors[(i + 1) % colors.Length];
                }
            }
        currentColor = colors.First();
    }
}
