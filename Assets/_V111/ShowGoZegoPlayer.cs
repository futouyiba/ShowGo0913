using System;
using Ludiq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using ZEGO;
using Random = UnityEngine.Random;

namespace ET._V111
{
    public class ShowGoZegoPlayer : MonoBehaviour
    {
        private ZegoExpressEngine engine;
        public string zegoAppSign = "cb6616491e3edc0e407ebc804f6f6dc124ea200ed4953ff6e90121f500ba0acc";
        public uint zegoAppId = 1666228805;
        private static string playStreamID;
        private string roomId;
        [SerializeField] private string
            publishStreamID;

        [SerializeField] private Renderer remoteVideoPlane;
        [SerializeField] private ScreenVideoDisplay remoteVideoSurface;
        [SerializeField] private string roomID = "0010";
        private const string CDNStreamerAddr = "rtmp://streamer.zanyule.cn/live/simple";


        private async void Start()
        {
            CreateEngine();
            // engine.onEngineStateUpdate += state =>
            // {
                // if (state == ZegoEngineState.Start)
                {
                    InitAll();
                    LoginRoom();

                }
            // };
            engine.onRoomStateChanged += async (id, reason, code, data) =>
            {
                switch (reason)
                {
                    case ZegoRoomStateChangedReason.Logined:
                        StartPlaying();
                        break;
                    case ZegoRoomStateChangedReason.Reconnected:
                        StartPlaying();
                        break;
                    case ZegoRoomStateChangedReason.KickOut:
                        StopPlaying();
                        break;
                    case ZegoRoomStateChangedReason.Logout:
                        try
                        {
                            StopPlaying();
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                        break;
                    case ZegoRoomStateChangedReason.LoginFailed:
                        LoginRoom();
                        break;
                    case ZegoRoomStateChangedReason.ReconnectFailed:
                        await TimerComponent.Instance.WaitAsync(3000);
                        LoginRoom();
                        break;
                }
            };
        }

        private void InitAll()
        {
            roomId = "0010";
            publishStreamID = "0010";
            playStreamID = "0010";

            if (remoteVideoPlane != null)
            {
                if (remoteVideoSurface == null)//Avoid repeated Add Component causing strange problems such as video freeze
                {
                    remoteVideoSurface = remoteVideoPlane.AddComponent<ScreenVideoDisplay>();
                    remoteVideoSurface.SetPlayVideoInfo(playStreamID);
                    remoteVideoSurface.SetVideoSource(engine);
                }
            }
        }
        
        void LoginRoom()
        {
            ZegoUser user = new ZegoUser(Application.identifier + Random.Range(0,65535))
            {
                userName = Application.identifier + Random.Range(0,65535),
            };
            ZegoUtilHelper.PrintLogToView($"LoginRoom, roomID:{roomID}, userID:{user.userID}, userName:{user.userName}");
            engine.LoginRoom(roomID, user);
        
        }

        private void OnDestroy()
        {
            engine.LogoutRoom();
            DestroyEngine();
        }

        private void CreateEngine()
        {
            if (engine != null) return;
            ZegoEngineProfile profile = new ZegoEngineProfile()
            {
                appSign = zegoAppSign,
                appID = zegoAppId,
                scenario = ZegoScenario.General,
            };
            ZegoUtilHelper.PrintLogToView(
                $"CreateEngine, appID:{profile.appID}, appSign:{profile.appSign}, scenario:{profile.scenario}");
            engine = ZegoExpressEngine.CreateEngine(profile);
#if (UNITY_ANDROID||  UNITY_IPHONE ) && !UNITY_EDITOR

            if (engine!=null)
            {
                engine.SetAppOrientation(ZegoOrientation.ZegoOrientation_0);
            }
            #endif
        }
        
        [Button("logZegoVersion")]
        void LogZegoVersion()
        {
            ZegoExpressEngineImpl.GetVersion();
        }

        public void DestroyEngine()
        {
            ZegoUtilHelper.PrintLogToView("DestroyEngine");
            ZegoExpressEngine.DestroyEngine();
            engine = null;
        }

        [Button("StartPlaying")]
        public void StartPlaying()
        {
            ZegoUtilHelper.PrintLogToView("StartPlayingStream");
            ZegoPlayerConfig config = new ZegoPlayerConfig
            {
                cdnConfig = new ZegoCDNConfig
                {
                    url = CDNStreamerAddr
                }
            };
            ;
            config.resourceMode = ZegoStreamResourceMode.OnlyCDN;
            playStreamID = "0010";
            engine.StartPlayingStream(playStreamID, config);
        }

        public void StopPlaying()
        {
            ZegoUtilHelper.PrintLogToView($"StopPlayingStream, streamID:{playStreamID}");
            engine.StopPlayingStream(playStreamID);
        }
    }




}