using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCannonHp : MonoBehaviour
{
    [SerializeField] SingleCannonScriptableObject scriptableObject;
    float maxHp;
    [SerializeField] float currentHp;

    // Start is called before the first frame update
    void Start()
    {
        maxHp = scriptableObject.maxHp;
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
