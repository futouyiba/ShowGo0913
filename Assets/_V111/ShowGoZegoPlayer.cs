using System;
using DG.Tweening;
using ET.Utility;
using Ludiq;
using ShowGo;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility;
using ZEGO;
using Random = UnityEngine.Random;

namespace ET._V111
{
    public class ShowGoZegoPlayer : SingletonMono<ShowGoZegoPlayer>
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
        private float _volume = 0;
        private const string CDNStreamerAddr = "rtmp://streamer.zanyule.cn/live/simple";


        private async void Start()
        {
            CreateEngine();
            InitAll();
            LoginRoom();

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

            engine.onPlayerStateUpdate += (streamID, state, errorCode, extendedData) =>
            {
                switch (state)
                {
                    case ZegoPlayerState.Playing:
                        engine.StartAudioSpectrumMonitor();
                        engine.StartSoundLevelMonitor();
                        break;
                }
            };

            engine.onCapturedAudioSpectrumUpdate += (float[] audioSpectrum) =>
            {
                AudioVisualizer.AudioSampler.instance.UpdateSample(audioSpectrum);
            };

            engine.onCapturedSoundLevelUpdate += (float soundLevel) =>
            {
                AudioVisualizer.AudioSampler.instance.UpdateSoundLevel(soundLevel);
            };
        }

        private void InitAll()
        {
            roomId = "0010";
            publishStreamID = "0010";
            playStreamID = "0010";

            if (remoteVideoPlane != null)
            {
                if (remoteVideoSurface ==
                    null) //Avoid repeated Add Component causing strange problems such as video freeze
                {
                    remoteVideoSurface = remoteVideoPlane.AddComponent<ScreenVideoDisplay>();
                    remoteVideoSurface.SetPlayVideoInfo(playStreamID);
                    remoteVideoSurface.SetVideoSource(engine);
                }
            }
        }

        void LoginRoom()
        {
            ZegoUser user = new ZegoUser(Application.identifier + Random.Range(0, 65535))
            {
                userName = Application.identifier + Random.Range(0, 65535),
            };
            ZegoUtilHelper.PrintLogToView(
                $"LoginRoom, roomID:{roomID}, userID:{user.userID}, userName:{user.userName}");
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
#if (UNITY_ANDROID|| UNITY_IPHONE ) && !UNITY_EDITOR

            if (engine != null)
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
            _volume = 0f;
            engine.SetPlayVolume(playStreamID, (int)_volume);
        }
        
        public void SetSoundVolume(int targetVolume,
         bool bInstant, float duration)
        {
            if (bInstant)
            {
                _volume = targetVolume;
                engine.SetPlayVolume(playStreamID, (int)targetVolume);
            }
            else
            {
                DOTween.To(() => _volume, vol =>
                {
                    _volume = vol;
                    engine.SetPlayVolume(playStreamID, (int)vol);
                }, targetVolume, duration);
            }
        }

        // private void Setter(float pnewvalue)
        // {
        //     _volume = pnewvalue;
        //     engine.SetPlayVolume(playStreamID, pnewvalue);
        // }

        public void StopPlaying()
        {
            ZegoUtilHelper.PrintLogToView($"StopPlayingStream, streamID:{playStreamID}");
            engine.StopPlayingStream(playStreamID);
        }

        [Button("ToggleLobbySound")]
        public void ToggleLobbySound()
        {
            var targetVolume = 100;
            if (_volume < 1f)
            {
                targetVolume = 100;
            }
            else
            {
                targetVolume = 0;
            }

            ContinentalMessenger.Instance.Native2UnityMsg( ShowGo.NativeUtility.MakeOp( (new SetClubSound()
            {
                bInstant = true,
                targetVolume = targetVolume,
            })));
        }
    }
}