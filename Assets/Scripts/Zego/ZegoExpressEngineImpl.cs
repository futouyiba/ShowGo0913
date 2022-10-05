using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;
using static ZEGO.ZegoExpressEngineCallBack;
using static ZEGO.ZegoImplCallChangeUtil;
using static ZEGO.ZegoAudioEffectPlayerImpl;
using static ZEGO.ZegoExpressErrorCode;
using static ZEGO.ZegoConstans;
using static ZEGO.ZegoRangeAudioImpl;
using static ZEGO.ZegoMediaPlayerImpl;
using static ZEGO.ZegoCopyrightedMusicImpl;
namespace ZEGO
{
    public class ZegoExpressEngineImpl : ZegoExpressEngine
    {
        public class ZegoCallBackQueue : MonoBehaviour
        {
            private static Queue<Action> actionQueue = new Queue<Action>();

            public void ClearActionQueue()
            {
                lock (actionQueue)
                {
                    actionQueue.Clear();
                }
            }

            public void PostAsynAction(Action action)
            {
                lock (actionQueue)
                {
                    actionQueue.Enqueue(action);
                }
            }

            private Action GetAction()
            {
                lock (actionQueue)
                {
                    Action action = actionQueue.Dequeue();
                    return action;
                }
            }

            void Update()
            {
                while (actionQueue.Count > 0)
                {
                    Action action = GetAction();
                    action();
                }
            }

        }
        public static ZegoExpressEngineImpl enginePtr = null;
        static IntPtr jvm = IntPtr.Zero;
        static IntPtr application = IntPtr.Zero;
        // public static zego_engine_config engineConfig = new zego_engine_config();
        protected static GameObject zegoGameObj = null;
        public static ZegoCallBackQueue callBackQueue = null;//new ZegoCallBackQueue();
        private static object zegoExpressEngineLock = new object();
        private static readonly object zegoMediaPlayerLock = new object();
        private static readonly object zegoCopyrightedMusicLock = new object();
        public VideoDataRingBuffer localVideoBufMain = new VideoDataRingBuffer();//主流本地预览
        public VideoDataRingBuffer localVideoBufAux = new VideoDataRingBuffer();//辅流本地预览
        private static ConcurrentDictionary<string, VideoDataRingBuffer> playBuffers = new ConcurrentDictionary<string, VideoDataRingBuffer>();//可能存在同时多股拉流
        public static ConcurrentDictionary<int, OnPublisherUpdateCdnUrlResult> onPublisherUpdateCdnUrlResultDics = new ConcurrentDictionary<int, OnPublisherUpdateCdnUrlResult>();
        public static IZegoDestroyCompletionCallback onDestroyCompletion;
        public static ConcurrentDictionary<int, OnIMSendBroadcastMessageResult> onIMSendBroadcastMessageResultDics = new ConcurrentDictionary<int, OnIMSendBroadcastMessageResult>();
        public static ConcurrentDictionary<int, OnIMSendCustomCommandResult> onIMSendCustomCommandResultDics = new ConcurrentDictionary<int, OnIMSendCustomCommandResult>();
        public static ConcurrentDictionary<int, OnIMSendBarrageMessageResult> onIMSendBarrageMessageResultDics = new ConcurrentDictionary<int, OnIMSendBarrageMessageResult>();
        public static ConcurrentDictionary<int, OnPublisherSetStreamExtraInfoResult> onPublisherSetStreamExtraInfoResultDics = new ConcurrentDictionary<int, OnPublisherSetStreamExtraInfoResult>();
        public static ConcurrentDictionary<int, OnRoomSetRoomExtraInfoResult> onRoomSetRoomExtraInfoResultDics = new ConcurrentDictionary<int, OnRoomSetRoomExtraInfoResult>();
        //private static bool setEngineConfigFlag = false;
        public static ConcurrentDictionary<int, OnMixerStartResult> onMixerStartResultDics = new ConcurrentDictionary<int, OnMixerStartResult>();
        public static ConcurrentDictionary<int, OnMixerStopResult> onMixerStopResultDics = new ConcurrentDictionary<int, OnMixerStopResult>();
        public static ConcurrentDictionary<int, OnMixerStartResult> onAutoMixerStartResultDics = new ConcurrentDictionary<int, OnMixerStartResult>();
        public static ConcurrentDictionary<int, OnMixerStopResult> onAutoMixerStopResultDics = new ConcurrentDictionary<int, OnMixerStopResult>();
        public static ConcurrentDictionary<int, ZegoAudioEffectPlayer> audioEffectPlayerAndIndex = new ConcurrentDictionary<int, ZegoAudioEffectPlayer>();
        public static ConcurrentDictionary<zego_range_audio_instance_index, ZegoRangeAudio> rangeAudioAndIndex = new ConcurrentDictionary<zego_range_audio_instance_index, ZegoRangeAudio>();
        public static ConcurrentDictionary<string, OnPlayerTakeSnapshotResult> onPlayerTakeSnapshotResultDics = new ConcurrentDictionary<string, OnPlayerTakeSnapshotResult>();
        public static ConcurrentDictionary<int, OnPublisherTakeSnapshotResult> onPublisherTakeSnapshotResultDics = new ConcurrentDictionary<int, OnPublisherTakeSnapshotResult>();
        public static ConcurrentDictionary<int, ZegoMediaPlayer> mediaPlayerAndIndex = new ConcurrentDictionary<int, ZegoMediaPlayer>();
        public static ZegoCopyrightedMusic copyrightedMusicInstance = null;
        public static ConcurrentDictionary<int, OnRoomLoginResult> onRoomLoginResultDics = new ConcurrentDictionary<int, OnRoomLoginResult>();
        public static ConcurrentDictionary<int, OnRoomLogoutResult> onRoomLogoutResultDics = new ConcurrentDictionary<int, OnRoomLogoutResult>();

        void AddPlayerItem(string streamId)
        {
            VideoDataRingBuffer video_buf = new VideoDataRingBuffer();
            playBuffers.AddOrUpdate(streamId, video_buf, (key, oldValue) => video_buf);
        }
        void RemovePlayerItemByStreamId(string streamId)
        {
            if (!string.IsNullOrEmpty(streamId))
            {
                playBuffers.TryRemove(streamId, out _);
            }

        }
        static void ClearPlayInfo()
        {
            playBuffers.Clear();

        }

        public VideoDataRingBuffer GetPlayerBufferByStreamId(string streamId)
        {
            VideoDataRingBuffer videoDataRingBuffer = null;
            playBuffers.TryGetValue(streamId, out videoDataRingBuffer);
            return videoDataRingBuffer;
        }

        public static new void SetEngineConfig(ZegoEngineConfig config)
        {
            zego_engine_config  engineConfig = ChangeZegoEngineConfigClassToStruct(config);
            //DefaultOpenCustomRender(ref engineConfig);//Unity跨平台，默认开启外部渲染
            IExpressEngineInternal.zego_express_set_engine_config(engineConfig);
            if (engineConfig.log_config != IntPtr.Zero)
            {
                ZegoUtil.ReleaseStructPointer(engineConfig.log_config);
            }
            //setEngineConfigFlag = true;
        }

        public static new void SetLogConfig(ZegoLogConfig config)
        {
            zego_log_config log_config = ChangeZegoLogConfigClassToStruct(config);

            IExpressEngineInternal.zego_express_set_log_config(log_config);
        }

        public static new ZegoExpressEngine CreateEngine(ZegoEngineProfile profile)
        {
            if (enginePtr == null) //双if +lock
            {
                lock (zegoExpressEngineLock)
                {
                    if (enginePtr == null)
                    {
                        // init framework for log upload
                        ZegoEngineConfig config = new ZegoEngineConfig();
                        config.advancedConfig = new Dictionary<string, string>();
                        config.advancedConfig.Add("thirdparty_framework_info", "unity3d");
                        SetEngineConfig(config);

                        InitCallBackGameObject();

#if UNITY_ANDROID && !UNITY_EDITOR
                        jvm = IExpressEngineInternal.GetJVM();
                        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                        application = currentActivity.Call<AndroidJavaObject>("getApplicationContext").GetRawObject();
                        int result = IExpressEngineInternal.zego_express_set_android_env(jvm, application);
                        if (result != 0)
                        {
                        Debug.Log(string.Format("set android env fail,error_code:{0}", result));
                        throw new Exception("set android env fail,error_code:" + result);
                        }
#endif
                        
                        RegisterCallback();

                        zego_engine_profile engine_profile = ChangeZegoEngineProfileClassToStruct(profile);
                        int createResult = IExpressEngineInternal.zego_express_engine_init_with_profile(engine_profile);
                        if (createResult != 0)
                        {
                            Debug.Log(string.Format("create Engine fail,error Code:{0}", createResult));
                            throw new Exception("create Engine fail,error Code:" + createResult);
                        }
                        else
                        {
                            zego_custom_video_render_config zego_Custom_Video_Render_Config = new zego_custom_video_render_config()
                            {
                                type = ZegoVideoBufferType.RawData,
                                series = ZegoVideoFrameFormatSeries.RGB,
                                is_internal_render = false

                            };
                            IntPtr renderConfigPtr = ZegoUtil.GetStructPointer(zego_Custom_Video_Render_Config);
                            int errorCode = IExpressCustomVideoInternal.zego_express_enable_custom_video_render(true, renderConfigPtr);
                            string log = string.Format("zego_express_enable_custom_video_render  errorCode {0}", errorCode);
                            ZegoUtil.ZegoPrivateLog(errorCode, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO);
                            ZegoUtil.ReleaseStructPointer(renderConfigPtr);
                            Debug.Log("create Enigne success!!!");
                            enginePtr = new ZegoExpressEngineImpl();
                        }
                    }
                }
            }
            return enginePtr;
        }

