using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomSystem
{
    public static class CustomPhysics
    {

        /// <summary>
        /// 配列内のすべてのオブジェクト同士のコライダーが，互いに干渉しないようにするメソッド
        /// </summary>
        /// <param name="gameObjects">非干渉にするゲームオブジェクトの配列</param>
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






