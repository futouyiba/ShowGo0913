using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace ET
{
    public struct RoomInfo
    {
        public string RoomName;
        public int PlayerNumber;
        public bool Locked;
    }
    public struct PlayerInfo
    {
        //這玩意就是可以添加信息的，現在就一個位置
        public Vector3 TargetPos;
    }
    public class PlayerControl : NetworkBehaviour
    {

        public Dictionary<uint, int> ChatRoomPuppyInfos = new Dictionary<uint, int>();//包含所有语音房内玩家的id以及对应的房间号
        public Dictionary<int, RoomInfo> ChatRoomInfos = new Dictionary<int, RoomInfo>();//包含所有语音房号以及房间信息，最后应该不放这里吧……

        public Dictionary<uint, PlayerInfo> PuppyInfos = new Dictionary<uint, PlayerInfo>();//有空改成字典
        public float InterestRange = 1f;

        [SerializeField] private List<Button> EnterChatRoomButtons;//此为权宜之计，加入房间不是这样搞哦

        // Start is called before the first frame update
        void Start()
        {
            //以下为测试所用
            ChatRoomInfos.Add(1, new RoomInfo { RoomName = "Room1", PlayerNumber = 0, Locked = false });
            ChatRoomInfos.Add(2, new RoomInfo { RoomName = "Room2", PlayerNumber = 0, Locked = false });
            ChatRoomInfos.Add(3, new RoomInfo { RoomName = "Room3", PlayerNumber = 0, Locked = true });
            EnterChatRoomButtons[0].onClick.AddListener(() => CmdAddChatRoomPlayerInfo(NetworkClient.localPlayer.GetComponent<NetworkIdentity>(), 1));
            EnterChatRoomButtons[1].onClick.AddListener(() => CmdAddChatRoomPlayerInfo(NetworkClient.localPlayer.GetComponent<NetworkIdentity>(), 2));
            EnterChatRoomButtons[2].onClick.AddListener(() => CmdAddChatRoomPlayerInfo(NetworkClient.localPlayer.GetComponent<NetworkIdentity>(), 3));
            EnterChatRoomButtons[3].onClick.AddListener(() => CmdDelChatRoomPlayerInfo(NetworkClient.localPlayer.GetComponent<NetworkIdentity>().netId));

        }



        [Command(requiresAuthority = false)]
        public void CmdUpdatePlayerInfo()
        {
            PuppyInfos.Clear();
            foreach (var id in NetworkServer.connections.Values)
            {
                PuppyInfos.Add(id.identity.netId, new PlayerInfo { TargetPos = id.identity.transform.position });
            }
            List<uint> LocalPuppyInfos = new List<uint>();
            foreach (KeyValuePair<uint, PlayerInfo> info in PuppyInfos) { LocalPuppyInfos.Add(info.Key); }
            RpcUpdatePlayerInfo(LocalPuppyInfos);
        }

        [ClientRpc]
        public void RpcUpdatePlayerInfo(List<uint> LocalPuppyInfos)
        {

            NetworkClient.localPlayer.GetComponent<Puppy>().LocalPuppyInfos = LocalPuppyInfos;//此中位置信息吊用沒有，太困了先不改了 0907睡醒了忍痛改了

        }

        [Command(requiresAuthority = false)]
        public void CmdUpdateChatRoomPlayerInfo(NetworkIdentity newPlayer)
        {
            List<uint> ids = new List<uint>();
            List<int> rooms = new List<int>();
            List<RoomInfo> infos = new List<RoomInfo>();
            List<int> allrooms = new List<int>();
            foreach (uint i in ChatRoomPuppyInfos.Keys) { ids.Add(i); }
            foreach (int i in ChatRoomPuppyInfos.Values) { rooms.Add(i); }
            foreach (int i in ChatRoomInfos.Keys) { allrooms.Add(i); }
            foreach (RoomInfo i in ChatRoomInfos.Values) { infos.Add(i); }
            TargetUpdateChatRoomPlayerInfo(newPlayer.connectionToClient, ids, rooms, infos, allrooms);
        }

        [TargetRpc]
        public void TargetUpdateChatRoomPlayerInfo(NetworkConnection conn, List<uint> ids, List<int> rooms, List<RoomInfo> infos, List<int> allrooms)
        {
            //新进来的人没法获得之前已经在语音房间里的玩家信息，所以要他妈的管服务器要一下已经在房间里的小狗，妈的参数不让带字典，就他妈的拆成两个列表来传，顺序居然没有被打乱
            Dictionary<uint, int> t = new Dictionary<uint, int>();
            for (int i = 0; i < ids.Count; i++) { t[ids[i]] = rooms[i]; }
            NetworkClient.localPlayer.GetComponent<Puppy>().PlayerControl.ChatRoomPuppyInfos = t;
            Dictionary<int, RoomInfo> t2 = new Dictionary<int, RoomInfo>();
            for (int i = 0; i < allrooms.Count; i++) { t2[allrooms[i]] = infos[i]; }
            NetworkClient.localPlayer.GetComponent<Puppy>().PlayerControl.ChatRoomInfos = t2;

        }

        [Command(requiresAuthority = false)]
        public void CmdAddChatRoomPlayerInfo(NetworkIdentity id, int roomId)
        {
            foreach (uint i in ChatRoomPuppyInfos.Keys)
            {
                if (i == id.netId) { return; }
            }
            RpcAddChatRoomPlayerInfo(id, roomId);//这人进房间了告诉大家，他头上可以出现房间卡片了
        }

        [ClientRpc]
        public void RpcAddChatRoomPlayerInfo(NetworkIdentity id, int roomId)
        {

            ChatRoomPuppyInfos[id.netId] = roomId;
            RoomInfo addRoomPlayer = new RoomInfo { RoomName = ChatRoomInfos[roomId].RoomName, PlayerNumber = ChatRoomInfos[roomId].PlayerNumber + 1, Locked = ChatRoomInfos[roomId].Locked };
            ChatRoomInfos[roomId] = addRoomPlayer;

        }

        [Command(requiresAuthority = false)]
        public void CmdDelChatRoomPlayerInfo(uint id)
        {
            foreach (uint i in ChatRoomPuppyInfos.Keys)
            {
                if (i == id) { RpcDelChatRoomPlayerInfo(id); return; }//找到了让他滚蛋，找不到无事发生
            }

        }

        [ClientRpc]
        public void RpcDelChatRoomPlayerInfo(uint id)
        {

            int roomId = ChatRoomPuppyInfos[id];
            ChatRoomPuppyInfos.Remove(id);
            RoomInfo delRoomPlayer = new RoomInfo { RoomName = ChatRoomInfos[roomId].RoomName, PlayerNumber = ChatRoomInfos[roomId].PlayerNumber - 1, Locked = ChatRoomInfos[roomId].Locked };
            ChatRoomInfos[roomId] = delRoomPlayer;

        }

        [Command(requiresAuthority = false)]
        public void CmdPlayerLeave(uint id)
        {
            RpcPlayerLeave(id);
            Debug.Log("CmdPlayerLeave");

        }

        [ClientRpc]
        public void RpcPlayerLeave(uint id)
        {
            Puppy LeavingPlayer = NetworkClient.localPlayer.GetComponent<Puppy>();
            foreach (uint owner in LeavingPlayer.ActiveCardAndOwner.Keys)
            {
                if (owner == id)
                {
                    LeavingPlayer.RoomCardPoolList[LeavingPlayer.ActiveCardAndOwner[id]].SetActive(false);
                    //LeavingPlayer.ActiveCardAndOwner.Remove(id); //不可以这样哦，只能等协程把它干掉了就是说，我也他奶奶的不知道为什么，但是报错告诉我和协程有关吧，而且注掉就可以正常运行了捏，妈的
                }
                Debug.Log("RoomCardRemoved");
            }
            foreach (uint ap in LeavingPlayer.AvailablePuppys)
            {
                if (ap == id) { LeavingPlayer.AvailablePuppys.Remove(ap); }
            }
            ChatBubbleControl LeavingChatBubbleControl = NetworkClient.localPlayer.GetComponent<ChatBubbleControl>();
            foreach (KeyValuePair<uint, int> abao in LeavingChatBubbleControl.ActiveBubbleAndOwner)
            {
                if (abao.Key == id) { LeavingChatBubbleControl.BubblePoolList[abao.Value].SetActive(false); }
                Debug.Log("BubbleRemoved");

            }

        }

        public List<NetworkIdentity> FindNeighbors_Movemment(NetworkIdentity MovingPuppy, Vector3 targetPos)
        {
            List<NetworkIdentity> Neighbors = new List<NetworkIdentity>();
            Vector3 MovingPuppyPos = PuppyInfos[MovingPuppy.netId].TargetPos;
            foreach (KeyValuePair<uint, PlayerInfo> info in PuppyInfos)
            {
                if (Vector3.Distance(MovingPuppyPos, info.Value.TargetPos) < InterestRange || Vector3.Distance(targetPos, info.Value.TargetPos) < InterestRange)
                {
                    Neighbors.Add(NetworkClient.spawned[info.Key]);
                }
            }
            //Debug.Log(MovingPuppyPos);
            return Neighbors;
        }

        public List<NetworkIdentity> FindNeighbors_Bubble(uint BubbblingPuppy)
        {
            List<NetworkIdentity> Neighbors = new List<NetworkIdentity>();
            Vector3 MovingPuppyPos = PuppyInfos[BubbblingPuppy].TargetPos;
            foreach (KeyValuePair<uint, PlayerInfo> info in PuppyInfos)
            {
                if (Vector3.Distance(MovingPuppyPos, info.Value.TargetPos) < InterestRange)
                {
                    Neighbors.Add(NetworkClient.spawned[info.Key]);
                }
            }
            return Neighbors;
        }

    }
}
