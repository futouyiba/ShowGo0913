using System;
using static ZEGO.ZegoImplCallChangeUtil;
using static ZEGO.ZegoCallBackChangeUtil;
using static ZEGO.ZegoExpressEngineImpl;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AOT;
using System.Runtime.InteropServices;

namespace ZEGO
{
    public class ZegoCopyrightedMusicImpl : ZegoCopyrightedMusic
    {
        public static ConcurrentDictionary<int, OnCopyrightedMusicInitCallback> initCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicInitCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicSendExtendedRequestCallback> sendExtendedRequestCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicSendExtendedRequestCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicGetLrcLyricCallback> getLrcLyricCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicGetLrcLyricCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicGetKrcLyricByTokenCallback> getKrcLyricByTokenCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicGetKrcLyricByTokenCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicRequestSongCallback> requestSongCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicRequestSongCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicRequestAccompanimentCallback> requestAccompanimentCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicRequestAccompanimentCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicRequestAccompanimentClipCallback> requestAccompanimentClipCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicRequestAccompanimentClipCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicGetMusicByTokenCallback> getMusicByTokenCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicGetMusicByTokenCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicDownloadCallback> downloadCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicDownloadCallback>();
        public static ConcurrentDictionary<int, OnCopyrightedMusicGetStandardPitchCallback> getStandatdPitchCallbackDic = new ConcurrentDictionary<int, OnCopyrightedMusicGetStandardPitchCallback>();
        private static object zegoCopyrightedMusicHandlerLock = new object();
        //private ZegoCopyRightedMusicIn index = ZegoMediaPlayerInstanceIndex.Null;

        public override void InitCopyrightedMusic(ZegoCopyrightedMusicConfig config, OnCopyrightedMusicInitCallback callback)
        {
            zego_copyrighted_music_config music_config = ChangeZegoCopyrightedMusicConfigClassToStruct(config);
            int ret = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_init(music_config);
            ZegoUtil.ZegoPrivateLog(0, string.Format("InitCopyrightedMusic, result:{0}", 0), true, ZegoConstans.ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC);

            if(callback != null)
            {
                initCallbackDic.AddOrUpdate(ret, callback, (key, old_value)=>callback);
            }
        }
        public override ulong GetCacheSize()
        {
            ulong cache_size = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_cache_size();
            return cache_size;
        }
        public override void ClearCache()
        {
            int ret = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_clear_cache();
            ZegoUtil.ZegoPrivateLog(ret, string.Format("ClearCache, result:{0}", ret), true, ZegoConstans.ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC);
        }
        public override void SendExtendedRequest(string command, string param, OnCopyrightedMusicSendExtendedRequestCallback callback)
        {
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_send_extended_request(command, param);
            //ZegoUtil.ZegoPrivateLog(result, string.Format("SendExtendedRequest, seq:{0}", seq), true, ZegoConstans.ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC);
            if(callback != null)
            {
                sendExtendedRequestCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }
        }

        public override void GetLrcLyric(string songID, OnCopyrightedMusicGetLrcLyricCallback callback)
        {
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_lrc_lyric(songID);
            //ZegoUtil.ZegoPrivateLog(result, string.Format("GetLrcLyric, seq:{0}", seq), true, ZegoConstans.ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC);
            if(callback != null)
            {
                getLrcLyricCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }
        }

        public override void GetKrcLyricByToken(string krcToken, OnCopyrightedMusicGetKrcLyricByTokenCallback callback)
        {
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_krc_lyric_by_token(krcToken);
            //ZegoUtil.ZegoPrivateLog(result, string.Format("GetLrcLyric, seq:{0}", seq), true, ZegoConstans.ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC);
            if(callback != null)
            {
                getKrcLyricByTokenCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }
        }
        public override void RequestSong(ZegoCopyrightedMusicRequestConfig config, OnCopyrightedMusicRequestSongCallback callback)
        {
            zego_copyrighted_music_request_config request_config = ChangeZegoCopyrightedMusicRequestConfigClassToStruct(config);
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_request_song(request_config);
            if(callback != null)
            {
                requestSongCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }
        }

