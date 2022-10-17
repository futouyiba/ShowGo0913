using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShowGo
{
    [CreateAssetMenu(fileName = "ShowGoPlayerCfg", menuName = "ShowGoCfg/Player")]
    public class ShowGoPlayerCfg :SerializedScriptableObject
    {      
        [BoxGroup("CharAprcPrefabs")]
        [NonSerialized, OdinSerialize] public Dictionary<int, GameObject> CharacterPrefabs=new Dictionary<int, GameObject>();
        
        [BoxGroup("Movements")]
        [NonSerialized, OdinSerialize] public float MoveSpeed;

        [BoxGroup("CmdPrefixes")]
        [NonSerialized, OdinSerialize]
        public Dictionary<string, string> commandPrefixes = new Dictionary<string, string>()
        {
            {"镜头", "ServerDriveVcamSwitch" },
            {"墨镜","ClientWearGoggles"},
            {"move", "PrefixMove"},
        };
        
        // odin serialize this one and above.
        [NonSerialized, OdinSerialize]
        public Dictionary<string, string> clientCommandPrefixes = new Dictionary<string, string>()
        {
            {"定位自己", "ClientPrefixFocusSelf"},
        };

        [BoxGroup("L1Prefab")] [NonSerialized, OdinSerialize]
        public GameObject ShowGoPlayerPrefab;


        public GameObject GetRandomPrefab()
        {
            var randId = Random.Range(0, CharacterPrefabs.Count);
            return CharacterPrefabs.ElementAt(randId).Value;
        }

        public int GetRandomAprcId()
        {
            var randId = Random.Range(0, CharacterPrefabs.Count);
            return CharacterPrefabs.ElementAt(randId).Key;
        }

    }
}

