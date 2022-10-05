using static ZEGO.IZegoMediaPlayerHandler;
using System;

namespace ZEGO
{
    /**
     * Zego MediaPlayer.
     *
     * Yon can use ZegoMediaPlayer to play media resource files on the local or remote server, and can mix the sound of the media resource files that are played into the publish stream to achieve the effect of background music.
     */
    public abstract class ZegoMediaPlayer {

        public OnMediaPlayerStateUpdate onMediaPlayerStateUpdate;

        public OnMediaPlayerNetworkEvent onMediaPlayerNetworkEvent;

        public OnMediaPlayerPlayingProgress onMediaPlayerPlayingProgress;

        public OnMediaPlayerRecvSEI onMediaPlayerRecvSEI;

        public OnMediaPlayerSoundLevelUpdate onMediaPlayerSoundLevelUpdate;

        public OnMediaPlayerFrequencySpectrumUpdate onMediaPlayerFrequencySpectrumUpdate;

        public OnVideoFrame onVideoFrame;

        public OnAudioFrame onAudioFrame;

        /**
         * Set video callback handler.
         *
         * Developers can set this callback to throw the video data of the media resource file played by the media player
         *
         * @param onVideoFrame Video event callback object for media player
         * @param format Video frame format for video data
         */
        public abstract void SetVideoHandler(OnVideoFrame onVideoFrame, ZegoVideoFrameFormat format);

        /**
         * Set audio callback handler.
         *
         * You can set this callback to throw the audio data of the media resource file played by the media player
         *
         * @param onAudioFrame Audio event callback object for media player
         */
        public abstract void SetAudioHandler(OnAudioFrame onAudioFrame);

        /**
         * Load media resource.
         *
         * Available: since 1.3.4
         * Description: Load media resources.
         * Use case: Developers can load the absolute path to the local resource or the URL of the network resource incoming.
         * When to call: It can be called after the engine by [createEngine] has been initialized and the media player has been created by [createMediaPlayer].
         * Related APIs: Resources can be loaded through the [loadResourceWithPosition] or [loadResourceFromMediaData] function.
         *
         * @param path The absolute resource path or the URL of the network resource and cannot be null or "".
         * @param callback Notification of resource loading results
         */
        public abstract void LoadResource(string path, OnMediaPlayerLoadResourceCallback callback);

        /**
         * Load media resource.
         *
         * Available: since 2.14.0
         * Description: Load media resources, and specify the progress, in milliseconds, at which playback begins.
         * Use case: Developers can load the absolute path to the local resource or the URL of the network resource incoming.
         * When to call: It can be called after the engine by [createEngine] has been initialized and the media player has been created by [createMediaPlayer].
         * Related APIs: Resources can be loaded through the [loadResource] or [loadResourceFromMediaData] function.
         * Caution: When [startPosition] exceeds the total playing time, it will start playing from the beginning.
         *
         * @param path The absolute resource path or the URL of the network resource and cannot be null or "".
         * @param startPosition The progress at which the playback started.
         * @param callback Notification of resource loading results
         */
        public abstract void LoadResourceWithPosition(string path, long startPosition, OnMediaPlayerLoadResourceCallback callback);

        /**
         * Load media resource.
         *
         * Available: since 2.10.0
         * Description: Load binary audio data.
         * Use case: Developers do not want to cache the audio data locally, and directly transfer the audio binary data to the media player, directly load and play the audio.
         * When to call: It can be called after the engine by [createEngine] has been initialized and the media player has been created by [createMediaPlayer].
         * Related APIs: Resources can be loaded through the [loadResource] or [loadResourceWithPosition] function.
         * Caution: When [startPosition] exceeds the total playing time, it will start playing from the beginning.
         *
         * @param mediaData Binary audio data.
         * @param mediaDataLength The length of the binary audio data.
         * @param startPosition Position of starting playback, in milliseconds.
         * @param callback Notification of resource loading results.
         */
        public abstract void LoadResourceFromMediaData(IntPtr mediaData, int mediaDataLength, long startPosition, OnMediaPlayerLoadResourceCallback callback);

