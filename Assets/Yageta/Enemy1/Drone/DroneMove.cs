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

    float knockBackDistance;
    float knockBackPower;

    float standardAltitude;
    float altitudeTolerance;
    Vector2 moveSpeed;

    DroneScriptableObject droneScriptableObject;
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
        knockBackDistance = droneScriptableObject.knockBackDistance;
        knockBackPower = droneScriptableObject.knockBackPower;
        standardAltitude = droneScriptableObject.standardAltitude;
        altitudeTolerance = droneScriptableObject.altitudeTolerance;
        moveSpeed = droneScriptableObject.moveSpeed;

        agent = droneNav.GetComponent<NavMeshAgent>();
        rb = droneNav.GetComponent<Rigidbody>();
        agent.speed = moveSpeed.x;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(yMoveState);
        

        agent.destination = player.transform.position;
        if (Vector3.Distance(player.transform.position, droneNav.transform.position) < knockBackDistance)
        {
            rb.AddRelativeForce(-Vector3.forward*knockBackPower, ForceMode.Impulse);
        }


        droneBody.transform.position = new Vector3(droneNav.transform.position.x, droneBody.transform.position.y, droneNav.transform.position.z);
        droneBody.transform.LookAt(player.transform.position);
        // 中心座標との距離を計算
        float distanceToCenter = droneBody.transform.position.y-droneNav.transform.position.y;
        Debug.Log(distanceToCenter);
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
            Vector3 moveAmount;
            switch (yMoveState)
            {
                case YMoveState.goDown:
                    
                    moveAmount = Vector3.down * moveSpeed.y * Time.deltaTime;
                    droneBody.transform.Translate(moveAmount);
                    if (distanceToCenter < standardAltitude)
                    {
                        yMoveState = YMoveState.keep;
                    }
                    break;
                case YMoveState.goUp:
                    moveAmount = Vector3.up * moveSpeed.y * Time.deltaTime;
                    droneBody.transform.Translate(moveAmount);
                    if (distanceToCenter > standardAltitude)
                    {
                        yMoveState = YMoveState.keep;
                    }
                    break;


            }
        }
    }
}
