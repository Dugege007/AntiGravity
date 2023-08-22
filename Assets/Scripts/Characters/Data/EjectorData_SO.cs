using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ejector Data", menuName = "EjectorStats/Ejector Data")]//在Create菜单中创建子集菜单
public class EjectorData_SO : ScriptableObject
{
    public int maxHealth;
    public int currentHealth;

    //自旋角速度
    public float baseSelfAngleSpeed;
    //最大移动速度
    public float maxSpeed;
    //绕行速度大小
    public float baseArcLengthSpeed;
}
