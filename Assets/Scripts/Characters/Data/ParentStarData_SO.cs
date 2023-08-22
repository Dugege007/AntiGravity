using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Parent Data", menuName = "CharacterStats/Parent Data")]//在Create菜单中创建子集菜单
public class ParentStarData_SO : ScriptableObject
{
    [Header("Skill")]
    public float baseGravitation;
    //自身重力
    public float currentGravitation;
    //技能距离
    public float controlDistance;
    //自旋角速度
    public float angleSpeed;
    //生成物体的最大冷却时间
    public float maxSpawnTime;

    [Header("Score")]
    //可额外获取分数
    public int scorePlus;

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int maxTime;
    public float levelUpMultiplier;

    [Header("Default UI Bar")]
    public int lastLevel;

    public float LevelUpMultiplier
    {
        get { return levelUpMultiplier; }
    }

    public void AutoUpdateLevel()
    {
        if ((int)UIManager.Instance.countTime >= maxTime)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        lastLevel = currentLevel;

        //所有ParentStar需要提升数据的方法都在这里
        currentLevel++;

        float gravitationStep = -Mathf.Pow(Mathf.Abs(baseGravitation), 1 + LevelUpMultiplier);

        currentGravitation += gravitationStep;
        controlDistance += controlDistance * LevelUpMultiplier * 0.15f;
        angleSpeed += angleSpeed * LevelUpMultiplier;
        maxSpawnTime -= maxSpawnTime * LevelUpMultiplier * 0.5f;

        scorePlus += (int)(scorePlus * 0.25f);
        maxTime += (int)(maxTime * LevelUpMultiplier * 3);

        Debug.Log("Level Up! " + currentLevel + "\nmaxTime: " + maxTime + "\ngravitation: " + currentGravitation);
    }
}
