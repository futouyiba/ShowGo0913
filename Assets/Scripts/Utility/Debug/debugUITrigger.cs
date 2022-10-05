using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ET
{
    public class debugUITrigger : SerializedMonoBehaviour,IDebugOnOff
    {
        [SerializeField] protected int clickTimes;
        
        private int clicked = 0;

        [SerializeField] private GameObject debugUI;

        [NonSerialized,OdinSerialize] public List<IDebugOnOff> debugOnOffs;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        private float idleTime = 0f;
        // Update is called once per frame
        void Update()
        {
            if (clicked > 0)
            {
                idleTime += Time.deltaTime;
                if (idleTime > 3f)
                {
                    ResetClicked();
                }
            }
        }

        private void ResetClicked()
        {
            idleTime = 0f;
            clicked = 0;
        }

        private void OnMouseDown()
        {
            clicked += 1;
            idleTime = 0;
            if (clicked >= clickTimes)
            {
                Switch();
                
                ResetClicked();
            }
            
            
        }
        
        public void UIPointerDown()
        {
            OnMouseDown();
        }

        public void Switch()
        {
            debugUI.SetActive(!debugUI.activeSelf);
            foreach (var debugOnOff in debugOnOffs)
            {
                debugOnOff.Switch();
            }
        }
    }


    /// <summary>
    /// implement this interface to enable and disable with debug mode
    /// </summary>
    public interface IDebugOnOff
    {
        public void Switch();
    }
}
