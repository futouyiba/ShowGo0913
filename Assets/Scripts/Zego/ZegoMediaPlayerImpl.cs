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
    public class ZegoMediaPlayerImpl : ZegoMediaPlayer
    {
        public static ConcurrentDictionary<int, OnMediaPlayerSeekToCallback> seekToTimeCallbackDic = new ConcurrentDictionary<int, OnMediaPlayerSeekToCallback>();
        public static OnMediaPlayerLoadResourceCallback loadResourceCallbackDic = null;
        private static object zegoMediaPlayerEventLock = new object();
        private ZegoMediaPlayerInstanceIndex index = ZegoMediaPlayerInstanceIndex.Null;

        public ZegoMediaPlayerImpl(ZegoMediaPlayerInstanceIndex index)
        {
            this.index = index;
        }

        public void ClearAll()
        {
            lock(zegoMediaPlayerEventLock)
            {
                loadResourceCallbackDic = null;
            }
        }
        public override void LoadResource(string path, OnMediaPlayerLoadResourceCallback callback)
        {
            int result = IExpressMediaPlayerInternal.zego_express_media_player_load_resource(path, index);
            string log = string.Format("LoadResource, result:{0}, path:{1} ", result, path);
            ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
            if(callback != null)
            {
                lock(zegoMediaPlayerEventLock)
                {
                    loadResourceCallbackDic = callback;
                }   
            }         
        }

        public override void LoadResourceWithPosition(string path, long startPosition, OnMediaPlayerLoadResourceCallback callback)
        {
            int result = IExpressMediaPlayerInternal.zego_express_media_player_load_resource_with_position(path, startPosition, index);
            ZegoUtil.ZegoPrivateLog(result, string.Format("LoadResourceWithPosition, result:{0}, path:{1}, startPosition:{2}",result, path, startPosition), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
            if(callback != null)
            {
                lock(zegoMediaPlayerEventLock)
                {
                    loadResourceCallbackDic = callback;
                }   
            }  
        }
        public override void LoadResourceFromMediaData(IntPtr mediaData, int mediaDataLength, long startPosition, OnMediaPlayerLoadResourceCallback callback)
        {
            int result = IExpressMediaPlayerInternal.zego_express_media_player_load_resource_from_media_data(mediaData, mediaDataLength, startPosition, index);
            ZegoUtil.ZegoPrivateLog(result, string.Format("LoadResourceFromMediaData, result:{0}, mediaDataLength:{1}, startPosition:{2}",result, mediaDataLength, startPosition), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
            if(callback != null)
            {
                lock(zegoMediaPlayerEventLock)
                {
                    loadResourceCallbackDic = callback;
                }   
            }  
        }

        public override void LoadCopyrightedMusicResourceWithPosition(string resourceID, long startPosition, OnMediaPlayerLoadResourceCallback callback)
        {
            int result = IExpressMediaPlayerInternal.zego_express_media_player_load_copyrighted_music_resource_with_position(resourceID, startPosition, index);
            ZegoUtil.ZegoPrivateLog(result, string.Format("LoadCopyrightedMusicResourceWithPosition, result:{0}, resourceID:{1}, startPosition:{2}",result, resourceID, startPosition), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
            if(callback != null)
            {
                lock(zegoMediaPlayerEventLock)
                {
                    loadResourceCallbackDic = callback;
                }   
            }  
        }

        public override void Start()
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_start(index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("Start, result:{0}", ret), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void Stop()
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_stop(index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("Stop, result:{0}", ret), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void Pause()
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_pause(index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("Pause, result:{0}", ret), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void Resume()
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_resume(index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("Resume, result:{0}", ret), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SeekTo(ulong millisecond, OnMediaPlayerSeekToCallback callback)
        {
            // no log
            int seq = IExpressMediaPlayerInternal.zego_express_media_player_seek_to(millisecond, index);
            if(callback != null)
            {
                seekToTimeCallbackDic.AddOrUpdate(seq, callback, (key, oldValue)=>callback);
            }
        }

        public override void EnableRepeat(bool enable)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_enable_repeat(enable, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("EnableRepeat, result:{0}, enable:{1}", ret, enable), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetPlayLoopCount(int count)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_play_loop_count(count, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetPlayLoopCount, result:{0}, count:{1}", ret, count), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetPlaySpeed(float speed)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_play_speed(speed, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetPlaySpeed, result:{0}, speed:{1}", ret, speed), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void EnableAux(bool enable)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_enable_aux(enable, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("EnableAux, result:{0}, enable:{1}", ret, enable), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void MuteLocal(bool mute)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_mute_local_audio(mute, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("MuteLocal, result:{0}, mute:{1}", ret, mute), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetVolume(int volume)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_volume(volume, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetVolume, result:{0}, volume:{1}", ret, volume), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetPlayVolume(int volume)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_play_volume(volume, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetPlayVolume, result:{0}, volume:{1}", ret, volume), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetPublishVolume(int volume)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_publish_volume(volume, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetPublishVolume, result:{0}, volume:{1}", ret, volume), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetProgressInterval(ulong millisecond)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_progress_interval(millisecond, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetProgressInterval, result:{0}, millisecond:{1}", ret, millisecond), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override int GetPlayVolume()
        {
            // no log
            int volume = IExpressMediaPlayerInternal.zego_express_media_player_get_play_volume(index);
            return volume;
        }

        public override int GetPublishVolume()
        {
            // no log
            int publish_volume = IExpressMediaPlayerInternal.zego_express_media_player_get_publish_volume(index);
            return publish_volume;
        }

        public override ulong GetTotalDuration()
        {
            // no log
            ulong duration = IExpressMediaPlayerInternal.zego_express_media_player_get_total_duration(index);
            return duration;
        }

        public override ulong GetCurrentProgress()
        {
            // no log
            ulong progress = IExpressMediaPlayerInternal.zego_express_media_player_get_current_progress(index);
            return progress;
        }

        public override uint GetAudioTrackCount()
        {
            // no log
            uint cnt = IExpressMediaPlayerInternal.zego_express_media_player_get_audio_track_count(index);
            return cnt;
        }

        public override void SetAudioTrackIndex(uint index)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_audio_track_index(index, this.index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetAudioTrackIndex, result:{0}, index:{1}", ret, index), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetVoiceChangerParam(ZegoMediaPlayerAudioChannel audioChannel, ZegoVoiceChangerParam param)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_voice_changer_param(audioChannel, param.pitch, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetVoiceChangerParam, result:{0}, audioChannel:{1}, pitch:{2}", ret, audioChannel, param.pitch), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override ZegoMediaPlayerState GetCurrentState()
        {
            //no log
            ZegoMediaPlayerState state = IExpressMediaPlayerInternal.zego_express_media_player_get_current_state(index);
            return state;
        }

        public override int GetIndex()
        {
            return (int)index;
        }

        public override void EnableAccurateSeek(bool enable, ZegoAccurateSeekConfig config)
        {
            zego_accurate_seek_config config_struct = ChangeZegoAccurateSeekConfigClassToStruct(config);
            IntPtr config_ptr = ZegoUtil.GetStructPointer(config_struct);
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_enable_accurate_seek(enable, config_ptr, index);
            ZegoUtil.ReleaseStructPointer(config_ptr);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("EnableAccurateSeek, result:{0}, enable:{1}, timeOut:{2}", ret, enable, config.timeout), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetNetWorkResourceMaxCache(uint time, uint size)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_network_resource_max_cache(time, size, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetNetWorkResourceMaxCache, result:{0}, time:{1}, size:{2}", ret, time, size), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override ZegoNetWorkResourceCache GetNetWorkResourceCache()
        {
            zego_network_resource_cache cache = new zego_network_resource_cache();
            IntPtr cache_ptr = ZegoUtil.GetStructPointer(cache);
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_get_network_resource_cache(cache_ptr, index);

            var tt = ZegoUtil.GetStructByPtr<zego_network_resource_cache>(cache_ptr);
            ZegoNetWorkResourceCache netWorkResourceCache = ChangeZegoNetWorkResourceCacheStructToClass(tt);
            ZegoUtil.ReleaseStructPointer(cache_ptr);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("GetNetWorkResourceCache, result:{0}, time:{1}, size:{2}", ret, netWorkResourceCache.time, netWorkResourceCache.size), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);

            return netWorkResourceCache;
        }

        public override void SetNetWorkBufferThreshold(uint threshold)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_set_network_buffer_threshold(threshold, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetNetWorkBufferThreshold, result:{0}, threshold:{1}", ret, threshold), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void EnableSoundLevelMonitor(bool enable, uint millisecond)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_enable_sound_level_monitor(enable, millisecond, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("EnableSoundLevelMonitor, result:{0}, enable:{1}, millisecond:{2}", ret, enable, millisecond), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void EnableFrequencySpectrumMonitor(bool enable, uint millisecond)
        {
            int ret = IExpressMediaPlayerInternal.zego_express_media_player_enable_frequency_spectrum_monitor(enable, millisecond, index);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("EnableFrequencySpectrumMonitor, result:{0}, enable:{1}, millisecond:{2}", ret, enable, millisecond), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetVideoHandler(IZegoMediaPlayerHandler.OnVideoFrame onVideoFrame, ZegoVideoFrameFormat format)
        {
            this.onVideoFrame = onVideoFrame;

            int result = 0;
            bool enable = false;
            if (onVideoFrame == null)
            {
                enable = false;
                result = IExpressMediaPlayerInternal.zego_express_media_player_enable_video_data(enable, format, index);
            }
            else
            {
                enable = true;
                result = IExpressMediaPlayerInternal.zego_express_media_player_enable_video_data(enable, format, index);
            }

            ZegoUtil.ZegoPrivateLog(result, string.Format("SetVideoHandler, enable:{0}, format:{1}, result:{2} ", enable, format, result), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        public override void SetAudioHandler(IZegoMediaPlayerHandler.OnAudioFrame onAudioFrame)
        {
            this.onAudioFrame = onAudioFrame;

            int result = 0;
            bool enable = false;
            if (onAudioFrame == null)
            {
                enable = false;
                result = IExpressMediaPlayerInternal.zego_express_media_player_enable_audio_data(enable, index);
            }
            else
            {
                enable = true;
                result = IExpressMediaPlayerInternal.zego_express_media_player_enable_audio_data(enable, index);
            }

            ZegoUtil.ZegoPrivateLog(result, string.Format("SetAudioHandler, enable:{0}, result:{1} ", enable, result), true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
        }

        
        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_state_update))]
        public static void zego_on_media_player_state_update(ZegoMediaPlayerState state, int error_code, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            ZegoMediaPlayerImpl zegoMediaPlayerImpl = (ZegoMediaPlayerImpl)GetObjectFromIndex(mediaPlayerAndIndex, (int)instance_index);
            if(zegoMediaPlayerImpl != null)
            {
                if(zegoMediaPlayerImpl.onMediaPlayerStateUpdate != null)
                {
                    callBackQueue.PostAsynAction(() =>
                    {
                        zegoMediaPlayerImpl?.onMediaPlayerStateUpdate?.Invoke(zegoMediaPlayerImpl, state, error_code);
                    });
                }
            }
        }


        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_network_event))]
        public static void zego_on_media_player_network_event(ZegoMediaPlayerNetworkEvent network_event, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            ZegoMediaPlayerImpl zegoMediaPlayerImpl = (ZegoMediaPlayerImpl)GetObjectFromIndex(mediaPlayerAndIndex, (int)instance_index);
            if(zegoMediaPlayerImpl.onMediaPlayerNetworkEvent != null)
            {
                callBackQueue.PostAsynAction(() =>
                {
                    zegoMediaPlayerImpl?.onMediaPlayerNetworkEvent?.Invoke(zegoMediaPlayerImpl, network_event);
                });
            }
        }


        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_playing_progress))]
        public static void zego_on_media_player_playing_progress(ulong millisecond, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            ZegoMediaPlayerImpl zegoMediaPlayerImpl = (ZegoMediaPlayerImpl)GetObjectFromIndex(mediaPlayerAndIndex, (int)instance_index);
            if(zegoMediaPlayerImpl.onMediaPlayerPlayingProgress != null)
            {
                callBackQueue.PostAsynAction(() =>
                {
                    zegoMediaPlayerImpl?.onMediaPlayerPlayingProgress?.Invoke(zegoMediaPlayerImpl, millisecond);
                });
            }
        }


        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_recv_sei))]
        public static void zego_on_media_player_recv_sei(IntPtr data, uint data_length, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            ZegoMediaPlayerImpl zegoMediaPlayerImpl = (ZegoMediaPlayerImpl)GetObjectFromIndex(mediaPlayerAndIndex, (int)instance_index);
            if(zegoMediaPlayerImpl.onMediaPlayerRecvSEI != null)
            {
                var data_list = new List<byte>();
                ZegoUtil.GetStructListByPtr(ref data_list, data, data_length);
                zegoMediaPlayerImpl?.onMediaPlayerRecvSEI?.Invoke(zegoMediaPlayerImpl, data_list);
            }
        }



        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_sound_level_update))]
        public static void zego_on_media_player_sound_level_update(float sound_level, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            ZegoMediaPlayerImpl zegoMediaPlayerImpl = (ZegoMediaPlayerImpl)GetObjectFromIndex(mediaPlayerAndIndex, (int)instance_index);
            if(zegoMediaPlayerImpl.onMediaPlayerSoundLevelUpdate != null)
            {
                zegoMediaPlayerImpl?.onMediaPlayerSoundLevelUpdate?.Invoke(zegoMediaPlayerImpl, sound_level);
            }
        }


        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_frequency_spectrum_update))]
        public static void zego_on_media_player_frequency_spectrum_update(IntPtr spectrum_list, uint spectrum_count, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            ZegoMediaPlayerImpl zegoMediaPlayerImpl = (ZegoMediaPlayerImpl)GetObjectFromIndex(mediaPlayerAndIndex, (int)instance_index);
            if(zegoMediaPlayerImpl.onMediaPlayerFrequencySpectrumUpdate != null)
            {
                 float[] spectrum_list_out = new float[spectrum_count];
                 Marshal.Copy(spectrum_list, spectrum_list_out, 0, (int)spectrum_count);
                 //ZegoUtil.GetStructListByPtr<float>(ref spectrum_list_out, spectrum_list, spectrum_count);
                 List<float> spectrum_list_ = new List<float>();
                 for(int i=0;i<spectrum_count;i++)
                 {
                     spectrum_list_.Add(spectrum_list_out[i]);
                 }
                zegoMediaPlayerImpl?.onMediaPlayerFrequencySpectrumUpdate?.Invoke(zegoMediaPlayerImpl, spectrum_list_);
            }
        }



        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_video_frame))]
        public static void zego_on_media_player_video_frame([In][MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] IntPtr[] data,
            [In][MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] uint[] data_length, 
            zego_video_frame_param param, 
            [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string extra_info,
            zego_media_player_instance_index instance_index, 
            System.IntPtr user_context)
        {
            ZegoMediaPlayerImpl zegoMediaPlayerImpl = (ZegoMediaPlayerImpl)GetObjectFromIndex(mediaPlayerAndIndex, (int)instance_index);
            if(zegoMediaPlayerImpl.onVideoFrame != null)
            {
                ZegoVideoFrameParam pp = ChangeZegoVideoFrameParamStructToClass(param);
                zegoMediaPlayerImpl?.onVideoFrame?.Invoke(zegoMediaPlayerImpl, data, data_length, pp, extra_info);
            }
        }

        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_audio_frame))]
        public static void zego_on_media_player_audio_frame(IntPtr data, uint data_length, zego_audio_frame_param param, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            ZegoMediaPlayerImpl zegoMediaPlayerImpl = (ZegoMediaPlayerImpl)GetObjectFromIndex(mediaPlayerAndIndex, (int)instance_index);
            if (zegoMediaPlayerImpl.onAudioFrame != null)
            {
                ZegoAudioFrameParam pp = ChangeZegoAudioFrameStructToClass(param);
                //直接在c层子线程返回回调，避免切换到ui线程导致界面卡顿

                zegoMediaPlayerImpl?.onAudioFrame?.Invoke(zegoMediaPlayerImpl, data, data_length, pp);

            }
        }


        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_load_resource))]
        public static void zego_on_media_player_load_resource(int error_code, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            lock(zegoMediaPlayerEventLock)
            {
                if(loadResourceCallbackDic != null)
                {
                    string log = string.Format("zego_on_media_player_load_resource, mediaplayerID:{0} error_code:{1}", instance_index, error_code);
                    ZegoUtil.ZegoPrivateLog(error_code, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
                    callBackQueue.PostAsynAction(() =>
                    {
                        loadResourceCallbackDic?.Invoke(error_code);
                    });
                }
            }
        }


        [MonoPInvokeCallback(typeof(IExpressMediaPlayerInternal.zego_on_media_player_seek_to))]
        public static void zego_on_media_player_seek_to(int seq, int error_code, ZegoMediaPlayerInstanceIndex instance_index, IntPtr user_context)
        {
            if(seekToTimeCallbackDic != null)
            {
                string log = string.Format("zego_on_audio_effect_player_seek_to instance_index:{0} seq:{1} error_code:{2}", instance_index, seq, error_code);
                ZegoUtil.ZegoPrivateLog(error_code, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER);
                OnMediaPlayerSeekToCallback callback = GetCallbackFromSeq<OnMediaPlayerSeekToCallback>(seekToTimeCallbackDic, seq);
                if (callback == null)
                {
                    return;
                }

                callBackQueue.PostAsynAction(() =>
                {
                    callback?.Invoke(error_code);
                    seekToTimeCallbackDic?.TryRemove(seq, out _);
                });
            }
        }
    }
}