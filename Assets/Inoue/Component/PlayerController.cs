using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;

    [SerializeField] float speed;
    [SerializeField] GameObject eye;

    Vector3 eyer;

    static int hashFront = Animator.StringToHash("Front");
    static int hashSide = Animator.StringToHash("Side");
    float x = 0;
    float y = 0;
    void Awake()
    {
        TryGetComponent(out animator);
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        
        eyer = eye.transform.eulerAngles;
        //var inputX = Input.GetAxis("Horizontal");
        //var inputY = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.D))
        {
            x = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x = -1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            y = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            y = -1;
        }

        var leftStick = new Vector3(x, 0, y).normalized;

        var velocity = speed * leftStick;

        this.transform.eulerAngles = eyer;

        animator.SetFloat(hashFront, velocity.z, 0.1f, Time.deltaTime);
        animator.SetFloat(hashSide, velocity.x, 0.1f, Time.deltaTime);

        x = 0;
        y = 0;
    }
}
