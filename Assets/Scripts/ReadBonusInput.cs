using TMPro;
using UnityEngine;

public class ReadBonusInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private int difficulty;

    private void Start()
    {
        input.onValueChanged.AddListener(ChangeTimer);
        input.text = Settings.I.GetDifficultyBonus(difficulty).ToString();
    }

    private void ChangeTimer(string newBonus)
    {
        Settings.I.SetDifficultyBonus(difficulty)(newBonus);
    }
}