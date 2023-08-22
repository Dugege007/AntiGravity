using System;
using UnityEngine;

public class EventHandler
{
    //���н����¼�
    public static event Action EndGameEvent;
    public static void CallEndGameEvent()
    {
        EndGameEvent?.Invoke();
    }

    //������ҵ���ĸ���¼�
    public static event Action ArrivedParentEvent;
    public static void CallArrivedParentEvent()
    {
        ArrivedParentEvent?.Invoke();
    }

    //��������뿪ĸ���¼�
    public static event Action LeaveParentEvent;
    public static void CallLeaveParentEvent()
    {
        LeaveParentEvent?.Invoke();
    }

    //������ײ��Ч�¼�
    public static event Action<GameObject> CollisionParticleEvent;
    public static void CallCollisionParticleEvent(GameObject obj)
    {
        CollisionParticleEvent?.Invoke(obj);
    }

    //���������﷢���¼�
    public static event Action LaunchEjectorEvent;
    public static void CallLaunchEjectorEvent()
    {
        LaunchEjectorEvent?.Invoke();
    }

    //��������������¼�
    public static event Action RecoveryEjectorEvent;
    public static void CallRecoveryEjectorEvent()
    {
        RecoveryEjectorEvent?.Invoke();
    }

    //�������ײ���ϰ�����ʾ�¼�
    public static event Action GetBuffByEjectorEvent;
    public static void CallGetBuffByEjectorEvent()
    {
        GetBuffByEjectorEvent?.Invoke();
    }

    //�������ײ���ϰ�����ʾ�¼�
    public static event Action GetHitByEjectorEvent;
    public static void CallGetHitByEjectorEvent()
    {
        GetHitByEjectorEvent?.Invoke();
    }

    //������Ҿ���ĸ�ǹ�Զ��ʾ�¼�
    public static event Action TooFarToParentEvent;
    public static void CallTooFarToParentEvent()
    {
        TooFarToParentEvent?.Invoke();
    }

    //�������Ѫ��������ʾ�¼�
    public static event Action HealthTooLowEvent;
    public static void CallHealthTooLowEvent()
    {
        HealthTooLowEvent?.Invoke();
    }

    //���е÷��¼�
    public static event Action<int> GetPointEvent;
    public static void CallGetPointEvent(int point)
    {
        GetPointEvent?.Invoke(point);
    }
}
