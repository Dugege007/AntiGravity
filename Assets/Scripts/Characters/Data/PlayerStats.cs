using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public List<PlayerData_SO> dataList = new List<PlayerData_SO>();
    public PlayerData_SO playerData;

    [Header("Default UI Bar")]
    public int lastScore;
    public int lastHealth;

    private void Awake()
    {
        SwitchData();
    }

    public void SwitchData()
    {
        switch (GameManager.Instance.level)
        {
            case Level.Easy:
            case Level.Hard:
                if (dataList[0] != null)
                    playerData = Instantiate(dataList[0]);
                break;
            case Level.Challenge:
                if (dataList[1] != null)
                    playerData = Instantiate(dataList[1]);
                break;
            case Level.Test:
                if (dataList[2] != null)
                    playerData = Instantiate(dataList[2]);
                break;
        }
    }

    public int MaxHealth
    {
        get { if (playerData != null) return playerData.maxHealth; else return 0; }
        set { playerData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { if (playerData != null) return playerData.currentHealth; else return 0; }
        set { playerData.currentHealth = value; }
    }

    public float MaxSpeed
    {
        get { if (playerData != null) return playerData.maxSpeed; else return 0; }
        set { playerData.maxSpeed = value; }
    }

    public float MaxMoveTime
    {
        get { if (playerData != null) return playerData.maxMoveTime; else return 0; }
        set { playerData.maxMoveTime = value; }
    }

    public float MoveCoolDownTime
    {
        get { if (playerData != null) return playerData.moveCoolDownTime; else return 0; }
        set { playerData.moveCoolDownTime = value; }
    }

    public float Gravitation
    {
        get { if (playerData != null) return playerData.gravitation; else return 0; }
        set { playerData.gravitation = value; }
    }

    public float SkillDistance
    {
        get { if (playerData != null) return playerData.skillDistance; else return 0; }
        set { playerData.skillDistance = value; }
    }

    public int FinalPoint
    {
        get { if (playerData != null) return playerData.finalPoint; else return 0; }
        set { playerData.finalPoint = value; }
    }

    public void GetBuff(BuffData_SO buff)
    {
        lastScore = FinalPoint;
        lastHealth = CurrentHealth;

        CurrentHealth = Mathf.Max(CurrentHealth + buff.healthIncrease, 0);//保证血最小值为0
        if (CurrentHealth > MaxHealth)
            MaxHealth = CurrentHealth;
        MaxSpeed = Mathf.Max(MaxSpeed + buff.speedIncrease, 0.5f);
        MaxMoveTime = Mathf.Clamp(MaxMoveTime + buff.moveTimeIncrease, 0.3f, 6f);
        MoveCoolDownTime = Mathf.Clamp(MoveCoolDownTime + buff.moveCoolDownTimeIncrease, 0.3f, 3f);
        Gravitation = Mathf.Clamp(Gravitation + buff.gravitationIncrease, 10, 900);
        SkillDistance = Mathf.Clamp(SkillDistance + buff.skillDistanceIncrease, 0.5f, 5f);
        FinalPoint = Mathf.Clamp(FinalPoint + buff.score, 0, 199999999);
    }

    public void GetHurt()
    {
        lastHealth = CurrentHealth;
        CurrentHealth = Mathf.Max(CurrentHealth -1, 0);//保证血最小值为0
    }
}
