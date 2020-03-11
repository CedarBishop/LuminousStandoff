using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clamp : MonoBehaviour
{
    Quaternion startingRotation;
    private void Start()
    {
        startingRotation = transform.rotation;
    }
    private void Update()
    {
        transform.rotation = startingRotation;
    }
}
