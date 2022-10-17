using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

namespace ShowGo
{
    /// <summary>
    /// 我们
    /// </summary>
    public partial class VcamBehaviour : NetworkBehaviour
    {
        public static VcamBehaviour instance;

        /// <summary>
        /// there are a list of vcams
        /// element 0 is for focusing, client uses it. server should never set syncedVcamIndex to 0.
        /// typical priority should be -100/+100.
        /// 1-n are driven by server. typical priortiy should
        /// </summary>
        public List<CinemachineVirtualCamera> vCams;

        public CinemachineVirtualCamera PreviousVC;
        [SerializeField] private float DJTimer = 0;
        [SerializeField] private float LocalTimer = 0;

        [SyncVar] public uint syncedTargetPuppy;
        public uint LocalTargetPuppy;//这已然是无用的东西了呢

        /// <summary>
        /// notice that server should never set this to 0
        /// </summary>
        [SyncVar(hook = "OnSyncedVCamChanged")] public int syncedVcamIndex;

        public override void OnStartClient()
        {
            base.OnStartClient();
            instance = this;
        }
        private void Start()
        {
            PreviousVC = vCams[0];
        }

        void OnSyncedVCamChanged(int oldIndex, int newIndex)
        {
            // todo add vcam client operations: adjust priorities @labike 
            if (0 < oldIndex && oldIndex < 4&& vCams[oldIndex].Priority==20) vCams[oldIndex].Priority -= 10;//这一句的条件是可能(为什么等于二十？因为我不得不在切default那边把老镜头优先级调回去，那么当一个用户切到了default镜头，为了不重复调整优先级)
            if (0 < newIndex && newIndex < 4) vCams[newIndex].Priority += 10;//这一句的条件实际上是必然的，syncedVcamIndex只能等于123因为零是default45是仅本地操作；后来又考虑到还可能是0因为自动回到（或者也可以手动？）
            if (newIndex == 3)//DJ（Server）roll一个人
            {
                //if (isServer) 
                //{
                //    int PuppyNum = NetworkClient.localPlayer.GetComponent<ET.Puppy>().LocalPuppyInfos.Count;
                //    syncedTargetPuppy = NetworkClient.localPlayer.GetComponent<ET.Puppy>().LocalPuppyInfos[UnityEngine.Random.Range(0, PuppyNum)];
                //}//【20220901】roll人功能统一转至ShowGoPlayer中！！！
                vCams[newIndex].LookAt = SceneShowgoMgr.Instance.GetUser(syncedTargetPuppy).transform;
                vCams[newIndex].Follow = SceneShowgoMgr.Instance.GetUser(syncedTargetPuppy).transform;
            }
            if (newIndex != 0 && isServer)
            {
                //DJTimer = 10f;
                StartCoroutine(CameraStateEnd(newIndex,DJTimer));
            }
            PreviousVC = FindPreviousVC();//PreviousVC仅作显示完全可改为CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();但是好像有点长
        }
        IEnumerator CameraStateEnd(int index,float time)
        {
            if (3 < index && index < 6) { yield return new WaitForSeconds(time); }
            if (0 < index && index < 4) { yield return new WaitForSeconds(time); }
            if (DJTimer <= 0 && vCams[index].Priority == 20)
            {
                //vCams[index].Priority -= 10;//意思就是说，每切换一次镜头就重置一次计时器，如果计时器用完了且当前镜头和协程要降低优先级的镜头是同一个，就说明这个镜头要被自动降级了因为他呆的时间太久了；那么就是说我们去手动切镜头的时候还是要给老镜头降级的，但是lookaround应该是个例外，它优先级一直不变
                syncedVcamIndex = 0;
            }
            if (LocalTimer <= 0 && vCams[index].Priority == 30)
            {
                if (PreviousVC == vCams[5]) { FloatingUiConnector.Instance.ClosePuppyCard(); }
                if(!FireworkBehaviour.instance.isFireworking&& !SceneShowgoMgr.Instance.Me.isVoluntarilyMoving && PreviousVC == vCams[4])vCams[index].Priority -= 20;//意思就是说，每切换一次镜头就重置一次计时器，如果计时器用完了且当前镜头和协程要降低优先级的镜头是同一个，就说明这个镜头要被自动降级了因为他呆的时间太久了；那么就是说我们去手动切镜头的时候还是要给老镜头降级的，但是lookaround应该是个例外，它优先级一直不变
            }
            PreviousVC = FindPreviousVC();
        }
        [Command(requiresAuthority =false)]
        public void CmdSeverCameraChange(int index, float time)
        {
            DJTimer = time;
            if (isServer) { syncedVcamIndex = 0; syncedVcamIndex = index; }
        }

