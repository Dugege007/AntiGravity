using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    //保存Player当前所在的场景名
    private string sceneName = "";
    public string SceneName
    {
        get { return PlayerPrefs.GetString(sceneName); }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {

    }
}
