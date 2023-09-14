using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torso : MonoBehaviour
{
    [SerializeField] GameObject cam;
    Vector3 camr; 
    void Start()
    {
        
    }

    void Update()
    {
        camr = cam.transform.eulerAngles;
        this.transform.eulerAngles = camr;
    }
}
