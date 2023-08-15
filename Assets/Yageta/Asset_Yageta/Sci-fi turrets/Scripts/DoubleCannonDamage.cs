using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class DoubleCannonDamage : MonoBehaviour
{
    [SerializeField] DoubleCannonScriptableObject scriptableObject;
    float damage;
    [SerializeField] GameObject doubleCanon;
    DoubleCannonHp doubleCannonHp;

    [SerializeField] Parts collisionPart;

    enum Parts
    {
        Found, CannonBottom, CannonTop, Shield
    }
    // Start is called before the first frame update
    void Start()
    {
        switch (collisionPart)
        {
            case Parts.Found:
                damage = scriptableObject.foundDamage; break;
            case Parts.CannonBottom:
                damage = scriptableObject.bottomDamage; break;
            case Parts.CannonTop:
                damage = scriptableObject.topDamage; break;
            case Parts.Shield:
                damage = scriptableObject.shieldDamage; break;

        }

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
