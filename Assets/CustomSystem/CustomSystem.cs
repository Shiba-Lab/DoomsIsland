using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomSystem
{
    public static class CustomPhysics
    {

        /// <summary>
        /// �z����̂��ׂẴI�u�W�F�N�g���m�̃R���C�_�[���C�݂��Ɋ����Ȃ��悤�ɂ��郁�\�b�h
        /// </summary>
        /// <param name="gameObjects">�񊱏ɂ���Q�[���I�u�W�F�N�g�̔z��</param>
        public static void IgnoreCollisions(GameObject[] gameObjects)
        {
            int count = gameObjects.Length;

            for (int i = 0; i < count; i++)
            {
                Collider[] colliders = gameObjects[i].GetComponents<Collider>();

                for (int j = 0; j < colliders.Length; j++)
                {
                    for (int k = i + 1; k < count; k++)
                    {
                        Collider[] otherColliders = gameObjects[k].GetComponents<Collider>();

                        for (int l = 0; l < otherColliders.Length; l++)
                        {
                            Physics.IgnoreCollision(colliders[j], otherColliders[l]);
                        }
                    }
                }
            }
        }
    }
}






