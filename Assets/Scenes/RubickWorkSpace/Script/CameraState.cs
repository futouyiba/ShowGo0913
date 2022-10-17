using UnityEngine;
using Cinemachine;
// using Unity.VisualScripting;
using Mirror;

namespace ET
{
    public class CameraState : NetworkBehaviour
    {
        [SerializeField] private Bolt.StateMachine CameraFSM;

        public CinemachineVirtualCamera LookAround;
        [SerializeField]private float LookAroundSpeed = 0.1f;
        private bool LookAroundDirection = true;

        public CinemachineVirtualCamera DJ;
        public CinemachineVirtualCamera Screen;
        public CinemachineVirtualCamera Audience;
        public CinemachineVirtualCamera Me;
        public CinemachineVirtualCamera DJChosenAudience;


        [SerializeField] private CinemachineVirtualCamera PreviousVC;


        private NetworkIdentity RandomAudienceId;

        //private DJControl dJControl;

        //[SerializeField] private GameObject PuppyMe;


        // Start is called before the first frame update
        void Start()
        {
            //LookAround.Priority += 1;
            //PreviousVC = LookAround;
            //CameraFSM.TriggerUnityEvent("ClubCameraStarted");
            //CameraFSM.TriggerUnityEvent("LookAround");
            //if(isClient) PuppyMe = NetworkClient.localPlayer.gameObject;
            //RandomAudienceId = this.GetComponent<NetworkIdentity>();
            //dJControl = FindObjectOfType<DJControl>();
        }

        // Update is called once per frame
        void Update()
        {
            //CameraLookAroundMovement();
        }

        public void InitCamera()
        {
            //LookAround.Priority += 1;
            PreviousVC = LookAround;
            CameraFSM.TriggerUnityEvent("ClubCameraStarted");
            CameraFSM.TriggerUnityEvent("LookAround");
        }

        public void CameraLookAroundMovement()
        {
            if (LookAround.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition < 0)
            {
                LookAroundDirection = !LookAroundDirection;
                LookAround.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0;
            }


            if (LookAround.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition > 1)
            {
                LookAroundDirection = !LookAroundDirection;
                LookAround.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 1;
            }


            if (LookAroundDirection)
                LookAround.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition +=
                    Time.deltaTime * LookAroundSpeed;
            else LookAround.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition -= Time.deltaTime * LookAroundSpeed;
        }

        public void GoLookAround()
        {
            if(PreviousVC!= LookAround)
            {
                //CameraFSM.TriggerUnityEvent("CameraStateChanged");
                //CameraFSM.TriggerUnityEvent("LookAround");
                PreviousVC.Priority -= 1;
                LookAround.Priority += 1;
                PreviousVC = LookAround;
            }
            
        }

        public void GoDJ()
        {
            if (PreviousVC != DJ)
            {
                PreviousVC.Priority -= 1;
                DJ.Priority += 1;
                PreviousVC = DJ;
            }
        }

        public void GoScreen()
        {
            if (PreviousVC != Screen)
            {
                
                PreviousVC.Priority -= 1;
                Screen.Priority += 1;
                PreviousVC = Screen;
            }
        }
        public void GoAudience()
        {
            if (PreviousVC != Audience)
            {
                int PuppyNum = NetworkClient.localPlayer.GetComponent<Puppy>().LocalPuppyInfos.Count;
                Transform TargetPuppy = NetworkClient.spawned[NetworkClient.localPlayer.GetComponent<Puppy>().LocalPuppyInfos[Random.Range(0, PuppyNum)]].transform;
                //Debug.Log(TargetPuppy.gameObject.GetComponent<NetworkIdentity>());
                Audience.LookAt = TargetPuppy;
                Audience.Follow = TargetPuppy;
                //Audience.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = Random.Range(0, 11) * 0.1f;
                PreviousVC.Priority -= 1;
                Audience.Priority += 1;
                PreviousVC = Audience;
            }
            else
            {
                //Audience.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = Random.Range(0, 11) * 0.1f;
                int PuppyNum = NetworkClient.localPlayer.GetComponent<Puppy>().LocalPuppyInfos.Count;
                Transform TargetPuppy = NetworkClient.spawned[NetworkClient.localPlayer.GetComponent<Puppy>().LocalPuppyInfos[Random.Range(0, PuppyNum)]].transform;
                Audience.LookAt = TargetPuppy;
                Audience.Follow = TargetPuppy;
            }
        }

