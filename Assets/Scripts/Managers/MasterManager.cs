using UnityEngine;

[CreateAssetMenu(fileName = "MasterManager", menuName = "Singleton/MasterManager", order = 0)]
public class MasterManager : SingletonScriptableObject<MasterManager>
{
    private Settings _settings;
    public Settings Settings { get => Instance._settings; }
}