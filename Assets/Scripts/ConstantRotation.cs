using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public int spinSpeed = 300;

    void Update()
    {
        transform.Rotate(0, 0, - spinSpeed * Time.deltaTime);
    }
}
