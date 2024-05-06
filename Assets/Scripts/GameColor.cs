public enum GameColor { White, Blue, Red, Green, Yellow };

namespace EnumExtension
{
    public static class Extensions
    {
        public static string ToStringPtBr(this GameColor color)
        {
            return color switch
            {
                GameColor.White => "Branco",
                GameColor.Blue => "Azul",
                GameColor.Red => "Vermelho",
                GameColor.Green => "Verde",
                GameColor.Yellow => "Amarelo",
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
