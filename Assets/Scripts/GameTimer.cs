using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class GameTimer : MonoBehaviour
{
    private TextMeshPro text;
    private float time = 0f;
    void Start()
    {
        text = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        time += Time.deltaTime;  //time is a float
        int seconds = ((int)time % 60);
        int minutes = ((int)time / 60);
        text.text = string.Format("Tempo de jogo:\n{0:00}:{1:00}", minutes, seconds);
    }
}
