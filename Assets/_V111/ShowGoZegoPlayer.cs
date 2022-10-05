using UnityEngine;
using UnityEngine.UI;
using ZEGO;

namespace ET._V111
{
    public class ShowGoZegoPlayer : MonoBehaviour
    {
        private ZegoExpressEngine engine;
        public string zegoAppSign = "cb6616491e3edc0e407ebc804f6f6dc124ea200ed4953ff6e90121f500ba0acc";
        public uint zegoAppId = 1666228805;
        private static string playStreamID;
        private const string CDNStreamerAddr = "rtmp://streamer.zanyule.cn/live/simple";


        private void Start()
        {
            CreateEngine();
        }

        private void OnApplicationQuit()
        {
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
        }

        public void DestroyEngine()
        {
            ZegoUtilHelper.PrintLogToView("DestroyEngine");
            ZegoExpressEngine.DestroyEngine();
            engine = null;
        }

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