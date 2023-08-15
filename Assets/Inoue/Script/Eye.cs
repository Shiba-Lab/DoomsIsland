using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    public Transform target;
    Vector3 tpos;
    Vector3 pos;
    void Start()
    {
        
    }
    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        tpos = target.position;
        tpos.y += 1.6f;
        this.transform.position = tpos;

        //pos = this.transform.position;

        if (Mathf.Abs(mx) > 0.001f)
        {
            //y‰ñ“]²‚Íƒ[ƒ‹ƒhÀ•W‚Ìx²
            transform.RotateAround(tpos, Vector3.up, 2.5f * mx);
        }
    }
}
