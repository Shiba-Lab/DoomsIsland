using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;

public class DroneMove : MonoBehaviour
{
    [SerializeField] GameObject droneBody;
    [SerializeField] GameObject droneNav;
    [SerializeField] GameObject player;

    [SerializeField] float knockBackDistance;
    [SerializeField] float knockBackPower;

    [SerializeField] float standardAltitude;
    [SerializeField] float altitudeTolerance;
    [SerializeField] Vector2 moveSpeed;

    YMoveState yMoveState;

    


    NavMeshAgent agent;
    Rigidbody rb;


    enum YMoveState
    {
        goUp,
        goDown, 
        keep
    }
    // Start is called before the first frame update
    void Start()
    {
        agent = droneNav.GetComponent<NavMeshAgent>();
        rb = droneNav.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player.transform.position;
        if (Vector3.Distance(player.transform.position, droneNav.transform.position) < knockBackDistance)
        {
            rb.AddRelativeForce(-Vector3.forward*knockBackPower, ForceMode.Impulse);
        }

        // 中心座標との距離を計算
        float distanceToCenter = droneBody.transform.position.y-droneNav.transform.position.y;

        if (yMoveState == YMoveState.keep)
        {
            // 許容差以上離れている場合
            if (distanceToCenter > standardAltitude + altitudeTolerance) 
            {
                yMoveState = YMoveState.goDown;
            }
            else if(distanceToCenter < standardAltitude - altitudeTolerance)
            {
                yMoveState = YMoveState.goUp;
            }
        }
        else
        {

            // 中心座標に向かう方向ベクトルを計算
            Vector3 moveDirection = yMoveState == YMoveState.goUp? Vector3.up : Vector3.down;

            // 移動速度を考慮して移動量を計算
            Vector3 moveAmount = moveDirection * moveSpeed.y * Time.deltaTime;

            // 移動
            transform.Translate(moveAmount);

            // 目標位置に十分に近づいたら移動終了
            if (Mathf.Abs(distanceToCenter) < altitudeTolerance/10)
            {
                yMoveState = YMoveState.keep;
            }
        }
    }
}
