using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "CharacterStats/Player Data")]//在Create菜单中创建子集菜单
public class PlayerData_SO : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("Move")]
    //最大移动速度
    public float maxSpeed;
    //可移动时间
    public float maxMoveTime;
    //移动冷却时间
    public float moveCoolDownTime;

    [Header("Skill")]
    //自身重力
    public float gravitation;
    //技能距离
    public float skillDistance;

    [Header("Score")]
    public int finalPoint;
}
