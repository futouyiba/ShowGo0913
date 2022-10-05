
namespace ZEGO
{

    public class IZegoCopyrightedMusicHandler
    {
        /**
         * Callback for download song or accompaniment progress rate.
         *
         * @param copyrightedMusic Copyrighted music instance that triggers this callback.
         * @param resourceID The resource ID of the song or accompaniment that triggered this callback.
         * @param progressRate download progress rate.
         */
        public delegate void OnDownloadProgressUpdate(ZegoCopyrightedMusic copyrightedMusic, string resourceID, float progressRate);

        /**
         * Real-time pitch line callback.
         *
         * @param copyrightedMusic Copyrighted music instance that triggers this callback.
         * @param resourceID The resource ID of the song or accompaniment that triggered this callback.
         * @param currentDuration Current playback progress.
         * @param pitchValue Real-time pitch accuracy or value.
         */
        public delegate void OnCurrentPitchValueUpdate(ZegoCopyrightedMusic copyrightedMusic, string resourceID, int currentDuration, int pitchValue);


    }
    /**
     * Callback for copyrighted music init.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     */
    public delegate void OnCopyrightedMusicInitCallback(int errorCode);

    /**
     * Callback for copyrighted music init.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     * @param command request command.
     * @param result request result, each request command has corresponding request result.
     */
    public delegate void OnCopyrightedMusicSendExtendedRequestCallback(int errorCode, string command, string result);

    /**
     * Get lrc format lyrics complete callback.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     * @param lyrics lrc format lyrics.
     */
    public delegate void OnCopyrightedMusicGetLrcLyricCallback(int errorCode, string lyrics);

    /**
     * Get krc format lyrics complete callback.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     * @param lyrics krc format lyrics.
     */
    public delegate void OnCopyrightedMusicGetKrcLyricByTokenCallback(int errorCode, string lyrics);

    /**
     * Callback for request song.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     * @param resource The JSON string returned by the song ordering service, including song resource information.
     */
    public delegate void OnCopyrightedMusicRequestSongCallback(int errorCode, string resource);

    /**
     * Callback for request accompaniment.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     * @param resource accompany resource information.
     */
    public delegate void OnCopyrightedMusicRequestAccompanimentCallback(int errorCode, string resource);

    /**
     * Callback for request accompaniment clip.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     * @param resource accompany clip resource information.
     */
    public delegate void OnCopyrightedMusicRequestAccompanimentClipCallback(int errorCode, string resource);

    /**
     * Callback for acquire songs or accompaniment through authorization token.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     * @param resource song or accompany resource information.
     */
    public delegate void OnCopyrightedMusicGetMusicByTokenCallback(int errorCode, string resource);

    /**
     * Callback for download song or accompaniment.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     */
    public delegate void OnCopyrightedMusicDownloadCallback(int errorCode);

    /**
     * Get standard pitch data complete callback.
     *
     * @param errorCode Error code, please refer to the error codes document https://docs.zegocloud.com/en/5548.html for details.
     * @param pitch Standard pitch data.
     */
    public delegate void OnCopyrightedMusicGetStandardPitchCallback(int errorCode, string pitch);


}
