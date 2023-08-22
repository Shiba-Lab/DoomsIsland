using UnityEngine;

[CreateAssetMenu]
public class DroneScriptableObject : ScriptableObject
{
    [Tooltip("ドローンがホバリングするプレイヤーとの距離")]
    public float followStoppingDistance;
    [Tooltip("パトロール中のドローンの停止距離")]
    public float patrolStoppingDistance;
    [Tooltip("パトロール中の各目標地点での停止時間")]
    public float patrolStopTime;
    [Tooltip("プレイヤーとの距離がDistance以下になるとドローンが後退（押しのける）")]
    public float knockBackDistance;
    [Tooltip("ドローンを押しのける力の強さ")]
    public float knockBackPower;
    [Tooltip("飛行高度")]
    public float standardAltitude;
    [Tooltip("高さ補正\n値が大きいほど，障害物を乗り越えたときの上下移動が少なくなる")]
    public float altitudeTolerance;
    [Tooltip("移動速度（水平，垂直）")]
    public Vector2 moveSpeed;
    [Tooltip("最大回転速度")]
    public float rotationSpeed;
    
    public int maxHp;
    public int bodyDamage;
    public int weakpointDamage;

    [Tooltip("弾の発射間隔")]
    public float bulletInterval;

    [Tooltip("索敵距離")]
    public float searchDistance;
}
