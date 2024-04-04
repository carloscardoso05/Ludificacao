using UnityEngine;

public class MasterManager : SingletonMonoBehaviour<MasterManager>
{
    [SerializeField]
    private GameSettings _settings;
    public static GameSettings Settings { get => Instance._settings; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}