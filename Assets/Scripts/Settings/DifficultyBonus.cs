using UnityEngine;

public class DifficultyBonus : MonoBehaviour
{
    public int easyBonus = 1;
    public int mediumBonus = 2;
    public int hardBonus = 3;
    public static DifficultyBonus I;
    
    private void Awake()
    {
        I = this;
    }
}