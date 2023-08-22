using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class DoubleCannonDamage : MonoBehaviour
{
    [SerializeField] DoubleCannonScriptableObject scriptableObject;
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
            switch (collisionPart)
            {
                case Parts.Found:
                    doubleCannonHp.GetDamage(scriptableObject.foundDamage); break;
                case Parts.CannonBottom:
                    doubleCannonHp.GetDamage(scriptableObject.bottomDamage); break;
                case Parts.CannonTop:
                    doubleCannonHp.GetDamage(scriptableObject.topDamage); break;
                case Parts.Shield:
                    doubleCannonHp.GetDamage(scriptableObject.shieldDamage); break;

            }

            Destroy(collision.gameObject);
        }
    }
}
