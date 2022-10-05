using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace ShowGo
{
    public class FireworkBehaviour : NetworkBehaviour
    {
        public static FireworkBehaviour instance;

        public GameObject FireworkPool;
        public GameObject FireworkPrefab;
        public List<GameObject> FireworkList;

        public Dictionary<uint, int> ActiveFireworkAndOwner;

        public bool isFireworking=false;


        public override void OnStartClient()
        {
            base.OnStartClient();
            instance = this;

            ActiveFireworkAndOwner = new Dictionary<uint, int>();
            InitFirework(10);

        }

        /*20220925 下面是送礼物呲花功能，我写的狗屎一样*/
        private void InitFirework(int maxFireworkNum)
        {
            FireworkPool = new GameObject("FireworkPool");
            FireworkPool.transform.position += new Vector3(0, 0, -100);
            for (int i = 0; i < maxFireworkNum; i++)
            {
                GameObject b;
                b = Instantiate(FireworkPrefab);
                FireworkList.Add(b);
                b.transform.SetParent(FireworkPool.transform);
                b.transform.localPosition = new Vector3(0, 0, 0);
            }
        }

        public int FindFreeFirework(uint id)
        {
            if (ActiveFireworkAndOwner.TryGetValue(id, out int index)) { return index; }//玩家已经有属于他的还没灭的烟花了，就用这个
            for (int i = 0; i < FireworkList.Count; i++)//给他找一个未婚烟花
            {
                if (!ActiveFireworkAndOwner.ContainsValue(i)) { return i; }
            }

            return -1;
        }

        public void OnGiftButtonDown()
        {
            CmdSetFirework(SceneShowgoMgr.Instance.Me.UserId);
        }
        public void OnGiftButtonUp()
        {
            CmdEndFirework(SceneShowgoMgr.Instance.Me.UserId);
        }

        [Command(requiresAuthority = false)]
        public void CmdSetFirework(uint id)
        {
            RpcSetFirework(id);
        }

        [ClientRpc]
        public void RpcSetFirework(uint id)
        {
            if (id == SceneShowgoMgr.Instance.Me.UserId) {
                isFireworking = true;
                VcamBehaviour.instance.LocalCameraChange(4);
            }
            
            int fireworkindex = FindFreeFirework(id);
            if (fireworkindex == -1) { return; }
            Firework firework = FireworkList[fireworkindex].GetComponent<Firework>();
            //firework.transform.SetParent(NetworkClient.spawned[id].transform);
            firework.gameObject.transform.position = SceneShowgoMgr.Instance.GetUser(id).transform.Find("PlayerTop").position;
            firework.StartFirework();
            if (!ActiveFireworkAndOwner.ContainsKey(id)) { ActiveFireworkAndOwner.Add(id, fireworkindex); }
        }

        [Command(requiresAuthority = false)]
        public void CmdEndFirework(uint id)
        {
            RpcEndFirework(id);
        }

        [ClientRpc]
        public void RpcEndFirework(uint id)
        {
            if (id == SceneShowgoMgr.Instance.Me.UserId) {
                isFireworking = false;
                if(VcamBehaviour.instance.PreviousVC == VcamBehaviour.instance.vCams[4]) VcamBehaviour.instance.LocalCameraChange(0);
            }

            VcamBehaviour.instance.LocalCameraChange(0);
            int fireworkindex;
            if (ActiveFireworkAndOwner.TryGetValue(id, out fireworkindex))
            {
                Firework firework = FireworkList[fireworkindex].GetComponent<Firework>();
                //firework.transform.SetParent(NetworkClient.localPlayer.GetComponent<Puppy>().FireworkPool.transform);
                firework.GetComponent<Firework>().EndFirework();
                StartCoroutine(FreeFirework(id));
                //ActiveFireworkAndOwner.Remove(id);
            }

        }

        IEnumerator FreeFirework(uint id)//用户长时间不放烟花他的烟花就重获自由可以移情别恋了，为啥这样，因为如果烟花可以立刻移情别恋，它会带走这个用户花钱放出的纸屑，就好像，你冷落妻子太久她跟人跑了是可以的，但是如果你俩打得火热她就跑了她就是过错方了，或者说抢走她的狗头人是过错方，毕竟在我创造的世道下烟花只是任人撷取的可悲玩物罢了（？
        {
            yield return new WaitForSeconds(5.3f);//等待时间是手测出来的
            if (ActiveFireworkAndOwner.ContainsKey(id) && !FireworkList[ActiveFireworkAndOwner[id]].GetComponent<Firework>().FireworkParticle.IsAlive()) { ActiveFireworkAndOwner.Remove(id); Debug.Log("我自由辣！！"); }
        }
    }

}
