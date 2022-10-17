using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShowGo
{
    
    [CreateAssetMenu(fileName = "AprcCfg", menuName = "ShowGoCfg/Aprc")]
    public class XGAprcCfg :SerializedScriptableObject
    {
        public GameObject characterPrefab;
        
        public Dictionary<int, EnableInfo> lv_AprcBaseDict;

        public Dictionary<string, List<string>> allowed_decors;
        
        public EnableInfo GetBaseInfo(int roomLv)
        {
            if (!lv_AprcBaseDict.TryGetValue(roomLv, out var result))
            {
                Debug.LogWarning($"base info for {roomLv} is not found");
                return null;
            }

            return result;
        }

        public int GetRandomRoomLv()
        {
            var randId = Random.Range(0, lv_AprcBaseDict.Count);
            return lv_AprcBaseDict.ElementAt(randId).Key;
        }
        
        public class EnableInfo
        {
            public string bodyName;
            public string decorName;
        }
    }
}