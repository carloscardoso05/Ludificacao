using TMPro;
using UnityEngine;

public class ReadTimerInput : MonoBehaviour {
    [SerializeField] private TMP_InputField input;
    [SerializeField] private int difficulty;

    private void Start() {
        input.onValueChanged.AddListener(ChangeTimer);
        input.text = GameSettings.Instance.GetDifficultyTimer(difficulty).ToString();
    }

    private void ChangeTimer(string newTimer) {
        GameSettings.Instance.SetDifficultyTimer(difficulty)(newTimer);
    }
}