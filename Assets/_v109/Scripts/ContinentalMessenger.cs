#nullable enable
using System;
using _v109.Test;
using ET._V111;
using ET.Utility;
using Mirror;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utility;

namespace ShowGo
{
    public class ContinentalMessenger: SingletonMono<ContinentalMessenger>
    {
        public static string ReqPrefixUserInfo(int userId)
        {
            return $"http://xg-cache-api.zanyule.cn/user/{userId}/detail?";
        }

        //int userId, string apply_type, string message, string message_extern
        public static string ReqPrefixFriendApply()
        {
            return $"http://xg-friend-api.zanyule.cn/apply?";
            // $"http://xg-friend-api.zanyule.cn/apply?userId={userId}&apply_type={apply_type}&message={message}&message_extern={message_extern}";
        }

        public enum FriendApplyType: int
        {
            //从房间申请
            APPLY_TYPE_ROOM = 1,
            //从搜索中申请
            APPLY_TYPE_SEARCH = 2,
            //从表白墙申请
            APPLY_TYPE_EXPRESS_WALL = 3,
            //系统推荐
            APPLY_TYPE_SYSTEM = 4,
            //从首页找CP中申请
            APPLY_TYPE_QUICK = 5,
            //从前度墙中申请
            APPLY_TYPE_EX_LIST = 6,
            //从房间黑名单中申请
            APPLY_TYPE_ROOM_BLACK = 7,
            //从设置黑名单中申请
            APPLY_TYPE_SETTING_BLACK = 8,
            //从世界公告中申请
            APPLY_TYPE_GLOBAL_NOTIFY = 9,
            //从账单中申请
            APPLY_TYPE_BILL = 10,
            //从契约墙
            APPLY_TYPE_CONTRACT = 11,
            //从H5
            APPLY_TYPE_H5 = 12,
            //从前度墙
            APPLY_TYPE_EX = 13,
            //从前度墙
            // APPLY_TYPE_SD = 14
        }

        public enum friendState : short
        {
            Not_Applied=0,
            Applied=1,
            Friend=2,
            Deleted= 3,
            FavFriend=4,
        }
        
        #region mocks

        public enum LogicServerMode
        {
            LocalMock,
            LeanMock,
            Actual
        }

        public enum NativeClientMode
        {
            LocalMock,
            Actual
        }
        // [SerializeField]
        private LogicServerMode _logicServerMode = LogicServerMode.LocalMock;

        public LogicServerMode logicServerMode
        {
            get => _logicServerMode;
            private set => _logicServerMode = value;
        }

        // [SerializeField]
        private NativeClientMode _nativeClientMode = NativeClientMode.LocalMock;

        public NativeClientMode nativeClientMode
        {
            get => _nativeClientMode;
            private set => _nativeClientMode = value;
        }

        #endregion
        
        public string PublicParams = null;
        public MyInfo _myInfo;

        protected override void Awake()
        {
            //todo init _logicServerMode and _nativeClientMode by if def here
            _logicServerMode = LogicServerMode.Actual;
            #if UNITY_EDITOR
            // _nativeClientMode = NativeClientMode.LocalMock;
            #elif PLATFORM_ANDROID
            _nativeClientMode = NativeClientMode.Actual;
            #elif ANDROID_LIB
            _nativeClientMode = NativeClientMode.Actual;
            #elif ANDROID_WHOLE_MOCK
            _nativeClientMode = NativeClientMode.LocalMock;
            #endif
            
            base.Awake();
        }

