using System;
using System.Collections.Generic;
using System.Linq;
using ET.Utility;
using LC.Newtonsoft.Json.Linq;
using ShowGo;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _v109.Test
{
    public class testContinentalMsg : SerializedMonoBehaviour
    {
    
    #region testUserId
    
        public static List<uint> userId = new List<uint>()
        {
            840930,
            840931,
            840932,
            840933,
            840934,
            840935,
            840936,
            840937,
            840938,
            840939,
            840940,
            840941,
            840942,
            840943,
            840944,
            840945,
            840946,
            840947,
            840948,
            840949,
            840950,
            840951,
            840952,
            840953,
            840954,
            840955,
            840956,
            840957,
            840958,
            840959,
            840960,
            840961,
            840962,
            840963,
            840964,
            840965,
            840966,
            840967,
            840968,
            840969,
            840970,
            840971,
            840972,
            840973,
            840974,
            840975,
            840976,
            840977,
            840978,
            840979,
            840980,
            840981,
            840982,
            840983,
            840984,
            840985,
            840986,
            840987,
            840988,
            840989,
            840990,
            840991,
            840992,
            840993,
            840994,
            840995,
            840996,
            840997,
            840998,
            840999,
            841000,
            841001,
            841002,
            841003,
            841004,
            841005,
            841006,
        };

        public static uint RandomUserId()
        {
            var randId = Random.Range(0, userId.Count);
            return userId.ElementAt(randId);
        }
        
    #endregion

        public int DJIdInput = -1;
        
        [Button("testGetUserInfo")]
        public void testGetUserInfo()
        {
            ContinentalMessenger.Instance.GetUserInfo(840258);
        }

        [Button("testApplyFriend")]
        public void testApplyFriend()
        {
            ContinentalMessenger.Instance.ApplyFriend(840961, ContinentalMessenger.FriendApplyType.APPLY_TYPE_ROOM);
        }

        
        
        [Button("mockMyInfo")]
        public void mockMyInfo()
        {
            var myinfo = new MyInfo()
            {
                userId = 840258,
                LogicServerInfo =
                    "_dzch=035f47991c6d8916&_imei=035f47991c6d8916&_t=200&_v=10010&_app=3&_s_v=9&_s_n=RedmiNote5&_net=NETWORK_WIFI&_token=857480_200_1663726822940_e92b55db88aaa196&_c=&_at=2&_time=1663845231362",
            };
            var op = NativeUtility.MakeOp(myinfo);
            
            Debug.LogWarning(op);
        }


        [SerializeField] private string testMyInfo2Input;
        [Button("testMyInfo2")]
        public void mockMyInfo2()
        {
            ContinentalMessenger.Instance.Native2UnityMsg(testMyInfo2Input);
        }

        [Button("UserInfoNullTest")]
        public void UserInfoNullTest()
        {
            
            var jObj= JObject.Parse(testMyInfo2Input);
            var dataInfo = jObj["dataInfo"];
     
            
            long? birthday = 0;
            short? friendState = -1;
            string? nickName = "BadName";
            bool isMale = true;
            string? userDesc="加我好友呀";
            try
            {
                friendState = (short?)dataInfo["friendState"];
                nickName = (string?)dataInfo["nickName"];
                isMale = (string?)dataInfo["sex"] == "1";
                userDesc = (string?) dataInfo["userDesc"];
                birthday = (long?)dataInfo["birthday"];
            }
            catch (Exception e)
            {
                Debug.LogWarning($"deserialize error:{e}");
            }

            Debug.Log(
                $"friendSate ={friendState}, nickname={nickName}, ismale={isMale}, userDesc={userDesc}, birthday= {birthday}");
        }

        [Button("RandomRoomPromotion")]
        public void RandomRoomPromotion()
        {
            var randUser = SceneShowgoMgr.Instance.GetRandomUser();
            PromoteRoom msg = new PromoteRoom()
            {
                userId = (int) randUser.UserId,
                roomId = Random.Range(100000, 999999),
                isRoomLocked = false,
                roomUserAmount = Random.Range(1, 15),
                roomIconUri = ""
            };
            var fin = NativeUtility.MakeOp(msg);
            ContinentalMessenger.Instance.Native2UnityMsg(fin);
        }

        /// <summary>
        /// set dj from server, this will not broadcast message
        /// but make you dj on server
        /// use this on server!
        /// </summary>
        [Button("SetDJ")]
        public void IamDJ()
        {
            
            DJChanged changed = new DJChanged()
            {
                oldDJ = -1,
                newDJ = DJIdInput,
                ts = DateTime.UtcNow.Millisecond
            };
            var msg = NativeUtility.MakeOp(changed);
            ContinentalMessenger.Instance.Native2UnityMsg(msg);
        }
    }
}

