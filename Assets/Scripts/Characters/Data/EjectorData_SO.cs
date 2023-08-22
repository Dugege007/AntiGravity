using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ejector Data", menuName = "EjectorStats/Ejector Data")]//��Create�˵��д����Ӽ��˵�
public class EjectorData_SO : ScriptableObject
{
    public int maxHealth;
    public int currentHealth;

    //�������ٶ�
    public float baseSelfAngleSpeed;
    //����ƶ��ٶ�
    public float maxSpeed;
    //�����ٶȴ�С
    public float baseArcLengthSpeed;
}
