
namespace ZEGO
{

    public class IZegoRangeAudioHandler
    {
        /**
         * Range audio microphone state callback.
         *
         * Available since: 2.11.0
         * Description: The status change notification of the microphone, starting to send audio is an asynchronous process, and the state switching in the middle is called back through this function.
         * When to Trigger: After the [enableMicrophone] function.
         * Caution: It must be monitored before the [enableMicrophone] function is called.
         *
         * @param rangeAudio Range audio instance that triggers this callback.
         * @param state The use state of the range audio.
         * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
         */
        public delegate void OnRangeAudioMicrophoneStateUpdate(ZegoRangeAudio rangeAudio, ZegoRangeAudioMicrophoneState state, int errorCode);


    }

}
