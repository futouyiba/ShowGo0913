using System;
using System.Collections.Generic;
using ShowGo;
using UnityEngine;
using Utility;
using TMPro;
using System.Collections;
using System.Linq;
using Bolt;
using Mirror;
using DG.Tweening;

public class FloatingUiConnector : SingletonMono<FloatingUiConnector>
{
    [SerializeField] private RectTransform RTCan;
    [SerializeField] private GameObject chatBubble;
    [SerializeField] private GameObject nameText;
    [SerializeField] private GameObject puppyCard;
    [SerializeField] private GameObject RecommendRoomCard;
    [SerializeField] private Queue RecommendRoomCardQueue;
    [SerializeField] private int RecommendRoomCardNumber;
    [SerializeField][Tooltip("����������Ҫ������ʱ��")] private float duration;
    [SerializeField] private float smallScale = 0f;
    [SerializeField] private float bigScale = 1f;
    
    /// <summary>
    /// index is userId, value is ugui panel in 109, and then switch to uiwidgets stateless widget.
    /// </summary>
    public Dictionary<uint, object> chatBubbles = new Dictionary<uint, object>();

    /// <summary>
    /// index is userId, value is ugui panel in v109, and then switch to uiwidgets stateless widget.
    /// </summary>
    public Dictionary<uint, object> nameTexts = new Dictionary<uint, object>();

    public Dictionary<uint, GameObject> puppyCards = new Dictionary<uint, GameObject>();
    
    public Dictionary<uint, GameObject> RecommendRoomCards = new Dictionary<uint, GameObject>();

    [Tooltip("������������ʾ����������")] private Dictionary<uint, ChatBubblesTime> chatBubblesTime = new Dictionary<uint, ChatBubblesTime>();
    [Tooltip("��Ҫ�رյ���������")] private List<uint> concealChatBubbles = new List<uint>();

    /// <summary>
    /// move chat bubbles and nameTexts with character world position.
    /// </summary>
    /// <param name="userId">the user id that should drive floating ui to move</param>
    public void DriveFloatingUiPos(uint userId)
    {
        // done @lizhaozhao
        if (chatBubbles.ContainsKey(userId))
        {
            // (chatBubbles[userId] as GameObject).transform.localPosition = WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.position);
            //todo the bubble offset in grace way
            (chatBubbles[userId] as GameObject).transform.localPosition = WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.Find("PlayerTop").position) + Vector2.up * 40f;
        }

