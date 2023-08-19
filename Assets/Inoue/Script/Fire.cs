using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject shot;
    public float shotSpeed;
    bool Rug = false;
    public int Numbershot = 30;
    void Start()
    {

    }

    void Update()
    {
        if ((Input.GetMouseButton(0))&&Rug==false&&Numbershot>0)
        {
            GameObject Shot = (GameObject)Instantiate(shot, transform.position, Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, 0));
            Rigidbody shotRb = Shot.GetComponent<Rigidbody>();
            shotRb.AddForce(transform.forward * -shotSpeed);
            Destroy(Shot, 3.0f);

            Rug = true;
            Invoke("ROG", 0.16f);
            Numbershot -= 1;
        }
    }

    void ROG()
    {
        Rug = false;
    }
}
