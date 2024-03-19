using UnityEngine;

public class DifficultyTimer : MonoBehaviour
{
    public float easyTimer = 30;
    public float mediumTimer = 60;
    public float hardTimer = 90;
    public static DifficultyTimer I;

    private void Awake()
    {
        I = this;
    }
}