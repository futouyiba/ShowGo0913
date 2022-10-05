using System;
using System.Collections;
using ET.Utility;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;
using Bolt;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using Sequence = DG.Tweening.Sequence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ET;
using Ludiq.FullSerializer.Internal;
using Utility;

namespace ShowGo
{
    /// <summary>
    /// client part
    /// </summary>
    public partial class ShowGoPlayer : NetworkBehaviour
    {
        /// <summary>
        /// supposed to initialize using id from 
        /// </summary>
        //[SyncVar] private uint UserId;
        [SyncVar(hook = "OnUserIdChanged")] public uint UserId = 99999;

        public void SetUserId(uint newUserId)
        {
            if(isServerOnly) OnUserIdChanged(UserId, newUserId);
            UserId = newUserId;
        }

        void OnUserIdChanged(uint oldId, uint newId)
        {
            if (oldId == newId)
            {
                Debug.Log("on user id changed equal causes return.." + oldId);
                return;
            }

            Debug.Log($"on user id changed, {oldId}=>{newId}");


            if (SceneShowgoMgr.Instance.GetUser(oldId))
            {
                SceneShowgoMgr.Instance.RemoveUser(oldId);
            }

            SceneShowgoMgr.Instance.AddUser(newId, this);
            // var showGoPlayers = SceneShowgoMgr.Instance.playersDic;
            // if (showGoPlayers.ContainsKey(oldId)) showGoPlayers.Remove(oldId);
            // if (!showGoPlayers.ContainsKey(newId))
            //     showGoPlayers.Add(newId, this);
        }

        /// <summary>
        /// supposed to be aligned with logic server (from http resp, or from mq)
        /// </summary>
        [SyncVar(hook = "OnUserNameChanged")] public string UserName="Placeholder";

        [SyncVar(hook = "OnDestinationChanged")]
        public Vector3 Destination;

        [SyncVar(hook = "OnAprcIdChanged")] public int AprcId;

        /// <summary>
        /// room lvl is synced by native message
        /// </summary>
        public int RoomLvl;

        private ShowGoPlayer currentFocusedPlayer;
        private ShowGoPlayer formerFocusedPlayer;
        private bool isAlreadyFocusing;
        [SerializeField] private ShowGoPlayerCfg _cfg;
        public StateMachine _fsm;

        private Sequence _moveSeq;

        public GameObject aprcObj;
        
        public const int DEFAULT_HEARTBEAT_INTERVAL = 10000;
        public int interval = DEFAULT_HEARTBEAT_INTERVAL;
        int timeFromLastSend = 0;

        [ReadOnly] public bool isMoving;
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTweenerCore;
        private static string totalCfgAddr = "Cfg/ShowGoPlayerCfg";

        [SyncVar(hook = "OnVoluntarilyMovingChanged")] public bool isVoluntarilyMoving = false;//20220923自主移动中，为了是否给特写镜头区分两种移动

        #region initialization



        void OnUserNameChanged(string oldName, string newName)
        {
            //  refresh name UI / show name UI done @lizhaozhao 
            //刷新名称UI /显示名称UI
            if (string.IsNullOrWhiteSpace(newName) || (newName == oldName)) return;
            FloatingUiConnector.Instance.SpawnOrUpdateNameText(this.UserId, newName);

            FloatingUiConnector.Instance.InstPuppyCard(this.UserId);
            FloatingUiConnector.Instance.InstRecommendRoomCard(this.UserId);


            // if (isLocalPlayer)
            // {
            //     DebugUI.Instance.OnChitchat_But(UserId) ;
            // }
        }

