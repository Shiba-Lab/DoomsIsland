using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class DoubleCannonDamage : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] GameObject doubleCanon;
    DoubleCannonHp doubleCannonHp;

    // Start is called before the first frame update
    void Start()
    {
        doubleCannonHp = doubleCanon.GetComponent<DoubleCannonHp>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            doubleCannonHp.GetDamage(damage);
            Destroy(collision.gameObject);
        }
    }
}
