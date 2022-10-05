using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;
using TMPro;


namespace ET
{

    public class Puppy : NetworkBehaviour
    {
        public Transform Anchor_Head;

        public float moveSpeed = 10f;

        public bool isMoving = false;
        private Vector3 moveTarget;
        private float moveTargetTolerance = 0.2f;


        public PlayerControl PlayerControl;

        public List<uint> LocalPuppyInfos = new List<uint>();//包含房间内所有玩家的id


        public GameObject RoomCardPrefab;
        public List<GameObject> RoomCardPoolList;
        public RectTransform RoomCardCanvas;
        private int MaxCardNum = 3;
        private float CardDuration = 4f;
        //private SortedDictionary<int,NetworkIdentity> CardAndOwner = new SortedDictionary<int, NetworkIdentity>();
        public Dictionary<uint, int> ActiveCardAndOwner = new Dictionary<uint, int>();
        public List<uint> AvailablePuppys = new List<uint>();




        // need to use FixedUpdate for rigidbody
        void FixedUpdate()
        {
            if (FindObjectOfType<DJControl>().DJPuppy == null || (this.gameObject.GetComponent<NetworkIdentity>() != FindObjectOfType<DJControl>().DJPuppy.GetComponent<NetworkIdentity>()))
            {
                if (isMoving)
                {
                    var rb = this.GetComponent<Rigidbody>();
                    if (Vector3.Distance(moveTarget, transform.position) <= moveTargetTolerance)
                    {
                        // fsm.TriggerUnityEvent("MoveEnded");
                        // moveTarget = Vector3.negativeInfinity;
                        isMoving = false;
                        rb.velocity = new Vector3(0, 0, 0);
                        return;
                    }
                    var moveDir = (moveTarget - this.transform.position).normalized;
                    rb.velocity = moveDir * moveSpeed;
                }

            }

            if (isLocalPlayer)
            {
                if (FindRoomCardAvailable() != -1 && PlayerControl.ChatRoomPuppyInfos.Count > 0)
                {
                    int index = FindRoomCardAvailable();
                    foreach (uint id in PlayerControl.ChatRoomPuppyInfos.Keys)
                    {
                        bool isAvaiable = true;
                        foreach (KeyValuePair<uint, int> acao in ActiveCardAndOwner)
                        {
                            if (id == acao.Key) isAvaiable = false;
                        }
                        if (isAvaiable) { AvailablePuppys.Add(id); }
                    }
                    uint PuppyId = AvailablePuppys[Random.Range(0, AvailablePuppys.Count)];
                    SetRoomCard(PuppyId, PlayerControl.ChatRoomInfos[PlayerControl.ChatRoomPuppyInfos[PuppyId]], index);
                    AvailablePuppys.Clear();//这一段吊代码就是说要遍历所有在语音房间里的狗子，遍历场景里头顶卡片的狗子，在语音房且不头顶卡片的狗子里roll一只把卡片顶他头上，但是记得清空这个列表
                }
                //下作用为让所有显示的卡片跟随狗主人移动
                foreach (KeyValuePair<uint, int> acao in ActiveCardAndOwner)
                {
                    if (RoomCardPoolList[acao.Value].activeInHierarchy) { RoomCardPoolList[acao.Value].transform.localPosition = UIPos(NetworkClient.spawned[acao.Key].gameObject.GetComponent<Puppy>().Anchor_Head); }
                    //活跃的卡片和主人同步，加判断是因为客户端退出的时候如果它有卡片这样会报错
                    //RoomCardPoolList[acao.Value].transform.localPosition = UIPos(NetworkClient.spawned[acao.Key].gameObject.GetComponent<Puppy>().Anchor_Head);
                }
            }

        }


        void Start()
        {
            if (isLocalPlayer)
            {
                //Bubble = Instantiate(BubblePrefab);//不放这儿出不来！！！妈的卡我一天
                //ChatBubbleControl = GameObject.Find("ChatBubbleCanvas").GetComponent<ChatBubbleControl>();
                //ChatBubbleControl.Puppy = this.gameObject;
                //ClubNetworkManager clubNetworkManager = FindObjectOfType<ClubNetworkManager>();
                //NetworkClient.localPlayer.GetComponent<Puppy>().PuppyInfos.Add(new PlayerInfo{ NetworkIdentity=this.netIdentity, TargetPos=this.transform.position});
                PlayerControl = FindObjectOfType<PlayerControl>();

                PlayerControl.CmdUpdatePlayerInfo();

                InitRoomCard(MaxCardNum);
                PlayerControl.CmdUpdateChatRoomPlayerInfo(netIdentity);
            }

        }


        public void MoveTo(Vector2 target)
        {
            //Debug.Log(target);
            var scenePos = DanceFloorHelper_XGYD.PosUnified2Scene(target);
            var targetPos = DanceFloorHelper_XGYD.BuildWorldPosition(scenePos);

            if (targetPos.x < -1000f) Debug.Log("太jb远辣");
            else { moveTarget = targetPos; isMoving = true; };
        }



