using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SingleCannonScriptableObject : ScriptableObject
{
    [Tooltip("�C���Ԋu�i�b�j")]
    public float shootInterval;
    [Tooltip("�ʏ�e�̑��x")]
    public float normalBulletSpeed;
    [Tooltip("�C��̍ő��]���x")]
    public float cannonRotateSpeed;
    [Tooltip("�C���͈͂̍ŏ��p�x�i�㉺�����C���E�����j\n" + "�㉺�����̉�]�́C����������\n" + "���E�����̉�]�́C180�x������")]
    public Vector2 cannonRotateMin;
    [Tooltip("�C���͈͂̍ő�p�x�i�㉺�����C���E�����j\n" + "�㉺�����̉�]�́C����������\n" + "���E�����̉�]�́C180�x������")]
    public Vector2 cannonRotateMax;
    [Tooltip("�e���Əe����]���̍����̍��ɂ��C�e�������̃Y����␳����l\n" + "�ʏ�́iCannonTop��y���W - �e���̍����j�̒l")]
    public float heightCorrectionVal;

    [Tooltip("���G�̒�����������")]
    public float searchDistance;
    [Tooltip("���G���ԁi�b�j\n" + "���ԓ��Ƀ^�[�Q�b�g��������Ȃ���Δ�퓬��Ԃ�")]
    public float searchTime;
    [Tooltip("��퓬��Ԏ��̍��G�͈͂̍ŏ��p�x�i�㉺�����C���E�����j\n" + "���E�����̉�]�́C180�x������")]
    public Vector2 searchRotateMin;
    [Tooltip("��퓬��Ԏ��̍��G�͈͂̍ő�p�x�i�㉺�����C���E�����j\n" + "���E�����̉�]�́C180�x������")]
    public Vector2 searchRotateMax;

    public float maxHp;
    public float topDamage;
    public float bottomDamage;
    public float foundDamage;


}