        /**
         * Load copyrighted music resource.
         *
         * Available: since 2.14.0
         * Description: Load media resources, and specify the progress, in milliseconds, at which playback begins.
         * Use case: Developers can load the resource ID of copyrighted music.
         * When to call: It can be called after the engine by [createEngine] has been initialized and the media player has been created by [createMediaPlayer].
         * Caution: When [startPosition] exceeds the total playing time, it will start playing from the beginning.
         *
         * @param resourceID The resource ID obtained from the copyrighted music module.
         * @param startPosition The progress at which the playback started.
         * @param callback Notification of resource loading results
         */
        public abstract void LoadCopyrightedMusicResourceWithPosition(string resourceID, long startPosition, OnMediaPlayerLoadResourceCallback callback);

        /**
         * Start playing.
         *
         * You need to load resources before playing
         */
        public abstract void Start();

        /**
         * Stop playing.
         */
        public abstract void Stop();

        /**
         * Pause playing.
         */
        public abstract void Pause();

        /**
         * resume playing.
         */
        public abstract void Resume();

        /**
         * Set the specified playback progress.
         *
         * Unit is millisecond
         *
         * @param millisecond Point in time of specified playback progress
         * @param callback the result notification of set the specified playback progress
         */
        public abstract void SeekTo(ulong millisecond, OnMediaPlayerSeekToCallback callback);

        /**
         * Whether to repeat playback.
         *
         * @param enable repeat playback flag. The default is false.
         */
        public abstract void EnableRepeat(bool enable);

        /**
         * Set the count of play loops.
         *
         * Available: since 2.10.0
         * Description: Set the count of play loops.
         * Use cases: Users can call this function when they need to use the media player to loop playback resources.
         * When to call /Trigger: It can be called after the engine by [createEngine] has been initialized and the media player has been created by [createMediaPlayer].
         * Restrictions: None.
         * Caution: Please note that after using this interface, the [enableRepeat] interface will become invalid.
         *
         * @param count the count of play loops
         */
        public abstract void SetPlayLoopCount(int count);

        /**
         * Set the speed of play.
         *
         * Available since: 2.12.0
         * Description: Set the playback speed of the player.
         * When to call: You should load resource before invoking this function.
         * Restrictions: None.
         * Related APIs: Resources can be loaded through the [loadResource] function.
         *
         * @param speed The speed of play. The range is 0.5 ~ 2.0. The default is 1.0.
         */
        public abstract void SetPlaySpeed(float speed);

        /**
         * Whether to mix the player's sound into the main stream channel being published.
         *
         * @param enable Aux audio flag. The default is false.
         */
        public abstract void EnableAux(bool enable);

        /**
         * Whether to play locally silently.
         *
         * If [enableAux] switch is turned on, there is still sound in the publishing stream. The default is false.
         *
         * @param mute Mute local audio flag, The default is false.
         */
        public abstract void MuteLocal(bool mute);

        /**
         * Set mediaplayer volume. Both the local play volume and the publish volume are set.
         *
         * @param volume The range is 0 ~ 200. The default is 60.
         */
        public abstract void SetVolume(int volume);

        /**
         * Set mediaplayer local play volume.
         *
         * @param volume The range is 0 ~ 200. The default is 60.
         */
        public abstract void SetPlayVolume(int volume);

        /**
         * Set mediaplayer publish volume.
         *
         * @param volume The range is 0 ~ 200. The default is 60.
         */
        public abstract void SetPublishVolume(int volume);

        /**
         * Set playback progress callback interval.
         *
         * This function can control the callback frequency of [onMediaPlayerPlayingProgress]. When the callback interval is set to 0, the callback is stopped. The default callback interval is 1s
         * This callback are not returned exactly at the set callback interval, but rather at the frequency at which the audio or video frames are processed to determine whether the callback is needed to call
         *
         * @param millisecond Interval of playback progress callback in milliseconds
         */
        public abstract void SetProgressInterval(ulong millisecond);

        /**
         * Gets the current local playback volume of the mediaplayer, the range is 0 ~ 200, with the default value of 60.
         */
        public abstract int GetPlayVolume();

        /**
         * Gets the current publish volume of the mediaplayer, the range is 0 ~ 200, with the default value of 60.
         */
        public abstract int GetPublishVolume();

        /**
         * Get the total progress of your media resources.
         *
         * You should load resource before invoking this function, otherwise the return value is 0
         *
         * @return Unit is millisecond
         */
        public abstract ulong GetTotalDuration();

