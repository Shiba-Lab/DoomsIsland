using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using CustomSystem;
using UnityEditor;

public class DoubleCannonMove : MonoBehaviour
{
    GameObject player;
    [SerializeField] DoubleCannonScriptableObject scriptableObject;

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

    [Tooltip("コライダーを非干渉にするオブジェクト")]
    [SerializeField] GameObject[] ignoreCollisionObjects;

    enum CannonState{
        combat,  //戦闘状態：ターゲットを発見し射撃
        search, //索敵状態：ターゲット喪失
        clear //非戦闘状態：パワーオフ
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");  //Playerタグがついているものをplayerとして認識
        isStartedTimer = false;
        cannonState = CannonState.clear;
        preCannonState = cannonState;
        stateIndicatorMat.color = Color.green;  //インジケータを緑色にする

        animator = cannonTop.GetComponent<Animator>();

        CustomPhysics.IgnoreCollisions(ignoreCollisionObjects); //配列内のオブジェクトのコライダーを非干渉にする．
    }

    // Update is called once per frame
    void Update()
    {
        SerchTarget(player);    //プレイヤーの探索を行う

        if(cannonState == CannonState.combat)   //戦闘状態時
        {
            LookTarget(player); //砲台をプレイヤーへ向ける
        }
        else if(cannonState == CannonState.clear)   //非戦闘状態時
        {
            StandbyPosition();  //砲台をスタンバイポジションに向ける
        }

        if(cannonState != preCannonState && cannonState == CannonState.combat)  //戦闘状態へ移行したとき
        {
            InvokeRepeating(nameof(NormalShoot), 0, scriptableObject.shootInterval); //砲撃メソッドを繰り返し実行させる
            stateIndicatorMat.color = Color.red;    //インジケータを赤色にする
        }
        else if(cannonState != preCannonState && cannonState == CannonState.search) //索敵状態へ移行したとき
        {
            CancelInvoke(nameof(NormalShoot));  //砲撃を中止
            stateIndicatorMat.color = Color.yellow; //インジケータを黄色にする
        }
        else if(cannonState != preCannonState && cannonState == CannonState.clear)  //非戦闘状態へ移行したとき
        {
            stateIndicatorMat.color = Color.green;  //インジケータを緑色にする
        }
        preCannonState = cannonState;

        
    }

    /// <summary>
    /// 通常砲撃メソッド
    /// </summary>
    void NormalShoot() 
    {
        GameObject bulletClone = Instantiate(normalBullet, bulletGeneratePoint.transform.position, Quaternion.identity); //通常弾のクローンを生成
        bulletClone.transform.eulerAngles = bulletGeneratePoint.transform.eulerAngles;  //  弾の向きをあわせる
        Vector3 force = bulletGeneratePoint.gameObject.transform.up * scriptableObject.normalBulletSpeed;// forceにy軸方向への力を代入する
        bulletClone.GetComponent<Rigidbody>().AddForce(force);// bulletsにforceの分だけ力をかける
        //animator.SetTrigger("Shoot");   //砲撃アニメーションを実行
        Destroy(bulletClone.gameObject, 4);// 作成されてから4秒後に消す
    }


    /// <summary>
    /// 銃口をターゲットへ向けるメソッド
    /// </summary>
    /// <param name="target">砲撃対象</param>
    void LookTarget(GameObject target)
    {
        Vector3 posDif = target.transform.position + new Vector3(0, scriptableObject.heightCorrectionVal, 0) - cannonTop.transform.position;   //グローバル座標での位置差を取得
        float distanceLocalZ = Mathf.Sqrt(Mathf.Pow(posDif.x, 2) + Mathf.Pow(posDif.z, 2)); //ローカル座標のZ方向の距離を取得
        float angleX = Mathf.Atan2(posDif.y, distanceLocalZ) * Mathf.Rad2Deg;    //対象物とのX回転角度を計算
        float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //対象物とのY回転角度を計算

        angleX = Mathf.Clamp(angleX, scriptableObject.cannonRotateMin.x, scriptableObject.cannonRotateMax.x);   //X回転を最大値・最小値でクランプ
        if (angleY >= 0 && angleY < scriptableObject.cannonRotateMax.y) angleY = scriptableObject.cannonRotateMax.y;    //Y回転を最大値・最小値でクランプ
        else if (angleY < 0 && angleY > scriptableObject.cannonRotateMin.y) angleY = scriptableObject.cannonRotateMin.y;    

        angleX = Mathf.Repeat(-angleX, 360);    //-180〜180表記を0〜360表記に変換（回転方向を反転）
        angleY = Mathf.Repeat(angleY, 360);     //-180〜180表記を0〜360表記に変換
        
        //cannonTopを指定速度で回転
        cannonTop.transform.localEulerAngles
            = new Vector3(Mathf.MoveTowardsAngle(cannonTop.transform.eulerAngles.x, angleX, scriptableObject.cannonRotateSpeed * Time.deltaTime), 0, 0);
        //cannonBottomを指定速度で回転
        cannonBottom.transform.localEulerAngles
            = new Vector3(0, Mathf.MoveTowards(cannonBottom.transform.localEulerAngles.y, angleY, scriptableObject.cannonRotateSpeed * Time.deltaTime), 0);
        

    }

