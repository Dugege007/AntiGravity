using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionManager : Singleton<FunctionManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        GetComponent<Canvas>().planeDistance = 10;
    }
}
