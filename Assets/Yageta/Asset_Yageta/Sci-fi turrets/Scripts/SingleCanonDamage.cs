using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SingleCannonDamage : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] GameObject singleCanon;
    SingleCannonHp singleCannonHp;

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
            singleCannonHp.GetDamage(damage);
            Destroy(collision.gameObject);
        }
    }
}
