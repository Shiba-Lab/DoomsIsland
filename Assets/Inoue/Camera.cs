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
            //y��]���̓��[���h���W��x��
            transform.RotateAround(player.transform.position, Vector3.up, mx);
        }
        if(Mathf.Abs(my) > 0.001f)
        {
            //x��]���͎��g��x��
            transform.RotateAround(player.transform.position, transform.right, my);
        }
    }
}
