using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using CustomSystem;
using UnityEditor;

public class SingleCannonMove : MonoBehaviour
{
    GameObject player;

    [Tooltip("�C���Ԋu�i�b�j")]
    [SerializeField] float shootInterval;
    [Tooltip("�ʏ�e�̑��x")]
    [SerializeField] float normalBulletSpeed;
    [Tooltip("�C��̍ő��]���x")]
    [SerializeField] float cannonRotateSpeed;
    [Tooltip("�C���͈͂̍ŏ��p�x�i�㉺�����C���E�����j\n"+"�㉺�����̉�]�́C����������\n" + "���E�����̉�]�́C180�x������")]
    [SerializeField] Vector2 cannonRotateMin;
    [Tooltip("�C���͈͂̍ő�p�x�i�㉺�����C���E�����j\n" + "�㉺�����̉�]�́C����������\n" + "���E�����̉�]�́C180�x������")]
    [SerializeField] Vector2 cannonRotateMax;
    [Tooltip("�e���Əe����]���̍����̍��ɂ��C�e�������̃Y����␳����l\n" + "�ʏ�́iCannonTop��y���W - �e���̍����j�̒l")]
    [SerializeField] float heightCorrectionVal;

    [Tooltip("���G�̒�����������")]
    [SerializeField] float searchDistance;
    [Tooltip("���G���ԁi�b�j\n" + "���ԓ��Ƀ^�[�Q�b�g��������Ȃ���Δ�퓬��Ԃ�")]
    [SerializeField] float searchTime;
    [Tooltip("��퓬��Ԏ��̍��G�͈͂̍ŏ��p�x�i�㉺�����C���E�����j\n" + "���E�����̉�]�́C180�x������")]
    [SerializeField] Vector2 searchRotateMin;
    [Tooltip("��퓬��Ԏ��̍��G�͈͂̍ő�p�x�i�㉺�����C���E�����j\n" + "���E�����̉�]�́C180�x������")]
    [SerializeField] Vector2 searchRotateMax;



    [SerializeField] GameObject normalBullet;
    [SerializeField] GameObject bulletGeneratePoint;

    [SerializeField] GameObject cannonBottom;
    [SerializeField] GameObject cannonTop;

    [SerializeField] GameObject lidar;
    [SerializeField] Material stateIndicatorMat;
    bool isStartedTimer;
    CannonState cannonState;
    CannonState preCannonState;

    Animator animator;

    [Tooltip("�R���C�_�[��񊱏ɂ���I�u�W�F�N�g")]
    [SerializeField] GameObject[] ignoreCollisionObjects;

    enum CannonState{
        combat,  //�퓬��ԁF�^�[�Q�b�g�𔭌����ˌ�
        search, //���G��ԁF�^�[�Q�b�g�r��
        clear //��퓬��ԁF�p���[�I�t
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");  //Player�^�O�����Ă�����̂�player�Ƃ��ĔF��
        isStartedTimer = false;
        cannonState = CannonState.clear;
        preCannonState = cannonState;
        stateIndicatorMat.color = Color.green;  //�C���W�P�[�^��ΐF�ɂ���

        animator = cannonTop.GetComponent<Animator>();

        CustomPhysics.IgnoreCollisions(ignoreCollisionObjects); //�z����̃I�u�W�F�N�g�̃R���C�_�[��񊱏ɂ���D
        

    }

    // Update is called once per frame
    void Update()
    {
        SerchTarget(player);    //�v���C���[�̒T�����s��

        if(cannonState == CannonState.combat)   //�퓬��Ԏ�
        {
            LookTarget(player); //�C����v���C���[�֌�����
        }
        else if(cannonState == CannonState.clear)   //��퓬��Ԏ�
        {
            StandbyPosition();  //�C����X�^���o�C�|�W�V�����Ɍ�����
        }

        if(cannonState != preCannonState && cannonState == CannonState.combat)  //�퓬��Ԃֈڍs�����Ƃ�
        {
            InvokeRepeating(nameof(NormalShoot), 0, shootInterval); //�C�����\�b�h���J��Ԃ����s������
            stateIndicatorMat.color = Color.red;    //�C���W�P�[�^��ԐF�ɂ���
        }
        else if(cannonState != preCannonState && cannonState == CannonState.search) //���G��Ԃֈڍs�����Ƃ�
        {
            CancelInvoke(nameof(NormalShoot));  //�C���𒆎~
            stateIndicatorMat.color = Color.yellow; //�C���W�P�[�^�����F�ɂ���
        }
        else if(cannonState != preCannonState && cannonState == CannonState.clear)  //��퓬��Ԃֈڍs�����Ƃ�
        {
            stateIndicatorMat.color = Color.green;  //�C���W�P�[�^��ΐF�ɂ���
        }
        preCannonState = cannonState;

        
    }

