using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float rotationSpeed = 3f;

    private Rigidbody rb;
    private bool isGrounded = true;

    public GameObject bulletGeneratePoint;
    public GameObject bullet;
    public float bulletSpeed;

    void Start()
    {
        LockCursor();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        Shoot();



    }

    private void Move()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 forwardMovement = transform.forward * verticalInput;
        Vector3 rightMovement = transform.right * horizontalInput;
        Vector3 moveDirection = (forwardMovement + rightMovement).normalized;
        Vector3 moveVelocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

        // キャラクタの回転
        float mouseX = Input.GetAxis("Mouse X");
        Vector3 rotation = new Vector3(0f, mouseX, 0f) * rotationSpeed;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
