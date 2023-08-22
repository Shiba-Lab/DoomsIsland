using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHp : MonoBehaviour
{
    [SerializeField] DroneScriptableObject scriptableObject;
    [SerializeField] float currentHp;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = scriptableObject.maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0)
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
