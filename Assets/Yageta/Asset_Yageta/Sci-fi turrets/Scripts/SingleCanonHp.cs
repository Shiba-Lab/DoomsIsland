using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCannonHp : MonoBehaviour
{
    [SerializeField] float maxHp;
    [SerializeField] float currentHp;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHp <= 0)
        {
            Killed();
        }
    }

    public void GetDamage(float damageVal)
    {
        currentHp -= damageVal;
    }

    void Killed()
    {
        Destroy(this.gameObject);
    }
}
