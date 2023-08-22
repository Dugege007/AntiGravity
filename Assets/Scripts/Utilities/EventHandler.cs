using System;
using UnityEngine;

public class EventHandler
{
    //呼叫结束事件
    public static event Action EndGameEvent;
    public static void CallEndGameEvent()
    {
        EndGameEvent?.Invoke();
    }

    //呼叫玩家到达母星事件
    public static event Action ArrivedParentEvent;
    public static void CallArrivedParentEvent()
    {
        ArrivedParentEvent?.Invoke();
    }

    //呼叫玩家离开母星事件
    public static event Action LeaveParentEvent;
    public static void CallLeaveParentEvent()
    {
        LeaveParentEvent?.Invoke();
    }

    //呼叫碰撞特效事件
    public static event Action<GameObject> CollisionParticleEvent;
    public static void CallCollisionParticleEvent(GameObject obj)
    {
        CollisionParticleEvent?.Invoke(obj);
    }

    //呼叫喷射物发射事件
    public static event Action LaunchEjectorEvent;
    public static void CallLaunchEjectorEvent()
    {
        LaunchEjectorEvent?.Invoke();
    }

    //呼叫喷射物回收事件
    public static event Action RecoveryEjectorEvent;
    public static void CallRecoveryEjectorEvent()
    {
        RecoveryEjectorEvent?.Invoke();
    }

    //呼叫玩家撞到障碍物提示事件
    public static event Action GetBuffByEjectorEvent;
    public static void CallGetBuffByEjectorEvent()
    {
        GetBuffByEjectorEvent?.Invoke();
    }

    //呼叫玩家撞到障碍物提示事件
    public static event Action GetHitByEjectorEvent;
    public static void CallGetHitByEjectorEvent()
    {
        GetHitByEjectorEvent?.Invoke();
    }

    //呼叫玩家距离母星过远提示事件
    public static event Action TooFarToParentEvent;
    public static void CallTooFarToParentEvent()
    {
        TooFarToParentEvent?.Invoke();
    }

    //呼叫玩家血量过低提示事件
    public static event Action HealthTooLowEvent;
    public static void CallHealthTooLowEvent()
    {
        HealthTooLowEvent?.Invoke();
    }

    //呼叫得分事件
    public static event Action<int> GetPointEvent;
    public static void CallGetPointEvent(int point)
    {
        GetPointEvent?.Invoke(point);
    }
}
