using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;


namespace ET
{
    public class InputCommend : NetworkBehaviour
    {
        //[SerializeField] private StateMachine CameraFSM;


        //[SerializeField] private CameraState CameraState;
        [SerializeField] private TMP_InputField InputField;

        private List<string> CommendList = new List<string>();

        public ChatBubbleControl ChatBubbleControl;

        public DJControl DJControl;

        private bool BubbleAvailable = true;
        private float BubbleCooldownTime = 5f;

        public ShowGo.VcamBehaviour VcamBehaviour;

        // Start is called before the first frame update
        void Start()
        {
            if (isClient)
            {
                //this.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(ChatBubbleControl.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
            }
            InitCommends();
        }

        // Update is called once per frame
        void Update()
        {
            if (!BubbleAvailable)
            {
                BubbleCooldownTime -= Time.deltaTime;
                if (BubbleCooldownTime < 0)
                {
                    BubbleAvailable = true;
                    BubbleCooldownTime = 5f;
                }
            }

        }

        private void InitCommends()
        {
            CommendList.Add("DJ");
            CommendList.Add("Look Around");
            CommendList.Add("Screen");
            CommendList.Add("Audience");
            CommendList.Add("Me");

        }

        public void OnSubmit()
        {
            if (BubbleAvailable)
            {
                if (DJControl.DJPuppy == null || DJControl.DJPuppy.GetComponent<NetworkIdentity>() != NetworkClient.localPlayer)
                {
                    if (InputField.text != "")
                    {
                        if (InputField.text == CommendList[0])
                        {
                            //CameraState.CameraFSMDJ();
                            Debug.Log("很抱歉自己看DJ功能已经他妈的删除了");
                        }
                        else if (InputField.text == CommendList[1])
                        {
                            //CameraState.CameraFSMLookAround();
                            VcamBehaviour.LocalCameraChange(0);
                        }
                        else if (InputField.text == CommendList[2])
                        {
                            //CameraState.CameraFSMScreen();
                            Debug.Log("很抱歉自己看屏幕功能已经他妈的删除了");
                        }
                        else if (InputField.text == CommendList[3])
                        {
                            //CameraState.CameraFSMAudience();
                            VcamBehaviour.LocalCameraChange(5);
                        }
                        else if (InputField.text == CommendList[4])
                        {
                            //CameraState.CameraFSMMe();
                            VcamBehaviour.LocalCameraChange(4);
                        }
                        //else Debug.Log(InputField.text);

                        ChatBubbleControl.CmdSetBubble(InputField.text, NetworkClient.localPlayer.GetComponent<NetworkIdentity>().netId);
                        BubbleAvailable = false;
                    }
                }
                else
                {
                    if (InputField.text != "")
                    {
                        if (InputField.text == CommendList[0])
                        {
                            //CameraState.CmdCameraFSMDJ();
                            VcamBehaviour.syncedVcamIndex = 1;
                        }
                        else if (InputField.text == CommendList[1])
                        {
                            //CameraState.CmdCameraFSMLookAround();
                            VcamBehaviour.syncedVcamIndex = 0;
                        }
                        else if (InputField.text == CommendList[2])
                        {
                            //CameraState.CmdCameraFSMScreen();
                            VcamBehaviour.syncedVcamIndex = 2;
                        }
                        else if (InputField.text == CommendList[3])
                        {
                            //NetworkIdentity TargetPuppy = NetworkClient.spawned[NetworkClient.localPlayer.GetComponent<Puppy>().LocalPuppyInfos[Random.Range(0, PuppyNum)]];
                            //CameraState.CmdCameraFSMDJChosenAudience(TargetPuppy);
                            VcamBehaviour.syncedVcamIndex = 3;
                        }
                        else if (InputField.text == CommendList[4])
                        {
                            //CameraState.CmdCameraFSMMe();
                        }
                        //else Debug.Log(InputField.text);

                        ChatBubbleControl.CmdSetBubble(InputField.text, NetworkClient.localPlayer.GetComponent<NetworkIdentity>().netId);
                        BubbleAvailable = false;
                    }
                }



                InputField.text = "";
            }
            else
            {
                Debug.Log("Slow down!");
            }

        }

    }
}