        public void GoDJChosenAudience()
        {
            if (PreviousVC != DJChosenAudience)
            {
                Transform TargetPuppy = RandomAudienceId.transform;
                DJChosenAudience.LookAt = TargetPuppy;
                DJChosenAudience.Follow = TargetPuppy;
                PreviousVC.Priority -= 1;
                DJChosenAudience.Priority += 1;
                PreviousVC = DJChosenAudience;
            }
            else
            {
                Transform TargetPuppy = RandomAudienceId.transform;
                DJChosenAudience.LookAt = TargetPuppy;
                DJChosenAudience.Follow = TargetPuppy;
            }
        }

        public void GoMe()
        {
            if (PreviousVC != Me)
            {
                
                Me.m_LookAt = NetworkClient.localPlayer.gameObject.transform;
                Me.m_Follow = NetworkClient.localPlayer.gameObject.transform;
                PreviousVC.Priority -= 1;
                Me.Priority += 1;
                PreviousVC = Me;
            }
        }


        [Command(requiresAuthority =false)]
        public void CmdCameraFSMLookAround()
        {
            RpcCameraFSMLookAround();
        }
        [Command(requiresAuthority = false)]
        public void CmdCameraFSMDJ()
        {
            RpcCameraFSMDJ();
        }
        [Command(requiresAuthority = false)]
        public void CmdCameraFSMScreen()
        {
            RpcCameraFSMScreen();
        }
        [Command(requiresAuthority = false)]
        public void CmdCameraFSMAudience()
        {
            RpcCameraFSMAudience();
        }
        [Command(requiresAuthority = false)]
        public void CmdCameraFSMDJChosenAudience(NetworkIdentity AudienceId)
        {
            RpcCameraFSMDJChosenAudience(AudienceId);
        }
        [Command(requiresAuthority = false)]
        public void CmdCameraFSMMe()
        {
            RpcCameraFSMMe();
        }

        [ClientRpc]
        public void RpcCameraFSMLookAround()
        {
            if (PreviousVC != Me && PreviousVC != Audience)
            {
                CameraFSM.TriggerUnityEvent("CameraStateChanged");
                CameraFSM.TriggerUnityEvent("LookAround");
            }
            
        }
        [ClientRpc]
        public void RpcCameraFSMDJ()
        {
            if (PreviousVC != Me && PreviousVC != Audience)
            {
                CameraFSM.TriggerUnityEvent("CameraStateChanged");
                CameraFSM.TriggerUnityEvent("DJ");
            }

        }
        [ClientRpc]
        public void RpcCameraFSMScreen()
        {
            if (PreviousVC != Me && PreviousVC != Audience)
            {
                CameraFSM.TriggerUnityEvent("CameraStateChanged");
                CameraFSM.TriggerUnityEvent("Screen");
            }

        }
        [ClientRpc]
        public void RpcCameraFSMAudience()
        {
            if (PreviousVC != Me && PreviousVC != Audience)
            {
                CameraFSM.TriggerUnityEvent("CameraStateChanged");
                CameraFSM.TriggerUnityEvent("Audience");
            }

        }
        [ClientRpc]
        public void RpcCameraFSMDJChosenAudience(NetworkIdentity AudienceId)
        {
            RandomAudienceId = AudienceId;
            if (PreviousVC != Me && PreviousVC != Audience)
            {
                CameraFSM.TriggerUnityEvent("CameraStateChanged");
                CameraFSM.TriggerUnityEvent("DJChosenAudience");
            }
        }

        [ClientRpc]
        public void RpcCameraFSMMe()
        {
            if (PreviousVC != Me && PreviousVC != Audience)
            {
                CameraFSM.TriggerUnityEvent("CameraStateChanged");
                CameraFSM.TriggerUnityEvent("Me");
            }

        }

        public void CameraFSMLookAround()
        {
            CameraFSM.TriggerUnityEvent("CameraStateChanged");
            CameraFSM.TriggerUnityEvent("LookAround");
        }
        public void CameraFSMDJ()
        {
            CameraFSM.TriggerUnityEvent("CameraStateChanged");
            CameraFSM.TriggerUnityEvent("DJ");
        }
        public void CameraFSMScreen()
        {
            CameraFSM.TriggerUnityEvent("CameraStateChanged");
            CameraFSM.TriggerUnityEvent("Screen");
        }
        public void CameraFSMAudience()
        {
            CameraFSM.TriggerUnityEvent("CameraStateChanged");
            CameraFSM.TriggerUnityEvent("Audience");
        }
        public void CameraFSMMe()
        {
            CameraFSM.TriggerUnityEvent("CameraStateChanged");
            CameraFSM.TriggerUnityEvent("Me");
        }
    }
}
