using static ZEGO.IZegoCopyrightedMusicHandler;

namespace ZEGO
{

    public abstract class ZegoCopyrightedMusic
    {
        /**
         * Initialize the copyrighted music module.
         *
         * Available since: 2.13.0
         * Description: Initialize the copyrighted music so that you can use the function of the copyrighted music later.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         * Restrictions: The real user information must be passed in, otherwise the song resources cannot be obtained for playback.
         *
         * @param config the copyrighted music configuration.
         * @param callback init result
         */
        public abstract void InitCopyrightedMusic(ZegoCopyrightedMusicConfig config, OnCopyrightedMusicInitCallback callback);

        /**
         * Get cache size.
         *
         * Available since: 2.13.0
         * Description: When using this module, some cache files may be generated, and the size of the cache file can be obtained through this interface.
         * Use case: Used to display the cache size of the App.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         *
         * @return cache file size, in byte.
         */
        public abstract ulong GetCacheSize();

        /**
         * Clear cache.
         *
         * Available since: 2.13.0
         * Description: When using this module, some cache files may be generated, which can be cleared through this interface.
         * Use case: Used to clear the cache of the App.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         */
        public abstract void ClearCache();

        /**
         * Send extended feature request.
         *
         * Available since: 2.13.0
         * Description: Initialize the copyrighted music so that you can use the function of the copyrighted music later.
         * Use case: Used to get a list of songs.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         *
         * @param command request command, see details for specific supported commands.
         * @param param request parameters, each request command has corresponding request parameters.
         * @param callback send extended feature request result
         */
        public abstract void SendExtendedRequest(string command, string param, OnCopyrightedMusicSendExtendedRequestCallback callback);

        /**
         * Get lyrics in lrc format.
         *
         * Available since: 2.13.0
         * Description: Get lyrics in lrc format, support parsing lyrics line by line.
         * Use case: Used to display lyrics line by line.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         *
         * @param songID the ID of the song or accompaniment, the song and accompaniment of a song share the same ID.
         * @param callback get lyrics result
         */
        public abstract void GetLrcLyric(string songID, OnCopyrightedMusicGetLrcLyricCallback callback);

        /**
         * Get lyrics in krc format.
         *
         * Available since: 2.13.0
         * Description: Get lyrics in krc format, support parsing lyrics word by word.
         * Use case: Used to display lyrics word by word.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         *
         * @param krcToken The krcToken obtained by calling requestAccompaniment.
         * @param callback get lyrics result.
         */
        public abstract void GetKrcLyricByToken(string krcToken, OnCopyrightedMusicGetKrcLyricByTokenCallback callback);

        /**
         * Request a song.
         *
         * Available since: 2.13.0
         * Description: In addition to obtaining the basic information of the song (duration, song name, singer, etc.), and the most important resource id that can be used for local playback, or share_token for sharing to others, there are also some related authentications. information. Support three ways to request a song, pay-per-use, monthly billing by user, and monthly billing by room.
         * Use case: Get copyrighted songs for local playback and sharing.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         * Restrictions: This interface will trigger billing. A song may have three sound qualities: normal, high-definition, and lossless. Each sound quality has a different resource file, and each resource file has a unique resource ID.
         *
         * @param config request configuration.
         * @param callback request a song result
         */
        public abstract void RequestSong(ZegoCopyrightedMusicRequestConfig config, OnCopyrightedMusicRequestSongCallback callback);

        /**
         * Request accompaniment.
         *
         * Available since: 2.13.0
         * Description: You can get the accompaniment resources of the song corresponding to the songID, including resource_id, krc_token, share_token, etc. Support three ways to request accompaniment, pay-per-use, monthly billing by user, and monthly billing by room.
         * Use case: Get copyrighted accompaniment for local playback and sharing.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         * Restrictions: This interface will trigger billing.
         *
         * @param config request configuration.
         * @param callback request accompaniment result.
         */
        public abstract void RequestAccompaniment(ZegoCopyrightedMusicRequestConfig config, OnCopyrightedMusicRequestAccompanimentCallback callback);

        /**
         * Request accompaniment clip.
         *
         * Available since: 2.13.0
         * Description: You can obtain the climax clip resources of the song corresponding to the songID, including resource_id, krc_token, share_token, etc. Support three ways to request accompaniment clip, pay-per-use, monthly billing by user, and monthly billing by room.
         * Use case: Get copyrighted accompaniment clip for local playback and sharing.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         * Restrictions: This interface will trigger billing.
         *
         * @param config request configuration.
         * @param callback request accompaniment clip result.
         */
        public abstract void RequestAccompanimentClip(ZegoCopyrightedMusicRequestConfig config, OnCopyrightedMusicRequestAccompanimentClipCallback callback);

        /**
         * Get a song or accompaniment.
         *
         * Available since: 2.13.0
         * Description: After the user successfully obtains the song/accompaniment/climax clip resource, he can get the corresponding shareToken, share the shareToken with other users, and other users call this interface to obtain the shared music resources.
         * Use case: In the online KTV scene, after receiving the song or accompaniment token shared by the lead singer, the chorus obtains the corresponding song or accompaniment through this interface, and then plays it on the local end.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         *
         * @param shareToken access the corresponding authorization token for a song or accompaniment.
         * @param callback get a song or accompaniment result.
         */
        public abstract void GetMusicByToken(string shareToken, OnCopyrightedMusicGetMusicByTokenCallback callback);

