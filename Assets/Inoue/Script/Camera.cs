using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float sens = 2.5f;
    void Update()
    {
        float my = Input.GetAxis("Mouse Y");

        if(Mathf.Abs(my) > 0.001f)
        {
            if ((this.transform.eulerAngles.x > 45)&&(this.transform.eulerAngles.x < 75))
            {
                if (my > 0)
                {
                    transform.RotateAround(player.transform.position, transform.right, -1 * sens * my);
                }
            }
            else
            {
                if ((this.transform.eulerAngles.x < 315)&& (this.transform.eulerAngles.x > 285))
                {
                    if (my < 0)
                    {
                        transform.RotateAround(player.transform.position, transform.right, -1 * sens * my);
                    }
                }
                else
                {
                    transform.RotateAround(player.transform.position, transform.right, -1 * sens * my);
                }
            }
        }
    }
}
