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
        combat,  //ターゲットを発見し射撃
        search, //ターゲット喪失
        clear //パワーオフ
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");  //Playerタグがついているものをplayerとして認識
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
        GameObject bulletClone = Instantiate(normalBullet, bulletGeneratePoint.transform.position, Quaternion.identity); //通常弾のクローンを生成
        bulletClone.transform.eulerAngles = bulletGeneratePoint.transform.eulerAngles;  //  弾の向きをあわせる
        Vector3 force = bulletGeneratePoint.gameObject.transform.up * normalBulletSpeed;// forceにy軸方向への力を代入する
        bulletClone.GetComponent<Rigidbody>().AddForce(force);// bulletsにforceの分だけ力をかける
        animator.SetTrigger(nameof(Shoot));
        Destroy(bulletClone.gameObject, 4);// 作成されてから4秒後に消す

        yield return null;
    }



    void LookTarget(GameObject target)
    {
        Vector3 posDif = target.transform.position - canonTop.transform.position;   //グローバル座標での位置差を取得
        Vector3 localPosDif = canonBottom.transform.InverseTransformPoint(target.transform.position);   //Bottomのローカル座標での位置差を取得

        float angleX = Mathf.Atan2(posDif.y, localPosDif.z) * Mathf.Rad2Deg;    //対象物とのX回転角度を計算
        float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //対象物とのY回転角度を計算

        angleX = Mathf.Clamp(angleX, rotateMinX, rotateMaxX);   //X回転を最大値・最小値でクランプ
        if (angleY >= 0 && angleY < rotateMaxY) angleY = rotateMaxY;    //Y回転を最大値・最小値でクランプ
        else if (angleY < 0 && angleY > rotateMinY) angleY = rotateMinY;    

        angleX = Mathf.Repeat(-angleX, 360);    //-180～180表記を0～360表記に変換（回転方向を反転）
        angleY = Mathf.Repeat(angleY, 360);     //-180～180表記を0～360表記に変換

        //canonTopを指定速度で回転
        canonTop.transform.localEulerAngles 
            = new Vector3(Mathf.MoveTowardsAngle(canonTop.transform.eulerAngles.x, angleX, rotateSpeed * Time.deltaTime), 0, 0);
        //canonBottomを指定速度で回転
        canonBottom.transform.localEulerAngles
            = new Vector3(0, Mathf.MoveTowards(canonBottom.transform.localEulerAngles.y, angleY, rotateSpeed * Time.deltaTime), 0);
    }


    void SerchTarget(GameObject target)
    {
        lidar.transform.LookAt(target.transform.position);
        

        RaycastHit hitInfo;//Raycastの当たり判定情報
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
        Vector3 localPosDif = lidar.transform.InverseTransformPoint(target.transform.position);   //Bottomのローカル座標での位置差を取得
        float angleX = Mathf.Atan2(posDif.y, localPosDif.z) * Mathf.Rad2Deg;    //対象物とのX回転角度を計算
        float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //対象物とのY回転角度を計算

        angleX = Mathf.Clamp(angleX, rotateMinX, rotateMaxX);   //X回転を最大値・最小値でクランプ
        angleY = Mathf.Clamp(angleY, rotateMinY, rotateMaxY);   //Y回転を最大値・最小値でクランプ

        angleX = Mathf.Repeat(-angleX, 360);    //-180～180表記を0～360表記に変換（回転方向を反転）
        angleY = Mathf.Repeat(angleY, 360);     //-180～180表記を0～360表記に変換

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
        //canonTopを指定速度で回転
        canonTop.transform.localEulerAngles
            = new Vector3(Mathf.MoveTowardsAngle(canonTop.transform.eulerAngles.x, 30, rotateSpeed/2 * Time.deltaTime), 0, 0);
        //canonBottomを指定速度で回転
        canonBottom.transform.localEulerAngles
            = new Vector3(0, Mathf.MoveTowards(canonBottom.transform.localEulerAngles.y, 180, rotateSpeed/2 * Time.deltaTime), 0);
    }

    


}
