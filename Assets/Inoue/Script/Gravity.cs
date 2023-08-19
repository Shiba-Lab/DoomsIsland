using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public Vector3 accel;
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(accel, ForceMode.Acceleration);
    }
}
