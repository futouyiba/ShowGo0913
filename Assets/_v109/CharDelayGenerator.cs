using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
// using Unity.VisualScripting;
using UnityEngine;

namespace ET
{
    public class CharDelayGenerator : MonoBehaviour
    {
        /// <summary>
        /// 每N帧创建角色
        /// </summary>
        [SerializeField] private int perNFrame;
        /// <summary>
        /// 创建时创建多少个,0=不限制
        /// </summary>
        [SerializeField] private int generateNum;
        [ReadOnly] public int toGenerateCounter;
        private Queue<Action> generateQueue;
        private void Awake()
        {
            toGenerateCounter = perNFrame;
            generateQueue = new Queue<Action>();
        }

        private void Update()
        {
            
            if (generateQueue.Count > 0)
            {
                toGenerateCounter -= 1;
                if (toGenerateCounter <= 0)
                {
                    int toGenerate = Mathf.Min(generateNum, generateQueue.Count);
                    if (toGenerate == 0)
                    {
                        toGenerate = generateQueue.Count;
                    }
                    for (int i = 0; i < toGenerate; i++)
                    {
                        var exec = generateQueue.Dequeue();
                        exec?.Invoke();
                    }

                    toGenerateCounter = perNFrame;
                }    
            }
            
        }


        public void AddCreateTask(Action createAction)
        {
            generateQueue.Enqueue(createAction);
        }
        
    }
}
