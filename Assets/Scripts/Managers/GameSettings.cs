using System;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField]
    private string _gameVersion = "0.0.0";
    public string GameVersion { get => _gameVersion; }
    [SerializeField]
    private string _nickName = "NickName";
    public string NickName { get => _nickName; set => _nickName = value; }

    public static GameSettings Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    #region Bonus

    public int easyBonus = 1;
    public int mediumBonus = 2;
    public int hardBonus = 3;
    public float easyTimer = 30;
    public float mediumTimer = 60;
    public float hardTimer = 90;

    #endregion

    public int GetDifficultyBonus(int difficulty)
    {
        return difficulty switch
        {
            0 => easyBonus,
            1 => mediumBonus,
            2 => hardBonus,
            _ => throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}")
        };
    }

    public float GetDifficultyTimer(int difficulty)
    {
        return difficulty switch
        {
            0 => easyTimer,
            1 => mediumTimer,
            2 => hardTimer,
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
                    easyTimer = float.Parse(newTimer); break;
                case 1:
                    mediumTimer = float.Parse(newTimer); break;
                case 2:
                    hardTimer = float.Parse(newTimer); break;
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
                    easyBonus = int.Parse(newBonus); break;
                case 1:
                    mediumBonus = int.Parse(newBonus); break;
                case 2:
                    hardBonus = int.Parse(newBonus); break;
                default:
                    throw new ArgumentOutOfRangeException($"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}");
            }
        };
    }
}