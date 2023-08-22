using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Parent Data", menuName = "CharacterStats/Parent Data")]//��Create�˵��д����Ӽ��˵�
public class ParentStarData_SO : ScriptableObject
{
    [Header("Skill")]
    public float baseGravitation;
    //��������
    public float currentGravitation;
    //���ܾ���
    public float controlDistance;
    //�������ٶ�
    public float angleSpeed;
    //��������������ȴʱ��
    public float maxSpawnTime;

    [Header("Score")]
    //�ɶ����ȡ����
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

        //����ParentStar��Ҫ�������ݵķ�����������
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
