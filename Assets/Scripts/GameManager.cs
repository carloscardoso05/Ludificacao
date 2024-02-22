using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public enum GameState { SelectPlayersNumber, RollingDice, SelectPiece, End };
public class GameManager : MonoBehaviour
{
    public static GameManager I;
    public int diceValue;
    public Image diceImage;
    public bool diceIsRolling = false;
    public SpriteLibraryAsset spriteLibrary;
    public GameState state = GameState.SelectPlayersNumber;
    private GameColor[] playersColors;
    public GameColor? currentPlayerColor;
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject piecePrefab;

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        MainMenu.SetActive(true);
        UI.SetActive(false);
        Board.I.gameObject.SetActive(false);
    }

    private void GeneratePlayersPieces()
    {
        var players = new GameObject("Players");
        foreach (GameColor color in playersColors)
        {
            if (color == GameColor.White) continue;
            var colorGO = new GameObject(color.ToString());
            colorGO.transform.parent = players.transform;
            for (int i = 0; i < 4; i++)
            {
                var piece = Instantiate(piecePrefab).GetComponent<Piece>();
                piece.name = "Piece" + i.ToString();
                piece.transform.parent = colorGO.transform;
                Vector2 homePosition = Board.I.GetHome(color).transform.position;
                Vector2 offset = Board.I.GetHomeOffset(i);
                piece.HomePosition = homePosition + offset;
                piece.color = color;
                piece.spriteResolver.SetCategoryAndLabel("Body", color.ToString());
            }
        }
    }

    private GameColor[] GetColorsByPlayersQty(int playersQuantity)
    {
        return playersQuantity switch
        {
            2 => new GameColor[] { GameColor.Blue, GameColor.Green },
            3 => new GameColor[] { GameColor.Blue, GameColor.Red, GameColor.Green },
            4 => new GameColor[] { GameColor.Blue, GameColor.Red, GameColor.Green, GameColor.Yellow },
            _ => throw new ArgumentOutOfRangeException("Os valores válidos são 2, 3 e 4")
        };
    }

    public void InitGame(int playersQuantity)
    {
        playersColors = GetColorsByPlayersQty(playersQuantity);
        GeneratePlayersPieces();
        MainMenu.SetActive(false);
        UI.SetActive(true);
        UpdateCurrentPlayerColor();
        Board.I.gameObject.SetActive(true);
    }

    public void UpdateCurrentPlayerColor()
    {
        if (currentPlayerColor == null)
        {
            currentPlayerColor = playersColors.First();
        }
        else
        {
            for (int i = 0; i < playersColors.Length; i++)
            {
                if (currentPlayerColor == playersColors[i])
                {
                    currentPlayerColor = playersColors[(i + 1) % playersColors.Length];
                    return;
                }
            }
        }
    }

    public void RollDice()
    {
        if (!diceIsRolling)
        StartCoroutine(RollDiceAnimation());
    }

    private IEnumerator RollDiceAnimation()
    {
        diceIsRolling = true;
        for (int i = 0; i < 7; i++)
        {
            diceValue = UnityEngine.Random.Range(1, 7);
            diceImage.sprite = spriteLibrary.GetSprite("Dice", diceValue.ToString());
            yield return new WaitForSeconds(0.2f);
        }
        diceIsRolling = false;
    }
}