using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace ET
{
    public class ChatBubble : NetworkBehaviour
    {
        public override void OnStartServer()
        {
            base.OnStartServer();
            gameObject.SetActive(false);
        }

    }
}