        /// <summary>
        /// client only hook
        /// </summary>
        /// <param name="oldDes"></param>
        /// <param name="newDes"></param>
        void OnDestinationChanged(Vector3 oldDes, Vector3 newDes)
        {
            if(_moveTweenerCore.IsActive() && _moveTweenerCore.IsPlaying()) _moveTweenerCore.Kill(); //todo @futou defensive code.
            if (oldDes == newDes) return;
            var speed = 1f; //todo @futou read from cfg
            var distance = Vector3.Distance(transform.position, newDes);
            var duration = distance / speed;
            _moveTweenerCore = this.transform.DOMove(newDes, duration);
            _moveTweenerCore.OnComplete(() =>
            {
                // _fsm?.TriggerUnityEvent("MoveEnd");
                if(_fsm!=null && _fsm.enabled) _fsm.TriggerUnityEvent("MoveEnd");
                moveTarget = Vector3.negativeInfinity;

                if (isLocalPlayer && isVoluntarilyMoving)
                {
                    //20220923当角色移动结束摄像机归位
                    CmdVoluntarilyMovingEnd();
                }

            });
        }

        /// <summary>
        /// room lvl changed, also change the apperance
        /// </summary>
        /// <param name="oldLvl"></param>
        /// <param name="newLvl"></param>
        void OnRoomLvlChanged(int oldLvl, int newLvl)
        {
            
        }
        
        public override void OnStopClient()
        {
            GetComponent<TrainState>()?.OnExit();
            DOTween.KillAll(transform);
            FloatingUiConnector.Instance.CleaseAll(UserId);
            
            base.OnStopClient();
        }

        [Command]public void CmdVoluntarilyMovingEnd()
        {
            isVoluntarilyMoving = false;
        }
        void OnVoluntarilyMovingChanged(bool oldMoving,bool newMoving)
        {
            if (isLocalPlayer)
            {
                if (oldMoving)
                {
                    if (VcamBehaviour.instance.PreviousVC == VcamBehaviour.instance.vCams[4]) VcamBehaviour.instance.LocalCameraChange(0);
                }
                else
                {
                    VcamBehaviour.instance.LocalCameraChange(4);
                }
            }
        }


        /// <summary>
        /// it's favorable if we can put aprc init and aprc change in one place.@tiaohai
        /// </summary>
        /// <param name="oldAprc"></param>
        /// <param name="newAprc"></param>
        public async void OnAprcIdChanged(int oldAprc, int newAprc)
        {
            //todo handle aprc stuff, 2 kinds of aesthetic stuff @tiaohai
            //find new aprc, instantiate
            var asset = await AssetCache.Instance.GetAsset<ShowGoPlayerCfg>(totalCfgAddr);
            // Debug.Log($"{UserId} is changing aprc to {newAprc},b");
            if (!asset.CharacterPrefabs.TryGetValue(newAprc, out GameObject prefabGot))
            {
                Debug.Log($"aprc for {newAprc} cannot be found");
            }
            
            void CreateAprc()
            {
                GameObject newAprcInst = Instantiate(prefabGot, transform);
            
                if (newAprcInst)
                {
                    if (aprcObj)
                    {
                        //old aprc exists, update
                        Destroy(aprcObj);
                    }
            
                    aprcObj = newAprcInst;
                }
                else
                {
                    Debug.Log($"{UserId} newAprcInst missing");
                }
            }

            SceneShowgoMgr.Instance.AddCreateTask(CreateAprc);
        }

        public override async void OnStartClient()
        {
            base.OnStartClient();
            if (isLocalPlayer)
            {
                switch (ContinentalMessenger.Instance.nativeClientMode)
                {
                    case ContinentalMessenger.NativeClientMode.LocalMock:
                        await TimerComponent.Instance.WaitAsync(1);
                        ContinentalMessenger.Instance.Native2UnityMsg(ContinentalMessenger.MockMyInfo());
                        break;
                }
                
            }
            else
            {
                // todo let players show frame by frame.    @lizhaozhao
                // this.transform.GetChild(0).gameObject.SetActive(false);
                // StartCoroutine("showPlayer", this.transform.GetChild(0).gameObject);//todo @futou refactor to instantiate aprc
            }

            
            if (!isServer)
            {
                //kill state machine if exist
                var fsm = GetComponent<StateMachine>();
                if (fsm)
                {
                    fsm.enabled = false;
                }
            }
            
        }

