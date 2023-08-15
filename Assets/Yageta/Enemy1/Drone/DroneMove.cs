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

        // ���S���W�Ƃ̋������v�Z
        float distanceToCenter = droneBody.transform.position.y-droneNav.transform.position.y;

        if (yMoveState == YMoveState.keep)
        {
            // ���e���ȏ㗣��Ă���ꍇ
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

            // ���S���W�Ɍ����������x�N�g�����v�Z
            Vector3 moveDirection = yMoveState == YMoveState.goUp? Vector3.up : Vector3.down;

            // �ړ����x���l�����Ĉړ��ʂ��v�Z
            Vector3 moveAmount = moveDirection * moveSpeed.y * Time.deltaTime;

            // �ړ�
            transform.Translate(moveAmount);

            // �ڕW�ʒu�ɏ\���ɋ߂Â�����ړ��I��
            if (Mathf.Abs(distanceToCenter) < altitudeTolerance/10)
            {
                yMoveState = YMoveState.keep;
            }
        }
    }
}
