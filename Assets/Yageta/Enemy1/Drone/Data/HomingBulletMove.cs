using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBulletMove : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(target.transform);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Destroy(gameObject);
    }
}
