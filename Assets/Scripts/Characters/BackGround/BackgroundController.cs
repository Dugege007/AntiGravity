using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundController : Singleton<BackgroundController>
{
    public GameObject bigCircle;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        bigCircle.transform.position = Vector3.zero;

    }
}
