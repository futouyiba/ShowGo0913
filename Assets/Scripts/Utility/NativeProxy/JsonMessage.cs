using System.Collections.Generic;
using UnityEngine;

namespace ET.Utility
{

    public class JsonMessage
    {
 
    }

    public class JsonCmd
    {
        
    }

    public class Operation : JsonMessage
    {
        public string Op;
        // public string OpData;
    }

    public class Operation<T> : JsonMessage where T:JsonCmd
    {
        public string Op;
        public T OpData;

        public Operation(T data)
        {
            OpData = data;
        }

    }

    public struct Vec2
    {
        public double x;
        public double y;

        public Vec2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class UserInfo : JsonCmd
    {
        public const int MALE = 1;
        public const int FEMALE = 2;

        public int userId;
        public int surfing;
        public string nickName = "";
        public string headPic = "";
        public string intro = "";
        public int sex; //1男，2女
        public long birthday;
        public int micId;
        public int headgearId;
        public int carId;
        public int chatBubbleId;
        public int nickPendantId;
        public int userState;
        public int friendState;
        public int deleteUserId;


        public string userDesc = "";

//禁言时长
        public long privateChatBanTime;
//房间禁言时长
        public long messageBanTime;
// 当前使用的个性进房提示信息
        public string currentIntoVoiceTips;
//陪伴时长
        public long voiceTime;
//封禁时长
        public long userBanTime;

        public int creditLevel;
        public string color;
        public int gifType;  //1v1聊天，GIF类型，1破冰GIF，2表情包，3话题卡
//1v1聊天，音频时长
        public int duration;
        public int userType;
        public string city;
        public long lastActiveTime; //上次活跃时间
        public bool online;   //是否在线
        public bool onlineHidden; //是否隐身
        public bool newUser;   //是否新注册用户
        // private List<UserLevelBean> levelList;
        // private List<CacheUserContractInfo> contractList;
        public bool inRoom;//当前是否在房间
        public bool isInviteMic;//是否已经邀请过上麦
        // private RoomContractInfo contractInfo;//当前显示的契约
        public Vector2 position;
        public int appearance;
    }

    public class UserMove : JsonCmd
    {
        public int userId;
        public long ts;
        public Vector2 position;
    }

    public class UserEnter : JsonCmd
    {
        public int userId;
        public long ts;
        public string nickName;
        public int sex;
        public Vector2 position;
        public int appearance;
    }

    public class MeEnter : JsonCmd
    {
        public int userId;
        public long ts;
        public string nickName;
        public int sex;
        public int appearance;
        public Vector2 position;
    }

    public class UserExit : JsonCmd
    {
        public int userId;
        public long ts;
    }

    public class UserList : JsonCmd
    {
        public List<UserInfo> userInfos;

    }

    // public List<int> uids;
    // public List<string> unames;
    // public List<Vector2> positions;
    // public List<int> appearances;
    
    public class MeMove : JsonCmd
    {
        public long ts;
        public Vector2 position;
    }

    public class MyPosition : JsonCmd
    {
        public long ts;
        public Vector2 position;
    }


    public class UserMsg : JsonCmd
    {
        public int userId;
        public long ts;
        public string text;
    }

    public class MeTap : JsonCmd
    {
        
    }

    // public class Break : JsonCmd
    // {
    //     public bool isStartBreak;
    //     
    // }

    public class UserSit : JsonCmd
    {
        public int userId;
        public int micId;
        // public int seatId;
    }

    public class Gift : JsonCmd
    {
        
    }

    public class RoomId : JsonCmd
    {
        public int roomId;
    }

    public class RoomLevel : JsonCmd
    {
        public int roomLv;
    }



    
    #region 0913v1.09

    public class MyInfo : JsonCmd
    {
        public int userId;
        //实时服务器地址
        //logic server authentication info
        public string LogicServerInfo;
    }

    public class InitFinish : JsonCmd
    {
        
    }

    public class CharUIShow : JsonCmd
    {
        
    }

    public class CharUIBack : JsonCmd
    {
        
    }

    public class UserChat : JsonCmd
    {
        public long ts;
        public string text;
    }

    /// <summary>
    /// Native=>Unity client
    /// acknowledging client dj change
    /// may not need after using MQ sync DJ
    /// </summary>
    public class DJChanged : JsonCmd
    {
        //userId of oldDJ
        public int oldDJ;
        //userId of newDJ
        public int newDJ;
        //timestamp when the data updated
        public long ts;
    }

    public class DanceRoomLvChange : JsonCmd
    {
        //whose lv is up
        public int userId;
        //level to what
        public int lvl;
    }

    public class FireWorkGift : JsonCmd
    {
        /// <summary>
        /// who sent the gift
        /// </summary>
        public int userId;
        
        /// <summary>
        /// how many 
        /// </summary>
        public int amount;
        
        /// <summary>
        /// in how long
        /// </summary>
        public float duration;

        /// <summary>
        /// combo stage 
        /// </summary>
        public int comboStage;
    }

    public class PromoteRoom : JsonCmd
    {
        public int userId;
        public int roomId;
        public int roomUserAmount;
        public bool isRoomLocked;
        public string roomIconUri;
    }

    //先用原来的UserMsg处理UserChat

    public class WheelFocus : JsonCmd
    {
        public int userId;
        
    }
    
    /// <summary>
    /// this is the json command that should be sent from native client, and let native client tell unity
    /// what volume should be set.
    /// </summary>
    public class SetClubSound : JsonCmd
    {
        /// <summary>
        /// what volume we want unity zego change to. it's an integer, from 0 to 200.
        /// normally 100 is the "full volume"
        /// we only set it above 100 on special circumstances.
        /// </summary>
        public int targetVolume;
        /// <summary>
        /// it says "should volume be instantly changed to target volume?
        /// if it's false, unity should do a tween to zego volume
        /// or to say, unity should let volume gradually go to target volume
        ///
        /// if it's true, the volume would instanly changed.
        /// in this circumstance, <paramref name="duration"/> would be useless.
        /// </summary>
        public bool bInstant;
        /// <summary>
        /// the tween duration in seconds.
        /// e.x. user click "派对", and we can set target volume to 0, duration to 10 seconds
        /// , bInstant to false. 
        /// </summary>
        public float duration;
    }

    public class fireworkstart : JsonCmd
    {
        
    }

    public class fireworkend : JsonCmd
    {
        
    }
    
    #endregion
    
}