    /// <summary>
    /// �ʏ�C�����\�b�h
    /// </summary>
    void NormalShoot() 
    {
        GameObject bulletClone = Instantiate(normalBullet, bulletGeneratePoint.transform.position, Quaternion.identity); //�ʏ�e�̃N���[���𐶐�
        bulletClone.transform.eulerAngles = bulletGeneratePoint.transform.eulerAngles;  //  �e�̌��������킹��
        Vector3 force = bulletGeneratePoint.gameObject.transform.up * normalBulletSpeed;// force��y�������ւ̗͂�������
        bulletClone.GetComponent<Rigidbody>().AddForce(force);// bullets��force�̕������͂�������
        animator.SetTrigger("Shoot");   //�C���A�j���[�V���������s
        Destroy(bulletClone.gameObject, 4);// �쐬����Ă���4�b��ɏ���
    }


    /// <summary>
    /// �e�����^�[�Q�b�g�֌����郁�\�b�h
    /// </summary>
    /// <param name="target">�C���Ώ�</param>
    void LookTarget(GameObject target)
    {
        Vector3 posDif = target.transform.position + new Vector3(0, heightCorrectionVal, 0) - cannonTop.transform.position;   //�O���[�o�����W�ł̈ʒu�����擾
        float distanceLocalZ = Mathf.Sqrt(Mathf.Pow(posDif.x, 2) + Mathf.Pow(posDif.z, 2)); //���[�J�����W��Z�����̋������擾
        float angleX = Mathf.Atan2(posDif.y, distanceLocalZ) * Mathf.Rad2Deg;    //�Ώە��Ƃ�X��]�p�x���v�Z
        float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //�Ώە��Ƃ�Y��]�p�x���v�Z
        Debug.Log(angleX);
        angleX = Mathf.Clamp(angleX, cannonRotateMin.x, cannonRotateMax.x);   //X��]���ő�l�E�ŏ��l�ŃN�����v
        if (angleY >= 0 && angleY < cannonRotateMax.y) angleY = cannonRotateMax.y;    //Y��]���ő�l�E�ŏ��l�ŃN�����v
        else if (angleY < 0 && angleY > cannonRotateMin.y) angleY = cannonRotateMin.y;    

        angleX = Mathf.Repeat(-angleX, 360);    //-180�`180�\�L��0�`360�\�L�ɕϊ��i��]�����𔽓]�j
        angleY = Mathf.Repeat(angleY, 360);     //-180�`180�\�L��0�`360�\�L�ɕϊ�
        Debug.Log(angleX);
        //cannonTop���w�葬�x�ŉ�]
        cannonTop.transform.localEulerAngles
            = new Vector3(Mathf.MoveTowardsAngle(cannonTop.transform.eulerAngles.x, angleX, cannonRotateSpeed * Time.deltaTime), 0, 0);
        //cannonBottom���w�葬�x�ŉ�]
        cannonBottom.transform.localEulerAngles
            = new Vector3(0, Mathf.MoveTowards(cannonBottom.transform.localEulerAngles.y, angleY, cannonRotateSpeed * Time.deltaTime), 0);
        

    }

