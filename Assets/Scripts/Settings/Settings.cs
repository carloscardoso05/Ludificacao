using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private string _gameVersion = "0.0.0";
    public string GameVersion { get => _gameVersion; }
    [SerializeField] private string _nickName = "0.0.0";
    public string NickName { get => _nickName; }
    private DifficultyBonus difficultyBonus;
    private DifficultyTimer difficultyTimer;
    public static Settings I;



    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        difficultyBonus = DifficultyBonus.I;
        difficultyTimer = DifficultyTimer.I;
    }

    public int GetDifficultyBonus(int difficulty)
    {
        return difficulty switch
        {
            0 => difficultyBonus.easyBonus,
            1 => difficultyBonus.mediumBonus,
            2 => difficultyBonus.hardBonus,
            _ => throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}")
        };
    }

    public float GetDifficultyTimer(int difficulty)
    {
        return difficulty switch
        {
            0 => difficultyTimer.easyTimer,
            1 => difficultyTimer.mediumTimer,
            2 => difficultyTimer.hardTimer,
            _ => throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}")
        };
    }

    public Action<string> SetDifficultyTimer(int difficulty)
    {
        return (newTimer) =>
        {
            switch (difficulty)
            {
                case 0:
                    difficultyTimer.easyTimer = float.Parse(newTimer); break;
                case 1:
                    difficultyTimer.mediumTimer = float.Parse(newTimer); break;
                case 2:
                    difficultyTimer.hardTimer = float.Parse(newTimer); break;
                default:
                    throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}");
            }
        };
    }
    public Action<string> SetDifficultyBonus(int difficulty)
    {
        return (newBonus) =>
        {
            switch (difficulty)
            {
                case 0:
                    difficultyBonus.easyBonus = int.Parse(newBonus); break;
                case 1:
                    difficultyBonus.mediumBonus = int.Parse(newBonus); break;
                case 2:
                    difficultyBonus.hardBonus = int.Parse(newBonus); break;
                default:
                    throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}");
            }
        };
    }
}