        public async void LocalCameraChange(int index,float time=20f)
        {
            PreviousVC = FindPreviousVC();
            if (PreviousVC == vCams[5]) { FloatingUiConnector.Instance.ClosePuppyCard(); }
            if (3 < vCams.IndexOf(PreviousVC) && vCams.IndexOf(PreviousVC) < 6) { PreviousVC.Priority -= 20; }
            if (0< vCams.IndexOf(PreviousVC) && vCams.IndexOf(PreviousVC) <4 && index==0) { PreviousVC.Priority -= 10; }
            if (3 < index && index < 6) { vCams[index].Priority += 20; }

            if (index == 4)
            {
                do
                {
                    await TimerComponent.Instance.WaitAsync(1);
                } while (SceneShowgoMgr.Instance.Me == null );
                vCams[index].LookAt = SceneShowgoMgr.Instance.Me.transform;
                vCams[index].Follow = SceneShowgoMgr.Instance.Me.transform;
            }
            if (index == 5)
            {
                //int PuppyNum = NetworkClient.localPlayer.GetComponent<ET.Puppy>().LocalPuppyInfos.Count;
                //LocalTargetPuppy = NetworkClient.localPlayer.GetComponent<ET.Puppy>().LocalPuppyInfos[UnityEngine.Random.Range(0, PuppyNum)];//【20220901】roll人功能统一转至ShowGoPlayer中！！！
                vCams[index].LookAt = SceneShowgoMgr.Instance.GetUser(LocalTargetPuppy).transform;
                vCams[index].Follow = SceneShowgoMgr.Instance.GetUser(LocalTargetPuppy).transform;
            }
            PreviousVC = vCams[index];
            if (index != 0)
            {
                LocalTimer = time;
                StartCoroutine(CameraStateEnd(index,time));
            }
            
        }
        private CinemachineVirtualCamera FindPreviousVC()//这玩意最好删掉没啥吊用；更正，应该是不能删，没想到更好的方法
        {
            //CinemachineVirtualCamera pCam= CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
            CinemachineVirtualCamera pCam=vCams[0];
            foreach (CinemachineVirtualCamera c in vCams)
            {
                if (c.Priority > pCam.Priority) { pCam = c; }
            }
            return pCam;
        }
    }

    /// <summary>
    /// server part
    /// </summary>
    public partial class VcamBehaviour
    {
        void SwitchToVcam(CinemachineVirtualCamera newVcam)
        {
            var index = vCams.FindIndex(vc => vc == newVcam);
            SwitchToVcamIndex(index);
        }

        void SwitchToVcamIndex(int index)
        {
            syncedVcamIndex = index;
        }

        private void Update()
        {
            //SyncedCamSwitchLogicOnUpdate();
            if (isServer && (0 < syncedVcamIndex && syncedVcamIndex < 4) && DJTimer >0) { DJTimer -= Time.deltaTime; }
            if (3 < vCams.IndexOf(PreviousVC) && vCams.IndexOf(PreviousVC) < 6 && LocalTimer > 0) { LocalTimer -= Time.deltaTime; }
            CameraLookAroundMovement();
        }

        private bool LookAroundDirection = true;
        private float LookAroundSpeed = 0.1f;
        public void CameraLookAroundMovement()
        {
            if (PreviousVC == vCams[0])
            {
                if (vCams[0].GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition < 0)
                {
                    LookAroundDirection = !LookAroundDirection;
                    vCams[0].GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0;
                }


                if (vCams[0].GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition > 1)
                {
                    LookAroundDirection = !LookAroundDirection;
                    vCams[0].GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1;
                }


                if (LookAroundDirection)
                    vCams[0].GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition +=
                        Time.deltaTime * LookAroundSpeed;
                else vCams[0].GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition -= Time.deltaTime * LookAroundSpeed;
            }
        }

        [Server]
        void SyncedCamSwitchLogicOnUpdate()
        {
            // todo add actual switch logic here(no vcam priorities and vcam set active here) @labike
        }
    }

}