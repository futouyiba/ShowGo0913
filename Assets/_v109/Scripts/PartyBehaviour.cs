using System;
using System.Collections.Generic;
using System.Linq;
// using System.Runtime.Remoting.Channels;
using Bolt;
using ET;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility;
using TMPro;
using Random = UnityEngine.Random;

namespace ShowGo
{
    /// <summary>
    /// multiple-user behaviours should be written here.
    /// for example, train logic, roar together.
    /// </summary>
    public class PartyBehaviour : SingletonNetworkBehaviour<PartyBehaviour>
    {
        //20220919 moved to SceneShowgoMgr
        // public Dictionary<uint, ShowGoPlayer> ShowGoPlayers = new Dictionary<uint, ShowGoPlayer>();
        
        // train patrol use a timestamp to calculate train head angle @tiaohai
        // head timestamp
        // use this variation because syncing train transform is expensive.
        [SyncVar] private long trainStartingTimeStamp;

        // [SyncVar(hook = "OnMusicChanged")] private string musicDigest;
        
        // train list
        private readonly SyncList<uint> trainUsers = new SyncList<uint>();

        public TextMeshPro musicDes;
        
        public GameObject walkablePlane;

        private void Start()
        {
            trainUsers.Callback += OnTrainUsersChanged;
        }


        public void SvrUserTrainStateChange(uint userId)
        {
            if (!isServer) return;
            //查看是否已经在火车中，然后给状态机发消息
            //其实这里可能有问题，若以后状态机进入火车状态并未立即改动synclist，就会出现暂时不同步的现象
            bool isJoin = !trainUsers.Contains(userId);

            var player = SceneShowgoMgr.Instance.GetUser(userId);
            var fsm = player.GetComponent<StateMachine>();
            if (!fsm || !fsm.enabled)
            {
                Debug.LogError($"fsm does not found or not active on {userId}");
                return;
            }

            fsm.TriggerUnityEvent(isJoin ? "TrainStart" : "TrainEnd");

        }
        

        /// <summary>
        /// used on server 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="posId"></param>
        public void SvrUserJoinTrain(uint userId, int posId=-1)
        {
            if (!isServer) return;
            if (trainUsers.Contains(userId))
            {
                Debug.Log($"user {userId} already in train");
                return;
            }

            if (posId < 0)
            {
                trainUsers.Add(userId);
                
                
            }
            else
            {
                trainUsers.Insert(posId, userId);
                
            }
        }

        public void SvrUserLeaveTrain(uint userId)
        {
            if (!isServer) return;
            if (!trainUsers.Contains(userId))
            {
                Debug.Log($"user {userId} does not in train");
                return;
            }

            trainUsers.Remove(userId);
            
        }

        public void SvrEndTrain()
        {
            if (!isServer) return;
            trainUsers.Clear();
        }
        
        
        [Button("get random position")]
        public Vector3 GetRandomWalkablePosition()
        {
            List<Vector3> VerticeList = new List<Vector3>(walkablePlane.GetComponent<MeshFilter>().sharedMesh.vertices);
            Vector3 leftTop = walkablePlane.transform.TransformPoint(VerticeList[0]);
            Vector3 rightTop = walkablePlane.transform.TransformPoint(VerticeList[10]);
            Vector3 leftBottom = walkablePlane.transform.TransformPoint(VerticeList[110]);
            Vector3 rightBottom = walkablePlane.transform.TransformPoint(VerticeList[120]);
            Vector3 XAxis = rightTop - leftTop;
            Vector3 ZAxis = leftBottom - leftTop;
            Vector3 RndPointonPlane = leftTop + XAxis * Random.value + ZAxis * Random.value;//不给中间概率增大就return这个！

            //20221003让大家随机走到场地中间的概率更大
            Vector3 middlePoint = leftTop + XAxis * 0.5f + ZAxis * 0.5f;//场地中心点
            float rollNummberX = Random.value;//roll一个零到一之间的随机数
            Vector3 weightedDistanceX;
            if (Random.value > 0.5f) { weightedDistanceX = rollNummberX * rollNummberX * rollNummberX * 0.5f * XAxis; }
            else { weightedDistanceX = -rollNummberX * rollNummberX * rollNummberX * 0.5f * XAxis; }
            //看起来很草吧,rollNummberX四次方分布在0-1,向量正负随机
            float rollNummberZ = Random.value;
            Vector3 weightedDistanceZ;
            if (Random.value > 0.5f) { weightedDistanceZ = rollNummberZ * rollNummberZ * rollNummberZ * 0.5f * ZAxis; }
            else { weightedDistanceZ = -rollNummberZ * rollNummberZ * rollNummberZ * 0.5f * ZAxis; }
            Vector3 CenteredRndPointonPlane = middlePoint + weightedDistanceX + weightedDistanceZ;


            // spawn a sphere on the plane to test the position
            // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // sphere.transform.position = RndPointonPlane + walkablePlane.transform.up * 0.5f;

            // Debug.Log(RndPointonPlane.ToString("F4"));
            return CenteredRndPointonPlane;
        }
        
