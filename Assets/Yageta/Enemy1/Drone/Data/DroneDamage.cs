using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDamage : MonoBehaviour
{
    [SerializeField] DroneScriptableObject scriptableObject;
    [SerializeField] GameObject drone;
    DroneHp droneHp;

    [SerializeField] Parts collisionPart;

    enum Parts
    {
        Body,WeakPoint
    }
    // Start is called before the first frame update
    void Start()
    {
        droneHp = drone.GetComponent<DroneHp>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            switch (collisionPart)
            {
                case Parts.Body:
                    droneHp.GetDamage(scriptableObject.bodyDamage); break;
                case Parts.WeakPoint:
                    droneHp.GetDamage(scriptableObject.weakpointDamage); break;

            }

            Destroy(collision.gameObject);
        }
    }
}
