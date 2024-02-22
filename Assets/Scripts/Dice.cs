using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] private Image diceImage;
    [SerializeField] private SpriteLibraryAsset spriteLibrary;
    [HideInInspector] public int value;
    [HideInInspector] public bool diceIsRolling = false;

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
            value = UnityEngine.Random.Range(1, 7);
            diceImage.sprite = spriteLibrary.GetSprite("Dice", value.ToString());
            yield return new WaitForSeconds(0.2f);
        }
        diceIsRolling = false;
    }
}