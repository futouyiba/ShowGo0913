using System;
using System.Collections.Concurrent;
using AOT;
using static ZEGO.ZegoExpressEngineImpl;
using static ZEGO.ZegoImplCallChangeUtil;
using static ZEGO.ZegoCallBackChangeUtil;
using System.Threading;

namespace ZEGO
{
    public class ZegoRangeAudioImpl : ZegoRangeAudio
    {
        private zego_range_audio_instance_index index;
        public ZegoRangeAudioImpl(zego_range_audio_instance_index instanceIndex)
        {
            index = instanceIndex;
        }

        public override void SetAudioReceiveRange(float range)
        {
            ZegoExpressEngineImpl.RangeAudioSetAudioReceiveRange(range, index);
        }

        public override void UpdateSelfPosition(float[] position, float[] axisForward, float[] axisRight, float[] axisUp)
        {
            ZegoExpressEngineImpl.RangeAudioUpdateSelfPosition(position, axisForward, axisRight, axisUp, index);
        }

        public override void UpdateAudioSource(string userID, float[] position)
        {
            ZegoExpressEngineImpl.RangeAudioUpdateAudioSource(userID, position, index);
        }

        public override void EnableSpatializer(bool enable)
        {
            ZegoExpressEngineImpl.RangeAudioEnableSpatializer(enable, index);
        }

        public override void EnableMicrophone(bool enable)
        {
            ZegoExpressEngineImpl.RangeAudioEnableMicrophone(enable, index);
        }

        public override void EnableSpeaker(bool enable)
        {
            ZegoExpressEngineImpl.RangeAudioEnableSpeaker(enable, index);
        }

        public override void SetRangeAudioMode(ZegoRangeAudioMode mode)
        {
            ZegoExpressEngineImpl.RangeAudioSetRangeAudioMode(mode, index);
        }

        public override void SetTeamID(string teamID)
        {
            ZegoExpressEngineImpl.RangeAudioSetTeamID(teamID, index);
        }

        public override void MuteUser(string userID, bool mute)
        {
            ZegoExpressEngineImpl.RangeAudioMuteUser(userID, mute, index);
        }

        private static ZegoRangeAudio GetRangeAudioFromIndex(zego_range_audio_instance_index index)
        {
            ZegoRangeAudio zegoRangeAudio = null;
            rangeAudioAndIndex.TryGetValue(index, out zegoRangeAudio);
            if (zegoRangeAudio == null)
            {
                throw new Exception("GetRangeAudioFromIndex null");
            }
            return zegoRangeAudio;
        }

        [MonoPInvokeCallback(typeof(IExpressRangeAudioInternal.zego_on_range_audio_microphone_state_update))]
        public static void zego_on_range_audio_microphone_state_update(zego_range_audio_microphone_state state, int error_code, zego_range_audio_instance_index instance_index, IntPtr user_context)
        {
            var zegoRangeAudio = GetRangeAudioFromIndex(instance_index);
            if (zegoRangeAudio.onRangeAudioMicrophoneStateUpdate != null)
            {
                callBackQueue.PostAsynAction(() =>
                {
                    zegoRangeAudio?.onRangeAudioMicrophoneStateUpdate?.Invoke(zegoRangeAudio, (ZegoRangeAudioMicrophoneState)state, error_code);
                });
            }
        }
    }
}