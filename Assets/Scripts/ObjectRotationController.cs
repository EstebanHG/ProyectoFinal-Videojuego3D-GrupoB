using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotationController : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
    }
}