    /// <summary>
    /// 索敵センサーをターゲットへ向けるメソッド
    /// </summary>
    /// <param name="target">索敵対象</param>
    void SerchTarget(GameObject target)
    {
        if(cannonState == CannonState.clear)
        {
            Vector3 posDif = target.transform.position - lidar.transform.position;   //グローバル座標での位置差を取得
            float distanceLocalZ = Mathf.Sqrt(Mathf.Pow(posDif.x, 2) + Mathf.Pow(posDif.z, 2)); //ローカル座標のZ方向の距離を取得
            float angleX = Mathf.Atan2(posDif.y, distanceLocalZ) * Mathf.Rad2Deg;    //対象物とのX回転角度を計算
            float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //対象物とのY回転角度を計算


            angleX = Mathf.Clamp(angleX, scriptableObject.searchRotateMin.x, scriptableObject.searchRotateMax.x);   //X回転を最大値・最小値でクランプ
            if (angleY >= 0 && angleY < scriptableObject.searchRotateMax.y) angleY = scriptableObject.searchRotateMax.y;    //Y回転を最大値・最小値でクランプ
            else if (angleY < 0 && angleY > scriptableObject.searchRotateMin.y) angleY = scriptableObject.searchRotateMin.y;

            angleX = Mathf.Repeat(-angleX, 360);    //-180〜180表記を0〜360表記に変換（回転方向を反転）
            angleY = Mathf.Repeat(angleY, 360);     //-180〜180表記を0〜360表記に変換

            lidar.transform.eulerAngles = new Vector3(angleX, angleY, 0);
        }
        else
        {
            lidar.transform.LookAt(target.transform.position);
        }
        


        /*  索敵範囲を銃口が向いている角度+-制限値に設定するスクリプト（開発途中）
        angleX = Mathf.Repeat(-angleX, 360);    //-180〜180表記を0〜360表記に変換（回転方向を反転）
        angleY = Mathf.Repeat(angleY, 360);     //-180〜180表記を0〜360表記に変換
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


        RaycastHit hitInfo;//Raycastの当たり判定情報
        Physics.Raycast(lidar.transform.position, lidar.transform.forward, out hitInfo, scriptableObject.searchDistance);    //センサーの正面方向へ，指定距離のRayを飛ばす

        if(hitInfo.collider != null && hitInfo.collider.gameObject.tag == target.tag)   //Rayがターゲットに当たっているとき
        {
            CancelInvoke(nameof(TurnOff));  //非戦闘状態への移行（索敵状態）を中止
            isStartedTimer = false; //非戦闘状態移行用のトリガーをリセット
            cannonState = CannonState.combat;   //戦闘状態へ移行
        }
        else   //Rayがターゲット以外に当たっているとき
        {
            if (!isStartedTimer)    //単発で実行させるためのトリガー判定
            {
                Invoke(nameof(TurnOff), scriptableObject.searchTime);    //指定時間後に非戦闘状態への移行
                isStartedTimer = true;  //トリガーを有効にし，重複実行を防止
                cannonState = CannonState.search;   //索敵状態へ移行
            }
        }

        Debug.DrawRay(lidar.transform.position, lidar.transform.forward*scriptableObject.searchDistance, Color.red); //Scene上でRayを可視化
    }

    /// <summary>
    /// 非戦闘状態へ移行するメソッド
    /// </summary>
    void TurnOff()
    {
        cannonState = CannonState.clear;    //非戦闘状態へ移行
    }

    /// <summary>
    /// 非戦闘状態時の砲台の向きを指定するメソッド
    /// </summary>
    void StandbyPosition()
    {
        //cannonTopを指定速度で回転
        cannonTop.transform.localEulerAngles                                      //↓X回転の角度
            = new Vector3(Mathf.MoveTowardsAngle(cannonTop.transform.eulerAngles.x, 30, scriptableObject.cannonRotateSpeed/2 * Time.deltaTime), 0, 0);
        //cannonBottomを指定速度で回転
        cannonBottom.transform.localEulerAngles                                         //↓Y回転の角度
            = new Vector3(0, Mathf.MoveTowards(cannonBottom.transform.localEulerAngles.y, 180, scriptableObject.cannonRotateSpeed/2 * Time.deltaTime), 0);
    }

}
