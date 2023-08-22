using System.Collections.Generic;
using UnityEngine;

public class ParentStarStats : MonoBehaviour
{
    [SerializeField]private Level level;
    public List<ParentStarData_SO> dataList = new List<ParentStarData_SO>();
    public ParentStarData_SO parentStarData;

    private void Awake()
    {
        //´«Èëlevel
        level = GameManager.Instance.level;
        SwitchData();
    }

    public void SwitchData()
    {
        switch (level)
        {
            case Level.Easy:
                if (dataList[0] != null)
                    parentStarData = Instantiate(dataList[0]);
                break;
            case Level.Hard:
                if (dataList[1] != null)
                    parentStarData = Instantiate(dataList[1]);
                break;
            case Level.Challenge:
                if (dataList[2] != null)
                    parentStarData = Instantiate(dataList[2]);
                break;
            case Level.Test:
                if (dataList[0] != null)
                    parentStarData = Instantiate(dataList[0]);
                break;
        }
    }

    public float CurrentGravitation
    {
        get { if (parentStarData != null) return parentStarData.currentGravitation; else return 0; }
        set { parentStarData.currentGravitation = value; }
    }

    public float ControlDistance
    {
        get { if (parentStarData != null) return parentStarData.controlDistance; else return 0; }
        set { parentStarData.controlDistance = value; }
    }

    public float AngleSpeed
    {
        get { if (parentStarData != null) return parentStarData.angleSpeed; else return 0; }
        set { parentStarData.angleSpeed = value; }
    }

    public float MaxSpawnTime
    {
        get { if (parentStarData != null) return parentStarData.maxSpawnTime; else return 0; }
        set { parentStarData.maxSpawnTime = Mathf.Max(value, 0.3f); }
    }

    public int ScorePlus
    {
        get { if (parentStarData != null) return parentStarData.scorePlus; else return 0; }
        set { parentStarData.scorePlus = value; }
    }
}
