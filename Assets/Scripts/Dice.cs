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
        if (!diceIsRolling)
            StartCoroutine(RollDiceAnimation());
    }

    private IEnumerator RollDiceAnimation()
    {
        diceIsRolling = true;
        for (int i = 0; i < 7; i++)
        {
            value = Random.Range(1, 7);
            spriteResolver.SetCategoryAndLabel("Dice", value.ToString());
            yield return new WaitForSeconds(0.2f);
        }
        diceIsRolling = false;
    }

    private void OnMouseDown()
    {
        RollDice();
    }
}