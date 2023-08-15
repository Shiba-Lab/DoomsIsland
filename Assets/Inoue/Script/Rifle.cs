using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    private Animator fire;
    // Start is called before the first frame update
    void Start()
    {
        fire = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))//(Input.GetMouseButtonDown(0))
        {
            fire.SetBool("Trigger", true);
        }
        else
        {
            fire.SetBool("Trigger", false);
        }
    }
}
