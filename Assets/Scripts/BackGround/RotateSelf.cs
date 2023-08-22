using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public float angleSpeed;

    void Update()
    {
        //зда§
        transform.Rotate(Vector3.forward, angleSpeed * Time.deltaTime);
    }
}
