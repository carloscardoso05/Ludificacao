using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteResolver))]
[RequireComponent(typeof(SpriteLibrary))]
public class Dice : MonoBehaviour
{
    private SpriteResolver spriteResolver;
    [SerializeField] private int _value = 6;
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            spriteResolver.SetCategoryAndLabel("Dice", value.ToString());
        }
    }
    private bool isAnimating = false;

    private void Start()
    {
        spriteResolver = GetComponent<SpriteResolver>();
        Value = 6;
    }

    public void RollDice()
    {
        var gameState = GameManager.Instance.state;
        if (gameState == GameState.RollingDice && !isAnimating)
        {
            StartCoroutine(RollDiceCore());
        }
    }

    private IEnumerator RollDiceCore()
    {
        isAnimating = true;
        var prev = Value;
        for (int i = 0; i < 5; i++)
        {
            while (Value == prev) Value = Random.Range(1, 7);
            spriteResolver.SetCategoryAndLabel("Dice", Value.ToString());
            prev = Value;
            yield return new WaitForSeconds(0.1f);
        }
        isAnimating = false;
        GameManager.Instance.ChangeState(GameState.SelectingPiece);
    }

    private void OnMouseUp()
    {
        if (ColorsManager.I.currentColor == (GameColor)PhotonNetwork.LocalPlayer.CustomProperties["Color"])
        RollDice();
    }
}