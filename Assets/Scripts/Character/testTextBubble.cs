using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ET
{
    public class testTextBubble : MonoBehaviour
    {
        [SerializeField] protected TextMeshPro tmp;
        // Start is called before the first frame update
        void Start()
        {
            var text=TextBubble.SpeakFormat("房东师姐离开房东看见分开了12434sdfdsfdsf的师姐疯狂老师的快乐房间宽带老师就分开了的师姐了",10);
            tmp.SetText(text);
            Debug.Log(text);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
