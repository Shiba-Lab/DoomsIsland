using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DroneScriptableObject : ScriptableObject
{
    public float knockBackDistance;
    public float knockBackPower;
    public float standardAltitude;
    public float altitudeTolerance;
    public Vector2 moveSpeed;

    public int maxHp;
    public int damage;
}
