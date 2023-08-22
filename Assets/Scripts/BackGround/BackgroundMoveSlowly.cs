using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMoveSlowly : MonoBehaviour
{
    public GameObject star;

    private void Update()
    {
        star.transform.position = (PlayerController.Instance.transform.position - ParentStarController.Instance.transform.position) * 0.1f;
    }
}
