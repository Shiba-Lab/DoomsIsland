using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;

    [SerializeField] float speed;

    static int hashFront = Animator.StringToHash("Front");
    static int hashSide = Animator.StringToHash("Side");
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
        var inputX = Input.GetAxis("Horizontal");
        var inputY = Input.GetAxis("Vertical");
        var leftStick = new Vector3(inputX, 0, inputY).normalized;

        var velocity = speed * leftStick;

        animator.SetFloat(hashFront, velocity.z, 0.1f, Time.deltaTime);
        animator.SetFloat(hashSide, velocity.z, 0.1f, Time.deltaTime);
    }
}
