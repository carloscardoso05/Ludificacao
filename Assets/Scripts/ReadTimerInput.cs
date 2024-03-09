using TMPro;
using UnityEngine;

public class ReadTimerInput : MonoBehaviour {
    [SerializeField] private TMP_InputField input;
    [SerializeField] private int difficulty;

    private void Start() {
        input.onValueChanged.AddListener(ChangeTimer);
        input.text = Settings.I.GetDifficultyTimer(difficulty).ToString();
    }

    private void ChangeTimer(string newTimer) {
        Settings.I.SetDifficultyTimer(difficulty)(newTimer);
    }
}