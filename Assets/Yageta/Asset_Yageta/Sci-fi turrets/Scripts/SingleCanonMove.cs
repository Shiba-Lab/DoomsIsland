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
        player = GameObject.FindWithTag("Player");  //Playerタグがついているものをplayerとして認識

        
        InvokeRepeating(nameof(Shoot), 0,shootInterval);
    }

    // Update is called once per frame
    void Update()
    {
        LookPlayer();
    }

    void Shoot()
    {
        GameObject bulletClone = Instantiate(energyBullet);// bulletを作成し、作成したものはbulletsとする
        bulletClone.transform.position = bulletGeneratePoint.transform.position;// bulletsをプレイヤーの場所に移動させる
        Vector3 force = bulletGeneratePoint.gameObject.transform.up * bulletSpeed;// forceにy軸方向への力を代入する
        bulletClone.GetComponent<Rigidbody>().AddForce(force);// bulletsにforceの分だけ力をかける
        Destroy(bulletClone.gameObject, 4);// 作成されてから4秒後に消す
    }

    void LookPlayer()
    {
        Vector3 posDif = player.transform.position - canonTop.transform.position;   //グローバル座標での位置差を取得
        Vector3 localPosDif = canonBottom.transform.InverseTransformPoint(player.transform.position);   //Bottomのローカル座標での位置差を取得

        float angleX = Mathf.Atan2(posDif.y, localPosDif.z) * Mathf.Rad2Deg;    //対象物とのX回転角度を計算
        float angleY = Mathf.Atan2(posDif.x, posDif.z) * Mathf.Rad2Deg;         //対象物とのY回転角度を計算

        angleX = Mathf.Clamp(angleX, rotateMinX, rotateMaxX);   //X回転を最大値・最小値でクランプ
        angleY = Mathf.Clamp(angleY, rotateMinY, rotateMaxY);   //Y回転を最大値・最小値でクランプ

        angleX = Mathf.Repeat(-angleX, 360);    //-180～180表記を0～360表記に変換（回転方向を反転）
        angleY = Mathf.Repeat(angleY, 360);     //-180～180表記を0～360表記に変換

        //canonTopを指定速度で回転
        canonTop.transform.localEulerAngles 
            = new Vector3(Mathf.MoveTowardsAngle(canonTop.transform.eulerAngles.x, angleX, rotateSpeed * Time.deltaTime), 0, 0);
        //canonBottomを指定速度で回転
        canonBottom.transform.localEulerAngles
            = new Vector3(0, Mathf.MoveTowards(canonBottom.transform.localEulerAngles.y, angleY, rotateSpeed * Time.deltaTime), 0);
    }


}