    /// <summary>
    /// ���G�Z���T�[���^�[�Q�b�g�֌����郁�\�b�h
    /// </summary>
    /// <param name="target">���G�Ώ�</param>
    void SerchTarget(GameObject target)
    {
        if(cannonState == CannonState.clear)
        {
            Vector3 posDif = target.transform.position - lidar.transform.position;   //�O���[�o�����W�ł̈ʒu�����擾
            float distanceLocalZ = Mathf.Sqrt(Mathf.Pow(posDif.x, 2) + Mathf.Pow(posDif.z, 2)); //���[�J�����W��Z�����̋������擾
            float angleX = Mathf.Atan2(posDif.y, distanceLocalZ) * Mathf.Rad2Deg;    //�Ώە��Ƃ�X��]�p�x���v�Z
            float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //�Ώە��Ƃ�Y��]�p�x���v�Z


            angleX = Mathf.Clamp(angleX, searchRotateMin.x, searchRotateMax.x);   //X��]���ő�l�E�ŏ��l�ŃN�����v
            if (angleY >= 0 && angleY < searchRotateMax.y) angleY = searchRotateMax.y;    //Y��]���ő�l�E�ŏ��l�ŃN�����v
            else if (angleY < 0 && angleY > searchRotateMin.y) angleY = searchRotateMin.y;

            angleX = Mathf.Repeat(-angleX, 360);    //-180�`180�\�L��0�`360�\�L�ɕϊ��i��]�����𔽓]�j
            angleY = Mathf.Repeat(angleY, 360);     //-180�`180�\�L��0�`360�\�L�ɕϊ�

            lidar.transform.eulerAngles = new Vector3(angleX, angleY, 0);
        }
        else
        {
            lidar.transform.LookAt(target.transform.position);
        }
        


        /*  ���G�͈͂��e���������Ă���p�x+-�����l�ɐݒ肷��X�N���v�g�i�J���r���j
        angleX = Mathf.Repeat(-angleX, 360);    //-180�`180�\�L��0�`360�\�L�ɕϊ��i��]�����𔽓]�j
        angleY = Mathf.Repeat(angleY, 360);     //-180�`180�\�L��0�`360�\�L�ɕϊ�
        Debug.Log(new Vector3(angleX, angleY, 0));

        
        Vector3 cannonTopAngle = cannonTop.transform.eulerAngles;
        angleX = Mathf.Clamp(angleX, cannonTopAngle.x - searchRotateLimit.x, cannonTopAngle.x + searchRotateLimit.x);
        if (cannonTopAngle.y + searchRotateLimit.y > 360)
        {
            if (angleY > (cannonTopAngle.y + searchRotateLimit.y) % 360 && angleY < 180)
                angleY = (cannonTopAngle.y + searchRotateLimit.y) % 360;
            else if (angleY < cannonTopAngle.y - searchRotateLimit.y && angleY >= 180)
                angleY = cannonTopAngle.y - searchRotateLimit.y;
        }
        else if (cannonTopAngle.y - searchRotateLimit.y < 0)
        {
            if (angleY > cannonTopAngle.y + searchRotateLimit.y && angleY < 180)
                angleY = cannonTopAngle.y + searchRotateLimit.y;
            else if (angleY < cannonTopAngle.y - searchRotateLimit.y + 360 && angleY >= 180)
                angleY = cannonTopAngle.y - searchRotateLimit.y + 360;
        }
        else
        {
            angleY = Mathf.Clamp(angleY, cannonTopAngle.y - searchRotateLimit.y, cannonTopAngle.y + searchRotateLimit.y);
        }
        
        lidar.transform.eulerAngles = new Vector3(angleX, angleY, 0);
        */


        RaycastHit hitInfo;//Raycast�̓����蔻����
        Physics.Raycast(lidar.transform.position, lidar.transform.forward, out hitInfo, searchDistance);    //�Z���T�[�̐��ʕ����ցC�w�苗����Ray���΂�

        if(hitInfo.collider != null && hitInfo.collider.gameObject.tag == target.tag)   //Ray���^�[�Q�b�g�ɓ������Ă���Ƃ�
        {
            CancelInvoke(nameof(TurnOff));  //��퓬��Ԃւ̈ڍs�i���G��ԁj�𒆎~
            isStartedTimer = false; //��퓬��Ԉڍs�p�̃g���K�[�����Z�b�g
            cannonState = CannonState.combat;   //�퓬��Ԃֈڍs
        }
        else   //Ray���^�[�Q�b�g�ȊO�ɓ������Ă���Ƃ�
        {
            if (!isStartedTimer)    //�P���Ŏ��s�����邽�߂̃g���K�[����
            {
                Invoke(nameof(TurnOff), searchTime);    //�w�莞�Ԍ�ɔ�퓬��Ԃւ̈ڍs
                isStartedTimer = true;  //�g���K�[��L���ɂ��C�d�����s��h�~
                cannonState = CannonState.search;   //���G��Ԃֈڍs
            }
        }

        Debug.DrawRay(lidar.transform.position, lidar.transform.forward*searchDistance, Color.red); //Scene���Ray������
    }

    /// <summary>
    /// ��퓬��Ԃֈڍs���郁�\�b�h
    /// </summary>
    void TurnOff()
    {
        cannonState = CannonState.clear;    //��퓬��Ԃֈڍs
    }

    /// <summary>
    /// ��퓬��Ԏ��̖C��̌������w�肷�郁�\�b�h
    /// </summary>
    void StandbyPosition()
    {
        //cannonTop���w�葬�x�ŉ�]
        cannonTop.transform.localEulerAngles                                      //��X��]�̊p�x
            = new Vector3(Mathf.MoveTowardsAngle(cannonTop.transform.eulerAngles.x, 30, cannonRotateSpeed/2 * Time.deltaTime), 0, 0);
        //cannonBottom���w�葬�x�ŉ�]
        cannonBottom.transform.localEulerAngles                                         //��Y��]�̊p�x
            = new Vector3(0, Mathf.MoveTowards(cannonBottom.transform.localEulerAngles.y, 180, cannonRotateSpeed/2 * Time.deltaTime), 0);
    }

}
