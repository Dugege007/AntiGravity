using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Data", menuName = "EjectorStats/Buff Data")]//在Create菜单中创建子集菜单
public class BuffData_SO : ScriptableObject
{
    //生命值增加
    public int healthIncrease;
    //速度值增加
    public float speedIncrease;
    //移动时间增加
    public float moveTimeIncrease;
    //移动冷却增加
    public float moveCoolDownTimeIncrease;
    //引力增加
    public float gravitationIncrease;
    //技能范围增加
    public float skillDistanceIncrease;
    //分数增加
    public int score;
}
