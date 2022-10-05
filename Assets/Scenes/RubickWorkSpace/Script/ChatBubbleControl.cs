using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

namespace ET
{
    public class ChatBubbleControl : NetworkBehaviour
    {
        public RectTransform RoomCardCanvas;

        //public ChatBubble Bubble;
        public GameObject BubblePrefab;
        public GameObject Puppy;
        public GameObject BlankBubble;//����ǳ�֮����ֱ�����±꼴�ɵ��������ø���
        public List<GameObject> BubblePoolList;

        public Dictionary<uint, int> ActiveBubbleAndOwner = new Dictionary<uint, int>();
        public int BubbleIndex;
        public float BubbleDuration = 5f;



        // Update is called once per frame
        void Start()
        {
            if (isClient)//�����ǲ���Ӧ�øĳ�localplayer��д��̫�����Ѿ��ǲ�����
            {
                Puppy = NetworkClient.localPlayer.gameObject;
                if (hasAuthority)
                {
                    InitBubble();
                    GameObject.Find("InputCanvas").GetComponent<InputCommend>().ChatBubbleControl = this;
                    //this.transform.SetParent(Puppy.transform);
                }
            }

        }

        void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                foreach (KeyValuePair<uint, int> abao in ActiveBubbleAndOwner)
                {
                    if (BubblePoolList[abao.Value].activeInHierarchy) { BubblePoolList[abao.Value].transform.localPosition = UIPos(NetworkClient.spawned[abao.Key].gameObject.GetComponent<Puppy>().Anchor_Head); }

                }
            }

        }


        [Command]
        public void CmdSetBubble(string message, uint networkIdentity)
        {
            PlayerControl playerControl = FindObjectOfType<PlayerControl>();
            foreach (NetworkIdentity id in playerControl.FindNeighbors_Bubble(networkIdentity)) { TargetSetBubble(id.connectionToClient, message, networkIdentity); }
        }

        [TargetRpc]
        public void TargetSetBubble(NetworkConnection networkConnection, string message, uint networkIdentity)
        {
            NetworkClient.localPlayer.GetComponent<ChatBubbleControl>().SetBubble(message, networkIdentity);
        }

        [ClientRpc]
        public void RpcSetBubble(string message, uint networkIdentity)
        {
            NetworkClient.localPlayer.GetComponent<ChatBubbleControl>().SetBubble(message, networkIdentity);
        }

        public void SetBubble(string message, uint networkIdentity)
        {
            if (FindAvailableBubble())
            {
                BubbleIndex = BubblePoolList.IndexOf(BlankBubble);
                //if blankbubble==null return
                ActiveBubbleAndOwner.Add(networkIdentity, BubbleIndex);
                if (message.Length < 4)
                {
                    BlankBubble.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(135, 60);
                }
                else BlankBubble.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(CalculateBubbleLength(message) * 45, 60);
                BlankBubble.GetComponentInChildren<TMP_Text>().text = message;
                BlankBubble.transform.localPosition = UIPos(NetworkClient.spawned[networkIdentity].gameObject.GetComponent<Puppy>().Anchor_Head);
                BlankBubble.gameObject.SetActive(true);
                //Invoke("BubbleOff", 5f);
                StartCoroutine(HideBubble(networkIdentity));
            }

        }

        IEnumerator HideBubble(uint id)
        {
            yield return new WaitForSeconds(BubbleDuration);
            uint index=new uint();
            foreach (uint owner in ActiveBubbleAndOwner.Keys)
            {
                if (id == owner)
                {
                    BubblePoolList[ActiveBubbleAndOwner[owner]].SetActive(false);
                    //ActiveBubbleAndOwner.Remove(owner);
                    index = owner;
                }
            }
            ActiveBubbleAndOwner.Remove(index);
        }


        private float CalculateBubbleLength(string message)
        {
            float len = 0f;
            foreach (char i in message)
            {
                if (char.IsUpper(i) || char.IsLower(i) || char.IsDigit(i)) { len += 0.7f; }
                else if (char.IsPunctuation(i) || char.IsWhiteSpace(i)) { len += 0.4f; }
                else { len += 1f; }
            }
            if (len < 4) { len = 3; }
            return len;
        }

        private bool FindAvailableBubble()
        {
            foreach (GameObject b in BubblePoolList)
            {
                if (!b.activeInHierarchy)
                {
                    bool hasowner = false;
                    foreach (KeyValuePair<uint, int> abao in ActiveBubbleAndOwner)//Ϊʲô�ж��������˵����뿪��ʱ������ڷ���������ݾͻ���ʧ���ǻ���abao������Ϊʲô��ֱ�Ӱ���remove���Ǻ��ҵ����뵫���±���Ķ԰��ö���Ҳ����Ĳ�֪��Ϊʲô���Ծ������������ˣ����ݽṹ���㷨��˯���������������
                    {
                        if (BubblePoolList.IndexOf(b) == abao.Value) { hasowner = true; }
                    }
                    if (!hasowner)
                    {
                        BlankBubble = b;
                        return true;
                    }

                }
            }
            return false;
        }


        //public void BubbleOff()
        //{
        //    BubbleIndex = ActiveBubbleAndOwner.Dequeue().Key;
        //    BubblePoolList[BubbleIndex].gameObject.SetActive(false);
        //    //BubblePoolList[BubbleIndex].transform.SetParent(BubblePool.transform);
        //}

        private Vector2 UIPos(Transform t)
        {
            Vector3 ve3 = Camera.main.WorldToScreenPoint(t.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RoomCardCanvas, ve3, null, out Vector2 ve2);
            return ve2;
        }

        private void InitBubble()
        {
            RoomCardCanvas = GameObject.Find("RoomCardCanvas").GetComponent<RectTransform>();

            for (int i = 0; i < 10; i++)
            {
                GameObject b;
                b = Instantiate(BubblePrefab);
                BubblePoolList.Add(b);
                b.transform.SetParent(RoomCardCanvas);
                b.SetActive(false);
            }

        }


    }
}