        public async void InitAprcMock()
        {
            var asset = await AssetCache.Instance.GetAsset<ShowGoPlayerCfg>(totalCfgAddr);
            // var asset = await AssetCache.Instance.GetAsset<ShowGoPlayerCfg>(totalCfgAddr);
            if(asset==null) Debug.Log($"character asset is null");
            var aprcIdDict = Random.Range(0, asset.CharacterPrefabs.Count);
            var aprcId = asset.CharacterPrefabs.ElementAt(aprcIdDict).Key;
            
            // Debug.Log($"{UserId} is changing aprc to {aprcId} in mock");
            CmdSetAprcId(aprcId);
            // ClientTrySetSyncValue("AprcId", aprcId);
        }

        // IEnumerator showPlayer(GameObject go) 
        // {
        // yield return new WaitForSeconds(5f);
        // go.SetActive(true);

        // }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            var op = NativeUtility.MakeOp(new InitFinish());
            ContinentalMessenger.Instance.Unity2NativeMsg(op);
        }

        [Command]
        public void InitNameMock(string newName)
        {
            UserName = newName;
        }

        [Command]
        void InitUserIdMock()
        {
            SetUserId(netId);
        }

        [Command]
        public void InitName(string newName)
        {
            UserName = newName;
        }
        
        [Command]
        public void InitUserId(int userId)
        {
            SetUserId((uint)userId);
        }

        /// <summary>
        /// should be called when native send init msg
        /// </summary>
        /// <param name="userId"></param>
        [Client]
        void InitUserIdFromNative(uint userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// I think the correct way is rt server holds the name, since it's better to handle mq stuff.
        /// </summary>
        [Client]
        void InitNameFromLS()
        {
            // send http request once, and set names by bunches.
        }

        public void CleanUpUIForOtherPlayer(uint userId)
        {
            RpcCleanUpUIForOtherPlayer(userId);
        }

        [ClientRpc]
        void RpcCleanUpUIForOtherPlayer(uint userId)
        {
            FloatingUiConnector.Instance.DelDicData(userId);
        }

        public void DelNameTextForSpecificUserId(uint userId)
        {
            RpcDelNameText(userId);
        }

        [ClientRpc]
        private void RpcDelNameText(uint userId)
        {
            FloatingUiConnector.Instance.DelDicData(userId);
        }

        #endregion

        #region chat

        /// <summary>
        /// this should called from native client.
        /// </summary>
        /// <param name="msg"></param>
        public void SendChatMsg(uint userId, string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                Debug.Log($"msg is null or empty");
                return;
            }
            CheckClientPrefix(msg);
            CmdChatMsg(userId, msg);
        }

        private async void CheckClientPrefix(string msg)
        {
            if (_cfg == null)
            {
                _cfg = await AssetCache.Instance.GetAsset<ShowGoPlayerCfg>(totalCfgAddr); 
            }
            foreach (var kvPair in _cfg.clientCommandPrefixes)
            {
                if (msg.StartsWith(kvPair.Key))
                {
                    Debug.Log($"client cmd {kvPair.Key} detected, getting method {kvPair.Value}");
                    var driedMsg = msg.TrimStart(kvPair.Key.ToCharArray());
                    var method = this.GetType().GetMethod(kvPair.Value);
                    method?.Invoke(this, new[] { driedMsg });
                }
            }
        }

        /// <summary>
        /// when user chat say"focus myself", trigger this.
        /// this should be a 50/-50 priority vcam
        /// </summary>
        /// <param name="driedMsg"></param>
        public void ClientPrefixFocusSelf(string driedMsg)
        {
            // todo vcam logic, should call vcam behaviour @labike
            VcamBehaviour.instance.LocalCameraChange(4);
        }

        #endregion

        #region focusing on other player

