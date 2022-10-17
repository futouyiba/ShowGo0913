using System;
using System.Collections.Generic;
using System.Linq;
using ET;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using Random = UnityEngine.Random;

namespace ShowGo
{
    /// <summary>
    /// 管理角色的创建，存储在场狗子的引用
    /// 为了性能慢慢创建狗子外观
    /// </summary>
    public class SceneShowgoMgr : SingletonMono<SceneShowgoMgr>
    {
        // static Vector3 defaultPos;

        // private uint myId=999998;
        private Dictionary<uint, ShowGoPlayer> playersDic = new Dictionary<uint, ShowGoPlayer>();

        public long currentDJSince;

        public int currentDJ = -1;


        public bool AmIDJ(int userId) => userId == currentDJ;

        /// <summary>
        /// 每N帧创建角色
        /// </summary>
        [BoxGroup("DelayGenerate")] [SerializeField]
        private int perNFrame;

        /// <summary>
        /// 创建时创建多少个,0=不限制
        /// </summary>
        [FormerlySerializedAs("generateNum")] [BoxGroup("DelayGenerate")] [SerializeField]
        private int cycWorkSum;

        [BoxGroup("DelayGenerate")] [ReadOnly] public int toGenerateCounter;
        private Queue<GenerateTask> generateQueue;

        [SerializeField] public Transform SceneCenter;


        protected override void Awake()
        {
            base.Awake();
            toGenerateCounter = perNFrame;
            generateQueue = new Queue<GenerateTask>();
        }

        public GameObject DJCat;
        public Transform DJSpot;
        public void SetDJ(int newDJ)//很重要的问题:DJ是否只能在Server上设定？目前是这样的，syncvar只能在server更改嘛
        {
            if (currentDJ != -1) { GetUser((uint)currentDJ).isDJ = false; }
            else { Me.CmdShowCat(false); }
            if (newDJ != -1) { GetUser((uint)newDJ).isDJ = true; }
            else { Me.CmdShowCat(true); }
            currentDJ = newDJ;
        }

        public bool AddUser(uint userId, ShowGoPlayer player)
        {

            if (playersDic.TryGetValue(userId, out var existPlayer))
            {
                if (userId == 0) return false; //todo deal with this carefully
                Debug.Log($"user{userId} already exists");
                return false;
            }

            playersDic.Add(userId, player);
            return true;
        }

        public void RemoveUser(uint userId)
        {
            //stop training before remove
            if (!playersDic.ContainsKey(userId))
            {
                Debug.LogWarning($"remove user of {userId} but dic dont contain");
                return;
            }

            var toRm = GetUser(userId);
            var trainComp = toRm.GetComponent<TrainState>();
            if (trainComp) trainComp.OnExit();

            playersDic.Remove(userId);
        }

        public ShowGoPlayer Me
        {
            get
            {
                var instanceMyInfo = ContinentalMessenger.Instance._myInfo;
                if (instanceMyInfo == null)
                {
                    return null;
                }

                var myUserIdFromNative = instanceMyInfo.userId;

                if (myUserIdFromNative <= 0)
                {
                    return null;
                }

                if (!playersDic.ContainsKey((uint)myUserIdFromNative))
                {
                    return null;
                }

                return playersDic[(uint)myUserIdFromNative];
            }
        }

        public ShowGoPlayer GetUser(uint uid)
        {
            var succeed = this.playersDic.TryGetValue(uid, out ShowGoPlayer result);
            if (!succeed)
            {
                Debug.LogWarning($"no user found for {uid}");
                return null;
            }

            return result;
        }

        public ShowGoPlayer GetRandomUser()
        {
            var randomId = Random.Range(0, playersDic.Count);
            return playersDic.ElementAt(randomId).Value;
        }

        private void Update()
        {

            if (generateQueue.Count > 0)
            {
                toGenerateCounter -= 1;
                if (toGenerateCounter <= 0)
                {
                    int cycWorkDone = 0;
                    while (cycWorkDone < cycWorkSum && generateQueue.Count != 0)
                    {
                        var exec = generateQueue.Dequeue();
                        exec.dlg?.Invoke();
                        cycWorkDone += exec.intensity;
                    }

                    //
                    //
                    // int toGenerate = Mathf.Min(cycWorkSum, generateQueue.Count);
                    // if (toGenerate == 0)
                    // {
                    //     toGenerate = generateQueue.Count;
                    // }
                    // for (int i = 0; i < toGenerate; i++)
                    // {
                    //     var exec = generateQueue.Dequeue();
                    //     exec?.Invoke();
                    // }

                    toGenerateCounter = perNFrame;
                }
            }


        }

        public void ClearEverything()
        {
            foreach (var player in playersDic.Values)
            {
                if (player == null)
                {
                    continue;
                }

                FloatingUiConnector.Instance.DelDicData(player.UserId);

                if (player.gameObject == null)
                {
                    continue;

                }

                Destroy(player.gameObject);
            }

            playersDic.Clear();
        }


        public void AddCreateTask(Action createAction, int intensity = 1)
        {
            var task = new GenerateTask(createAction, intensity);
            generateQueue.Enqueue(task);
        }


        /// <summary>
        /// 每个人都说某句话
        /// </summary>
        /// <param name="text"></param>
        public void EveryoneSpeak(string text)
        {
            foreach (var playerkv in playersDic)
            {
                FloatingUiConnector.Instance.SpawnChatBubble(playerkv.Key, text,true);
            }
        }

        private struct GenerateTask
        {
            public Action dlg;
            public int intensity;

            public GenerateTask(Action func, int intensity)
            {
                dlg = func;
                this.intensity = intensity;
            }
        }

        public List<uint> GetAllUserIds()
        {
            return this.playersDic.Keys.ToList();
        }
    }
}