        /// <summary>
        /// this is a client hook
        /// </summary>
        /// <param name="op"></param>
        /// <param name="itemindex">index in train list</param>
        /// <param name="olditem">user id</param>
        /// <param name="newitem">user id</param>
        public void OnTrainUsersChanged(SyncList<uint>.Operation op, int itemIndex, uint oldUserId, uint newItem)
        {
            // if(isServer) Debug.LogWarning($"server is calling OnTrainUsersChanged");
            if (isServer) return;
            // remove from train / add in train logic @tiaohai
            switch (op)
            {
                case SyncList<uint>.Operation.OP_ADD:
                    CharJoinTrain(newItem, itemIndex);
                    Debug.Log($"{newItem} appended to trainUsers , trainUser count is {trainUsers.Count}");
                    break;
                case SyncList<uint>.Operation.OP_INSERT:
                    CharJoinTrain(oldUserId, itemIndex);
                    Debug.Log($"{newItem} appended to trainUsers , trainUser count is {trainUsers.Count}");
                    break;
                case SyncList<uint>.Operation.OP_REMOVEAT:
                    CharLeaveTrain(itemIndex, oldUserId);
                    Debug.Log($"{oldUserId} removed from trainUsers , trainUser count is {trainUsers.Count}");
                    break;
                case SyncList<uint>.Operation.OP_CLEAR:
                    EndTrain();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        void OnMusicChanged(string oldMusicDes, string newMusicDes)
        {
            // call music title text change. done @lizhaozhao
            // todo @futou link it to music broadcasting
            // the change API should be called by this,and a random logic
            if (oldMusicDes != newMusicDes)
            {
                musicDes.text = newMusicDes;
            }
        }

        protected override void Update()
        {
            // if (!isServer)
            // {
                foreach (var runChar in runCharacterList)
                {
                    if (runChar == null) continue;
                    //客户端因为没有状态机，在这里更新它的Update
                    // var trainComp = SceneShowgoMgr.Instance.GetUser(runChar).GetComponent<TrainState>();
                    var trainComp = runChar != null ? runChar.GetComponent<TrainState>() : null;
                    if (trainComp != null) trainComp.TrainPosUpdate(SceneShowgoMgr.Instance.SceneCenter, radius);
                }    
            // }
            
        }


        #region train logic
        
        private float radius = 4f;//圆形半径
        public List<Transform> runCharacterList = new List<Transform>();//所有跑者（的Transform）
        /// <summary>
        /// 设置跑圈半径
        /// </summary>
        private void SetRadius(int currentLevel)
        {
            if (currentLevel ==3)
            {
                radius = 6f;
            }
            else if (currentLevel == 2)
            {
                radius = 4f;
            }
            else if (currentLevel == 1)
            {
                radius = 2f;
            }
        }

        /// <summary>
        /// add, insert
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private void CharJoinTrain(uint userId, int pos = -1)
        {
            // var charGot = CharMgr.instance.GetCharacter(userId);
            var charGot= SceneShowgoMgr.Instance.GetUser(userId).GetComponent<ShowGoPlayer>();
            var joinTrasform = charGot.transform;
            if (runCharacterList.Contains(joinTrasform))
            {
                Debug.LogWarning($"{userId} already in train, not adding");
                return;
            }
            var trainComp = charGot.GetComponent<TrainState>();
            if (pos < 0 || runCharacterList.Count<=pos)
            {//undefined pos or defined but bigger than count of char, append
                var followTransform = runCharacterList.Any() ? runCharacterList.Last() : null;
                pos = runCharacterList.Count; 
                runCharacterList.Add(joinTrasform);
                trainComp.JoinTrainInit(pos, followTransform);
            }
            else
            {//defined pos and pos has someone, insert
                var toReplaceGuy = runCharacterList.ElementAt(pos);
                trainComp.chasingTarget = toReplaceGuy.GetComponent<TrainState>().chasingTarget;
                toReplaceGuy.GetComponent<TrainState>().chasingTarget = joinTrasform;
                runCharacterList.Insert(pos, joinTrasform);
                trainComp.JoinTrainInit(pos, trainComp.chasingTarget);
            }
            
        }

        /// <summary>
        /// Removeat
        /// </summary>
        /// <param name="userId"></param>
        private void CharLeaveTrain(int index, uint userId)
        {
            var toRmTransform = runCharacterList.ElementAt(index);
            var charGot = toRmTransform.GetComponent<ShowGoPlayer>();
            if (charGot.UserId != userId)
            {
                Debug.Log($"{index} is not user:{userId}");
                return;
            }
            
            //把后面的顺过来
            for (int i = runCharacterList.Count - 1; i > index; i--)
            {
                runCharacterList[i].GetComponent<TrainState>().Index -= 1;
                
            }
            runCharacterList.RemoveAt(index);

            var trainComp = charGot.GetComponent<TrainState>();
            trainComp.OnExit();
            
            //后面那个人跟的人变了
            if (runCharacterList[index] == null)
            {
                Debug.Log($"following guy does not exist, skipping");
                return;
            }
            var trainCompNext = runCharacterList[index].GetComponent<TrainState>();
            var followingId = index - 1;
            trainCompNext.chasingTarget = followingId < 0 ? null : runCharacterList[index - 1];

        }

        /// <summary>
        /// clear
        /// </summary>
        private void EndTrain()
        {
            if (runCharacterList != null && runCharacterList.Count > 0)
            {
                foreach (var item in runCharacterList)
                {
                    var trainComp = item.GetComponent<TrainState>();
                    trainComp.OnExit();
                    // item.GetComponent<CharMain>().TrainStop();
                }
                runCharacterList.Clear();
            }
        }

        #endregion
    }
}