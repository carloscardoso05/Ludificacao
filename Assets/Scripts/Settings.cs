using System;
using UnityEngine;

[Serializable()]
public class Settings
{
    [SerializeField] private int easyBonus = 1;
    [SerializeField] private int mediumBonus = 2;
    [SerializeField] private int hardBonus = 3;
    [SerializeField] private float easyTimer = 30;
    [SerializeField] private float mediumTimer = 60;
    [SerializeField] private float hardTimer = 90;

    public event Action<Settings> SettingsChanged;

    public int EasyBonus
    {
        get => easyBonus; set
        {
            easyBonus = value;
            SettingsChanged?.Invoke(this);
        }
    }
    public int MediumBonus
    {
        get => mediumBonus; set
        {
            mediumBonus = value;
            SettingsChanged?.Invoke(this);
        }
    }
    public int HardBonus
    {
        get => hardBonus; set
        {
            hardBonus = value;
            SettingsChanged?.Invoke(this);
        }
    }
    public float EasyTimer
    {
        get => easyTimer; set
        {
            easyTimer = value;
            SettingsChanged?.Invoke(this);
        }
    }
    public float MediumTimer
    {
        get => mediumTimer; set
        {
            mediumTimer = value;
            SettingsChanged?.Invoke(this);
        }
    }
    public float HardTimer
    {
        get => hardTimer; set
        {
            hardTimer = value;
            SettingsChanged?.Invoke(this);
        }
    }

    public void ChangeSettings(Settings settings, bool raiseEvent) {
        easyBonus = settings.easyBonus;
        mediumBonus = settings.mediumBonus;
        hardBonus = settings.hardBonus;
        easyTimer = settings.easyTimer;
        mediumTimer = settings.mediumTimer;
        hardTimer = settings.hardTimer;
        if (raiseEvent) SettingsChanged?.Invoke(this);
    }
}