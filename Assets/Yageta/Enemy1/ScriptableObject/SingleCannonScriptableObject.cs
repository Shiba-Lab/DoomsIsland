using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SingleCannonScriptableObject : ScriptableObject
{
    [Tooltip("砲撃間隔（秒）")]
    public float shootInterval;
    [Tooltip("通常弾の速度")]
    public float normalBulletSpeed;
    [Tooltip("砲台の最大回転速度")]
    public float cannonRotateSpeed;
    [Tooltip("砲撃範囲の最小角度（上下方向，左右方向）\n" + "上下方向の回転は，下向きが正\n" + "左右方向の回転は，180度が正面")]
    public Vector2 cannonRotateMin;
    [Tooltip("砲撃範囲の最大角度（上下方向，左右方向）\n" + "上下方向の回転は，下向きが正\n" + "左右方向の回転は，180度が正面")]
    public Vector2 cannonRotateMax;
    [Tooltip("銃口と銃口回転軸の高さの差による，銃口向きのズレを補正する値\n" + "通常は（CannonTopのy座標 - 銃口の高さ）の値")]
    public float heightCorrectionVal;

    [Tooltip("索敵の直線距離制限")]
    public float searchDistance;
    [Tooltip("索敵時間（秒）\n" + "時間内にターゲットが見つからなければ非戦闘状態へ")]
    public float searchTime;
    [Tooltip("非戦闘状態時の索敵範囲の最小角度（上下方向，左右方向）\n" + "左右方向の回転は，180度が正面")]
    public Vector2 searchRotateMin;
    [Tooltip("非戦闘状態時の索敵範囲の最大角度（上下方向，左右方向）\n" + "左右方向の回転は，180度が正面")]
    public Vector2 searchRotateMax;

    public float maxHp;
    public float topDamage;
    public float bottomDamage;
    public float foundDamage;


}
