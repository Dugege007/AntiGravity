using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Data", menuName = "EjectorStats/Buff Data")]//��Create�˵��д����Ӽ��˵�
public class BuffData_SO : ScriptableObject
{
    //����ֵ����
    public int healthIncrease;
    //�ٶ�ֵ����
    public float speedIncrease;
    //�ƶ�ʱ������
    public float moveTimeIncrease;
    //�ƶ���ȴ����
    public float moveCoolDownTimeIncrease;
    //��������
    public float gravitationIncrease;
    //���ܷ�Χ����
    public float skillDistanceIncrease;
    //��������
    public int score;
}
