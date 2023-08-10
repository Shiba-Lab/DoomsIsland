using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] GameObject player;
    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        if(Mathf.Abs(mx) > 0.001f)
        {
            //y回転軸はワールド座標のy軸
            transform.RotateAround(player.transform.position, Vector3.up, 1.8f * mx);
        }
        if(Mathf.Abs(my) > 0.001f)
        {
            //x回転軸は自身のx軸
            transform.RotateAround(player.transform.position, transform.right, -1.8f * my);
        }
    }
}
