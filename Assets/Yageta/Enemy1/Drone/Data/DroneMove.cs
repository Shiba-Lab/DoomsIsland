using CustomSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneMove : MonoBehaviour
{
    [SerializeField] DroneScriptableObject scriptableObject;
    [SerializeField] GameObject droneBody;
    [SerializeField] GameObject droneNav;
    [SerializeField] GameObject bulletGeneratePoint;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject lidar;

    [SerializeField] GameObject player;

    [SerializeField] Transform[] patrolPoints;
    [Tooltip("ランダムにパトロールするか\nオフの場合は要素順に移動する")]
    [SerializeField] bool isRandomPatrol;
    int patrolPointIndex;   //目的地設定用インデックス

    YMoveState yMoveState;
    DroneState droneState;
    DroneState preDroneState;

    NavMeshAgent agent;
    Rigidbody rb;

    [SerializeField] Texture customRed;
    [SerializeField] Texture customBlue;
    [SerializeField] Renderer droneRenderer;
    [ColorUsage(false, true), SerializeField] Color colorRed;
    [ColorUsage(false, true), SerializeField] Color colorBlue;

    [Tooltip("コライダーを非干渉にするオブジェクト")]
    [SerializeField] GameObject[] ignoreCollisionObjects;

    enum YMoveState //高度補正用ステート
    {
        goUp,
        goDown, 
        keep
    }

    enum DroneState //ドローンの状態
    {
        combat,
        clear
    }

    enum ColorState //ドローンの色
    {
        red,
        blue
    }

    // Start is called before the first frame update
    void Start()
    {
        CustomPhysics.IgnoreCollisions(ignoreCollisionObjects); //配列内のオブジェクトのコライダーを非干渉にする．

        agent = droneNav.GetComponent<NavMeshAgent>();
        rb = droneNav.GetComponent<Rigidbody>();

        //ドローンの色を初期化
        ChangeColor(ColorState.blue);


        //NavMeshAgentの設定
        agent.speed = scriptableObject.moveSpeed.x;
        agent.angularSpeed = scriptableObject.rotationSpeed;

        agent.stoppingDistance = scriptableObject.patrolStoppingDistance;   //パトロール用の停止距離を設定
        patrolPointIndex = -1;  //目的地番号の初期値代入
        SetNewDestionation();   //最初の目的地を設定

        //ドローン状態の初期化
        droneState = DroneState.clear;
        preDroneState = droneState;
    }

    // Update is called once per frame
    void Update()
    {
        //ドローンの状態によって動作を変更
        switch (droneState)
        {
            case DroneState.combat: //戦闘状態のとき
                FollowTarget(player);   //プレイヤーを追従
                break;

            case DroneState.clear:  //非戦闘状態のとき
                SearchTarget(player);   //プレイヤーを探索
                Patrol();   //巡回行動
                break;
        }

        //ドローン状態の切り替わり時の処理
        if (droneState != preDroneState && droneState == DroneState.combat) //戦闘状態になったとき
        {
            ChangeColor(ColorState.red);  //ドローンの色を赤に変更
            InvokeRepeating(nameof(Shoot), 2, scriptableObject.bulletInterval); //射撃開始
            agent.stoppingDistance = scriptableObject.followStoppingDistance;   //追従用の停止距離を設定
        }
        else if (droneState != preDroneState && droneState == DroneState.clear) //非戦闘状態になったとき
        {
            ChangeColor(ColorState.blue);   //ドローンの色を青に変更
            CancelInvoke(nameof(Shoot));    //射撃停止
            agent.stoppingDistance = scriptableObject.patrolStoppingDistance;   //パトロール用の停止距離を設定
        }

        preDroneState = droneState; //ドローン状態を更新
    }

    /// <summary>
    /// ターゲットを追従する機能
    /// </summary>
    /// <param name="target">追従対象</param>
    void FollowTarget(GameObject target)
    {
        agent.destination = target.transform.position;  //NavMeshの目的地をターゲットに設定

        //ターゲットと近づき過ぎたときに後退（ノックバック）させる
        if (Vector3.Distance(target.transform.position, droneNav.transform.position) < scriptableObject.knockBackDistance)  
        {
            rb.AddRelativeForce(-Vector3.forward * scriptableObject.knockBackPower, ForceMode.Impulse);
        }

        //ドローンの向きをターゲットの方向に向ける
        Quaternion lookAtRotation = Quaternion.LookRotation(target.transform.position - droneBody.transform.position);
        droneBody.transform.rotation = Quaternion.RotateTowards(droneBody.transform.rotation, lookAtRotation, scriptableObject.rotationSpeed * Time.deltaTime); 

        //ドローンの位置とNavMeshの位置を同期
        droneBody.transform.position = new Vector3(droneNav.transform.position.x, droneBody.transform.position.y, droneNav.transform.position.z);  

        AltitudeCollection();   //高度補正
    }

    /// <summary>
    /// 射撃機能
    /// </summary>
    void Shoot()
    {
        GameObject bulletClone = Instantiate(bullet, bulletGeneratePoint.transform.position, Quaternion.identity); //通常弾のクローンを生成
        bulletClone.transform.eulerAngles = bulletGeneratePoint.transform.eulerAngles;  //  弾の向きをあわせる

    }

    /// <summary>
    /// 索敵機能
    /// </summary>
    void SearchTarget(GameObject target)
    {
        Debug.DrawRay(lidar.transform.position, lidar.transform.forward * scriptableObject.searchDistance); // レイキャストの可視化
        lidar.transform.LookAt(player.transform); // レイキャストの向きをプレイヤーの方向に向ける

          // レイキャストを飛ばす
        RaycastHit hit;
        if (Physics.Raycast(lidar.transform.position, lidar.transform.forward, out hit, scriptableObject.searchDistance))
        {
            if (hit.collider.gameObject.tag == target.tag)    //レイキャストがターゲットに当たったとき
            {
                droneState = DroneState.combat; //戦闘状態に移行
            }
            else
            {
                droneState = DroneState.clear;
            }
        }
        else
        {
            droneState = DroneState.clear;
        }
    }

    /// <summary>
    /// 非戦闘時の巡回機能
    /// </summary>
    void Patrol()
    {
        if (agent.remainingDistance<agent.stoppingDistance) //目的地に到着したとき
        {
            SetNewDestionation();   //目的地を更新
        }

        droneBody.transform.rotation = droneNav.transform.rotation; //ドローンの向きとNavMeshの向きを同期

        droneBody.transform.position = new Vector3(droneNav.transform.position.x, droneBody.transform.position.y, droneNav.transform.position.z);   //ドローンの位置とNavMeshの位置を同期

        AltitudeCollection();   //高度補正

    }

    /// <summary>
    /// 新しい目的地を設定する機能
    /// </summary>
    void SetNewDestionation()
    {
        switch (isRandomPatrol)
        {
            case true:  //ランダム巡回のとき
                int nextPoint = patrolPointIndex;
                while (patrolPointIndex == nextPoint)
                {
                    nextPoint = Random.Range(0, patrolPoints.Length);   //次の目的地番号をランダムに設定
                }
                patrolPointIndex = nextPoint;

                break;

            case false: //順番巡回のとき
                patrolPointIndex++; //次の目的地番号を設定
                if (patrolPointIndex >= patrolPoints.Length)
                {
                    patrolPointIndex = 0;
                }
                break;
        }

        agent.SetDestination(patrolPoints[patrolPointIndex].position);  //次の目的地を設定
    }


    /// <summary>
    /// NavMeshとの高さを調整する機能
    /// </summary>
    void AltitudeCollection()
    {
        //現在の高度を取得
        float currentAltitude = droneBody.transform.position.y - droneNav.transform.position.y;

        if (yMoveState == YMoveState.keep)  //高度維持状態のとき
        {
            if (currentAltitude > scriptableObject.standardAltitude + scriptableObject.altitudeTolerance)    // 許容差以上離れている場合
            {
                yMoveState = YMoveState.goDown; //下降状態に移行
            }
            else if (currentAltitude < scriptableObject.standardAltitude - scriptableObject.altitudeTolerance)   // 許容差以上近づいている場合
            {
                yMoveState = YMoveState.goUp;   //上昇状態に移行
            }
        }
        else  //高度補正時
        {
            Vector3 targetPos = droneNav.transform.position + new Vector3(0, scriptableObject.standardAltitude, 0); //目標高度の位置を設定
            switch (yMoveState)
            {
                case YMoveState.goDown: //下降補正のとき
                    droneBody.transform.position = Vector3.LerpUnclamped(droneBody.transform.position, targetPos, scriptableObject.moveSpeed.y * Time.deltaTime);   //指定速度で降下
                    
                    //目標高度に近づいたら高度維持状態に移行
                    if (currentAltitude < scriptableObject.standardAltitude)
                    {
                        yMoveState = YMoveState.keep;
                    }
                    break;

                case YMoveState.goUp:   //上昇補正のとき
                    droneBody.transform.position = Vector3.LerpUnclamped(droneBody.transform.position, targetPos, scriptableObject.moveSpeed.y * Time.deltaTime);   //指定速度で上昇

                    //目標高度に近づいたら高度維持状態に移行
                    if (currentAltitude > scriptableObject.standardAltitude)
                    {
                        yMoveState = YMoveState.keep;
                    }
                    break;


            }
        }
    }

    void ChangeColor(ColorState colorState)
    {
        switch (colorState)
        {
            case ColorState.blue:
                droneRenderer.material.mainTexture = customBlue;   //ドローンの色を青にする
                droneRenderer.material.SetColor("_EmissionColor", colorBlue);    //ドローンの発光色を青にする
                break;

            case ColorState.red:
                droneRenderer.material.mainTexture = customRed;   //ドローンの色を赤にする
                droneRenderer.material.SetColor("_EmissionColor", colorRed);    //ドローンの発光色を青にする
                break;
        }
    }

}