        public async void Native2UnityMsg(string msg)
        {
            // actual logic , and mock logic .
            Debug.Log($"native2unity:{msg}");
            var cmd = NativeUtility.GetOp(msg);
            // var cmdType = NativeUtility.MsgCode2Type(cmd);
            switch (cmd)
            {
                case "MyInfo":
                    _myInfo = NativeUtility.GetOpdata<MyInfo>(msg);
                    GetMyInfoByUserId();
                    break;
                case "InitFinish":
                    Debug.Log($"this message is used for unity client acknowledging native client");
                    break;
                case "CharUIShow":
                    Debug.Log($"this message is used for unity client acknowledging native client");
                    break;
                case "CharUIBack":
                    // Debug.Log($"this message is used for unity client acknowledging native client");
                    VcamBehaviour.instance.LocalCameraChange(0);
                    // hide add friend card
                    FloatingUiConnector.Instance.ClosePuppyCard();
                    break;
                case "UserChat":
                    var userMsg = NativeUtility.GetOpdata<UserChat>(msg);
                    var content = userMsg.text;
                    var me = NetworkClient.localPlayer.GetComponent<ShowGoPlayer>();
                    Debug.Log($"I {me.UserId} am sending msg :{content}");
                    me.SendChatMsg(me.UserId, content);
                    
                    // var charMain = CharMgr.instance.GetCharacter(uid);
                    // charMain.Speak(userMsg.text);
                    break;
                case "DJChanged":
                    var djChanged = NativeUtility.GetOpdata<DJChanged>(msg);
                    //var oldDj = djChanged.oldDJ;
                    var newDJ = djChanged.newDJ;
                    SceneShowgoMgr.Instance.SetDJ(newDJ);
                    //var ts = djChanged.ts;
                    //SceneShowgoMgr.Instance.DJChanged(oldDj, newDJ, ts);
                    //throw new NotImplementedException();
                    break;
                case "DanceRoomLvChange":
                    var lvChange = NativeUtility.GetOpdata<DanceRoomLvChange>(msg);
                    var lvChangeUser = lvChange.userId;
                    var lvl = lvChange.lvl;
                    
                    Debug.Log($"received dance room level change msg, lvl change user is {lvChangeUser}, lvl is {lvl}," +
                                   $"\b unity side should get conf from server, and show corresponding ");
                    break;
                case "FireWorkGift":
                    var fireworkGift = NativeUtility.GetOpdata<FireWorkGift>(msg);
                    var giftUser = fireworkGift.userId;
                    throw new NotImplementedException();
                    break;
                case "PromoteRoom":
                    var promoteRoom = NativeUtility.GetOpdata<PromoteRoom>(msg);
                    var userId = promoteRoom.userId;
                    var roomId = promoteRoom.roomId;
                    var roomUserAmount = promoteRoom.roomUserAmount;
                    var isLocked = promoteRoom.isRoomLocked;
                    FloatingUiConnector.Instance.SpawnRecommendRoomCards((uint)userId);
                    
                    break;
                
                case "SetClubSound":
                    var setClubSound = NativeUtility.GetOpdata<SetClubSound>(msg);
                    ShowGoZegoPlayer.Instance.SetSoundVolume(setClubSound.targetVolume, setClubSound.bInstant,
                        setClubSound.duration);
                    break;

                case "fireworkstart":
                    FireworkBehaviour.instance.OnGiftButtonDown();
                    break;

                case "fireworkend":
                    FireworkBehaviour.instance.OnGiftButtonUp();
                    break;
            }
        }

        public async void GetMyInfoByUserId()
        {
            do
            {
                Debug.Log("network client . local player is null , wait");
                await TimerComponent.Instance.WaitAsync(1);
            } while (NetworkClient.localPlayer == null);

            do
            {
                Debug.Log("network client . local player does not have component show go player , wait");
                await TimerComponent.Instance.WaitAsync(1);
            } while (NetworkClient.localPlayer.GetComponent<ShowGoPlayer>() == null);

            do
            {
                Debug.Log($"myinfo not init right now, waiting...");
                await TimerComponent.Instance.WaitAsync(1);
            } while (_myInfo == null);
            
            NetworkClient.localPlayer.GetComponent<ShowGoPlayer>().InitUserId(_myInfo.userId);
            PublicParams = _myInfo.LogicServerInfo;
            Debug.Log($"public params is {PublicParams}");
            //query my id from logic server
            GetUserInfo(_myInfo.userId, OnGetMyUserInfo);
        }

