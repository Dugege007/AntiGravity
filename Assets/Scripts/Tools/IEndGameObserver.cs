public interface IEndGameObserver
{
    //结束游戏的广播
    void EndNotify();

    //关卡升级的广播
    void LevelUpNotify();
}