        /**
         * Download song or accompaniment.
         *
         * Available since: 2.13.0
         * Description: Download a song or accompaniment. It can only be played after downloading successfully.
         * Use case: Get copyrighted accompaniment for local playback and sharing.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         * Restrictions: Loading songs or accompaniment resources is affected by the network.
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         * @param callback download song or accompaniment result.
         */
        public abstract void Download(string resourceID, OnCopyrightedMusicDownloadCallback callback);

        /**
         * Query the resource's cache is existed or not.
         *
         * Available since: 2.13.0
         * Description: Query the resource is existed or not.
         * Use case: Can be used to check the resource's cache is existed or not
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         *
         * @param songID the ID of the song or accompaniment, the song and accompaniment of a song share the same ID.
         * @param type the song resource type.
         */
        public abstract bool QueryCache(string songID, ZegoCopyrightedMusicType type);

        /**
         * Get the playing time of a song or accompaniment file.
         *
         * Available since: 2.13.0
         * Description: Get the playing time of a song or accompaniment file.
         * Use case: Can be used to display the playing time information of the song or accompaniment on the view.
         * When to call: After initializing the copyrighted music [createCopyrightedMusic].
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract ulong GetDuration(string resourceID);

        /**
         * Start scoring.
         *
         * Available since: 2.15.0
         * Description: Start the scoring function.After starting scoring, the scoring result OnCurrentPitchValueUpdate callback will be received according to the set callback time interval.
         * Use case: Can be used to display the singing score on the view.
         * When to call: After obtaining krc verbatim lyrics and playing the accompaniment resources of copyrighted music.
         * Restrictions: Only support use this api after [startPublishingStream].
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         * @param pitchValueInterval the time interval of real-time pitch line callback, in milliseconds, the default is 50 milliseconds.
         */
        public abstract int StartScore(string resourceID, int pitchValueInterval);

        /**
         * Pause scoring.
         *
         * Available since: 2.15.0
         * Description: Pause ongoing scoring,will stop the [OnCurrentPitchValueUpdate] callback.
         * Use case: You can call this interface to pause the scoring function while scoring.
         * When to call: It can be called while grading. 
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract int PauseScore(string resourceID);

        /**
         * Resume scoring.
         *
         * Available since: 2.15.0
         * Description: Resume currently paused scoring.
         * Use case: When there is currently paused scoring, this interface can be called to resume the scoring function.
         * When to call: It can be called when there is currently a paused scoring. 
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract int ResumeScore(string resourceID);

        /**
         * Stop scoring.
         *
         * Available since: 2.15.0
         * Description: End the current rating.The [OnCurrentPitchValueUpdate] callback will be stopped, but the average or total score can still be obtained normally.
         * Use case: You can call this interface to end the scoring while scoring.
         * When to call: It can be called while grading. 
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract int StopScore(string resourceID);

        /**
         * Reset scoring.
         *
         * Available since: 2.15.0
         * Description: Reset the scores that have already been performed,The [OnCurrentPitchValueUpdate] callback will be stopped and the average or total score will be 0.
         * Use case: Often used in scenes where the same song is re-sung.
         * When to call: It can be called after scoring has been performed. 
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract int ResetScore(string resourceID);

        /**
         * Get the score of the previous sentence.
         *
         * Available since: 2.15.0
         * Description: Get the score of the previous sentence.
         * Use case: Can be used to display the score of each sentence on the view.
         * When to call: After obtaining krc verbatim lyrics and playing the accompaniment resources of copyrighted music.The user gets it once after singing each sentence.
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract int GetPreviousScore(string resourceID);

        /**
         * Get average score.
         *
         * Available since: 2.15.0
         * Description: Get the average score.
         * Use case: Can be used to display the average score on the view.
         * When to call: After obtaining krc verbatim lyrics and playing the accompaniment resources of copyrighted music.
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract int GetAverageScore(string resourceID);

        /**
         * Get total score .
         *
         * Available since: 2.15.0
         * Description: Get the total score.
         * Use case: Can be used to display the total score on the view.
         * When to call: After obtaining krc verbatim lyrics and playing the accompaniment resources of copyrighted music. 
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract int GetTotalScore(string resourceID);

        /**
         * Get standard pitch data.
         *
         * Available since: 2.15.0
         * Description: Get standard pitch data.
         * Use case: Can be used to display standard pitch lines on the view.
         * Restrictions: Only accompaniment or climactic clip assets have pitch lines.
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         * @param callback get standard pitch data result.
         */
        public abstract void GetStandardPitch(string resourceID, OnCopyrightedMusicGetStandardPitchCallback callback);

        /**
         * Get total score .
         *
         * Available since: 2.15.0
         * Description: Get real-time pitch data.
         * Use case: Can be used to display real-time pitch lines on the view.
         * When to call: It can be called after playing the copyright accompaniment and starting to score.
         *
         * @param resourceID the resource ID corresponding to the song or accompaniment.
         */
        public abstract int GetCurrentPitch(string resourceID);

        public OnDownloadProgressUpdate onDownloadProgressUpdate;

        public OnCurrentPitchValueUpdate onCurrentPitchValueUpdate;


    }

}
