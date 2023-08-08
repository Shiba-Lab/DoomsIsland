using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SingleCanonMove : MonoBehaviour
{
    GameObject player;

    [SerializeField] float shootInterval;
    [SerializeField] float bulletSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float rotateMinX;
    [SerializeField] float rotateMaxX;
    [SerializeField] float rotateMinY;
    [SerializeField] float rotateMaxY;

    [SerializeField] GameObject energyBullet;
    [SerializeField] GameObject bulletGeneratePoint;
    
    
    //[SerializeField] GameObject hinge;
    [SerializeField] GameObject canonBottom;
    [SerializeField] GameObject canonTop;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");  //Player�^�O�����Ă�����̂�player�Ƃ��ĔF��

        
        InvokeRepeating(nameof(Shoot), 0,shootInterval);
    }

    // Update is called once per frame
    void Update()
    {
        LookPlayer();
    }

    void Shoot()
    {
        GameObject bulletClone = Instantiate(energyBullet);// bullet���쐬���A�쐬�������̂�bullets�Ƃ���
        bulletClone.transform.position = bulletGeneratePoint.transform.position;// bullets���v���C���[�̏ꏊ�Ɉړ�������
        Vector3 force = bulletGeneratePoint.gameObject.transform.up * bulletSpeed;// force��y�������ւ̗͂�������
        bulletClone.GetComponent<Rigidbody>().AddForce(force);// bullets��force�̕������͂�������
        Destroy(bulletClone.gameObject, 4);// �쐬����Ă���4�b��ɏ���
    }

    void LookPlayer()
    {
        Vector3 posDif = player.transform.position - canonTop.transform.position;   //�O���[�o�����W�ł̈ʒu�����擾
        Vector3 localPosDif = canonBottom.transform.InverseTransformPoint(player.transform.position);   //Bottom�̃��[�J�����W�ł̈ʒu�����擾

        float angleX = Mathf.Atan2(posDif.y, localPosDif.z) * Mathf.Rad2Deg;    //�Ώە��Ƃ�X��]�p�x���v�Z
        float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //�Ώە��Ƃ�Y��]�p�x���v�Z

        angleX = Mathf.Clamp(angleX, rotateMinX, rotateMaxX);   //X��]���ő�l�E�ŏ��l�ŃN�����v
        angleY = Mathf.Clamp(angleY, rotateMinY, rotateMaxY);   //Y��]���ő�l�E�ŏ��l�ŃN�����v

        angleX = Mathf.Repeat(-angleX, 360);    //-180�`180�\�L��0�`360�\�L�ɕϊ��i��]�����𔽓]�j
        angleY = Mathf.Repeat(angleY, 360);     //-180�`180�\�L��0�`360�\�L�ɕϊ�

        //canonTop���w�葬�x�ŉ�]
        canonTop.transform.localEulerAngles 
            = new Vector3(Mathf.MoveTowardsAngle(canonTop.transform.eulerAngles.x, angleX, rotateSpeed * Time.deltaTime), 0, 0);
        //canonBottom���w�葬�x�ŉ�]
        canonBottom.transform.localEulerAngles
            = new Vector3(0, Mathf.MoveTowards(canonBottom.transform.localEulerAngles.y, angleY, rotateSpeed * Time.deltaTime), 0);
    }


}