        void OnOtherPlayerClicked(ShowGoPlayer clickedPlayer)
        {
            Debug.LogWarning($"{clickedPlayer.UserId} is clicked");

            //if (isAlreadyFocusing)
            //{
            //    formerFocusedPlayer = currentFocusedPlayer;
            //    currentFocusedPlayer = clickedPlayer;
            //    //20220919改了一下但是不知道这么改对不对
            //    // PopUpUserCard();
            //    MoveVcamFocus(clickedPlayer);
            //    //HideFormerUserCard();20220925关闭用户信息卡放到localcamerachange里面
            //    UIFocused(clickedPlayer);
            //    //PopUpUserCard();记得注回来
            //}
            //else
            //{
            //    // UIFocused();
            //    MoveVcamFocus(clickedPlayer);

            //    UIFocused(clickedPlayer);
            //    // PopUpUserCard();
            //    currentFocusedPlayer = clickedPlayer;
            //    isAlreadyFocusing = true;
            //}

            MoveVcamFocus(clickedPlayer);

            UIFocused(clickedPlayer);

            ContinentalMessenger.Instance.GetUserInfo((int)clickedPlayer.UserId,
                item => ContinentalMessenger.Instance.OnGetFriendState((int)clickedPlayer.UserId, item));
            // ContinentalMessenger.Instance.GetUserInfo((int)clickedPlayer.UserId);
        }

        private void MockUIFocused()
        {
            // todo use ui widgets to mock @xujihua
        }

        private void UIFocused(ShowGoPlayer focusPlayer)
        {
            PopUpUserCard(focusPlayer);
            // tell native
            CharUIShow showMsg = new CharUIShow();
            var op = NativeUtility.MakeOp(showMsg);
            ContinentalMessenger.Instance.Unity2NativeMsg(op);
            
            // throw new NotImplementedException();
        }

        void PopUpUserCard(ShowGoPlayer focusPlayer)
        {
            // wait a small period. approximately equals the hide animation duration.
            //todo @lizhaozhao
            //等待一小段时间。大约等于隐藏动画的持续时间。
            Debug.LogWarning($"{focusPlayer.UserId} clicked");
            FloatingUiConnector.Instance.SpawnPuppyCard(focusPlayer.UserId);
        }

        void HideFormerUserCard()
        {
            //todo
            FloatingUiConnector.Instance.ClosePuppyCard();
        }

        async void SwitchCurrentFocusedPlayer(ShowGoPlayer other)
        {
            MoveVcamFocus(other);
            HideFormerUserCard();
            PopUpUserCard(other);
        }

        void MoveVcamFocus(ShowGoPlayer newFocusedPlayer)
        {
            // todo set target @labike
            if (VcamBehaviour.instance.PreviousVC == VcamBehaviour.instance.vCams[5] && newFocusedPlayer.UserId == VcamBehaviour.instance.LocalTargetPuppy)//20200926点同个人不切！！！
            {
                return;
            }
            VcamBehaviour.instance.LocalTargetPuppy = newFocusedPlayer.UserId;
            VcamBehaviour.instance.LocalCameraChange(5);
        }

        #endregion

        [ClientRpc]
        private void RpcShowOrUpdateChatBubble(uint userId, string msg)
        {
            FloatingUiConnector.Instance.SpawnChatBubble(userId, msg);
        }

