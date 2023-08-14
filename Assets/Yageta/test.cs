using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomSystem;

public class test : MonoBehaviour
{
    [SerializeField] GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        CustomPhysics.IgnoreCollisions(gameObjects);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