        [Obsolete("Deprecated since 2.14.0, please use the method with the same name without [isTestEnv] parameter instead.",false)]
        public static new ZegoExpressEngine CreateEngine(uint appId, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string appSign, bool isTestEnv, ZegoScenario scenario)
        {
            if (enginePtr == null) //双if +lock
            {
                lock (zegoExpressEngineLock)
                {
                    if (enginePtr == null)
                    {
                        InitCallBackGameObject();

#if UNITY_ANDROID && !UNITY_EDITOR
                        jvm = IExpressEngineInternal.GetJVM();
                        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                        application = currentActivity.Call<AndroidJavaObject>("getApplicationContext").GetRawObject();
                        int result = IExpressEngineInternal.zego_express_set_android_env(jvm, application);
                        if (result != 0)
                        {
                        Debug.Log(string.Format("set android env fail,error_code:{0}", result));
                        throw new Exception("set android env fail,error_code:" + result);
                        }
#endif
                        //if (!setEngineConfigFlag)
                        //{
                        //    DefaultOpenCustomRender(ref engineConfig);//Unity跨平台，默认开启外部渲染
                        //    IExpressEngineInternal.zego_express_set_engine_config(engineConfig);
                        //    if (engineConfig.log_config != IntPtr.Zero)
                        //    {
                        //        ZegoUtil.ReleaseStructPointer(engineConfig.log_config);
                        //    }
                        //}
                        RegisterCallback();
                        int createResult = IExpressEngineInternal.zego_express_engine_init(appId, appSign, isTestEnv, scenario);
                        if (createResult != 0)
                        {
                            Debug.Log(string.Format("create Engine fail,error Code:{0}", createResult));
                            throw new Exception("create Engine fail,error Code:" + createResult);
                        }
                        else
                        {
                            zego_custom_video_render_config zego_Custom_Video_Render_Config = new zego_custom_video_render_config()
                            {
                                type = ZegoVideoBufferType.RawData,
                                series = ZegoVideoFrameFormatSeries.RGB,
                                is_internal_render = false

                            };
                            IntPtr renderConfigPtr = ZegoUtil.GetStructPointer(zego_Custom_Video_Render_Config);
                            int errorCode = IExpressCustomVideoInternal.zego_express_enable_custom_video_render(true, renderConfigPtr);
                            string log = string.Format("zego_express_enable_custom_video_render  errorCode {0}", errorCode);
                            ZegoUtil.ZegoPrivateLog(errorCode, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO);
                            ZegoUtil.ReleaseStructPointer(renderConfigPtr);
                            Debug.Log("create Enigne success!!!");
                            enginePtr = new ZegoExpressEngineImpl();
                        }
                    }
                }
            }
            return enginePtr;

        }