        public void Unity2NativeMsg(string msg)
        {
            // actual logic , and mock logic 
            Debug.Log($"unity2native:{msg}");
            #if UNITY_EDITOR
            switch (nativeClientMode)
            {
                case NativeClientMode.LocalMock:
                    Debug.Log($"Sending mock msg to Native:{msg}");
                    break;
                case NativeClientMode.Actual:
                    Debug.Log($"Actual native client not available in unity editor");
                    break;
            }
            #endif
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("Native2UnityMsg", msg);
            #endif

            #if UNITY_IOS && !UNITY_EDITOR
			// Unity2NativeMsgIOS(msg);
            throw new NotImplementedException();
            #endif
        }
        
        //  mock native messages here
        #region mockFuncs

        public static string MockMyInfo(int userId = -1,
            string LogicServerInfo =
                "_dzch=035f47991c6d8916&_imei=035f47991c6d8916&_t=200&_v=10010&_app=3&_s_v=9&_s_n=RedmiNote5&_net=NETWORK_WIFI&_token=857480_200_1663726822940_e92b55db88aaa196&_c=&_at=2&_time=1663845231362")
        {
            var uid = userId;
            if (uid < 0)
            {
                uid = (int)testContinentalMsg.RandomUserId();
            }
            
            var myinfo = new MyInfo()
            {
                userId = uid,
                LogicServerInfo = LogicServerInfo,
            };
            var op = NativeUtility.MakeOp(myinfo);
            return op;
        }

        public static string MockInitFinish()
        {
            var initFinish = new InitFinish();
            return NativeUtility.MakeOp(initFinish);
        }
        
        public static string MockUIShow()
        {
            var camFocus = new CharUIShow();
            return NativeUtility.MakeOp(camFocus);
        }
        
        public static string MockUIBack()
        {
            var camServer = new CharUIBack();
            return NativeUtility.MakeOp(camServer);
        }

        public static string MockUserMsg(int userId = 9000, long ts = 1663225714000, string msg = "msg placeholder")
        {
            var userMsg = new UserMsg()
            {
                userId = userId,
                ts = ts,
                text = msg
            };
            return NativeUtility.MakeOp(userMsg);
        }
        

        #endregion

        void Unity2LogicSvr(object req)
        {
            // use token url userId etc. to http request(and handle resp) @futou
            
            
        }

        void Unity2LogicSvrMock(object req)
        {
            // use leancloud @futou
        }

       
        public void GetUserInfo(int userId, Action<string> callback=null)
        {
            var uri = ReqPrefixUserInfo(userId) + PublicParams;
            Debug.Log(uri);
            StartCoroutine(WebHelper.Instance.Get(uri, callback));
            // using (UnityWebRequest www = UnityWebRequest.Get(ReqPrefixUserInfo(userId)+PublicParams))
            // {
            //     yield return www.SendWebRequest();
            //     if (www.result == UnityWebRequest.Result.Success)
            //     {
            //         Debug.Log($"{www.downloadHandler.text}");
            //         OnAcquireUserInfo(www.downloadHandler.text);
            //     }
            //     
            // }
        }

        /// <summary>
        /// 获得UserInfo返回值后
        /// </summary>
        /// <param name="text"></param>
        public async void OnGetMyUserInfo(string text)
        {
            Debug.Log($"on get my user info {text}");
            var jObj= JObject.Parse(text);
            Debug.Log($"jobj is {jObj}");
            var dataInfo = jObj["dataInfo"];
            Debug.Log($"data info is {dataInfo}");
            var nickName = (string?) dataInfo["nickName"];
            Debug.Log($"nick name is {nickName}");
            //todo logic server add aprcId @futou
            var cfg = await AssetCache.Instance.GetConfig<XGAprcCfg>(EConfig.XGAprcCfg);
            var randomAprcId = cfg.GetRandomRoomLv();
            var networkIdentity = NetworkClient.localPlayer;
            var localPlayer = networkIdentity.GetComponent<ShowGoPlayer>();
            localPlayer.CmdSetAprcId(randomAprcId);
            if(nickName!=null) localPlayer.InitName(nickName);

        }


