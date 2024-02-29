using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteResolver))]
[RequireComponent(typeof(SpriteLibrary))]
public class Dice : MonoBehaviour
{
    private SpriteResolver spriteResolver;
    [HideInInspector] public int value = 1;
    [HideInInspector] public bool diceIsRolling = false;

    private void Start()
    {
        spriteResolver = GetComponent<SpriteResolver>();
    }
    public void RollDice()
    {
        if (!diceIsRolling && !GameManager.I.diceWasRolled)
            StartCoroutine(RollDiceCore());
    }

    private IEnumerator RollDiceCore()
    {
        diceIsRolling = true;
        var prev = value;
        for (int i = 0; i < 5; i++)
        {
            while (value == prev) value = Random.Range(1, 7);
            prev = value;
            spriteResolver.SetCategoryAndLabel("Dice", value.ToString());
            yield return new WaitForSeconds(0.1f);
        }
        diceIsRolling = false;
        GameManager.I.diceWasRolled = true;
    }

    private void OnMouseDown()
    {
        RollDice();
    }
}