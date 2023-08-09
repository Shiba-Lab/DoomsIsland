using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SingleCanonDamage : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] GameObject singleCanon;
    SingleCanonHp singleCanonHp;

    // Start is called before the first frame update
    void Start()
    {
        singleCanonHp = singleCanon.GetComponent<SingleCanonHp>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            singleCanonHp.GetDamage(damage);
            Destroy(collision.gameObject);
        }
    }
}
