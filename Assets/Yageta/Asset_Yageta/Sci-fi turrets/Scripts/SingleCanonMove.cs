using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SingleCanonMove : MonoBehaviour
{
    GameObject player;

    [SerializeField] float shootInterval;
    [SerializeField] float normalBulletSpeed;

    [SerializeField] float rotateSpeed;
    [SerializeField] float rotateMinX;
    [SerializeField] float rotateMaxX;
    [SerializeField] float rotateMinY;
    [SerializeField] float rotateMaxY;

    [SerializeField] float searchDistance;
    [SerializeField] float searchTime;


    [SerializeField] GameObject normalBullet;
    [SerializeField] GameObject bulletGeneratePoint;

    [SerializeField] GameObject canonBottom;
    [SerializeField] GameObject canonTop;

    [SerializeField] GameObject lidar;
    bool isFindTarget;
    bool isStartedTimer;
    bool isFire;
    CanonState canonState;
    CanonState preCanonState;

    Animator animator;

    enum CanonState{
        combat,  //�^�[�Q�b�g�𔭌����ˌ�
        search, //�^�[�Q�b�g�r��
        clear //�p���[�I�t
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");  //Player�^�O�����Ă�����̂�player�Ƃ��ĔF��
        isFindTarget = false;
        isStartedTimer = false;
        isFire = false;
        canonState = CanonState.clear;
        preCanonState = canonState;

        animator = canonTop.GetComponent<Animator>();
        

    }

    // Update is called once per frame
    void Update()
    {
        SerchTarget(player);

        if(canonState == CanonState.combat)
        {
            LookTarget(player);
        }
        else if(canonState == CanonState.clear)
        {
            StandbyPosition();
        }
        if(canonState != preCanonState && canonState == CanonState.combat)
        {
            isFire = true;
            StartCoroutine(Shoot());
        }
        if(canonState == CanonState.search || canonState == CanonState.clear)
        {
            isFire = false;
        }
        preCanonState = canonState;

        Debug.Log(canonState);
        
    }

    IEnumerator Shoot()
    {
        while (isFire)
        {
            StartCoroutine(NormalShoot());
            yield return new WaitForSeconds(shootInterval);
        }


        yield return null;
    }

    IEnumerator NormalShoot() 
    {
        GameObject bulletClone = Instantiate(normalBullet, bulletGeneratePoint.transform.position, Quaternion.identity); //�ʏ�e�̃N���[���𐶐�
        bulletClone.transform.eulerAngles = bulletGeneratePoint.transform.eulerAngles;  //  �e�̌��������킹��
        Vector3 force = bulletGeneratePoint.gameObject.transform.up * normalBulletSpeed;// force��y�������ւ̗͂�������
        bulletClone.GetComponent<Rigidbody>().AddForce(force);// bullets��force�̕������͂�������
        animator.SetTrigger(nameof(Shoot));
        Destroy(bulletClone.gameObject, 4);// �쐬����Ă���4�b��ɏ���

        yield return null;
    }



    void LookTarget(GameObject target)
    {
        Vector3 posDif = target.transform.position - canonTop.transform.position;   //�O���[�o�����W�ł̈ʒu�����擾
        Vector3 localPosDif = canonBottom.transform.InverseTransformPoint(target.transform.position);   //Bottom�̃��[�J�����W�ł̈ʒu�����擾

        float angleX = Mathf.Atan2(posDif.y, localPosDif.z) * Mathf.Rad2Deg;    //�Ώە��Ƃ�X��]�p�x���v�Z
        float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //�Ώە��Ƃ�Y��]�p�x���v�Z

        angleX = Mathf.Clamp(angleX, rotateMinX, rotateMaxX);   //X��]���ő�l�E�ŏ��l�ŃN�����v
        if (angleY >= 0 && angleY < rotateMaxY) angleY = rotateMaxY;    //Y��]���ő�l�E�ŏ��l�ŃN�����v
        else if (angleY < 0 && angleY > rotateMinY) angleY = rotateMinY;    

        angleX = Mathf.Repeat(-angleX, 360);    //-180�`180�\�L��0�`360�\�L�ɕϊ��i��]�����𔽓]�j
        angleY = Mathf.Repeat(angleY, 360);     //-180�`180�\�L��0�`360�\�L�ɕϊ�

        //canonTop���w�葬�x�ŉ�]
        canonTop.transform.localEulerAngles 
            = new Vector3(Mathf.MoveTowardsAngle(canonTop.transform.eulerAngles.x, angleX, rotateSpeed * Time.deltaTime), 0, 0);
        //canonBottom���w�葬�x�ŉ�]
        canonBottom.transform.localEulerAngles
            = new Vector3(0, Mathf.MoveTowards(canonBottom.transform.localEulerAngles.y, angleY, rotateSpeed * Time.deltaTime), 0);
    }


    void SerchTarget(GameObject target)
    {
        lidar.transform.LookAt(target.transform.position);
        

        RaycastHit hitInfo;//Raycast�̓����蔻����
        Physics.Raycast(lidar.transform.position, lidar.transform.forward, out hitInfo, searchDistance);
        if(hitInfo.collider.gameObject.tag == target.tag)
        {
            isFindTarget = true;
            CancelInvoke(nameof(TurnOff));
            isStartedTimer = false;
            canonState = CanonState.combat;
        }
        else
        {
            isFindTarget = false;

            if (!isStartedTimer)
            {
                Invoke(nameof(TurnOff), searchTime);
                isStartedTimer = true;
                canonState = CanonState.search;
            }
        }

        Debug.DrawRay(lidar.transform.position, lidar.transform.forward*searchDistance, Color.red);
        Debug.Log(isFindTarget);


        /*
        Vector3 posDif = target.transform.position - lidar.transform.position;
        Vector3 localPosDif = lidar.transform.InverseTransformPoint(target.transform.position);   //Bottom�̃��[�J�����W�ł̈ʒu�����擾
        float angleX = Mathf.Atan2(posDif.y, localPosDif.z) * Mathf.Rad2Deg;    //�Ώە��Ƃ�X��]�p�x���v�Z
        float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //�Ώە��Ƃ�Y��]�p�x���v�Z

        angleX = Mathf.Clamp(angleX, rotateMinX, rotateMaxX);   //X��]���ő�l�E�ŏ��l�ŃN�����v
        angleY = Mathf.Clamp(angleY, rotateMinY, rotateMaxY);   //Y��]���ő�l�E�ŏ��l�ŃN�����v

        angleX = Mathf.Repeat(-angleX, 360);    //-180�`180�\�L��0�`360�\�L�ɕϊ��i��]�����𔽓]�j
        angleY = Mathf.Repeat(angleY, 360);     //-180�`180�\�L��0�`360�\�L�ɕϊ�

        lidar.transform.localEulerAngles
            = new Vector3(angleX, angleY, 0);
        */

    }

    void TurnOff()
    {
        canonState = CanonState.clear;
    }

    void StandbyPosition()
    {
        //canonTop���w�葬�x�ŉ�]
        canonTop.transform.localEulerAngles
            = new Vector3(Mathf.MoveTowardsAngle(canonTop.transform.eulerAngles.x, 30, rotateSpeed/2 * Time.deltaTime), 0, 0);
        //canonBottom���w�葬�x�ŉ�]
        canonBottom.transform.localEulerAngles
            = new Vector3(0, Mathf.MoveTowards(canonBottom.transform.localEulerAngles.y, 180, rotateSpeed/2 * Time.deltaTime), 0);
    }

    


}