        public override void RequestAccompaniment(ZegoCopyrightedMusicRequestConfig config, OnCopyrightedMusicRequestAccompanimentCallback callback)
        {
            zego_copyrighted_music_request_config request_config = ChangeZegoCopyrightedMusicRequestConfigClassToStruct(config);
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_request_accompaniment(request_config);
            if(callback != null)
            {
                requestAccompanimentCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }
        }

        public override void RequestAccompanimentClip(ZegoCopyrightedMusicRequestConfig config, OnCopyrightedMusicRequestAccompanimentClipCallback callback)
        {
            zego_copyrighted_music_request_config request_config = ChangeZegoCopyrightedMusicRequestConfigClassToStruct(config);
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_request_accompaniment_clip(request_config);
            if(callback != null)
            {
                requestAccompanimentClipCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }
        }

        public override void GetMusicByToken(string shareToken, OnCopyrightedMusicGetMusicByTokenCallback callback)
        {
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_music_by_token(shareToken);
            if(callback != null)
            {
                getMusicByTokenCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }
        }

        public override void Download(string resourceID, OnCopyrightedMusicDownloadCallback callback)
        {
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_download(resourceID);
            if(callback != null)
            {
                downloadCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }

        }

        public override bool QueryCache(string songID, ZegoCopyrightedMusicType type)
        {
            bool ret = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_query_cache(songID, type);
            return ret;
        }

        public override ulong GetDuration(string resourceID)
        {
            ulong duration = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_duration(resourceID);
            return duration;
        }

        public override int StartScore(string resourceID, int pitchValueInterval)
        {
            int ret = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_start_score(resourceID, pitchValueInterval);
            return ret;
        }

        public override int PauseScore(string resourceID)
        {
            int ret = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_pause_score(resourceID);
            return ret;
        }

        public override int ResumeScore(string resourceID)
        {
            int ret = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_resume_score(resourceID);
            return ret;
        }

        public override int StopScore(string resourceID)
        {
            int ret = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_stop_score(resourceID);
            return ret;
        }

        public override int ResetScore(string resourceID)
        {
            int ret = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_reset_score(resourceID);
            return ret;
        }

        public override int GetPreviousScore(string resourceID)
        {
            int score = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_previous_score(resourceID);
            return score;
        }

        public override int GetAverageScore(string resourceID)
        {
            int average_score = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_average_score(resourceID);
            return average_score;
        }
        public override int GetTotalScore(string resourceID)
        {
            int total_score = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_total_score(resourceID);
            return total_score;
        }