        /*20220907 下面都是房间卡片功能*/

        private void InitRoomCard(int MaxCardNum)
        {
            RoomCardCanvas = GameObject.Find("RoomCardCanvas").GetComponent<RectTransform>();
            for (int i = 0; i < MaxCardNum; i++)
            {
                GameObject b;
                b = Instantiate(RoomCardPrefab);
                RoomCardPoolList.Add(b);
                b.transform.SetParent(RoomCardCanvas);
                b.SetActive(false);
            }

        }

        public void SetRoomCard(uint PuppyId, RoomInfo roomInfo, int index)//哪个狗，他所在的房间的信息，他用的卡片的下标
        {
            //RoomCardPoolList[index].transform.localPosition = UIPos(PuppyId.transform);
            RoomCardPoolList[index].SetActive(true);
            ActiveCardAndOwner.Add(PuppyId, index);
            //Invoke("HideRoomCard", CardDuration);
            StartCoroutine(HideRoomCard(PuppyId));
            RoomCardPoolList[index].transform.DOScale(new Vector3(4f, 4f, 4f), 0.5f).OnComplete(() =>
            {
                RoomCardPoolList[index].transform.DOLocalRotate(new Vector3(0, 20, 0), 1f).SetLoops(-1, LoopType.Yoyo);
            });

            RoomCardPoolList[index].transform.GetChild(1).GetComponent<TMP_Text>().text = roomInfo.RoomName;//显示房间名
            RoomCardPoolList[index].transform.GetChild(3).GetComponent<TMP_Text>().text = roomInfo.PlayerNumber.ToString() + " Players";//显示房间人数
            RoomCardPoolList[index].transform.GetChild(2).gameObject.SetActive(roomInfo.Locked);//显示是否上锁
            RoomCardPoolList[index].transform.GetChild(4).gameObject.SetActive(roomInfo.PlayerNumber > 1);//人多显示HOT,这里只要有俩人就热了啊，记得改嗷
        }

        IEnumerator HideRoomCard(uint owner)
        {
            yield return new WaitForSeconds(CardDuration);
            foreach (uint id in ActiveCardAndOwner.Keys)
            {
                if (id == owner)
                {
                    RoomCardPoolList[ActiveCardAndOwner[owner]].transform.DOScale(new Vector3(0, 0, 0), 0.2f).OnComplete(() =>
                    {
                        RoomCardPoolList[ActiveCardAndOwner[owner]].transform.localRotation = Quaternion.Euler(new Vector3(0, -20, 0));
                        RoomCardPoolList[ActiveCardAndOwner[owner]].SetActive(false);
                        ActiveCardAndOwner.Remove(owner);
                    });
                }
            }

        }

        //private void HideRoomCard()
        //{

        //    KeyValuePair<int, uint> LastAcao = ActiveCardAndOwner.Dequeue(); //Returns the object at the beginning of the Queue without removing it.
        //    RoomCardPoolList[LastAcao.Key].transform.DOScale(new Vector3(0, 0, 0), 0.2f).OnComplete(() =>
        //    {
        //        RoomCardPoolList[LastAcao.Key].transform.localRotation = Quaternion.Euler(new Vector3(0, -20, 0));
        //        RoomCardPoolList[LastAcao.Key].SetActive(false);
        //        //ActiveCardAndOwner.Dequeue();
        //    });
        //}

        private Vector2 UIPos(Transform t)
        {
            Vector3 ve3 = Camera.main.WorldToScreenPoint(t.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RoomCardCanvas, ve3, null, out Vector2 ve2);
            //ve2 += new Vector2(0,280f);//注意一下偏移问题
            return ve2;
        }

        public int FindRoomCardAvailable()
        {
            //这块儿啥意思呢就隐藏的卡片数量大于卡片总数减去进房间的玩家数就要找一个消极怠工的卡片放到一个玩家（头顶上没卡片的）头上去，没玩家时所有卡片就休息，玩家小于最大卡片数可以有卡片摸鱼，大于等于大家都不许休息
            int sleeping = 0;
            foreach (GameObject card in RoomCardPoolList) { if (!card.activeInHierarchy) { sleeping++; } }
            if (sleeping > MaxCardNum - PlayerControl.ChatRoomPuppyInfos.Count)
            {
                for (int i = 0; i < MaxCardNum; i++)
                {
                    if (!RoomCardPoolList[i].activeInHierarchy)
                    {
                        bool hasowner = false;
                        foreach (KeyValuePair<uint, int> acao in ActiveCardAndOwner)
                        {
                            if (acao.Value == i) { hasowner = true; }
                        }
                        if (!hasowner) { return i; }//这边是因为当一个有房卡的玩家退出时，他的房卡会被沉默但是实际上它还在acao队列中，我们不会选用这个房卡直到它被踢出队列

                    }
                }
            }
            return -1;
        }


    }
}
