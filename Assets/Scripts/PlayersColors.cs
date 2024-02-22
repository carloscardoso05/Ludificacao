using System;
using System.Linq;

public enum GameColor { White, Blue, Red, Green, Yellow };

public static class Colors
{
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

    public static GameColor GetNextColor(GameColor? currentColor, GameColor[] colors)
    {
        if (currentColor != null)
            for (int i = 0; i < colors.Length; i++)
            {
                if (currentColor == colors[i])
                {
                    return colors[(i + 1) % colors.Length];
                }
            }
        return colors.First();
    }
}