        public override void GetStandardPitch(string resourceID, OnCopyrightedMusicGetStandardPitchCallback callback)
        {
            int seq = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_standard_pitch(resourceID);
            if(callback != null)
            {
                getStandatdPitchCallbackDic.AddOrUpdate(seq, callback, (key, old_value)=>callback);
            }
        }
        public override int GetCurrentPitch(string resourceID)
        {
            int current_pitch = IExpressCopyrightedMusicInternal.zego_express_copyrighted_music_get_current_pitch(resourceID);
            return current_pitch;
        }


        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_download_progress_update))]
        public static void zego_on_copyrighted_music_download_progress_update(
            int seq, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string resource_id, 
            float progress_rate, 
            IntPtr user_context)
        {
            if(copyrightedMusicInstance != null)
            {
                if(copyrightedMusicInstance.onDownloadProgressUpdate != null)
                {
                    callBackQueue.PostAsynAction(()=>{
                        copyrightedMusicInstance?.onDownloadProgressUpdate?.Invoke(copyrightedMusicInstance, resource_id, progress_rate);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_current_pitch_value_update))]
        public static void zego_on_copyrighted_music_current_pitch_value_update(
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string resource_id, 
            int current_duration, 
            int pitch_value, 
            IntPtr user_context)
        {
            if(copyrightedMusicInstance != null)
            {
                if(copyrightedMusicInstance.onCurrentPitchValueUpdate != null)
                {
                    callBackQueue.PostAsynAction(()=>{
                        copyrightedMusicInstance?.onCurrentPitchValueUpdate?.Invoke(copyrightedMusicInstance, resource_id, current_duration, pitch_value);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_send_extended_request))]
        public static void zego_on_copyrighted_music_init(int seq, int error_code, IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicInitCallback>(initCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code);
                    initCallbackDic.TryRemove(seq, out _);
                });
            }
        }


        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_send_extended_request))]
        public static void zego_on_copyrighted_music_send_extended_request(
            int seq, 
            int error_code, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string command, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string result, 
            IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicSendExtendedRequestCallback>(sendExtendedRequestCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code, command, result);
                    sendExtendedRequestCallbackDic.TryRemove(seq, out _);
                });
            }
        }

        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_get_lrc_lyric))]
        public static void zego_on_copyrighted_music_get_lrc_lyric(
            int seq, 
            int error_code, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string lyrics, 
            IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicGetLrcLyricCallback>(getLrcLyricCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code, lyrics);
                    getLrcLyricCallbackDic.TryRemove(seq, out _);
                });
            }
        }

        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_get_krc_lyric_by_token))]
        public static void zego_on_copyrighted_music_get_krc_lyric_by_token(
            int seq, 
            int error_code, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string lyrics, 
            IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicGetKrcLyricByTokenCallback>(getKrcLyricByTokenCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code, lyrics);
                    getKrcLyricByTokenCallbackDic.TryRemove(seq, out _);
                });
            }
        }



        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_request_song))]
        public static void zego_on_copyrighted_music_request_song(
            int seq, 
            int error_code, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string resource, 
            IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicRequestSongCallback>(requestSongCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code, resource);
                    requestSongCallbackDic.TryRemove(seq, out _);
                });
            }
        }

        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_request_accompaniment))]
        public static void zego_on_copyrighted_music_request_accompaniment(
            int seq, 
            int error_code, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string resource, 
            IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicRequestAccompanimentCallback>(requestAccompanimentCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code, resource);
                    requestAccompanimentCallbackDic.TryRemove(seq, out _);
                });
            }
        }

        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_request_accompaniment_clip))]
        public static void zego_on_copyrighted_music_request_accompaniment_clip(
            int seq, 
            int error_code, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string resource, 
            IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicRequestAccompanimentClipCallback>(requestAccompanimentClipCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code, resource);
                    requestAccompanimentClipCallbackDic.TryRemove(seq, out _);
                });
            }
        }

        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_get_music_by_token))]
        public static void zego_on_copyrighted_music_get_music_by_token(
            int seq, 
            int error_code, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string resource, 
            IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicGetMusicByTokenCallback>(getMusicByTokenCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code, resource);
                    getMusicByTokenCallbackDic.TryRemove(seq, out _);
                });
            }
        }


        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_download))]
        public static void zego_on_copyrighted_music_download(int seq, int error_code, IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicDownloadCallback>(downloadCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code);
                    downloadCallbackDic.TryRemove(seq, out _);
                });
            }
        }

        [MonoPInvokeCallback(typeof(IExpressCopyrightedMusicInternal.zego_on_copyrighted_music_get_standard_pitch))]
        public static void zego_on_copyrighted_music_get_standard_pitch(
            int seq, 
            int error_code, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string pitch, 
            IntPtr user_context)
        {
            var callback = GetCallbackFromSeq<OnCopyrightedMusicGetStandardPitchCallback>(getStandatdPitchCallbackDic, seq);
            if(callback != null)
            {
                callBackQueue.PostAsynAction(()=>{
                    callback?.Invoke(error_code, pitch);
                    getStandatdPitchCallbackDic.TryRemove(seq, out _);
                });
            }
        }




    }
}