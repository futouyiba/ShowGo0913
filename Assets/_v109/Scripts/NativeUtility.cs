using System;
using System.Collections.Generic;
using ET.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ShowGo
{
    
    public static class NativeUtility
    {
        public static string GetOp(string json)
        {
            // var data = JsonMapper.ToObject(json);
            // return data["Op"].ToString();
            try
            {
                var data = JObject.Parse(json);
                return data["Op"]?.ToString();
            }
            catch (Exception e)
            {
                Debug.Log($"json parse error:{e}");
                return null;
            }
        }
        
        
        public static T GetOpdata<T>(string json) where T:JsonCmd
        {
            try
            {
                var data = JObject.Parse(json);
                var opData = data["OpData"];
                return opData.ToObject<T>();
            }
            catch (Exception e)
            {
                Debug.Log($"json parse error:{e}");
                return null;
            }
                
            // var data = JsonMapper.ToObject(json);
            // var opData = data["OpData"];
            // return JsonMapper.ToObject<T>(opData.ToJson());
        }
        
        
        public static Dictionary<string, Type> cmdDict = new Dictionary<string, Type>()
        {
            {"UserEnter",typeof(UserEnter)},
            {"MeEnter",typeof(MeEnter)},
            {"UserExit",typeof(UserExit)},
            {"UserList",typeof(UserList)},
            {"UserMove",typeof(UserMove)},
            {"UserMsg",typeof(UserMsg)},
            {"MeMove",typeof(MeMove)},
            {"MeTap",typeof(MeTap)},
            {"MyPos",typeof(MyPosition)},
            {"UserSit", typeof(UserSit)},
            {"MyInfo", typeof(MyInfo)},
            {"InitFinish", typeof(InitFinish)},
            {"CharUIShow", typeof(CharUIShow)},
            {"CharUIBack", typeof(CharUIBack)},
            {"UserChat",typeof(UserChat)},
            {"DJChanged", typeof(DJChanged)},
            {"DanceRoomLvChange", typeof(DanceRoomLvChange)},
            {"FireWorkGift", typeof(FireWorkGift)},
            {"PromoteRoom", typeof(PromoteRoom)},
        };
            
        public static Type MsgCode2Type(string code)
        {
            var succeed = cmdDict.TryGetValue(code, out Type type);
            if (!succeed)
            {
                Debug.Log($"failed coverting code for {code}");
                return null;
            }

            return type;
        }
        
        public static string MsgType2Code(Type type)
        {
            foreach (var kvpair in cmdDict)
            {
                if (kvpair.Value == type)
                {
                    return kvpair.Key;
                }
            }

            Debug.Log($"failed converting type for {type.ToString()}");
            return null;
        }
        
        
        /// <summary>
        /// 构建Op消息
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string MakeOp<T>(T obj) where T : JsonCmd
        {
            var code = MsgType2Code(typeof(T));
            Operation<T> op = new Operation<T>(obj)
            {
                Op = code
            };
            // var data = JsonMapper.ToJson(op);
            var data = JsonConvert.SerializeObject(op);
            //todo: get rid of "_t"'s
            return data;
        }

    }
}
