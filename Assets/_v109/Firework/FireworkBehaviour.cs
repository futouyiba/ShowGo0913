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

        /*20220925 �������������ڻ����ܣ���д�Ĺ�ʺһ��*/
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
            if (ActiveFireworkAndOwner.TryGetValue(id, out int index)) { return index; }//����Ѿ����������Ļ�û����̻��ˣ��������
            for (int i = 0; i < FireworkList.Count; i++)//������һ��δ���̻�
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

        IEnumerator FreeFirework(uint id)//�û���ʱ�䲻���̻������̻����ػ����ɿ�����������ˣ�Ϊɶ��������Ϊ����̻�����������������������������û���Ǯ�ų���ֽм���ͺ�������������̫�������������ǿ��Եģ��������������û����������������ǹ����ˣ�����˵�������Ĺ�ͷ���ǹ������Ͼ����Ҵ�����������̻�ֻ������ߢȡ�Ŀɱ�������ˣ���
        {
            yield return new WaitForSeconds(5.3f);//�ȴ�ʱ�����ֲ������
            if (ActiveFireworkAndOwner.ContainsKey(id) && !FireworkList[ActiveFireworkAndOwner[id]].GetComponent<Firework>().FireworkParticle.IsAlive()) { ActiveFireworkAndOwner.Remove(id); Debug.Log("������������"); }
        }
    }

}
