using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SingleCannonDamage : MonoBehaviour
{
    [SerializeField] SingleCannonScriptableObject scriptableObject;
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
            switch (collisionPart)
            {
                case Parts.Found:
                    singleCannonHp.GetDamage(scriptableObject.foundDamage); break;
                case Parts.CannonBottom:
                    singleCannonHp.GetDamage(scriptableObject.bottomDamage); break;
                case Parts.CannonTop:
                    singleCannonHp.GetDamage(scriptableObject.topDamage); break;
            }
            Destroy(collision.gameObject);
        }
    }
}