        /**
         * Get current playing progress.
         *
         * You should load resource before invoking this function, otherwise the return value is 0
         */
        public abstract ulong GetCurrentProgress();

        /**
         * Get the number of audio tracks of the playback file.
         */
        public abstract uint GetAudioTrackCount();

        /**
         * Set the audio track of the playback file.
         *
         * @param index Audio track index, the number of audio tracks can be obtained through the [getAudioTrackCount] function.
         */
        public abstract void SetAudioTrackIndex(uint index);

        /**
         * Setting up the specific voice changer parameters.
         *
         * @param audioChannel The audio channel to be voice changed
         * @param param Voice changer parameters
         */
        public abstract void SetVoiceChangerParam(ZegoMediaPlayerAudioChannel audioChannel, ZegoVoiceChangerParam param);

        /**
         * Get the current playback status.
         */
        public abstract ZegoMediaPlayerState GetCurrentState();

        /**
         * Get media player index.
         */
        public abstract int GetIndex();

        /**
         * Open precise seek and set relevant attributes.
         *
         * Call the setting before loading the resource. After setting, it will be valid throughout the life cycle of the media player. For multiple calls to ‘enableAccurateSeek’, the configuration is an overwrite relationship, and each call to ‘enableAccurateSeek’ only takes effect on the resources loaded later.
         *
         * @param enable Whether to enable accurate seek
         * @param config The property setting of precise seek is valid only when enable is true.
         */
        public abstract void EnableAccurateSeek(bool enable, ZegoAccurateSeekConfig config);

        /**
         * Set the maximum cache duration and cache data size of web materials.
         *
         * The setting must be called before loading the resource, and it will take effect during the entire life cycle of the media player.
         * Time and size are not allowed to be 0 at the same time. The SDK internal default time is 5000, and the size is 15*1024*1024 byte.When one of time and size reaches the set value first, the cache will stop.
         *
         * @param time The maximum length of the cache time, in ms, the SDK internal default is 5000; the effective value is greater than or equal to 2000; if you fill in 0, it means no limit.
         * @param size The maximum size of the cache, the unit is byte, the internal default size of the SDK is 15*1024*1024 byte; the effective value is greater than or equal to 5000000, if you fill in 0, it means no limit.
         */
        public abstract void SetNetWorkResourceMaxCache(uint time, uint size);

        /**
         * Get the playable duration and size of the cached data of the current network material cache queue
         *
         * @return Returns the current cached information, including the length of time the data can be played and the size of the cached data.
         */
        public abstract ZegoNetWorkResourceCache GetNetWorkResourceCache();

        /**
         * Use this interface to set the cache threshold that the media player needs to resume playback. The SDK default value is 5000ms，The valid value is greater than or equal to 1000ms
         *
         * @param threshold Threshold that needs to be reached to resume playback, unit ms.
         */
        public abstract void SetNetWorkBufferThreshold(uint threshold);

        /**
         * Whether to enable sound level monitoring.
         *
         * Available since: 2.15.0
         * Description: Whether to enable sound level monitoring.
         * When to call: It can be called after the engine by [createEngine] has been initialized and the media player has been created by [createMediaPlayer].
         * Restrictions: None.
         * Related callbacks: After it is turned on, user can use the [onMediaPlayerSoundLevelUpdate] callback to monitor sound level updates.
         *
         * @param enable Whether to enable monitoring, true is enabled, false is disabled.
         * @param millisecond Monitoring time period of the sound level, in milliseconds, has a value range of [100, 3000].
         */
        public abstract void EnableSoundLevelMonitor(bool enable, uint millisecond);

        /**
         * Whether to enable frequency spectrum monitoring.
         *
         * Available since: 2.15.0
         * Description: Whether to enable frequency spectrum monitoring.
         * When to call: It can be called after the engine by [createEngine] has been initialized and the media player has been created by [createMediaPlayer].
         * Restrictions: None.
         * Related APIs: After it is turned on, user can use the [onMediaPlayerFrequencySpectrumUpdate] callback to monitor frequency spectrum updates.
         *
         * @param enable Whether to enable monitoring, true is enabled, false is disabled.
         * @param millisecond Monitoring time period of the frequency spectrum, in milliseconds, has a value range of [100, 3000].
         */
        public abstract void EnableFrequencySpectrumMonitor(bool enable, uint millisecond);

    }


}
