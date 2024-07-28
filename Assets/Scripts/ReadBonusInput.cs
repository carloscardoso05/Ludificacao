using Managers;
using TMPro;
using UnityEngine;

public class ReadBonusInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private int difficulty;

    private void Start()
    {
        input.onValueChanged.AddListener(ChangeTimer);
        input.text = GameSettings.Instance.GetDifficultyBonus(difficulty).ToString();
    }

    private void ChangeTimer(string newBonus)
    {
        GameSettings.Instance.SetDifficultyBonus(difficulty)(newBonus);
    }
}