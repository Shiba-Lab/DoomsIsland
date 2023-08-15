using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SingleCannonDamage : MonoBehaviour
{
    [SerializeField] SingleCannonScriptableObject scriptableObject;
    float damage;
    [SerializeField] GameObject singleCanon;
    SingleCannonHp singleCannonHp;
    [SerializeField] Parts collisionPart;

    enum Parts
    {
        Found,CannonBottom,CannonTop
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

        }

        singleCannonHp = singleCanon.GetComponent<SingleCannonHp>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            singleCannonHp.GetDamage(damage);
            Destroy(collision.gameObject);
        }
    }
}