        private static void RegisterCallback()
        {
            IExpressEngineInternal.zego_register_engine_uninit_callback(zego_on_engine_uninit, IntPtr.Zero);
            IExpressEngineInternal.zego_register_network_quality_callback(zego_on_network_quality, IntPtr.Zero);

            IExpressRoomInternal.zego_register_room_state_update_callback(zego_on_room_state_update, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_user_update_callback(zego_on_room_user_update, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_stream_update_callback(zego_on_room_stream_update, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_login_result_callback(zego_on_room_login_result, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_logout_result_callback(zego_on_room_logout_result, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_state_changed_callback(zego_on_room_state_changed, IntPtr.Zero);

            IExpressPublisherInternal.zego_register_publisher_state_update_callback(zego_on_publisher_state_update, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_quality_update_callback(zego_on_publisher_quality_update, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_captured_audio_first_frame_callback(zego_on_publisher_captured_audio_first_frame, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_captured_video_first_frame_callback(zego_on_publisher_captured_video_first_frame, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_video_size_changed_callback(zego_on_publisher_video_size_changed, IntPtr.Zero);
            IExpressCustomVideoInternal.zego_register_custom_video_render_captured_frame_data_callback(zego_on_custom_video_render_captured_frame_data, IntPtr.Zero);
            IExpressCustomVideoInternal.zego_register_custom_video_render_remote_frame_data_callback(zego_on_custom_video_render_remote_frame_data, IntPtr.Zero);
            
#if UNITY_STANDALONE_WIN
            // for windows
            IExpressCustomVideoInternal.zego_register_custom_video_process_captured_unprocessed_raw_data_callback(zego_on_custom_video_process_captured_unprocessed_raw_data, IntPtr.Zero);
#elif UNITY_IOS || UNITY_STANDALONE_OSX
            // for iOS/macOS
            IExpressCustomVideoInternal.zego_register_custom_video_process_captured_unprocessed_cvpixelbuffer_callback(zego_on_custom_video_process_captured_unprocessed_cvpixelbuffer, IntPtr.Zero);
#elif UNITY_ANDROID
            // for android
            IExpressCustomVideoInternal.zego_register_custom_video_process_captured_unprocessed_texture_data_callback(zego_on_custom_video_process_captured_unprocessed_texture_data, IntPtr.Zero);
#endif
            IExpressPlayerInternal.zego_register_player_state_update_callback(zego_on_player_state_update, IntPtr.Zero);
            IExpressPlayerInternal.zego_register_player_quality_update_callback(zego_on_player_quality_update, IntPtr.Zero);
            IExpressPlayerInternal.zego_register_player_media_event_callback(zego_on_player_media_event, IntPtr.Zero);
            IExpressPlayerInternal.zego_register_player_recv_audio_first_frame_callback(zego_on_player_recv_audio_first_frame, IntPtr.Zero);
            IExpressPlayerInternal.zego_register_player_recv_video_first_frame_callback(zego_on_player_recv_video_first_frame, IntPtr.Zero);
            IExpressPlayerInternal.zego_register_player_render_video_first_frame_callback(zego_on_player_render_video_first_frame, IntPtr.Zero);
            IExpressPlayerInternal.zego_register_player_video_size_changed_callback(zego_on_player_video_size_changed, IntPtr.Zero);
            IExpressIMInternal.zego_register_im_recv_barrage_message_callback(zego_on_im_recv_barrage_message, IntPtr.Zero);
            IExpressIMInternal.zego_register_im_recv_broadcast_message_callback(zego_on_im_recv_broadcast_message, IntPtr.Zero);
            IExpressIMInternal.zego_register_im_recv_custom_command_callback(zego_on_im_recv_custom_command, IntPtr.Zero);
            IExpressIMInternal.zego_register_im_send_barrage_message_result_callback(zego_on_im_send_barrage_message_result, IntPtr.Zero);
            IExpressIMInternal.zego_register_im_send_broadcast_message_result_callback(zego_on_im_send_broadcast_message_result, IntPtr.Zero);
            IExpressIMInternal.zego_register_im_send_custom_command_result_callback(zego_on_im_send_custom_command_result, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_update_cdn_url_result_callback(zego_on_publisher_update_cdn_url_result, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_relay_cdn_state_update_callback(zego_on_publisher_relay_cdn_state_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_captured_sound_level_update_callback(zego_on_captured_sound_level_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_remote_sound_level_update_callback(zego_on_remote_sound_level_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_remote_sound_level_info_update_callback(zego_on_remote_sound_level_info_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_captured_sound_level_info_update_callback(zego_on_captured_sound_level_info_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_captured_audio_spectrum_update_callback(zego_on_captured_audio_spectrum_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_remote_audio_spectrum_update_callback(zego_on_remote_audio_spectrum_update, IntPtr.Zero);
            IExpressPlayerInternal.zego_register_player_recv_sei_callback(zego_on_player_recv_sei, IntPtr.Zero);
            IExpressEngineInternal.zego_register_debug_error_callback(zego_on_debug_error, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_stream_extra_info_update_callback(zego_on_room_stream_extra_info_update, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_update_stream_extra_info_result_callback(zego_on_publisher_update_stream_extra_info_result, IntPtr.Zero);
            IExpressCustomVideoInternal.zego_register_custom_video_capture_start_callback(zego_on_custom_video_capture_start, IntPtr.Zero);
            IExpressCustomVideoInternal.zego_register_custom_video_capture_stop_callback(zego_on_custom_video_capture_stop, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_set_room_extra_info_result_callback(zego_on_room_set_room_extra_info_result, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_extra_info_update_callback(zego_on_room_extra_info_update, IntPtr.Zero);
            IExpressEngineInternal.zego_register_engine_state_update_callback(zego_on_engine_state_update, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_online_user_count_update_callback(zego_on_room_online_user_count_update, IntPtr.Zero);
            IExpressMixerInternal.zego_register_mixer_start_result_callback(zego_on_mixer_start_result, IntPtr.Zero);
            IExpressMixerInternal.zego_register_mixer_stop_result_callback(zego_on_mixer_stop_result, IntPtr.Zero);
            IExpressMixerInternal.zego_register_auto_mixer_start_result_callback(zego_on_auto_mixer_start_result, IntPtr.Zero);
            IExpressMixerInternal.zego_register_auto_mixer_stop_result_callback(zego_on_auto_mixer_stop_result, IntPtr.Zero);
            IExpressMixerInternal.zego_register_mixer_relay_cdn_state_update_callback(zego_on_mixer_relay_cdn_state_update, IntPtr.Zero);
            IExpressMixerInternal.zego_register_mixer_sound_level_update_callback(zego_on_mixer_sound_level_update, IntPtr.Zero);
            IExpressMixerInternal.zego_register_auto_mixer_sound_level_update_callback(zego_on_auto_mixer_sound_level_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_device_error_callback(zego_on_device_error, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_remote_camera_state_update_callback(zego_on_remote_camera_state_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_remote_mic_state_update_callback(zego_on_remote_mic_state_update, IntPtr.Zero);
            IExpressAudioEffectPlayerInternal.zego_register_audio_effect_player_seek_to_callback(zego_on_audio_effect_player_seek_to, IntPtr.Zero);
            IExpressAudioEffectPlayerInternal.zego_register_audio_effect_player_load_resource_callback(zego_on_audio_effect_player_load_resource, IntPtr.Zero);
            IExpressAudioEffectPlayerInternal.zego_register_audio_effect_play_state_update_callback(zego_on_audio_effect_play_state_update, IntPtr.Zero);
            IExpressCustomAudioIOInternal.zego_register_captured_audio_data_callback(zego_on_captured_audio_data, IntPtr.Zero);
            IExpressCustomAudioIOInternal.zego_register_mixed_audio_data_callback(zego_on_mixed_audio_data, IntPtr.Zero);
            IExpressCustomAudioIOInternal.zego_register_playback_audio_data_callback(zego_on_playback_audio_data, IntPtr.Zero);
            IExpressCustomAudioIOInternal.zego_register_player_audio_data_callback(zego_on_player_audio_data, IntPtr.Zero);
            IExpressRecordInternal.zego_register_captured_data_record_state_update_callback(zego_on_captured_data_record_state_update,IntPtr.Zero);
            IExpressRecordInternal.zego_register_captured_data_record_progress_update_callback(zego_on_captured_data_record_progress_update,IntPtr.Zero);
            IExpressRangeAudioInternal.zego_register_range_audio_microphone_state_update_callback(zego_on_range_audio_microphone_state_update, IntPtr.Zero);
            IExpressEngineInternal.zego_register_recv_experimental_api_callback(zego_on_recv_experimental_api, IntPtr.Zero);
            IExpressEngineInternal.zego_register_api_called_result_callback(zego_on_api_called_result, IntPtr.Zero);
            IExpressRoomInternal.zego_register_room_token_will_expire_callback(zego_on_room_token_will_expire, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_remote_speaker_state_update_callback(zego_on_remote_speaker_state_update, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_local_device_exception_occurred_callback(zego_on_local_device_exception_occurred, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_render_video_first_frame_callback(zego_on_publisher_render_video_first_frame, IntPtr.Zero);
            IExpressPublisherInternal.zego_register_publisher_take_snapshot_result_callback(zego_on_publisher_take_snapshot_result, IntPtr.Zero);
            IExpressDeviceInternal.zego_register_audio_route_change_callback(zego_on_audio_route_change, IntPtr.Zero);
            // Media Player
            IExpressMediaPlayerInternal.zego_register_media_player_audio_frame_callback(zego_on_media_player_audio_frame, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_video_frame_callback(zego_on_media_player_video_frame, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_state_update_callback(zego_on_media_player_state_update, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_network_event_callback(zego_on_media_player_network_event, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_playing_progress_callback(zego_on_media_player_playing_progress, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_recv_sei_callback(zego_on_media_player_recv_sei, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_sound_level_update_callback(zego_on_media_player_sound_level_update, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_frequency_spectrum_update_callback(zego_on_media_player_frequency_spectrum_update, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_load_resource_callback(zego_on_media_player_load_resource, IntPtr.Zero);
            IExpressMediaPlayerInternal.zego_register_media_player_seek_to_callback(zego_on_media_player_seek_to, IntPtr.Zero);
            // Copyrighted Music
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_download_progress_update_callback(zego_on_copyrighted_music_download_progress_update, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_current_pitch_value_update_callback(zego_on_copyrighted_music_current_pitch_value_update, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_init_callback(zego_on_copyrighted_music_init, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_send_extended_request_callback(zego_on_copyrighted_music_send_extended_request, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_get_lrc_lyric_callback(zego_on_copyrighted_music_get_lrc_lyric, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_get_krc_lyric_by_token_callback(zego_on_copyrighted_music_get_krc_lyric_by_token, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_request_song_callback(zego_on_copyrighted_music_request_song, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_request_accompaniment_callback(zego_on_copyrighted_music_request_accompaniment, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_request_accompaniment_clip_callback(zego_on_copyrighted_music_request_accompaniment_clip, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_get_music_by_token_callback(zego_on_copyrighted_music_get_music_by_token, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_download_callback(zego_on_copyrighted_music_download, IntPtr.Zero);
            IExpressCopyrightedMusicInternal.zego_register_copyrighted_music_get_standard_pitch_callback(zego_on_copyrighted_music_get_standard_pitch, IntPtr.Zero);

        }

        private static void InitCallBackGameObject()
        {
            zegoGameObj = new GameObject(ZegoConstans.ZEGO_CALLBACK_GAME_OBJ_NAME);
            zegoGameObj.AddComponent<ZegoCallBackQueue>();
            GameObject.DontDestroyOnLoad(zegoGameObj);
            zegoGameObj.hideFlags = HideFlags.HideInHierarchy;
            callBackQueue = zegoGameObj.GetComponent<ZegoCallBackQueue>();
        }

        public static void UnInitCallBackGameObject()
        {
            GameObject obj = GameObject.Find(ZegoConstans.ZEGO_CALLBACK_GAME_OBJ_NAME);
            if (!ReferenceEquals(obj, null))
            {
                callBackQueue.ClearActionQueue();
                GameObject.Destroy(zegoGameObj);
                zegoGameObj = null;
                callBackQueue = null;
            }
        }




        public static new void DestroyEngine(IZegoDestroyCompletionCallback onDestroyCompletion = null)
        {
            if (enginePtr != null) //双if +lock
            {
                lock (zegoExpressEngineLock)
                {
                    if (enginePtr != null)
                    {
                        ClearAll();
                        ZegoExpressEngineImpl.onDestroyCompletion = onDestroyCompletion;
                        IExpressEngineInternal.zego_express_engine_uninit_async();//异步回调，不能在结果回调前release内存,否则异常
                    }
                }
            }
        }

        public static void release()
        {
            enginePtr = null;
            jvm = IntPtr.Zero;
            application = IntPtr.Zero;
            //engineConfig = new zego_engine_config();
            //setEngineConfigFlag = false;
        }

        public static void ClearAll()
        {
            // Destroy
            if (audioEffectPlayerAndIndex != null)
            {
                foreach (var item in audioEffectPlayerAndIndex)
                {
                    enginePtr.DestroyAudioEffectPlayer(item.Value);
                }
            }
            if(mediaPlayerAndIndex != null)
            {
                foreach(var item in mediaPlayerAndIndex)
                {
                    enginePtr.DestroyMediaPlayer(item.Value);
                }
            }
            if(rangeAudioAndIndex != null)
            {
                foreach(var item in rangeAudioAndIndex)
                {
                    enginePtr.DestroyRangeAudio(item.Value);
                }
            }
            if(copyrightedMusicInstance != null)
            {
                enginePtr.DestroyCopyrightedMusic(copyrightedMusicInstance);
                copyrightedMusicInstance = null;
            }
            audioEffectPlayerAndIndex.Clear();
            mediaPlayerAndIndex.Clear();

            onIMSendBarrageMessageResultDics.Clear();
            onIMSendBroadcastMessageResultDics.Clear();
            onIMSendCustomCommandResultDics.Clear();
            onPublisherSetStreamExtraInfoResultDics.Clear();
            onPublisherUpdateCdnUrlResultDics.Clear();
            onRoomSetRoomExtraInfoResultDics.Clear();
            onMixerStartResultDics.Clear();
            onMixerStopResultDics.Clear();
            onAutoMixerStartResultDics.Clear();
            onAutoMixerStopResultDics.Clear();
            onPlayerTakeSnapshotResultDics.Clear();
            onPublisherTakeSnapshotResultDics.Clear();
            seekToTimeCallbackDic.Clear();

            sendExtendedRequestCallbackDic.Clear();
            getLrcLyricCallbackDic.Clear();
            getKrcLyricByTokenCallbackDic.Clear();
            requestSongCallbackDic.Clear();
            requestAccompanimentCallbackDic.Clear();
            requestAccompanimentClipCallbackDic.Clear();
            getMusicByTokenCallbackDic.Clear();
            downloadCallbackDic.Clear();
            getStandatdPitchCallbackDic.Clear();
            onRoomLoginResultDics.Clear();
            onRoomLogoutResultDics.Clear();

            ClearPlayInfo();
        }
        public override void SetAudioCaptureStereoMode(ZegoAudioCaptureStereoMode mode)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_audio_capture_stereo_mode(mode);
                string log = string.Format("SetAudioCaptureStereoMode  mode {0}", mode);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public override ZegoAudioConfig GetAudioConfig()
        {
            ZegoAudioConfig zegoAudioConfig = null;
            if (enginePtr != null)
            {
                zego_audio_config zego_Audio_Config = IExpressPublisherInternal.zego_express_get_audio_config();
                zegoAudioConfig = ChangeAudioConfigStructToClass(zego_Audio_Config);
                string log = string.Format("GetAudioConfig");
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
            return zegoAudioConfig;
        }
        public override ZegoVideoConfig GetVideoConfig(ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            ZegoVideoConfig zegoVideoConfig = null;
            if (enginePtr != null)
            {
                zego_video_config zego_Video_Config = IExpressPublisherInternal.zego_express_get_video_config(channel);
                zegoVideoConfig = ChangeVideoConfigStructToClass(zego_Video_Config);
                string log = string.Format("GetVideoConfig");
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
            return zegoVideoConfig;
        }


        public override void SwitchRoom(string fromRoomID, string toRoomID, ZegoRoomConfig config = null)
        {
            if (enginePtr != null)
            {
                IntPtr ptr = ChangeZegoRoomConfigClassToStructPoniter(config);
                int result = IExpressRoomInternal.zego_express_switch_room(fromRoomID, toRoomID, ptr);
                ZegoUtil.ReleaseStructPointer(ptr);
                string log = string.Format("SwitchRoom  fromRoomID:{0}  toRoomID:{1} result:{2}", fromRoomID, toRoomID, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_ROOM);
            }
        }

        [Obsolete("This method has been deprecated after version 2.9.0 If you want to access the multi-room feature, Please set [setRoomMode] to select multi-room mode before the engine started, and then call [loginRoom] to use multi-room. If you call [loginRoom] function to log in to multiple rooms, please make sure to pass in the same user information.",false)]
        public override void LoginMultiRoom(string roomID, ZegoRoomConfig config = null)
        {
            if (enginePtr != null)
            {
                IntPtr ptr = ChangeZegoRoomConfigClassToStructPoniter(config);
                int result = IExpressRoomInternal.zego_express_login_multi_room(roomID, ptr);
                ZegoUtil.ReleaseStructPointer(ptr);
                string log = string.Format("LoginMultiRoom  roomId:{0}  result:{1}", roomID, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_ROOM);

            }
        }
        public override void SetRoomExtraInfo(string roomID, string key, string value, OnRoomSetRoomExtraInfoResult onRoomSetRoomExtraInfoResult)
        {
            if (enginePtr != null)
            {

                int result = IExpressRoomInternal.zego_express_set_room_extra_info(roomID, key, value);
                string log = string.Format("SetRoomExtraInfo roomID:{0}  key:{1} value:{2} result:{3}", roomID, key, value, result);
                onRoomSetRoomExtraInfoResultDics.AddOrUpdate(result, onRoomSetRoomExtraInfoResult, (key1, oldvalue) => onRoomSetRoomExtraInfoResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }
        public override void SendCustomVideoCaptureRawData(byte[] data, uint dataLength, ZegoVideoFrameParam param, ulong referenceTimeMillisecond, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                zego_video_frame_param zego_Video_Frame_Param = ChangeZegoVideoFrameParamClassToStruct(param);
                int result = IExpressCustomVideoInternal.zego_express_send_custom_video_capture_raw_data(data, dataLength, zego_Video_Frame_Param, referenceTimeMillisecond, 1000, channel);
                //string log = string.Format("SendCustomVideoCaptureRawData result:{0}", result);

                if (channel == ZegoPublishChannel.Main)
                {
                    enginePtr.localVideoBufMain.WriteBuffer(data, param.width, param.height, param.strides, param.format);
                }
                else
                {
                    enginePtr.localVideoBufAux.WriteBuffer(data, param.width, param.height, param.strides, param.format);
                }
                //ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, false);

            }
        }
        public override void SendCustomVideoCaptureRawData(IntPtr data, uint dataLength, ZegoVideoFrameParam param, ulong referenceTimeMillisecond, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                zego_video_frame_param zego_Video_Frame_Param = ChangeZegoVideoFrameParamClassToStruct(param);
                int result = IExpressCustomVideoInternal.zego_express_send_custom_video_capture_raw_data(data, dataLength, zego_Video_Frame_Param, referenceTimeMillisecond, 1000, channel);
                //string log = string.Format("SendCustomVideoCaptureRawData result:{0}", result);

                if (channel == ZegoPublishChannel.Main)
                {
                    enginePtr.localVideoBufMain.WriteBuffer(data, param.width, param.height, param.strides, param.format);
                }
                else
                {
                    enginePtr.localVideoBufAux.WriteBuffer(data, param.width, param.height, param.strides, param.format);
                }
                //ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, false);

            }
        }



        public override void EnableCustomVideoCapture(bool enable, ZegoCustomVideoCaptureConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                zego_custom_video_capture_config zego_Custom_Video_Capture_Config = ChangeCustomVideoCaptureConfigClassToStruct(config);
                IntPtr ptr = ZegoUtil.GetStructPointer(zego_Custom_Video_Capture_Config);
                int result = IExpressCustomVideoInternal.zego_express_enable_custom_video_capture(enable, ptr, channel);
                string log = string.Format("EnableCustomVideoCapture  enable:{0}  channel:{1}   result:{2}", enable, channel, result);
                ZegoUtil.ReleaseStructPointer(ptr);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO);
            }
        }



        public override void LoginRoom(string roomId, ZegoUser user, ZegoRoomConfig config = null)
        {
            if (enginePtr != null)
            {
                zego_user zegoUser = ChangeZegoUserClassToStruct(user);
                int error_code = 0;
                if (config == null)
                {
                    error_code = IExpressRoomInternal.zego_express_login_room(roomId, zegoUser, IntPtr.Zero);
                    Debug.Log("LoginRoom ZegoRoomConfig is null");
                }
                else
                {
                    IntPtr ptr = ChangeZegoRoomConfigClassToStructPoniter(config);
                    error_code = IExpressRoomInternal.zego_express_login_room(roomId, zegoUser, ptr);
                    ZegoUtil.ReleaseStructPointer(ptr);
                }
                string log = string.Format("LoginRoom  roomId:{0}  userId:{1}  userName:{2} result:{3}", roomId, zegoUser.user_id, zegoUser.user_name, error_code);

                ZegoUtil.ZegoPrivateLog(error_code, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_ROOM);
            }
        }

        public override void LoginRoom(string roomID, ZegoUser user, ZegoRoomConfig config, OnRoomLoginResult callback = null)
        {
            if (enginePtr != null)
            {
                zego_user zegoUser = ChangeZegoUserClassToStruct(user);
                int seq = 0;

                IntPtr ptr = ChangeZegoRoomConfigClassToStructPoniter(config);
                seq = IExpressRoomInternal.zego_express_login_room_with_callback(roomID, zegoUser, ptr);
                if(ptr != IntPtr.Zero)
                {
                    ZegoUtil.ReleaseStructPointer(ptr);
                }

                string log = string.Format("LoginRoom  roomID:{0}  userId:{1}  userName:{2} seq:{3}", roomID, zegoUser.user_id, zegoUser.user_name, seq);

                ZegoUtil.ZegoPrivateLog(0, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_ROOM);

                if(callback != null)
                {
                    onRoomLoginResultDics.AddOrUpdate(seq, callback, (key, old_value)=>callback);
                }
            }
        }

        public override void LogoutRoom(OnRoomLogoutResult callback = null)
        {
            if(enginePtr != null)
            {
                int seq = IExpressRoomInternal.zego_express_logout_all_room_with_callback();
                string log = string.Format("LogoutRoom seq:{0}", seq);
                ZegoUtil.ZegoPrivateLog(0, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_ROOM);

                if(callback != null)
                {
                    onRoomLogoutResultDics.AddOrUpdate(seq, callback, (key, old_value)=>callback);
                }
            }
        }

        public override void LogoutRoom(string roomId, OnRoomLogoutResult callback = null)
        {
            if (enginePtr != null)
            {
                int seq = IExpressRoomInternal.zego_express_logout_room_with_callback(roomId);
                string log = string.Format("LogoutRoom roomId:{0}  seq:{1}", roomId, seq);
                ZegoUtil.ZegoPrivateLog(0, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_ROOM);

                if(callback != null)
                {
                    onRoomLogoutResultDics.AddOrUpdate(seq, callback, (key, old_value)=>callback);
                }
            }
        }

        public override void StartPublishingStream(string streamID, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int error_code = IExpressPublisherInternal.zego_express_start_publishing_stream(streamID, channel);
                string log = string.Format("StartPublishingStream streamID:{0} channel:{1} result:{2}", streamID, channel, error_code);
                ZegoUtil.ZegoPrivateLog(error_code, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void StartPublishingStream(string streamID, ZegoPublisherConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                zego_publisher_config config_struct = ChangeZegoPublisherConfigToStruct(config);
                int error_code = IExpressPublisherInternal.zego_express_start_publishing_stream_with_config(streamID, config_struct, channel);
                string log = string.Format("StartPublishingStream streamID:{0} roomID:{1}, channel:{2} result:{3}", streamID, config.roomID, channel, error_code);
                ZegoUtil.ZegoPrivateLog(error_code, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void StopPublishingStream(ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int error_code = IExpressPublisherInternal.zego_express_stop_publishing_stream(channel);
                string log = string.Format("StopPublishingStream channel:{0} result:{1}", channel, error_code);
                ZegoUtil.ZegoPrivateLog(error_code, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public override void SendBarrageMessage(string roomID, string message, OnIMSendBarrageMessageResult onIMSendBarrageMessageResult)
        {
            if (enginePtr != null)
            {

                int result = IExpressIMInternal.zego_express_send_barrage_message(roomID, message);
                string log = string.Format("SendBarrageMessage roomID:{0}  message:{1} result:{2}", roomID, message, result);
                onIMSendBarrageMessageResultDics.AddOrUpdate(result, onIMSendBarrageMessageResult, (key, oldValue) => onIMSendBarrageMessageResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }

        public override void SendBroadcastMessage(string roomID, string message, OnIMSendBroadcastMessageResult onIMSendBroadcastMessageResult)
        {
            if (enginePtr != null)
            {
                int result = IExpressIMInternal.zego_express_send_broadcast_message(roomID, message);
                string log = string.Format("SendBroadcastMessage roomID:{0}  message:{1} result:{2}", roomID, message, result);
                onIMSendBroadcastMessageResultDics.AddOrUpdate(result, onIMSendBroadcastMessageResult, (key, oldValue) => onIMSendBroadcastMessageResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }
        public override void SetStreamExtraInfo(string extraInfo, OnPublisherSetStreamExtraInfoResult onPublisherSetStreamExtraInfoResult, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {

                int result = IExpressPublisherInternal.zego_express_set_stream_extra_info(extraInfo, channel);
                string log = string.Format("SetStreamExtraInfo extraInfo:{0}  channel:{1} result:{2}", extraInfo, channel, result);
                onPublisherSetStreamExtraInfoResultDics.AddOrUpdate(result, onPublisherSetStreamExtraInfoResult, (key, oldValue) => onPublisherSetStreamExtraInfoResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }
        public override void SendCustomCommand(string roomID, string command, List<ZegoUser> toUserList, OnIMSendCustomCommandResult onIMSendCustomCommandResult)
        {
            if (enginePtr != null)
            {

                zego_user[] zegoUsers = ChangeZegoUserClassListToStructList(toUserList);
                int result = IExpressIMInternal.zego_express_send_custom_command(roomID, command, zegoUsers, (uint)zegoUsers.Length);
                string log = string.Format("SendCustomCommand roomID:{0}  command:{1} result:{2}", roomID, command, result);
                onIMSendCustomCommandResultDics.AddOrUpdate(result, onIMSendCustomCommandResult, (key, oldValue) => onIMSendCustomCommandResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }
        public override void AddPublishCdnUrl(string streamID, string targetURL, OnPublisherUpdateCdnUrlResult onPublisherUpdateCdnUrlResult)
        {
            if (enginePtr != null)
            {

                int result = IExpressPublisherInternal.zego_express_add_publish_cdn_url(streamID, targetURL);
                string log = string.Format("AddPublishCdnUrl streamID:{0} targetURL:{1} result:{2}", streamID, targetURL, result);
                onPublisherUpdateCdnUrlResultDics.AddOrUpdate(result, onPublisherUpdateCdnUrlResult, (key, oldValue) => onPublisherUpdateCdnUrlResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }
        public override void RemovePublishCdnUrl(string streamID, string targetURL, OnPublisherUpdateCdnUrlResult onPublisherUpdateCdnUrlResult)
        {
            if (enginePtr != null)
            {

                int result = IExpressPublisherInternal.zego_express_remove_publish_cdn_url(streamID, targetURL);
                string log = string.Format("RemovePublishCdnUrl streamID:{0} targetURL:{1} result:{2}", streamID, targetURL, result);
                onPublisherUpdateCdnUrlResultDics.AddOrUpdate(result, onPublisherUpdateCdnUrlResult, (key, oldValue) => onPublisherUpdateCdnUrlResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }






        public override ZegoDeviceInfo[] GetAudioDeviceList(ZegoAudioDeviceType deviceType)
        {
#if UNITY_ANDROID || UNITY_IPHONE
            throw new Exception("Only Windows and Mac support  GetAudioDeviceList");
#else
            ZegoDeviceInfo[] zegoDeviceInfos = null;
            if (enginePtr != null)
            {
                int count = 0;
                IntPtr deviceInfo = IExpressDeviceInternal.zego_express_get_audio_device_list(deviceType, ref count);
                zego_device_info[] zego_Device_Info = new zego_device_info[count];
                ZegoUtil.GetStructListByPtr<zego_device_info>(ref zego_Device_Info, deviceInfo, count);
                zegoDeviceInfos = ChangeZegoDeviceInfoStructListToClassList(zego_Device_Info);
                Debug.Log(string.Format("GetAudioDeviceList count:{0}", count));

            }
            return zegoDeviceInfos;
#endif
        }




        public override ZegoDeviceInfo[] GetVideoDeviceList()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            throw new Exception("Only Windows and Mac support  GetVideoDeviceList");
#else
            ZegoDeviceInfo[] zegoDeviceInfos = null;
            if (enginePtr != null)
            {
                int count = 0;
                IntPtr deviceInfo = IExpressDeviceInternal.zego_express_get_video_device_list(ref count);
                zego_device_info[] zego_Device_Info = new zego_device_info[count];
                ZegoUtil.GetStructListByPtr<zego_device_info>(ref zego_Device_Info, deviceInfo, count);
                zegoDeviceInfos = ChangeZegoDeviceInfoStructListToClassList(zego_Device_Info);
                Debug.Log(string.Format("GetVideoDeviceList count:{0}", count));

            }
            return zegoDeviceInfos;
#endif
        }
        public override void UseAudioDevice(ZegoAudioDeviceType deviceType, string deviceID)
        {
#if UNITY_ANDROID||  UNITY_IPHONE
            throw new Exception("Only Windows and Mac support  UseAudioDevice");
#else

            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_use_audio_device(deviceType, deviceID);
                string log = string.Format("UseAudioDevice result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
#endif

        }
        public override void UseVideoDevice(string deviceID, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {

#if UNITY_ANDROID || UNITY_IPHONE
            throw new Exception("Only Windows and Mac support UseVideoDevice");
#else 
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_use_video_device(deviceID, channel);
                string log = string.Format("UseVideoDevice result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
#endif
        }

        public override void StartPreview(ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_start_preview(System.IntPtr.Zero, channel);
                string log = string.Format("StartPreview  channel:{0} result:{1}", channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);

            }
        }
        public override void StopPreview(ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_stop_preview(channel);
                string log = string.Format("StopPreview  channel:{0} result:{1}", channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public override void StartPlayingStream(string streamId, ZegoPlayerConfig config = null)
        {
            if (enginePtr != null)
            {
                if (GetPlayerBufferByStreamId(streamId) == null)
                {
                    AddPlayerItem(streamId);
                }
                int result = 0;
                if (config != null)
                {
                    zego_player_config zego_Player_Config = ChangeZegoPlayerConfigClassToStruct(config);
                    result = IExpressPlayerInternal.zego_express_start_playing_stream_with_config(streamId, IntPtr.Zero, zego_Player_Config);
                    if (zego_Player_Config.cdn_config != IntPtr.Zero)
                    {
                        ZegoUtil.ReleaseStructPointer(zego_Player_Config.cdn_config);
                    }
                }
                else
                {
                    Debug.Log("StartPlayingStream ZegoPlayerConfig is null");
                    result = IExpressPlayerInternal.zego_express_start_playing_stream(streamId, System.IntPtr.Zero);
                }
                string log = string.Format("StartPlayingStream streamId:{0} result:{1}", streamId, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PLAYER);
            }
        }



        public override void EnablePublishDirectToCDN(bool enable, ZegoCDNConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                IntPtr ptr = ZegoUtil.GetStructPointer(ChangeCDNConfigClassToStruct(config));
                int result = IExpressPublisherInternal.zego_express_enable_publish_direct_to_cdn(enable, ptr, channel);
                ZegoUtil.ReleaseStructPointer(ptr);
                string log = string.Format("EnablePublishDirectToCDN enable:{0} channel:{1} result:{2}", enable, channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void SetAppOrientation(ZegoOrientation orientation, ZegoPublishChannel channel = ZegoPublishChannel.Main)//设置采集视频的朝向（仅Android平台）,逆时针
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_app_orientation(orientation, channel);
                string log = string.Format("SetAppOrientation orientation:{0} channel:{1} result:{2}", orientation, channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }







        public override void StopPlayingStream(string streamId)
        {
            if (enginePtr != null)
            {
                RemovePlayerItemByStreamId(streamId);
                int result = IExpressPlayerInternal.zego_express_stop_playing_stream(streamId);
                string log = string.Format("StopPlayingStream streamId:{0} result:{1}", streamId, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PLAYER);
            }
        }

        public override void SetVideoConfig(ZegoVideoConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_video_config(ChangeVideoConfigClassToStruct(config), channel);
                string log = string.Format("SetVideoConfig channel:{0}  result:{1}", channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void SetVideoMirrorMode(ZegoVideoMirrorMode mirrorMode, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_video_mirror_mode(mirrorMode, channel);
                string log = string.Format("SetVideoMirrorMode mirrorMode:{0}  channel:{1}  result:{2}", mirrorMode, channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void SetAudioConfig(ZegoAudioConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_audio_config_by_channel(ChangeAudioConfigClassToStruct(config), channel);
                string log = string.Format("SetAudioConfig result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void MutePublishStreamAudio(bool mute, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_mute_publish_stream_audio(mute, channel);
                string log = string.Format("MutePublishStreamAudio mute:{0} channel:{1} result:{2}", mute, channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public override void MutePublishStreamVideo(bool mute, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_mute_publish_stream_video(mute, channel);
                string log = string.Format("MutePublishStreamVideo mute:{0} channel:{1} result:{2}", mute, channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public override void EnableTrafficControl(bool enable, int property)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_enable_traffic_control(enable, property);
                string log = string.Format("EnableTrafficControl enable:{0} property:{1} result:{2}", enable, property, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public override void SetMinVideoBitrateForTrafficControl(int bitrate, ZegoTrafficControlMinVideoBitrateMode mode)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_min_video_bitrate_for_traffic_control(bitrate, mode);
                string log = string.Format("SetMinVideoBitrateForTrafficControl bitrate:{0} mode:{1} result:{2}", bitrate, mode, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public override void SetCaptureVolume(int volume)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_capture_volume(volume);
                string log = string.Format("SetCaptureVolume volume:{0} result:{1}", volume, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public override void EnableHardwareEncoder(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_enable_hardware_encoder(enable);
                string log = string.Format("EnableHardwareEncoder enable:{0} result:{1}", enable, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }
        public static new string GetVersion()
        {
            string reuslt = ZegoUtil.PtrToString(IExpressEngineInternal.zego_express_get_version());
            string log = string.Format("GetVersion SDK version:{0}", reuslt);
            ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            return reuslt;

        }
        public static new ZegoExpressEngine GetEngine()
        {
            lock(zegoExpressEngineLock)
            {
                return enginePtr;
            }
        }
        public override void UploadLog()
        {
            if (enginePtr != null)
            {
                IExpressEngineInternal.zego_express_upload_log();

            }
        }
        public override void SetCapturePipelineScaleMode(ZegoCapturePipelineScaleMode mode)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_capture_pipeline_scale_mode(mode);
                string log = string.Format("SetCapturePipelineScaleMode mode:{0} result:{1}", mode, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        [Obsolete("This function is deprecated in version 2.3.0, please use [enableDebugAssistant] to achieve the original function.",false)]
        public override void SetDebugVerbose(bool enable, ZegoLanguage language)
        {
            if (enginePtr != null)
            {
                IExpressEngineInternal.zego_express_set_debug_verbose(enable, language);
                string log = string.Format("SetDebugVerbose enable:{0} language:{1}", enable, language);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }
        public override void SetPlayVolume(string streamId, int volume)
        {
            if (enginePtr != null)
            {
                int result = IExpressPlayerInternal.zego_express_set_play_volume(streamId, volume);
                string log = string.Format("SetPlayVolume streamId:{0} volume:{1} result:{2}", streamId, volume, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PLAYER);
            }
        }
        public override void MutePlayStreamAudio(string streamId, bool mute)
        {
            if (enginePtr != null)
            {
                int result = IExpressPlayerInternal.zego_express_mute_play_stream_audio(streamId, mute);
                string log = string.Format("MutePlayStreamAudio streamId:{0} mute:{1} result:{2}", streamId, mute, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PLAYER);
            }
        }
        public override void MutePlayStreamVideo(string streamId, bool mute)
        {
            if (enginePtr != null)
            {
                int result = IExpressPlayerInternal.zego_express_mute_play_stream_video(streamId, mute);
                string log = string.Format("MutePlayStreamVideo streamId:{0} mute:{1} result:{2}", streamId, mute, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PLAYER);
            }
        }
        public override void EnableCheckPoc(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressPlayerInternal.zego_express_enable_check_poc(enable);
                string log = string.Format("EnableCheckPoc enable:{0} result:{1}", enable, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PLAYER);
            }
        }
        public override void UseFrontCamera(bool enable, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_use_front_camera(enable, channel);
                string log = string.Format("UseFrontCamera enable:{0} channel:{1} result:{2}", enable, channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);

            }
        }

        [Obsolete("Deprecated since 2.16.0, please use the [enableEffectsBeauty] function instead.",false)]
        public override void EnableBeautify(int featureBitmask, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            throw new Exception("PC Platform do not support EnableBeautify");
#else
            if (enginePtr != null)
            {

                int result = IExpressPreprocessInternal.zego_express_enable_beautify(featureBitmask, channel);
                string log = string.Format("EnableBeautify featureBitmask:{0}  channel:{1} result:{2}", featureBitmask, channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
            }
#endif
        }

        [Obsolete("Deprecated since 2.16.0, please use the [setEffectsBeautyParam] function instead.",false)]
        public override void SetBeautifyOption(ZegoBeautifyOption option, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            throw new Exception("PC Platform do not support SetBeautifyOption");
#else
            if (enginePtr != null)
            {
                int result = IExpressPreprocessInternal.zego_express_set_beautify_option(ChangeZegoBeautifyOptionToStruct(option), channel);
                string log = string.Format("SetBeautifyOption  channel:{0} result:{1}", channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
            }
#endif
        }



        public override void EnableHardwareDecoder(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressPlayerInternal.zego_express_enable_hardware_decoder(enable);
                string log = string.Format("EnableHardwareDecoder enable:{0} result:{1}", enable, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PLAYER);
            }
        }
        public override void SendSEI(byte[] data, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_send_sei(Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), (uint)data.Length, channel);
                string log = string.Format("SendSEI channel:{0} result:{1}", channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER, false);
            }
        }


        //public override void SetPublishWatermark(ZegoWatermark watermark, bool isPreviewVisible, zego_publish_channel channel = zego_publish_channel.zego_publish_channel_main)
        //{
        //    if (enginePtr != null)
        //    {
        //        IExpressPublisherInternal.zego_express_set_publish_watermark(isPreviewVisible, ZegoUtil.GetStructPointer(ChangeWaterMarkClassToStruct(watermark)), channel);
        //        ZegoUtil.ReleaseAllStructPointers();
        //    }
        //}

        public override void StartAudioSpectrumMonitor(uint millisecond = 100)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_start_audio_spectrum_monitor(millisecond);
                string log = string.Format("StartAudioSpectrumMonitor result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }
        public override void StartSoundLevelMonitor(uint millisecond = 100)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_start_sound_level_monitor(millisecond);
                string log = string.Format("StartSoundLevelMonitor result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }

        public override void StartSoundLevelMonitor(ZegoSoundLevelConfig config)
        {
            if (enginePtr != null)
            {
                zego_sound_level_config config_struct = ChangeZegoSoundLevelConfigToStruct(config);
                int result = IExpressDeviceInternal.zego_express_start_sound_level_monitor_with_config(config_struct);
                string log = string.Format("StartSoundLevelMonitor result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }

        public override void StopAudioSpectrumMonitor()
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_stop_audio_spectrum_monitor();
                string log = string.Format("StopAudioSpectrumMonitor result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }
        public override void StopSoundLevelMonitor()
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_stop_sound_level_monitor();
                string log = string.Format("StopSoundLevelMonitor result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }
        public override void MuteMicrophone(bool mute)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_mute_microphone(mute);
                string log = string.Format("MuteMicrophone mute:{0} result:{1}", mute, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }
        public override void MuteSpeaker(bool mute)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_mute_speaker(mute);
                string log = string.Format("MuteSpeaker mute:{0} result:{1}", mute, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }
        public override void EnableCamera(bool enable, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_enable_camera(enable, channel);
                string log = string.Format("EnableCamera enable:{0} channel:{1} result:{2}", enable, channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }
        public override void StartMixerTask(ZegoMixerTask task, OnMixerStartResult onMixerStartResult)
        {
            if (enginePtr != null)
            {
                zego_mixer_task zego_Mixer_Task = ChangeZegoMixerTaskClassToStruct(task);
                int result = IExpressMixerInternal.zego_express_start_mixer_task(zego_Mixer_Task);//result is seq
                string log = string.Format("StartMixerTask  result:{0}", result);
                ZegoUtil.ReleaseAllStructPointers(mixerPtrArrayList);
                ZegoUtil.ReleaseStructPointer(zego_Mixer_Task.watermark);
                onMixerStartResultDics.AddOrUpdate(result, onMixerStartResult, (key, oldValue) => onMixerStartResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
        }

        public override void StopMixerTask(ZegoMixerTask task, OnMixerStopResult onMixerStopResult)
        {
            if (enginePtr != null)
            {
                zego_mixer_task zego_Mixer_Task = ChangeZegoMixerTaskClassToStruct(task);
                int result = IExpressMixerInternal.zego_express_stop_mixer_task(zego_Mixer_Task);
                string log = string.Format("StopMixerTask  result:{0}", result);
                ZegoUtil.ReleaseAllStructPointers(mixerPtrArrayList);
                ZegoUtil.ReleaseStructPointer(zego_Mixer_Task.watermark);
                onMixerStopResultDics.AddOrUpdate(result, onMixerStopResult, (key, oldValue) => onMixerStopResult);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }

        }
        public override bool IsMicrophoneMuted()
        {
            bool result = false;
            if (enginePtr != null)
            {
                result = IExpressDeviceInternal.zego_express_is_microphone_muted();
                string log = string.Format("IsMicrophoneMuted  result:{0}", result);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
            return result;
        }
        public override bool IsSpeakerMuted()
        {
            bool result = false;
            if (enginePtr != null)
            {
                result = IExpressDeviceInternal.zego_express_is_speaker_muted();
                string log = string.Format("IsSpeakerMuted  result:{0}", result);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
            }
            return result;
        }
        public override void EnableAudioCaptureDevice(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_enable_audio_capture_device(enable);
                string log = string.Format("EnableAudioCaptureDevice  result:{0} enable:{1}", result, enable);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }

        [Obsolete("This function has been deprecated since version 2.3.0 Please use [setAudioRouteToSpeaker] instead.",false)]
        public override void SetBuiltInSpeakerOn(bool enable)
        {

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            throw new Exception("PC Platform do not support SetBuiltInSpeakerOn");
#else
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_set_built_in_speaker_on(enable);
                string log = string.Format("SetBuiltInSpeakerOn  result:{0} enable:{1}", result, enable);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
#endif
        }

        public override void EnableHeadphoneMonitor(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_enable_headphone_monitor(enable);
                string log = string.Format("EnableHeadphoneMonitor  result:{0} enable:{1}", result, enable);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }

        public override void SetHeadphoneMonitorVolume(int volume)
        {
            if (enginePtr != null)
            {
                int result = IExpressDeviceInternal.zego_express_set_headphone_monitor_volume(volume);
                string log = string.Format("SetHeadphoneMonitorVolume  result:{0} volume:{1}", result, volume);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_DEVICE);
            }
        }

        public override bool LoadCaptureVideoDataToTexture(ZegoPublishChannel channelIndex, Texture2D tid, ref int w, ref int h, ref ZegoVideoFrameFormat format)
        {//推流/预览数据渲染（读数据）
            if (enginePtr == null)
            {
                return false;
            }
            if (channelIndex == ZegoPublishChannel.Main)
            {
                return enginePtr.localVideoBufMain.ReadBufferToTextureId(tid, ref w, ref h, ref format);
            }
            else
            {
                return enginePtr.localVideoBufAux.ReadBufferToTextureId(tid, ref w, ref h, ref format);
            }
        }

        public override bool LoadPlayVideoDataToTexture(string streamId, Texture2D tid, ref int w, ref int h, ref ZegoVideoFrameFormat format)
        {//拉流数据渲染（读数据）
            if (enginePtr == null)
            {
                return false;
            }
            VideoDataRingBuffer video_buf = enginePtr.GetPlayerBufferByStreamId(streamId);
            if (video_buf != null)
            {
                return video_buf.ReadBufferToTextureId(tid, ref w, ref h, ref format);
            }
            return false;
        }

        public override ZegoAudioEffectPlayer CreateAudioEffectPlayer()
        {
            if (enginePtr != null)
            {
                ZegoAudioEffectPlayerInstanceIndex index = IExpressAudioEffectPlayerInternal.zego_express_create_audio_effect_player();
                string log = string.Format("CreateAudioEffectPlayer index:{0}", index);
                ZegoUtil.ZegoPrivateLog(0, log, false, 0);
                if (index >= 0)
                {
                    ZegoAudioEffectPlayer zegoAudioEffectPlayer = new ZegoAudioEffectPlayerImpl();
                    audioEffectPlayerAndIndex.AddOrUpdate((int)index, zegoAudioEffectPlayer, (key, oldValue) => zegoAudioEffectPlayer);
                    return zegoAudioEffectPlayer;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public override void DestroyAudioEffectPlayer(ZegoAudioEffectPlayer audioEffectPlayer)
        {
            if (enginePtr != null)
            {
                int index = GetIndexFromObject(audioEffectPlayerAndIndex, audioEffectPlayer);
                if (index == -1)
                {
                    string log1 = "DestroyAudioEffectPlayer not found audioEffectPlayer,may be you have already destroy the audioEffectPlayer or not create it in Fact!";
                    ZegoUtil.ZegoPrivateLog(0, log1, false, 0);
                    return;
                }
                int result = IExpressAudioEffectPlayerInternal.zego_express_destroy_audio_effect_player((ZegoAudioEffectPlayerInstanceIndex)index);
                string log = string.Format("AudioEffectPlayer Destroy index:{0} result:{1} ", index, result);
                ((ZegoAudioEffectPlayerImpl)audioEffectPlayer).onAudioEffectPlayerLoadResourceCallbackDics.Clear();
                ((ZegoAudioEffectPlayerImpl)audioEffectPlayer).onAudioEffectPlayerSeekToCallbackDics.Clear();
                audioEffectPlayerAndIndex.TryRemove(index, out _);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER);
            }
        }

        public override void EnableCustomVideoProcessing(bool enable, ZegoCustomVideoProcessConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if(enginePtr != null)
            {
                zego_custom_video_process_config custom_video_process_config_ = ChangeZegoCustomVideoProcessConfigToStruct(config);
                IntPtr config_ptr_ = ZegoUtil.GetStructPointer(custom_video_process_config_);
                int result = IExpressCustomVideoInternal.zego_express_enable_custom_video_processing(enable, config_ptr_, channel);
                ZegoUtil.ReleaseStructPointer(config_ptr_);
                string log = string.Format("EnableCustomVideoProcessing, enable:{0}, buffer_type:{1}, channel:{2}", enable, config.bufferType, channel);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO);
            }
        }

#if UNITY_STANDALONE_WIN
        public override void SendCustomVideoProcessedRawData(ref IntPtr data, ref uint data_length, ZegoVideoFrameParam param, ulong referenceTimeMillisecond, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if(enginePtr != null)
            {
                zego_video_frame_param param_ = ChangeZegoVideoFrameParamClassToStruct(param);
                int result = IExpressCustomVideoInternal.zego_express_send_custom_video_processed_raw_data(ref data, ref data_length, param_, referenceTimeMillisecond, channel);
            }
        }
#endif

#if UNITY_STANDALONE_OSX || UNITY_IOS
        public override void SendCustomVideoProcessedCVPixelBuffer(IntPtr buffer, ulong timestamp, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if(enginePtr != null)
            {
                int result = IExpressCustomVideoInternal.zego_express_send_custom_video_processed_cv_pixel_buffer(buffer, timestamp, channel);
            }
        }
#endif

#if UNITY_ANDROID
        public override void SendCustomVideoProcessedTextureData(int textureID, int width, int height, ulong referenceTimeMillisecond, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if(enginePtr != null)
            {
                int result = IExpressCustomVideoInternal.zego_express_send_custom_video_processed_texture_data(textureID, width, height, referenceTimeMillisecond, channel);
            }
        }
#endif

        [Obsolete("This function has been deprecated since version 2.7.0, please use [startAudioDataObserver] and [stopAudioDataObserver] instead.",false)]
        public override void EnableAudioDataCallback(bool enable, uint callbackBitMask, ZegoAudioFrameParam param)
        {
            if (enginePtr != null)
            {
                zego_audio_frame_param audio_Frame_Param = ChangeZegoAudioFrameParamClassToStruct(param);
                int result = IExpressCustomAudioIOInternal.zego_express_enable_audio_data_callback(enable, callbackBitMask, audio_Frame_Param);
                string log = string.Format("EnableAudioDataCallback  enable:{0} callbackBitMask:{1} channel:{2} sampleRate:{3} result:{4}", enable, callbackBitMask, param.channel, param.sampleRate, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO);
            }
        }

        public override void EnableCustomAudioIO(bool enable, ZegoCustomAudioConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                zego_custom_audio_config custom_Audio_Config = ChangeZegoCustomAudioConfigClassToStruct(config);
                IntPtr ptr = ZegoUtil.GetStructPointer(custom_Audio_Config);
                int result = IExpressCustomAudioIOInternal.zego_express_enable_custom_audio_io(enable, ptr, channel);
                string log = string.Format("EnableCustomAudioIO  enable:{0} source_type:{1} channel:{2}  result:{3}", enable, config.sourceType, channel, result);
                ZegoUtil.ReleaseStructPointer(ptr);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO);
            }
        }
        public override void EnableAEC(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressPreprocessInternal.zego_express_enable_aec(enable);
                string log = string.Format("EnableAEC enable:{0} result:{1}", enable, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
            }
        }
        public override void EnableHeadphoneAEC(bool enable)
        {
            if (enginePtr != null)
            {
#if UNITY_ANDROID || UNITY_IPHONE
                int result = IExpressPreprocessInternal.zego_express_enable_headphone_aec(enable);
                string log = string.Format("EnableHeadphoneAEC enable:{0} result:{1}", enable, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
#else
                throw new Exception("Only Android and iOS support EnableHeadphoneAEC");
#endif
            }
        }
        public override void SetAECMode(ZegoAECMode mode)
        {
            if (enginePtr != null)
            {
                int result = IExpressPreprocessInternal.zego_express_set_aec_mode(mode);
                string log = string.Format("SetAECMode ZegoAECMode:{0} result:{1}", mode, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
            }
        }
        public override void EnableAGC(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressPreprocessInternal.zego_express_enable_agc(enable);
                string log = string.Format("EnableAGC enable:{0} result:{1}", enable, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
            }
        }
        public override void EnableANS(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressPreprocessInternal.zego_express_enable_ans(enable);
                string log = string.Format("EnableANS enable:{0} result:{1}", enable, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
            }
        }
        public override void EnableTransientANS(bool enable)
        {
            if (enginePtr != null)
            {
                int result = IExpressPreprocessInternal.zego_express_enable_transient_ans(enable);
                string log = string.Format("EnableTransientANS enable:{0} result:{1}", enable, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
            }
        }
        public override void SetANSMode(ZegoANSMode mode)
        {
            if (enginePtr != null)
            {
                int result = IExpressPreprocessInternal.zego_express_set_ans_mode(mode);
                string log = string.Format("SetANSMode ZegoANSMode:{0} result:{1}", mode, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PREPROCESS);
            }
        }

        public override void SetVoiceChangerPreset(ZegoVoiceChangerPreset preset)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_voice_changer_preset(preset);
                string log = string.Format("SetVoiceChangerPreset preset:{0} result:{1}", preset, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void SetVoiceChangerParam(ZegoVoiceChangerParam _param)
        {
            if (enginePtr != null)
            {
                float pitch = _param.pitch;
                int result = IExpressPublisherInternal.zego_express_set_voice_changer_param(pitch);
                string log = string.Format("SetVoiceChangerParam pitch:{0} result:{1}", pitch, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void SetReverbPreset(ZegoReverbPreset preset)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_reverb_preset(preset);
                string log = string.Format("SetReverbPreset preset:{0} result:{1}", preset, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void SetReverbAdvancedParam(ZegoReverbAdvancedParam _param)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_reverb_advanced_param(ChangeZegoReverbAdvancedParamClassToStruct(_param));
                string log = string.Format("SetReverbAdvancedParam result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        public override void SetReverbEchoParam(ZegoReverbEchoParam _param)
        {
            if (enginePtr != null)
            {
                int result = IExpressPublisherInternal.zego_express_set_reverb_echo_param(ChangeZegoReverbEchoParamClassToStruct(_param));
                string log = string.Format("SetReverbEchoParam result:{0}", result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            }
        }

        [Obsolete("This function has been deprecated since version 2.3.0, Please use [setPlayStreamVideoType] instead.",false)]
        public override void SetPlayStreamVideoLayer(string streamID, ZegoPlayerVideoLayer videoLayer)
        {
            if (enginePtr != null)
            {
                int result = IExpressPlayerInternal.zego_express_set_play_stream_video_layer(streamID,videoLayer);
                string log = string.Format("SetPlayStreamVideoLayer streamId:{0} videoLayer:{1} result:{2}", streamID, videoLayer,result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PLAYER);
            }
        }

        public override void StartRecordingCapturedData(ZegoDataRecordConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressRecordInternal.zego_express_start_recording_captured_data(ChangeZegoDataRecordConfigClassToStruct(config), channel);
                string log = string.Format("StartRecordingCapturedData channel:{0} result:{1} filePath:{2} recordType:{3}", channel, result,config.filePath,config.recordType);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_RECORD);
            }
        }
        public override void StopRecordingCapturedData(ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            if (enginePtr != null)
            {
                int result = IExpressRecordInternal.zego_express_stop_recording_captured_data(channel);
                string log = string.Format("StopRecordingCapturedData channel:{0} result:{1}", channel, result);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_RECORD);
            }
        }

        public override ZegoRangeAudio CreateRangeAudio()
        {
            var index = IExpressRangeAudioInternal.zego_express_create_range_audio();
            string log = string.Format("CreateRangeAudio, index:{0}", index);

            if (index == zego_range_audio_instance_index.zego_range_audio_instance_index_null)
            {
                ZegoUtil.ZegoPrivateLog(ZEGO_ERROR_CODE_RANGE_AUDIO_EXCEED_MAX_COUNT, log, true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
                return null;
            }
            else
            {
                ZegoUtil.ZegoPrivateLog(ZEGO_ERROR_CODE_COMMON_SUCCESS, log, true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
                var zegoRangeAudio = new ZegoRangeAudioImpl(index);
                rangeAudioAndIndex.AddOrUpdate(index, zegoRangeAudio, (key, oldValue) => zegoRangeAudio);
                return zegoRangeAudio;
            }
        }

        public static zego_range_audio_instance_index GetIndexFromZegoRangeAudio(ZegoRangeAudio zegoRangeAudio)
        {
            var result = zego_range_audio_instance_index.zego_range_audio_instance_index_null;
            foreach (KeyValuePair<zego_range_audio_instance_index, ZegoRangeAudio> kvp in rangeAudioAndIndex)
            {
                if (kvp.Value == zegoRangeAudio)
                {
                    result = kvp.Key;
                }
            }
            if (result == zego_range_audio_instance_index.zego_range_audio_instance_index_null)
            {
                throw new Exception("GetIndexFromZegoRangeAudio found null, maybe you have already release the range audio instance.");
            }
            else
            {
                return result;
            }
        }

        public override void DestroyRangeAudio(ZegoRangeAudio rangeAudio)
        {
            var index = GetIndexFromZegoRangeAudio(rangeAudio);
            var result = IExpressRangeAudioInternal.zego_express_destroy_range_audio(index);
            string log = string.Format("DestroyRangeAudio, result:{0}", result);
            ZegoUtil.ZegoPrivateLog(result, log, true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioSetAudioReceiveRange(float range, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_range_audio_set_audio_receive_range(range, instance_index);
            string log = string.Format("SetAudioReceiveRange, result:{0}", result);
            ZegoUtil.ZegoPrivateLog(result, log, true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioUpdateSelfPosition(float[] position, float[] axisForward, float[] axisRight, float[] axisUp, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_range_audio_update_self_position(position, axisForward, axisRight, axisUp, instance_index);

            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioUpdateAudioSource(string userID, float[] position, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_range_audio_update_audio_source(userID, position, instance_index);

            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioEnableSpatializer(bool enable, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_range_audio_enable_spatializer(enable, instance_index);

            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioEnableMicrophone(bool enable, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_range_audio_enable_microphone(enable, instance_index);

            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioEnableSpeaker(bool enable, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_range_audio_enable_speaker(enable, instance_index);

            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioSetRangeAudioMode(ZegoRangeAudioMode mode, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_set_range_audio_mode((zego_range_audio_mode)mode, instance_index);

            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioSetTeamID(string teamID, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_range_audio_set_team_id(teamID, instance_index);

            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public static void RangeAudioMuteUser(string userID, bool mute, zego_range_audio_instance_index instance_index)
        {
            int result = IExpressRangeAudioInternal.zego_express_range_audio_mute_user(userID, mute, instance_index);

            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_RANGEAUDIO);
        }

        public override string CallExperimentalAPI(string param)
        {
            string result = ZegoUtil.PtrToString(IExpressEngineInternal.zego_express_call_experimental_api(param));
            return result;
        }

        public override void RenewToken(string roomID, string token)
        {
            int result = IExpressRoomInternal.zego_express_renew_token(roomID, token);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_ROOM);
        }

        public override void TakePlayStreamSnapshot(string streamID, OnPlayerTakeSnapshotResult callback)
        {
            int result = IExpressPlayerInternal.zego_express_take_play_stream_snapshot(streamID);
            onPlayerTakeSnapshotResultDics.AddOrUpdate(streamID, callback, (key, old_value) => callback);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PLAYER);
        }

        public override void TakePublishStreamSnapshot(OnPublisherTakeSnapshotResult callback, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            int result = IExpressPublisherInternal.zego_express_take_publish_stream_snapshot(channel);
            onPublisherTakeSnapshotResultDics.AddOrUpdate((int)channel, callback, (key, old_value) => callback);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PUBLISHER);
        }

        public override void SetAllPlayStreamVolume(int volume)
        {
            int result = IExpressPlayerInternal.zego_express_set_all_play_stream_volume(volume);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PLAYER);
        }

        public override void SetPlayStreamBufferIntervalRange(string streamID, uint minBufferInterval, uint maxBufferInterval)
        {
            int result = IExpressPlayerInternal.zego_express_set_play_stream_buffer_interval_range(streamID, minBufferInterval, maxBufferInterval);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PLAYER);
        }

        public override void SetPlayStreamFocusOn(string streamID)
        {
            int result = IExpressPlayerInternal.zego_express_set_play_stream_focus_on(streamID);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PLAYER);
        }

        public override void MuteAllPlayStreamAudio(bool mute)
        {
            int result = IExpressPlayerInternal.zego_express_mute_all_play_stream_audio(mute);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PLAYER);
        }

        public override void MuteAllPlayStreamVideo(bool mute)
        {
            int result = IExpressPlayerInternal.zego_express_mute_all_play_stream_video(mute);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PLAYER);
        }

        public override void SetPlayStreamVideoType(string streamID, ZegoVideoStreamType streamType)
        {
            int result = IExpressPlayerInternal.zego_express_set_play_stream_video_type(streamID, streamType);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PLAYER);       
        }

#if UNITY_ANDROID || UNITY_IOS
        public override ZegoAudioRoute GetAudioRouteType()
        {
            ZegoUtil.ZegoPrivateLog(0, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, 0), true, ZEGO_EXPRESS_MODULE_DEVICE); 
            return IExpressDeviceInternal.zego_express_get_audio_route_type();
        }
#endif

        public override void EnableVirtualStereo(bool enable, int angle)
        {
            int result = IExpressPublisherInternal.zego_express_enable_virtual_stereo(enable, angle);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PUBLISHER);  
        }

        public override void EnablePlayStreamVirtualStereo(bool enable, int angle, string streamID)
        {
            int result = IExpressPublisherInternal.zego_express_enable_play_stream_virtual_stereo(enable, angle, streamID);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_PUBLISHER);
        }

#if UNITY_ANDROID || UNITY_IOS
        public override void SetAudioRouteToSpeaker(bool defaultToSpeaker)
        {
            int result = IExpressDeviceInternal.zego_express_set_audio_route_to_speaker(defaultToSpeaker);
            ZegoUtil.ZegoPrivateLog(result, string.Format("{0}, result:{1}", MethodBase.GetCurrentMethod().Name, result), true, ZEGO_EXPRESS_MODULE_DEVICE);
        }
#endif

        public override ZegoMediaPlayer CreateMediaPlayer()
        {
            lock(zegoMediaPlayerLock)
            {
                ZegoMediaPlayerInstanceIndex result = IExpressMediaPlayerInternal.zego_express_create_media_player();
                string log = string.Format("CreateMediaPlayer  result:{0}", result);
                ZegoUtil.ZegoPrivateLog(0, log, true, ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
                if (result >= 0)
                {
                    ZegoMediaPlayer zegoMediaPlayer = new ZegoMediaPlayerImpl(result);
                    mediaPlayerAndIndex.AddOrUpdate((int)result, zegoMediaPlayer, (key, oldValue) => zegoMediaPlayer);
                    return zegoMediaPlayer;
                }
                else
                {
                    return null;
                }
            }
        }

        public static ZegoMediaPlayerInstanceIndex GetIndexFromZegoMediaPlayer(ZegoMediaPlayer zegoMediaPlayer)
        {
            ZegoMediaPlayerInstanceIndex result = ZegoMediaPlayerInstanceIndex.Null;
            foreach (KeyValuePair<int, ZegoMediaPlayer> kvp in mediaPlayerAndIndex)
            {
                if (kvp.Value == zegoMediaPlayer)
                {
                    result = (ZegoMediaPlayerInstanceIndex)kvp.Key;
                    break;
                }
            }
            if (result == ZegoMediaPlayerInstanceIndex.Null)
            {
                throw new Exception("GetIndexFromZegoMediaPlayer found null，Maybe you have already release the mediaplayer");
            }
            else
            {
                return result;
            }
        }

        public override void DestroyMediaPlayer(ZegoMediaPlayer mediaPlayer)
        {
            lock(zegoMediaPlayerLock)
            {
                ZegoMediaPlayerInstanceIndex index = GetIndexFromZegoMediaPlayer(mediaPlayer);
                int result = IExpressMediaPlayerInternal.zego_express_destroy_media_player(index);
                string log = string.Format("DestroyMediaPlayer, index:{0} result:{1} ", index, result);
                ((ZegoMediaPlayerImpl)mediaPlayer).ClearAll();
                mediaPlayerAndIndex.TryRemove((int)index, out _);
                ZegoUtil.ZegoPrivateLog(result, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_MEDIAPLAYER);
            }
        }


        public override ZegoCopyrightedMusic CreateCopyrightedMusic()
        {
            lock(zegoCopyrightedMusicLock)
            {
                IExpressCopyrightedMusicInternal.zego_express_create_copyrighted_music();
                ZegoUtil.ZegoPrivateLog(0, "CreateCopyrightedMusic", true, ZegoConstans.ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC);

                copyrightedMusicInstance = new ZegoCopyrightedMusicImpl();

                return copyrightedMusicInstance;
            }
        }

        public override void DestroyCopyrightedMusic(ZegoCopyrightedMusic copyrightedMusic)
        {
            lock(zegoCopyrightedMusicLock)
            {
                int ret = IExpressCopyrightedMusicInternal.zego_express_destroy_copyrighted_music();
                ZegoUtil.ZegoPrivateLog(ret, "DestroyCopyrightedMusic", true, ZegoConstans.ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC);
                copyrightedMusicInstance = null;
            }
        }

        public override void SetStreamAlignmentProperty(int alignment, ZegoPublishChannel channel)
        {
            int ret = IExpressPublisherInternal.zego_express_set_stream_alignment_property(alignment, channel);
            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetStreamAlignmentProperty, alignment:{0}, channel:{1}", alignment, channel), true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
        }

        public override void StartAutoMixerTask(ZegoAutoMixerTask task, OnMixerStartResult callback)
        {
            zego_auto_mixer_task mixer_task = ChangeZegoAutoMixerTaskFromClassToStruct(task);
            int ret = IExpressMixerInternal.zego_express_start_auto_mixer_task(mixer_task);

            ZegoUtil.ReleaseAllStructPointers(autoMixerPtrArrayList);

            onAutoMixerStartResultDics.AddOrUpdate(ret, callback, (key, old_value)=>callback);
            ZegoUtil.ZegoPrivateLog(0, string.Format("StartAutoMixerTask, result:{0}", ret), false, ZegoConstans.ZEGO_EXPRESS_MODULE_MIXER);
        }

        public override void StopAutoMixerTask(ZegoAutoMixerTask task, OnMixerStopResult callback)
        {
            zego_auto_mixer_task mixer_task = ChangeZegoAutoMixerTaskFromClassToStruct(task);
            int ret = IExpressMixerInternal.zego_express_stop_auto_mixer_task(mixer_task);

            ZegoUtil.ReleaseAllStructPointers(autoMixerPtrArrayList);

            onAutoMixerStopResultDics.AddOrUpdate(ret, callback, (key, old_value)=>callback);
            ZegoUtil.ZegoPrivateLog(0, string.Format("StopAutoMixerTask, result:{0}", ret), false, ZegoConstans.ZEGO_EXPRESS_MODULE_MIXER);
        }

        public override ZegoNetworkTimeInfo GetNetworkTimeInfo()
        {
            zego_network_time_info time_info_c = new zego_network_time_info();
            IntPtr ptr_c = ZegoUtil.GetStructPointer(time_info_c);
            int ret = IExpressUtilsInternal.zego_express_get_network_time_info(ptr_c);

            var tt = ZegoUtil.GetStructByPtr<zego_network_time_info>(ptr_c);

            if(ret != 0)
            {
                ZegoUtil.ZegoPrivateLog(ret, string.Format("GetNetworkTimeInfo, ret:{0}", ret), true, ZegoConstans.ZEGO_EXPRESS_MODULE_UTILITIES);
            }

            ZegoNetworkTimeInfo time_info = ZegoCallBackChangeUtil.ChangeZegoNetworkTimeInfoStructToClass(tt);

            ZegoUtil.ReleaseStructPointer(ptr_c);
            return time_info;
        }

        public override void SetTrafficControlFocusOn(ZegoTrafficControlFocusOnMode mode, ZegoPublishChannel channel = ZegoPublishChannel.Main)
        {
            int ret = IExpressPublisherInternal.zego_express_set_traffic_control_focus_on_by_channel(mode, channel);

            ZegoUtil.ZegoPrivateLog(ret, string.Format("SetTrafficControlFocusOn, mode:{0}, channel:{1}", mode, channel), true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
        }
    }
}