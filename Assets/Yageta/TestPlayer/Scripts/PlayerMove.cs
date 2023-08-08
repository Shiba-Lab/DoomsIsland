using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float rotationSpeed = 2.0f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float ySpeed = 0.0f;

    public GameObject bulletGeneratePoint;
    public GameObject bullet;
    public float bulletSpeed;

    void Start()
    {
        LockCursor();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        Shoot();



    }

    private void Move()
    {

        // Calculate movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 forwardMovement = transform.forward * verticalInput;
        Vector3 rightMovement = transform.right * horizontalInput;
        moveDirection = (forwardMovement + rightMovement).normalized;

        // Apply rotation based on mouse input
        float mouseX = Input.GetAxis("Mouse X");
        Vector3 rotation = new Vector3(0, mouseX * rotationSpeed, 0);
        transform.Rotate(rotation);

        // Apply gravity
        if (characterController.isGrounded)
        {
            ySpeed = 0.0f;

            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ySpeed = jumpForce;
            }
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        // Apply movement and gravity to the character controller
        moveDirection.y = ySpeed;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullets = Instantiate(bullet);// bulletを作成し、作成したものはbulletsとする
            bullets.transform.position = bulletGeneratePoint.transform.position;// bulletsをプレイヤーの場所に移動させる
            Vector3 force = this.gameObject.transform.forward * bulletSpeed;// forceに前方への力を代入する
            bullets.GetComponent<Rigidbody>().AddForce(force);// bulletsにforceの分だけ力をかける
            Destroy(bullets.gameObject, 4);// 作成されてから4秒後に消す
        }
    }


    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //isCursorLocked = true;
    }
}