        if (nameTexts.ContainsKey(userId))
        {
            (nameTexts[userId] as GameObject).transform.localPosition = WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.position);
            // if (SceneShowgoMgr.Instance.GetUser(userId).transform.gameObject.activeSelf)
            // {
            (nameTexts[userId] as GameObject).SetActive(true);
            // }
        }
        if (RecommendRoomCards.ContainsKey(userId))
        {
            (RecommendRoomCards[userId] as GameObject).transform.localPosition = WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.Find("PlayerTop").position) + Vector2.up * 30f;
            
        }
        //if (roomCards.ContainsKey(userId)&& roomCards[userId].activeSelf)
        //{
        //    roomCards[userId].transform.localPosition = WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.position) + new Vector2(0, 500f);
        //}
    }

    public void SpawnChatBubble(uint userId, string msg)
    {
        // done @lizhaozhao
        if (!chatBubbles.ContainsKey(userId))
        {
            // chatBubbles.Add(userId, Instantiate(chatBubble, WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.position), Quaternion.identity, RTCan));
            chatBubbles.Add(userId, Instantiate(chatBubble, WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.Find("PlayerTop").position), Quaternion.identity, RTCan));
        }

        if (!chatBubblesTime.ContainsKey(userId))
        {
            ChatBubblesTime time = new ChatBubblesTime();
            time.chatBubbles = chatBubbles[userId] as GameObject;
            time.duration = duration;
            chatBubblesTime.Add(userId, time);
        }
        else
        {
            chatBubblesTime[userId].passedTime = 0;
            chatBubblesTime[userId].isRemoved = false;
        }
        (chatBubbles[userId] as GameObject).SetActive(true);
        StartDOTween((chatBubbles[userId] as GameObject), true);
        (chatBubbles[userId] as GameObject).transform.Find("ChatBubbleBG/Text").GetComponent<TextMeshProUGUI>().text = msg;

        //2022030他妈妈的我怎么让镜头回去啊，动画结束的时候回去但他所有都用到startdotween了，我操
        if (userId == SceneShowgoMgr.Instance.Me.UserId) VcamBehaviour.instance.LocalCameraChange(4);
        if(msg!="定位自己") Invoke("CamBack", duration+0.1f);
    }

    private void CamBack()
    {
        if (VcamBehaviour.instance.PreviousVC == VcamBehaviour.instance.vCams[4]&& !SceneShowgoMgr.Instance.Me.isVoluntarilyMoving&& !chatBubblesTime.ContainsKey(SceneShowgoMgr.Instance.Me.UserId)) VcamBehaviour.instance.LocalCameraChange(0);
    }

    public void SpawnOrUpdateNameText(uint userId, string userName)
    {
        // done @lizhaozhao
        // Debug.Log($"trying to spawn or update name text, userid is {userId}, userName is {userName}, showgoplayers has {PartyBehaviour.Instance. ShowGoPlayers.Count} members");

        if (!nameTexts.ContainsKey(userId))
        {
            GameObject go = Instantiate(nameText, WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.position), Quaternion.identity);
            go.transform.SetParent(RTCan, false);
            nameTexts.Add(userId, go);

            //如果是localplayer颜色改变
            if (NetworkClient.localPlayer != null)//先spawn别人就gg了，所以判断一下我localplayer本人spawn了没有
            {
                if (userId == SceneShowgoMgr.Instance.Me.UserId)
                {
                    (nameTexts[userId] as GameObject).transform.Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(0, 200, 255);
                }

            }


        }
        (nameTexts[userId] as GameObject).transform.Find("Text").GetComponent<TextMeshProUGUI>().text = userName;
        (nameTexts[userId] as GameObject).SetActive(false);
    }

    public void DelDicData(uint userid)
    {
        if (chatBubbles.ContainsKey(userid))
        {
            Destroy(chatBubbles[userid] as GameObject);
            chatBubbles.Remove(userid);
        }

        if (puppyCards.ContainsKey(userid) && puppyCards[userid].activeSelf)
        {
            StartDOTween(puppyCards[userid], false);
        }

        if (nameTexts.ContainsKey(userid))
        {
            Destroy(nameTexts[userid] as GameObject);
            nameTexts.Remove(userid);
        }

        if (puppyCards.ContainsKey(userid))
        {
            Destroy(puppyCards[userid] as GameObject);
            puppyCards.Remove(userid);
        }
        if (RecommendRoomCards.ContainsKey(userid))
        {
            Destroy(RecommendRoomCards[userid] as GameObject);
            RecommendRoomCards.Remove(userid);
        }
        
    }


    public void InstPuppyCard(uint userId)
    {
        if (!puppyCards.ContainsKey(userId))
        {
            //GameObject go = Instantiate(roomCard, WolrdToUIPos(SceneShowgoMgr.Instance.GetUser(userId).transform.position) + new Vector2(0, 500f), Quaternion.identity);
            GameObject go = Instantiate(puppyCard);
            go.GetComponent<PuppyCardBut>().AddBtnFunc(() =>
                ContinentalMessenger.Instance.ApplyFriend((int) userId,
                    ContinentalMessenger.FriendApplyType.APPLY_TYPE_QUICK));
            puppyCards.Add(userId, go) ;
            go.transform.SetParent(RTCan, false);
            go.SetActive(false);
        }
    }
    public void InstRecommendRoomCard(uint userId)
    {
        if (!RecommendRoomCards.ContainsKey(userId))
        {
            GameObject go = Instantiate(RecommendRoomCard);
            RecommendRoomCards.Add(userId, go) ;
            go.transform.SetParent(RTCan, false);
            go.SetActive(false);
        }
    }

    public void SpawnRecommendRoomCards(uint userId)
    {
        //判断当前队列是否已满
        if (RecommendRoomCardQueue.Count < RecommendRoomCardNumber)
        {
            //判断当前队列是否已经包含了传入的userId
            if (!RecommendRoomCardQueue.Contains(userId))
            {
                (RecommendRoomCards[userId] as GameObject).SetActive(true);
                RecommendRoomCardQueue.Enqueue(userId);
                Debug.Log(userId+":进入队列");
            }
            else
            {
                Debug.Log("队列里已经包含:"+userId);
            }
        }
        //如果当前队列满了，那么将执行队首useId出队列，新的userId从队尾进入队列
        else
        {
           
            //判断当前队列是否已经包含了传入的userId
            if (!RecommendRoomCardQueue.Contains(userId))
            {
                (RecommendRoomCards[userId] as GameObject).SetActive(true);
                RecommendRoomCardQueue.Enqueue(userId);
                Debug.Log(userId+":进入队列");
                //将队列首元素setfalse
                (RecommendRoomCards[(uint)RecommendRoomCardQueue.Dequeue()] as GameObject).SetActive(false);
                Debug.Log( RecommendRoomCardQueue.Dequeue()+"出队列了");
              
              
            }
            else
            {
                Debug.Log("队列里已经包含:"+userId);
            }
           
        }

    }

    [Tooltip("焦点ID")] private int focusDog = -2;
    public void SpawnPuppyCard(uint userId)
    {
        if (focusDog == userId)
        {
            focusDog = -2;
            //点击相同人物，关闭卡片
            StartDOTween(puppyCards[userId], false);
        }
        else
        {
            StartDOTween(puppyCards[userId], true);
            if (focusDog >= 0)
            {
                StartDOTween(puppyCards[(uint)focusDog], false);
            }
            focusDog = (int)userId;
        }
    }

    //关闭卡片
    public void ClosePuppyCard()
    {
        if (focusDog < 0) return;
        StartDOTween(puppyCards[(uint)focusDog], false);
        focusDog = -2;
    }


    private void StartDOTween(GameObject obj, bool bo) 
    {
        try
        {
            if (bo)
            {
                obj.SetActive(true);
                obj.transform.localScale = Vector3.one * smallScale;
                //obj.transform.DOScale(Vector3.one * bigScale, 0.5f);
                obj.transform.DOScale(Vector3.one , 0.5f).SetDelay(0.5f);

            }
            else
            {
                //obj.transform.localScale = Vector3.one * bigScale;
                obj.transform.localScale = Vector3.one;
                obj.transform.DOScale(Vector3.one * smallScale, 0.5f).OnComplete(() => { obj?.SetActive(false); });
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"caught dotween error:{e}");
        }

    }

    private Vector2 WolrdToUIPos(Vector3 ve3)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RTCan, Camera.main.WorldToScreenPoint(ve3), null, out Vector2 ve2);
        return ve2;
    }


    public void CleaseAll(uint userId)
    {
        if (chatBubbles.TryGetValue(userId, out var bubbleGot))
        {
            chatBubbles.Remove(userId);
            Destroy(bubbleGot as GameObject);
        }

        if (puppyCards.TryGetValue(userId, out var cardGot))
        {
            puppyCards.Remove(userId);
            Destroy(cardGot);
        }

        if (nameTexts.TryGetValue(userId, out var nameGot))
        {
            nameTexts.Remove(userId);
            Destroy(nameGot as GameObject);
        }
        if (RecommendRoomCards.TryGetValue(userId, out var recommendroomGot))
        {
            RecommendRoomCards.Remove(userId);
            Destroy(nameGot as GameObject);
        }
    }


    private void Start()
    {
        
        RecommendRoomCardQueue = new Queue();
    }

    private void Update()
    {
        foreach (var timer in chatBubblesTime)
        {
            if (timer.Value.isRemoved) continue;
 
            timer.Value.passedTime += Time.deltaTime;

            if (timer.Value.passedTime >= timer.Value.duration)
            {
                this.concealChatBubbles.Add(timer.Key);
                timer.Value.isRemoved = true;
                //timer.Value.chatBubbles.SetActive(false);
                StartDOTween(timer.Value.chatBubbles, false);
            }
        }

        for (int i = 0; i < concealChatBubbles.Count; i++)
        {
            chatBubblesTime.Remove(concealChatBubbles[i]);
        }

        concealChatBubbles.Clear();
        
    }
}

class ChatBubblesTime
{
    [Tooltip("��������")]public GameObject chatBubbles;
    [Tooltip("��Ҫ������ʱ��")] public float duration;      
    [Tooltip("�Ѿ�������ʱ��")] public float passedTime = 0;    
    [Tooltip("�Ƿ�Ӧ�ùر�")] public bool isRemoved = false;    
}