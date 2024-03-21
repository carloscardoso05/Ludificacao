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
    public bool wasRolled = false;
    public int value = 1;
    public bool isRolling = false;
    public static Dice I;
    
    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        spriteResolver = GetComponent<SpriteResolver>();
    }
    public void RollDice()
    {
        if (!isRolling && !wasRolled)
            StartCoroutine(RollDiceCore());
    }

    private IEnumerator RollDiceCore()
    {
        isRolling = true;
        var prev = value;
        for (int i = 0; i < 5; i++)
        {
            while (value == prev) value = Random.Range(1, 7);
            prev = value;
            spriteResolver.SetCategoryAndLabel("Dice", value.ToString());
            yield return new WaitForSeconds(0.1f);
        }
        isRolling = false;
        wasRolled = true;
    }

    private void OnMouseUp()
    {
        RollDice();
    }
}