using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHp : MonoBehaviour
{
    DroneScriptableObject droneScriptableObject;
    float maxHp;
    // Start is called before the first frame update
    void Start()
    {
        maxHp = droneScriptableObject.maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
