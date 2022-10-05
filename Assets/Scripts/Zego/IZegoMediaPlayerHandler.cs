
using System;
using System.Collections.Generic;
namespace ZEGO
{

    public class IZegoMediaPlayerHandler
    {
        /**
         * MediaPlayer playback status callback.
         *
         * Available since: 1.3.4
         * Description: MediaPlayer playback status callback.
         * Trigger: The callback triggered when the state of the media player changes.
         * Restrictions: None.
         *
         * @param mediaPlayer Callback player object.
         * @param state Media player status.
         * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
         */
        public delegate void OnMediaPlayerStateUpdate(ZegoMediaPlayer mediaPlayer, ZegoMediaPlayerState state, int errorCode);

        /**
         * The callback triggered when the network status of the media player changes.
         *
         * Available since: 1.3.4
         * Description: The callback triggered when the network status of the media player changes.
         * Trigger: When the media player is playing network resources, this callback will be triggered when the status change of the cached data.
         * Restrictions: The callback will only be triggered when the network resource is played.
         * Related APIs: [setNetWorkBufferThreshold].
         *
         * @param mediaPlayer Callback player object.
         * @param networkEvent Network status event.
         */
        public delegate void OnMediaPlayerNetworkEvent(ZegoMediaPlayer mediaPlayer, ZegoMediaPlayerNetworkEvent networkEvent);

        /**
         * The callback to report the current playback progress of the media player.
         *
         * Available since: 1.3.4
         * Description: The callback triggered when the network status of the media player changes. Set the callback interval by calling [setProgressInterval]. When the callback interval is set to 0, the callback is stopped. The default callback interval is 1 second.
         * Trigger: When the media player is playing network resources, this callback will be triggered when the status change of the cached data.
         * Restrictions: None.
         * Related APIs: [setProgressInterval].
         *
         * @param mediaPlayer Callback player object.
         * @param millisecond Progress in milliseconds.
         */
        public delegate void OnMediaPlayerPlayingProgress(ZegoMediaPlayer mediaPlayer, ulong millisecond);

        /**
         * The callback triggered when the media player got media side info.
         *
         * Available since: 2.2.0
         * Description: The callback triggered when the media player got media side info.
         * Trigger: When the media player starts playing media files, the callback is triggered if the SEI is resolved to the media file.
         * Caution: The callback does not actually take effect until call [setEventHandler] to set.
         *
         * @param mediaPlayer Callback player object.
         * @param data SEI content.
         */
        public delegate void OnMediaPlayerRecvSEI(ZegoMediaPlayer mediaPlayer, List<byte> data);

        /**
         * The callback of sound level update.
         *
         * Available since: 2.15.0
         * Description: The callback of sound level update.
         * Trigger: The callback frequency is specified by [EnableSoundLevelMonitor].
         * Caution: The callback does not actually take effect until call [setEventHandler] to set.
         * Related APIs: To monitor this callback, you need to enable it through [EnableSoundLevelMonitor].
         *
         * @param mediaPlayer Callback player object.
         * @param soundLevel Sound level value, value range: [0.0, 100.0].
         */
        public delegate void OnMediaPlayerSoundLevelUpdate(ZegoMediaPlayer mediaPlayer, float soundLevel);

        /**
         * The callback of frequency spectrum update.
         *
         * Available since: 2.15.0
         * Description: The callback of frequency spectrum update.
         * Trigger: The callback frequency is specified by [EnableFrequencySpectrumMonitor].
         * Caution: The callback does not actually take effect until call [setEventHandler] to set.
         * Related APIs: To monitor this callback, you need to enable it through [EnableFrequencySpectrumMonitor].
         *
         * @param mediaPlayer Callback player object.
         * @param spectrumList Locally captured frequency spectrum value list. Spectrum value range is [0-2^30].
         */
        public delegate void OnMediaPlayerFrequencySpectrumUpdate(ZegoMediaPlayer mediaPlayer, List<float> spectrumList);

        /**
         * The callback triggered when the media player throws out video frame data.
         *
         * Available since: 2.16.0
         * Description: The callback triggered when the media player throws out video frame data.
         * Trigger: The callback is generated when the media player starts playing.
         * Caution: The callback does not actually take effect until call [setVideoHandler] to set.
         *
         * @param mediaPlayer Callback player object.
         * @param data Raw data of video frames (eg: RGBA only needs to consider data[0], I420 needs to consider data[0,1,2]).
         * @param dataLength Data length (eg: RGBA only needs to consider dataLength[0], I420 needs to consider dataLength[0,1,2]).
         * @param param Video frame flip mode.
         * @param extraInfo Video frame extra info.
         */
        public delegate void OnVideoFrame(ZegoMediaPlayer mediaPlayer, IntPtr[] data, uint[] dataLength, ZegoVideoFrameParam param, string extraInfo);

        /**
         * The callback triggered when the media player throws out audio frame data.
         *
         * Available since: 1.3.4
         * Description: The callback triggered when the media player throws out audio frame data.
         * Trigger: The callback is generated when the media player starts playing.
         * Caution: The callback does not actually take effect until call [setAudioHandler] to set.
         *
         * @param mediaPlayer Callback player object.
         * @param data Raw data of audio frames.
         * @param dataLength Data length.
         * @param param audio frame flip mode.
         */
        public delegate void OnAudioFrame(ZegoMediaPlayer mediaPlayer, IntPtr data, uint dataLength, ZegoAudioFrameParam param);


    }
    /**
     * Callback for media player loads resources.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     */
    public delegate void OnMediaPlayerLoadResourceCallback(int errorCode);

    /**
     * Callback for media player seek to playback progress.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     */
    public delegate void OnMediaPlayerSeekToCallback(int errorCode);


}
