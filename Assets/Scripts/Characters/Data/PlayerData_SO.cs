using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "CharacterStats/Player Data")]//��Create�˵��д����Ӽ��˵�
public class PlayerData_SO : ScriptableObject
{
    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("Move")]
    //����ƶ��ٶ�
    public float maxSpeed;
    //���ƶ�ʱ��
    public float maxMoveTime;
    //�ƶ���ȴʱ��
    public float moveCoolDownTime;

    [Header("Skill")]
    //��������
    public float gravitation;
    //���ܾ���
    public float skillDistance;

    [Header("Score")]
    public int finalPoint;
}