        public void ApplyFriend(int userId, FriendApplyType applyType)
        {

            var uri = ReqPrefixFriendApply() + PublicParams;
            
            WWWForm form = new WWWForm();
            form.AddField("user_id",userId);
            form.AddField("apply_type", (int) applyType);
            form.AddField("message","");
            form.AddField("message_extern","");
            Debug.Log($"{ReqPrefixFriendApply() + PublicParams}&{form}");
            StartCoroutine(WebHelper.Instance.Post(uri, form, OnApplyFriendFinish));
      
            // using (UnityWebRequest www = UnityWebRequest.Post(ReqPrefixFriendApply()+PublicParams, form))
            // {
            //     yield return www.SendWebRequest();
            //
            //     if (www.result != UnityWebRequest.Result.Success)
            //     {
            //         Debug.Log(www.error);
            //     }
            //     else
            //     {
            //         Debug.Log(www.downloadHandler.text);
            //         OnApplyFriendFinish(www.downloadHandler.text);
            //     }
            // }
        }

        /// <summary>
        /// 获得目标与我的好友状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="text"></param>
        public void OnGetFriendState(int userId, string text)
        {
            var jObj= JObject.Parse(text);
            var dataInfo = jObj["dataInfo"];
     
            
            long? birthday = 0;
            short? friendState = -1;
            string? nickName = "BadName";
            bool isMale = true;
            string? userDesc="加我好友呀";
            string? headPicUrl = "";//20201004头像，这里也许大概应该放一个系统默认头像
            try
            {
                friendState = (short?)dataInfo["friendState"];
                nickName = (string?)dataInfo["nickName"];
                isMale = (string?)dataInfo["sex"] == "1";
                userDesc = (string?) dataInfo["userDesc"];
                birthday = (long?)dataInfo["birthday"];
                //20201004头像
                headPicUrl= (string?)dataInfo["headPic"];
            }
            catch (Exception e)
            {
                Debug.LogWarning($"deserialize error:{e}");
            }

            
            //find card instance and set btn state
            if (!FloatingUiConnector.Instance.puppyCards.TryGetValue((uint)userId, out var cardGot))
            {
                Debug.Log($"roomcard for {userId} is not found");
                return;
            }

            var cardInst = cardGot.GetComponent<PuppyCardBut>();
            //name, detail and description
            cardInst.SetNickname(nickName);
            int age = 0;
            if (birthday.HasValue) age = TimeHelper.ts2Age(birthday.Value);
            cardInst.SetDetail(isMale, age, "");
            if(userDesc!=null) cardInst.SetDesc(userDesc);

            //头像
            if (cardInst.gameObject.activeInHierarchy) cardInst.SetHeadPic(headPicUrl);

            //add friend btn
            if (friendState == 0 || friendState== 3)
            {
                cardInst.alterStart(userId != NetworkClient.localPlayer.GetComponent<ShowGoPlayer>().UserId);
            }
            else
            {
                cardInst.alterStart(false);
            }
            
            
            
        }
        
        
        /// <summary>
        /// 加好友返回结果
        /// </summary>
        /// <param name="text"></param>
        public void OnApplyFriendFinish(string text)
        {
            var jObj= JObject.Parse(text);
            var code = (int)jObj["code"];
            switch (code)
            {
                case 0:
                    //success
                    Debug.Log($"apply friend success");                
                    break;
                case 1:
                    //fail
                    Debug.Log($"apply friend fail");
                    break;
                default:
                    throw new NotImplementedException();
                
            }
                
        }
    }
}