        public void ClientTrySetSyncValue(string varName, object value)
        {
            CmdSetSyncValue(varName, value);
        }
        
             
    }

    /// <summary>
    /// server part
    /// </summary>
    public partial class ShowGoPlayer
    {
        public override void OnStartServer()
        {
            base.OnStartServer();

            //get fsm
            _fsm = GetComponent<StateMachine>();
            var trainState = GetComponent<TrainState>();
            if(trainState) Destroy(trainState);
            _fsm.TriggerUnityEvent("InitFinish");
            
            
            //set random start position
            // var randPos = PartyBehaviour.Instance.GetRandomWalkablePosition();
            // SetDestination(randPos);

            SceneShowgoMgr.Instance.AddUser(UserId, this);
        }

        [Command]
        void CmdSetSyncValue(string varName, object value)
        {
            var syncVar = this.GetType().GetField(varName);
            if (syncVar != null)
            {
                syncVar.SetValue(this, value);
            }
        }

        /// <summary>
        /// in method name "cmd" means mirror network command,not chat text "command" 
        /// </summary>
        /// <param name="msg"></param>
        [Command]
        private void CmdChatMsg(uint userId, string msg)
        {
            Debug.Log($"server get user{userId} chat msg :{msg}");
            RpcShowOrUpdateChatBubble(userId, msg);
            CheckServerPrefix(msg);
        }

        private async void CheckServerPrefix(string msg)
        {
            if (_cfg == null)
            {
                _cfg = await AssetCache.Instance.GetAsset<ShowGoPlayerCfg>(totalCfgAddr); 
            }

            foreach (var kvPair in _cfg.commandPrefixes)
            {
                if (msg.StartsWith(kvPair.Key))
                {
                    var driedMsg = msg.TrimStart(kvPair.Key.ToCharArray());
                    var type = this.GetType();
                    var method = type.GetDeclaredMethod(kvPair.Value);
                    method?.Invoke(this, new object[] { driedMsg });
                }
            }
        } 

        /// <summary>
        /// DJ功能-切歌
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SvrPreDJ_CutSong(string text)
        {
            if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
            throw new NotImplementedException();
        }

        // public void SvrPreDJ_GetHigh(string text)
        // {
        //     if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
        //     throw new NotImplementedException();
        // }

        /// <summary>
        /// 看电影
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SvrPreDJ_Movie(string text)
        {
            if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
            throw new NotImplementedException();
        }

        /// <summary>
        /// 一起来 XXX
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SvrPreDJ_EverybodySay(string text)
        {
            if (!isServer)
            {
                Debug.LogError($"calling server func from client");
                return;
            }
            if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
            SceneShowgoMgr.Instance.EveryoneSpeak(text);
        }

        /// <summary>
        /// 墨镜
        /// </summary>
        /// <param name="driedMsg"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SvrPreDecor_Glass(string driedMsg)
        {
            if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
            throw new NotImplementedException();
        }

        public void SvrPreDecor_Cola(string driedMsg)
        {
            if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
            throw new NotImplementedException();
        }

        public void SvrPreDecor_Milktea(string driedMsg)
        {
            if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
            throw new NotImplementedException();
        }

        public void SvrPreDecor_Chips(string driedMsg)
        {
            if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
            throw new NotImplementedException();
        }

        public void SvrPreDecor_Phone(string driedMsg)
        {
            if(!SceneShowgoMgr.Instance.AmIDJ((int)UserId)) return;
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// user chatted "镜头"
        /// </summary>
        /// <param name="driedMsg"></param>
        public void ServerDriveVcamSwitch(string driedMsg)
        {
            // todo vcam logic. @labike
            int randIndex = Random.Range(0, 4);
            if (randIndex == 3)
            {
                VcamBehaviour.instance.syncedTargetPuppy = SceneShowgoMgr.Instance.GetRandomUser().UserId;
            }

            VcamBehaviour.instance.syncedVcamIndex = randIndex;
        }

        // public void ClientWearGoggles(string driedMsg)
        // {
        //     // todo @xinminhang
        // }

        public void PrefixMove(string driedMsg)
        {
            // todo add direction information lobic
            
            isVoluntarilyMoving = true;//20220923表示我在自愿移动，这个在服务器上执行

            var position = PartyBehaviour.Instance.GetRandomWalkablePosition();
            SetDestination(position);
        }
 

        [Command]
        public void CmdSetAprcId(int aprcId)
        {
            AprcId = aprcId;
        }

        private Action moveDlg = null;
        private Vector3 moveTarget= Vector3.negativeInfinity;
        public void SetDestination(Vector3 target)
        {
            moveTarget = target;
   
            _fsm.TriggerUnityEvent("MoveStart");
        }

        public void ftOnEnterMove()
        {
            //set destination to trigger change on client
            var oldDest=Destination ;
            Destination = moveTarget;
            // moveDlg?.Invoke();
            OnDestinationChanged(oldDest, Destination);

        }
        
        
        
        public async void ftOnEnterIdle()
        {
            //wait
            // Debug.LogWarning($"{UserId} wait started");
            await StartWait(3,7);
            // Debug.LogWarning($"{UserId} wait finished");
            //get rand pos
            //set dest
            var randPos = PartyBehaviour.Instance.GetRandomWalkablePosition();
            SetDestination(randPos);
        }

        public void ftOnEnterTrain()
        {
            PartyBehaviour.Instance.SvrUserJoinTrain(UserId);
        }

        public void ftOnLeaveTrain()
        {
            PartyBehaviour.Instance.SvrUserLeaveTrain(UserId);
        }


        protected Task StartWait(int minWaitTime, int maxWaitTime)
        {
            var randomPause = Random.Range(minWaitTime, maxWaitTime) * 1000;
            return TimerComponent.Instance.WaitAsync(randomPause);
        }


        
        public void SvrTrainStateChange(string notUsedDryMsg)
        {
            PartyBehaviour.Instance.SvrUserTrainStateChange(this.UserId);
            
            // var trainComp = GetComponent<TrainState>();
            // if(!trainComp) Debug.Log($"train comp cannot found on {UserId}");
            // if (trainComp.Index < 0)
            // {
            //     PartyBehaviour.Instance.SvrUserJoinTrain(UserId);
            // }
            // else
            // {
            //     PartyBehaviour.Instance.SvrUserLeaveTrain(UserId);
            // }
        }

    }


    /// <summary>
    /// generic part
    /// </summary>
    public partial class ShowGoPlayer
    {
        async void Start()
        {
            if (isLocalPlayer)
            {
                do
                {
                    Debug.Log("look at my self, vcam behaviour not spawned yet.");
                    await TimerComponent.Instance.WaitAsync(1);
                } while (VcamBehaviour.instance == null);
                VcamBehaviour.instance.LocalCameraChange(4);//20220923进场先看自己
                StartCoroutine(CamBack(4.5f));
            }
        }

        IEnumerator CamBack(float t)
        {
            yield return new WaitForSeconds(t);
            if (!isVoluntarilyMoving) { VcamBehaviour.instance.LocalCameraChange(0); }//如果进场4.5秒我自主移动，不会自动退回，移动结束会自动退回的
        }

        // public Rigidbody rb;
        void Update()
        {
            // heartbeat logic
                if (isLocalPlayer)
                {
                    timeFromLastSend += (int)(Time.deltaTime * 1000);
                    if (timeFromLastSend >= interval)
                    {
                        CmdHeartbeat();
                        timeFromLastSend = 0;
                    }
                }
            
            // todo when touch/click raycast & call focus <ref=OnDogClicked"> @xujihua 
            if (isClient)
            {
                FloatingUiConnector.Instance.DriveFloatingUiPos(UserId);
            }

            if (isLocalPlayer)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    Ray rays = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Debug.DrawRay(rays.origin, rays.direction * 100, Color.yellow);
                    RaycastHit hit;
                    if (Physics.Raycast(rays, out hit))
                    {
                        GameObject currentObject = hit.transform.gameObject;
                        // Debug.Log(currentObject.name);
                        if (currentObject.TryGetComponent<ShowGoPlayer>(out ShowGoPlayer puppy))
                        {
                            OnOtherPlayerClicked(puppy); //20220919当我点击一只小狗调用SwitchCurrentFocusedPlayer还是OnOtherPlayerClicked？MoveVcamFocus应该在这两个方法中被调用但我也不知道咋改比较好
                        }
                    }
                }
            }
        }

        //#region 测试聊天气泡

        //string str = ""; 

        //void OnGUI()
        //{
        //    if (!isLocalPlayer) return;

        //    str = GUI.TextField(new Rect(100, 200, 300, 100), str);
        //    if (GUI.Button(new Rect(100, 300, 100, 100), "发言"))
        //    {
        //        this.SendChatMsg(UserId, str);
        //    }

        //    #endregion
        //}
        [Command]
        private void CmdHeartbeat()
        {
            Debug.Log($"Heartbeat from {this.UserId}");
        }

        public void OnDestroy()
        {
            DOTween.KillAll(transform);
        }
    }
}