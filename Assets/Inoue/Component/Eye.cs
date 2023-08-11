using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    public Transform target;
    Vector3 pos;
    void Start()
    {
        
    }
    void Update()
    {
        pos = target.position;
        pos.y += 1.6f;
        GetComponent<Transform>().position = pos;
    }
}
