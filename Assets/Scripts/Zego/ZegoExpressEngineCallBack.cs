using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using static ZEGO.ZegoExpressEngineImpl;
using static ZEGO.ZegoCallBackChangeUtil;
namespace ZEGO
{
    public class ZegoExpressEngineCallBack
    {


        [MonoPInvokeCallback(typeof(IExpressEngineInternal.zego_on_engine_uninit))]
        public static void zego_on_engine_uninit(System.IntPtr userContext)
        {
            if (enginePtr == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                Debug.Log("destroy engine success");
                if (onDestroyCompletion != null)
                {
                    onDestroyCompletion();
                    Debug.Log("destroy engine callback success");
                }
                release();
                UnInitCallBackGameObject();
            });
        }

        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_state_update))]
        public static void zego_on_room_state_update(string roomId, ZegoRoomState state, int errorCode, string extendedData, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onRoomStateUpdate == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onRoomStateUpdate?.Invoke(roomId, state, errorCode, extendedData);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_set_room_extra_info_result))]
        public static void zego_on_room_set_room_extra_info_result(int error_code, [In()][MarshalAs(UnmanagedType.LPStr)] string room_id, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string key, int seq, System.IntPtr user_context)
        {
            if (enginePtr == null || onRoomSetRoomExtraInfoResultDics == null) return;
            OnRoomSetRoomExtraInfoResult onRoomSetRoomExtraInfoResult = GetCallbackFromSeq<OnRoomSetRoomExtraInfoResult>(onRoomSetRoomExtraInfoResultDics, seq);
            if (onRoomSetRoomExtraInfoResult == null)
            {
                return;
            }
            callBackQueue.PostAsynAction(() =>
            {
                onRoomSetRoomExtraInfoResult?.Invoke(error_code);
                onRoomSetRoomExtraInfoResultDics?.TryRemove(seq, out _);
            });

        }
        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_online_user_count_update))]
        public static void zego_on_room_online_user_count_update([In()] [MarshalAs(UnmanagedType.LPStr)] string room_id, int count, IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onRoomOnlineUserCountUpdate == null) return;
            callBackQueue.PostAsynAction(() =>
            {

                enginePtr?.onRoomOnlineUserCountUpdate?.Invoke(room_id, count);

            });
        }
        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_extra_info_update))]
        public static void zego_on_room_extra_info_update([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string room_id, IntPtr room_extra_info_list, uint room_extra_info_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onRoomExtraInfoUpdate == null) return;
            zego_room_extra_info[] zego_Room_Extra_Infos = new zego_room_extra_info[room_extra_info_count];
            ZegoUtil.GetStructListByPtr<zego_room_extra_info>(ref zego_Room_Extra_Infos, room_extra_info_list, room_extra_info_count);//get StructLists by pointer
            List<ZegoRoomExtraInfo> result = ChangeZegoRoomExtraInfoStructListToClassList(zego_Room_Extra_Infos);

            callBackQueue.PostAsynAction(() =>
            {

                enginePtr?.onRoomExtraInfoUpdate?.Invoke(room_id, result);

            });
        }

        [MonoPInvokeCallback(typeof(IExpressEngineInternal.zego_on_debug_error))]
        public static void zego_on_debug_error(int error_code, [In()][MarshalAs(UnmanagedType.LPStr)] string func_name, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string info, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onDebugError == null) return;
            callBackQueue.PostAsynAction(() =>
            {

                enginePtr?.onDebugError?.Invoke(error_code, func_name, info);

            });
        }
        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_stream_extra_info_update))]
        public static void zego_on_room_stream_extra_info_update([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string room_id, System.IntPtr stream_info_list, uint stream_info_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onRoomStreamExtraInfoUpdate == null) return;
            zego_stream[] zego_streams = new zego_stream[stream_info_count];
            ZegoUtil.GetStructListByPtr<zego_stream>(ref zego_streams, stream_info_list, stream_info_count);//get StructLists by pointer
            List<ZegoStream> result = ChangeZegoStreamStructListToClassList(zego_streams);
            callBackQueue.PostAsynAction(() =>
            {

                enginePtr?.onRoomStreamExtraInfoUpdate?.Invoke(room_id, result);

            });
        }
        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_quality_update))]
        public static void zego_on_player_quality_update([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string streamId, zego_play_stream_quality quality, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPlayerQualityUpdate == null) return;
            ZegoPlayStreamQuality result = ChangePlayerQualityStructToClass(quality);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPlayerQualityUpdate?.Invoke(streamId, result);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_stream_update))]
        public static void zego_on_room_stream_update([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string roomId, ZegoUpdateType updateType, System.IntPtr streamInfoList, uint streamInfoCount, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string extend_data, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onRoomStreamUpdate == null) return;
            zego_stream[] zego_streams = new zego_stream[streamInfoCount];
            ZegoUtil.GetStructListByPtr<zego_stream>(ref zego_streams, streamInfoList, streamInfoCount);//get StructLists by pointer
            List<ZegoStream> result = ChangeZegoStreamStructListToClassList(zego_streams);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onRoomStreamUpdate?.Invoke(roomId, updateType, result, extend_data);
            });
        }


        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_media_event))]
        public static void zego_on_player_media_event([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string streamId, ZegoPlayerMediaEvent mediaEvent, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPlayerMediaEvent == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPlayerMediaEvent?.Invoke(streamId, mediaEvent);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_recv_audio_first_frame))]
        public static void zego_on_player_recv_audio_first_frame([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string streamId, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPlayerRecvAudioFirstFrame == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPlayerRecvAudioFirstFrame?.Invoke(streamId);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_recv_video_first_frame))]
        public static void zego_on_player_recv_video_first_frame([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string streamId, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPlayerRecvVideoFirstFrame == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPlayerRecvVideoFirstFrame?.Invoke(streamId);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_render_video_first_frame))]
        public static void zego_on_player_render_video_first_frame([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string streamId, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPlayerRenderVideoFirstFrame == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPlayerRenderVideoFirstFrame?.Invoke(streamId);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_video_size_changed))]
        public static void zego_on_player_video_size_changed([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string streamId, int width, int height, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPlayerVideoSizeChanged == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPlayerVideoSizeChanged?.Invoke(streamId, width, height);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressCustomVideoInternal.zego_on_custom_video_capture_start))]
        public static void zego_on_custom_video_capture_start(ZegoPublishChannel channel, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onCustomVideoCaptureStart == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onCustomVideoCaptureStart?.Invoke(channel);
            });
        }

        /// Return Type: void
        ///channel: zego_publish_channel
        ///user_context: void*
        [MonoPInvokeCallback(typeof(IExpressCustomVideoInternal.zego_on_custom_video_capture_stop))]
        public static void zego_on_custom_video_capture_stop(ZegoPublishChannel channel, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onCustomVideoCaptureStop == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onCustomVideoCaptureStop?.Invoke(channel);
            });
        }


        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_state_update))]
        public static void zego_on_publisher_state_update(string streamId, ZegoPublisherState state, int errorCode, string extendedData, System.IntPtr user_Context)
        {
            if (enginePtr == null || enginePtr.onPublisherStateUpdate == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPublisherStateUpdate?.Invoke(streamId, state, errorCode, extendedData);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_update_cdn_url_result))]
        public static void zego_on_publisher_update_cdn_url_result([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string stream_id, int error_code, int seq, System.IntPtr user_context)
        {
            if (enginePtr == null || onPublisherUpdateCdnUrlResultDics == null) return;
            OnPublisherUpdateCdnUrlResult onPublisherUpdateCdnUrlResult = GetCallbackFromSeq<OnPublisherUpdateCdnUrlResult>(onPublisherUpdateCdnUrlResultDics, seq);
            if (onPublisherUpdateCdnUrlResult == null)
            {
                return;
            }
            callBackQueue.PostAsynAction(() =>
            {
                onPublisherUpdateCdnUrlResult?.Invoke(error_code);
                onPublisherUpdateCdnUrlResultDics?.TryRemove(seq, out _);
            });
        }


        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_relay_cdn_state_update))]
        public static void zego_on_publisher_relay_cdn_state_update([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string stream_id, System.IntPtr cdn_info_list, uint info_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onPublisherRelayCDNStateUpdate == null) return;
            zego_stream_relay_cdn_info[] infos = new zego_stream_relay_cdn_info[info_count];
            ZegoUtil.GetStructListByPtr<zego_stream_relay_cdn_info>(ref infos, cdn_info_list, info_count);//get StructLists by pointer
            List<ZegoStreamRelayCDNInfo> infoList = ChangeZegoStreamRelayCDNInfoStructListToClassList(infos);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPublisherRelayCDNStateUpdate?.Invoke(stream_id, infoList);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_state_update))]
        public static void zego_on_player_state_update(string streamId, ZegoPlayerState state, int errorCode, string extendedData, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPlayerStateUpdate == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPlayerStateUpdate?.Invoke(streamId, state, errorCode, extendedData);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_quality_update))]
        public static void zego_on_publisher_quality_update([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string streamId, zego_publish_stream_quality quality, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPublisherQualityUpdate == null) return;
            ZegoPublishStreamQuality result = ChangePublishQualityToClass(quality);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPublisherQualityUpdate?.Invoke(streamId, result);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_captured_audio_first_frame))]
        public static void zego_on_publisher_captured_audio_first_frame(System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPublisherCapturedAudioFirstFrame == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPublisherCapturedAudioFirstFrame?.Invoke();
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_captured_video_first_frame))]
        public static void zego_on_publisher_captured_video_first_frame(ZegoPublishChannel channel, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPublisherCapturedVideoFirstFrame == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPublisherCapturedVideoFirstFrame?.Invoke(channel);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_video_size_changed))]
        public static void zego_on_publisher_video_size_changed(int width, int height, ZegoPublishChannel channel, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onPublisherVideoSizeChanged == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPublisherVideoSizeChanged?.Invoke(width, height, channel);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_user_update))]
        public static void zego_on_room_user_update([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string roomId, ZegoUpdateType updateType, IntPtr userList, uint userCount, System.IntPtr userContext)
        {
            if (enginePtr == null || enginePtr.onRoomUserUpdate == null) return;
            zego_user[] users = new zego_user[userCount];
            ZegoUtil.GetStructListByPtr<zego_user>(ref users, userList, userCount);//get StructLists by pointer
            List<ZegoUser> result = ChangeZegoUserStructListToClassList(users);

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onRoomUserUpdate?.Invoke(roomId, updateType, result, userCount);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressIMInternal.zego_on_im_recv_barrage_message))]
        public static void zego_on_im_recv_barrage_message([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string room_id, System.IntPtr message_info_list, uint message_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onIMRecvBarrageMessage == null) return;
            zego_barrage_message_info[] zego_messages = new zego_barrage_message_info[message_count];
            ZegoUtil.GetStructListByPtr<zego_barrage_message_info>(ref zego_messages, message_info_list, message_count);//get StructLists by pointer
            List<ZegoBarrageMessageInfo> result = ChangeBarrageMessageStructListToClassList(zego_messages);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onIMRecvBarrageMessage?.Invoke(room_id, result);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressIMInternal.zego_on_im_recv_broadcast_message))]
        public static void zego_on_im_recv_broadcast_message([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string room_id, System.IntPtr message_info_list, uint message_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onIMRecvBroadcastMessage == null) return;
            zego_broadcast_message_info[] zego_messages = new zego_broadcast_message_info[message_count];
            ZegoUtil.GetStructListByPtr<zego_broadcast_message_info>(ref zego_messages, message_info_list, message_count);//get StructLists by pointer
            List<ZegoBroadcastMessageInfo> result = ChangeBroadMessageStructListToClassList(zego_messages);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onIMRecvBroadcastMessage?.Invoke(room_id, result);

            });
        }

        [MonoPInvokeCallback(typeof(IExpressIMInternal.zego_on_im_recv_custom_command))]
        public static void zego_on_im_recv_custom_command([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string room_id, zego_user from_user, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string content, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onIMRecvCustomCommand == null) return;
            ZegoUser result = ChangeZegoUserStructToClass(from_user);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onIMRecvCustomCommand?.Invoke(room_id, result, content);

            });
        }
        [MonoPInvokeCallback(typeof(IExpressIMInternal.zego_on_im_send_barrage_message_result))]
        public static void zego_on_im_send_barrage_message_result([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string room_id, [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string message_id, int error_code, int seq, System.IntPtr user_context)
        {
            if (enginePtr == null || onIMSendBarrageMessageResultDics == null) return;
            OnIMSendBarrageMessageResult onIMSendBarrageMessageResult = GetCallbackFromSeq<OnIMSendBarrageMessageResult>(onIMSendBarrageMessageResultDics, seq);
            if (onIMSendBarrageMessageResult == null)
            {
                return;
            }
            callBackQueue.PostAsynAction(() =>
            {
                onIMSendBarrageMessageResult?.Invoke(error_code, message_id);
                onIMSendBarrageMessageResultDics?.TryRemove(seq, out _);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_recv_sei))]
        public static void zego_on_player_recv_sei([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string stream_id, IntPtr data, uint data_length, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onPlayerRecvSEI == null) return;
            byte[] result = new byte[data_length];
            ZegoUtil.GetStructListByPtr<byte>(ref result, data, data_length);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPlayerRecvSEI?.Invoke(stream_id, result);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressIMInternal.zego_on_im_send_broadcast_message_result))]
        public static void zego_on_im_send_broadcast_message_result([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string room_id, ulong message_id, int error_code, int seq, System.IntPtr user_context)
        {
            if (enginePtr == null || onIMSendBroadcastMessageResultDics == null) return;
            OnIMSendBroadcastMessageResult onIMSendBroadcastMessageResult = GetCallbackFromSeq<OnIMSendBroadcastMessageResult>(onIMSendBroadcastMessageResultDics, seq);
            if (onIMSendBroadcastMessageResult == null)
            {
                return;
            }
            callBackQueue.PostAsynAction(() =>
            {
                onIMSendBroadcastMessageResult?.Invoke(error_code, message_id);
                onIMSendBroadcastMessageResultDics?.TryRemove(seq, out _);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_update_stream_extra_info_result))]
        public static void zego_on_publisher_update_stream_extra_info_result(int error_code, int seq, System.IntPtr user_context)
        {
            if (enginePtr == null || onPublisherSetStreamExtraInfoResultDics == null) return;
            string log = string.Format("onPublisherSetStreamExtraInfoResult error_code:{0}  seq:{1}", error_code, seq);
            ZegoUtil.ZegoPrivateLog(error_code, log, true, ZegoConstans.ZEGO_EXPRESS_MODULE_PUBLISHER);
            OnPublisherSetStreamExtraInfoResult onPublisherSetStreamExtraInfoResult = GetCallbackFromSeq<OnPublisherSetStreamExtraInfoResult>(onPublisherSetStreamExtraInfoResultDics, seq);
            if (onPublisherSetStreamExtraInfoResult == null)
            {
                return;
            }
            callBackQueue.PostAsynAction(() =>
            {
                onPublisherSetStreamExtraInfoResult?.Invoke(error_code);
                onPublisherSetStreamExtraInfoResultDics?.TryRemove(seq, out _);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressIMInternal.zego_on_im_send_custom_command_result))]
        public static void zego_on_im_send_custom_command_result([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string room_id, int error_code, int seq, System.IntPtr user_context)
        {
            if (enginePtr == null || onIMSendCustomCommandResultDics == null) return;

            OnIMSendCustomCommandResult onIMSendCustomCommandResult = GetCallbackFromSeq<OnIMSendCustomCommandResult>(onIMSendCustomCommandResultDics, seq);
            if (onIMSendCustomCommandResult == null)
            {
                return;
            }
            callBackQueue.PostAsynAction(() =>
            {
                onIMSendCustomCommandResult?.Invoke(error_code);
                onIMSendCustomCommandResultDics?.TryRemove(seq, out _);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_captured_sound_level_update))]
        public static void zego_on_captured_sound_level_update(System.IntPtr sound_level_info, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onCapturedSoundLevelUpdate == null) return;
            zego_sound_level_info soundLevelInfo = ZegoUtil.GetStructByPtr<zego_sound_level_info>(sound_level_info);

            //直接在c层子线程返回回调，避免切换到ui线程导致界面卡顿

            enginePtr?.onCapturedSoundLevelUpdate?.Invoke(soundLevelInfo.sound_level);

            //callBackQueue.PostAsynAction(() =>
            //{
            //    enginePtr.onCapturedSoundLevelUpdate(soundLevelInfo.sound_level);
            //});
        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_remote_sound_level_update))]
        public static void zego_on_remote_sound_level_update(System.IntPtr sound_level_info_list, uint info_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onRemoteSoundLevelUpdate == null) return;
            zego_sound_level_info[] infos = new zego_sound_level_info[info_count];
            ZegoUtil.GetStructListByPtr<zego_sound_level_info>(ref infos, sound_level_info_list, info_count);
            Dictionary<string, float> results = ChangeZegoSoundLevelInfoStructListToDictionary(infos);
            //直接在c层子线程返回回调，避免切换到ui线程导致界面卡顿

            enginePtr?.onRemoteSoundLevelUpdate?.Invoke(results);

            //callBackQueue.PostAsynAction(() =>
            //{
            //    enginePtr.onRemoteSoundLevelUpdate(results);
            //});
        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_captured_audio_spectrum_update))]
        public static void zego_on_captured_audio_spectrum_update(System.IntPtr audio_spectrum_info, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onCapturedAudioSpectrumUpdate == null) return;
            zego_audio_spectrum_info zegoAudioSpectrumInfo = ZegoUtil.GetStructByPtr<zego_audio_spectrum_info>(audio_spectrum_info);
            float[] result = GetZegoAudioSpectrumInfoStructSpectrumList(zegoAudioSpectrumInfo);
            //直接在c层子线程返回回调，避免切换到ui线程导致界面卡顿

            enginePtr?.onCapturedAudioSpectrumUpdate?.Invoke(result);

            //callBackQueue.PostAsynAction(() =>
            //{

            //    enginePtr.onCapturedAudioSpectrumUpdate(result);
            //});
        }
        [MonoPInvokeCallback(typeof(IExpressEngineInternal.zego_on_engine_state_update))]
        public static void zego_on_engine_state_update(ZegoEngineState state, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onEngineStateUpdate == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onEngineStateUpdate?.Invoke(state);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_remote_audio_spectrum_update))]
        public static void zego_on_remote_audio_spectrum_update(System.IntPtr audio_spectrum_info_list, uint info_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onRemoteAudioSpectrumUpdate == null) return;
            zego_audio_spectrum_info[] infos = new zego_audio_spectrum_info[info_count];
            ZegoUtil.GetStructListByPtr<zego_audio_spectrum_info>(ref infos, audio_spectrum_info_list, info_count);
            Dictionary<string, float[]> results = ChangeZegoAudioSpectrumInfoListToDictionary(infos);
            //直接在c层子线程返回回调，避免切换到ui线程导致界面卡顿

            enginePtr?.onRemoteAudioSpectrumUpdate?.Invoke(results);

            //callBackQueue.PostAsynAction(() =>
            //{

            //    enginePtr.onRemoteAudioSpectrumUpdate(results);
            //});
        }

        [MonoPInvokeCallback(typeof(IExpressCustomVideoInternal.zego_on_custom_video_render_captured_frame_data))]
        public static void zego_on_custom_video_render_captured_frame_data(ref System.IntPtr data, ref uint dataLength, zego_video_frame_param param, ZegoVideoFlipMode flipMode, ZegoPublishChannel channel, System.IntPtr userContext)
        {//预览数据回调（写数据） 推流不会触发该回调
            if (enginePtr != null)
            {
                if (flipMode == ZegoVideoFlipMode.X)//means front camera
                {
                    CameraInformation.isFrontCamera = true;
                }
                else if (flipMode == ZegoVideoFlipMode.None)//means Rear camera
                {
                    CameraInformation.isFrontCamera = false;
                }
                if (channel == ZegoPublishChannel.Main)
                {
                    enginePtr.localVideoBufMain.WriteBuffer(data, param.width, param.height, param.strides, param.format);
                }
                else
                {
                    enginePtr.localVideoBufAux.WriteBuffer(data, param.width, param.height, param.strides, param.format);
                }
            }
        }
        [MonoPInvokeCallback(typeof(IExpressCustomVideoInternal.zego_on_custom_video_render_remote_frame_data))]
        public static void zego_on_custom_video_render_remote_frame_data([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string streamID, ref System.IntPtr data, ref uint dataLength, zego_video_frame_param param, System.IntPtr userContext)
        {//拉流数据回调（写数据）
            if (enginePtr != null)
            {
                VideoDataRingBuffer play_buffer = enginePtr.GetPlayerBufferByStreamId(streamID);
                if (play_buffer != null)
                {
                    play_buffer.WriteBuffer(data, param.width, param.height, param.strides, param.format);
                }
            }
        }

        [MonoPInvokeCallback(typeof(IExpressMixerInternal.zego_on_mixer_start_result))]
        public static void zego_on_mixer_start_result(int error_code, int seq, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string extended_data, System.IntPtr user_context)
        {
            if (enginePtr == null || onMixerStartResultDics == null) return;

            OnMixerStartResult onMixerStartResult = GetCallbackFromSeq<OnMixerStartResult>(onMixerStartResultDics, seq);
            if (onMixerStartResult == null)
            {
                return;
            }

            callBackQueue.PostAsynAction(() =>
            {
                onMixerStartResult?.Invoke(error_code, extended_data);
                onMixerStartResultDics?.TryRemove(seq, out _);
            });


        }

        /// Return Type: void
        ///error_code: int
        ///seq: int
        ///user_context: void*
        [MonoPInvokeCallback(typeof(IExpressMixerInternal.zego_on_mixer_stop_result))]
        public static void zego_on_mixer_stop_result(int error_code, int seq, System.IntPtr user_context)
        {
            if (enginePtr == null || onMixerStopResultDics == null) return;

            OnMixerStopResult onMixerStopResult = GetCallbackFromSeq<OnMixerStopResult>(onMixerStopResultDics, seq);
            if (onMixerStopResult == null)
            {
                return;
            }
            callBackQueue.PostAsynAction(() =>
            {
                onMixerStopResult?.Invoke(error_code);
                onMixerStopResultDics?.TryRemove(seq, out _);
            });

        }

        [MonoPInvokeCallback(typeof(IExpressMixerInternal.zego_on_auto_mixer_start_result))]
        public static void zego_on_auto_mixer_start_result(int error_code, int seq, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string extended_data, System.IntPtr user_context)
        {
            if (enginePtr == null || onAutoMixerStartResultDics == null) return;

            OnMixerStartResult onAutoMixerStartResult = GetCallbackFromSeq<OnMixerStartResult>(onAutoMixerStartResultDics, seq);
            if (onAutoMixerStartResult == null)
            {
                return;
            }

            callBackQueue.PostAsynAction(() =>
            {
                onAutoMixerStartResult?.Invoke(error_code, extended_data);
                onAutoMixerStartResultDics?.TryRemove(seq, out _);
            });


        }

        [MonoPInvokeCallback(typeof(IExpressMixerInternal.zego_on_auto_mixer_stop_result))]
        public static void zego_on_auto_mixer_stop_result(int error_code, int seq, System.IntPtr user_context)
        {
            if (enginePtr == null || onMixerStopResultDics == null) return;

            OnMixerStopResult onAutoMixerStopResult = GetCallbackFromSeq<OnMixerStopResult>(onAutoMixerStopResultDics, seq);
            if (onAutoMixerStopResult == null)
            {
                return;
            }
            callBackQueue.PostAsynAction(() =>
            {
                onAutoMixerStopResult?.Invoke(error_code);
                onAutoMixerStopResultDics?.TryRemove(seq, out _);
            });

        }

        [MonoPInvokeCallback(typeof(IExpressMixerInternal.zego_on_auto_mixer_sound_level_update))]
        public static void zego_on_auto_mixer_sound_level_update(IntPtr sound_levels, uint info_count, IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onAutoMixerSoundLevelUpdate == null) return;

            zego_sound_level_info[] zego_Mixer_Sound_Level_Infos = new zego_sound_level_info[info_count];
            ZegoUtil.GetStructListByPtr(ref zego_Mixer_Sound_Level_Infos, sound_levels, info_count);
            Dictionary<string, float> result = ChangeZegoSoundLevelInfoStructListToDictionary(zego_Mixer_Sound_Level_Infos);
            //callBackQueue.PostAsynAction(() =>
            //{
            enginePtr?.onAutoMixerSoundLevelUpdate?.Invoke(result);
            //});
        }


        [MonoPInvokeCallback(typeof(IExpressMixerInternal.zego_on_mixer_relay_cdn_state_update))]
        public static void zego_on_mixer_relay_cdn_state_update([In()][MarshalAs(UnmanagedType.LPStr)] string task_id, System.IntPtr info_list, uint info_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onMixerRelayCDNStateUpdate == null) return;

            zego_stream_relay_cdn_info[] zego_Stream_Relay_Cdn_Infos = new zego_stream_relay_cdn_info[info_count];
            ZegoUtil.GetStructListByPtr(ref zego_Stream_Relay_Cdn_Infos, info_list, info_count);
            List<ZegoStreamRelayCDNInfo> result = ChangeZegoStreamRelayCDNInfoStructListToClassList(zego_Stream_Relay_Cdn_Infos);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onMixerRelayCDNStateUpdate?.Invoke(task_id, result);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressMixerInternal.zego_on_mixer_sound_level_update))]
        public static void zego_on_mixer_sound_level_update(IntPtr sound_levels, uint info_count, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onMixerSoundLevelUpdate == null) return;

            zego_mixer_sound_level_info[] zego_Mixer_Sound_Level_Infos = new zego_mixer_sound_level_info[info_count];
            ZegoUtil.GetStructListByPtr(ref zego_Mixer_Sound_Level_Infos, sound_levels, info_count);
            Dictionary<uint, float> result = ChangeZegoMixerSoundLevelInfoStructListToDictionary(zego_Mixer_Sound_Level_Infos);
            //callBackQueue.PostAsynAction(() =>
            //{
            enginePtr?.onMixerSoundLevelUpdate?.Invoke(result);
            //});

        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_device_error))]
        public static void zego_on_device_error(int error_code, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string device_name, System.IntPtr user_context)
        {
            #pragma warning disable 0618
            if (enginePtr == null || enginePtr.onDeviceError == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onDeviceError?.Invoke(error_code, device_name);
            });
            #pragma warning restore 0618
        }
        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_remote_camera_state_update))]
        public static void zego_on_remote_camera_state_update([In()] [MarshalAs(UnmanagedType.LPStr)] string stream_id, ZegoRemoteDeviceState state, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onRemoteCameraStateUpdate == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onRemoteCameraStateUpdate?.Invoke(stream_id, state);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_remote_mic_state_update))]
        public static void zego_on_remote_mic_state_update([In()] [MarshalAs(UnmanagedType.LPStr)] string stream_id, ZegoRemoteDeviceState state, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onRemoteMicStateUpdate == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onRemoteMicStateUpdate?.Invoke(stream_id, state);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressCustomAudioIOInternal.zego_on_captured_audio_data))]
        public static void zego_on_captured_audio_data(IntPtr data, uint data_length, zego_audio_frame_param param, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onCapturedAudioData == null) return;
            ZegoAudioFrameParam zegoAudioFrameParam = ChangeZegoAudioFrameStructToClass(param);
            enginePtr.onCapturedAudioData(data, data_length, zegoAudioFrameParam);
        }

        [MonoPInvokeCallback(typeof(IExpressCustomAudioIOInternal.zego_on_mixed_audio_data))]
        public static void zego_on_mixed_audio_data(IntPtr data, uint data_length, zego_audio_frame_param param, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onMixedAudioData == null) return;
            ZegoAudioFrameParam zegoAudioFrameParam = ChangeZegoAudioFrameStructToClass(param);
            enginePtr.onMixedAudioData(data, data_length, zegoAudioFrameParam);
        }

        [MonoPInvokeCallback(typeof(IExpressCustomAudioIOInternal.zego_on_playback_audio_data))]
        public static void zego_on_playback_audio_data(IntPtr data, uint data_length, zego_audio_frame_param param, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onPlaybackAudioData == null) return;
            ZegoAudioFrameParam zegoAudioFrameParam = ChangeZegoAudioFrameStructToClass(param);
            enginePtr.onPlaybackAudioData(data, data_length, zegoAudioFrameParam);
        }

        [MonoPInvokeCallback(typeof(IExpressCustomAudioIOInternal.zego_on_player_audio_data))]
        public static void zego_on_player_audio_data(IntPtr data, uint data_length, zego_audio_frame_param param, [In()] [MarshalAs(UnmanagedType.LPStr)] string stream_id, IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onPlayerAudioData == null) return;
            ZegoAudioFrameParam zegoAudioFrameParam = ChangeZegoAudioFrameStructToClass(param);
            enginePtr.onPlayerAudioData(data, data_length, zegoAudioFrameParam, stream_id);
        }
        [MonoPInvokeCallback(typeof(IExpressRecordInternal.zego_on_captured_data_record_state_update))]
        public static void zego_on_captured_data_record_state_update(ZegoDataRecordState state, int error_code, zego_data_record_config config, ZegoPublishChannel channel, IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onCapturedDataRecordStateUpdate == null) return;
            ZegoDataRecordConfig zegoDataRecordConfig = ChangeZegoDataRecordConfigStructToClass(config);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onCapturedDataRecordStateUpdate?.Invoke(state, error_code, zegoDataRecordConfig, channel);
            });
        }
        [MonoPInvokeCallback(typeof(IExpressRecordInternal.zego_on_captured_data_record_progress_update))]
        public static void zego_on_captured_data_record_progress_update(zego_data_record_progress progress, zego_data_record_config config, ZegoPublishChannel channel, IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onCapturedDataRecordProgressUpdate == null) return;
            ZegoDataRecordProgress zegoDataRecordProgress = ChangeZegoDataRecordProgresstructToClass(progress);
            ZegoDataRecordConfig zegoDataRecordConfig = ChangeZegoDataRecordConfigStructToClass(config);
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onCapturedDataRecordProgressUpdate?.Invoke(zegoDataRecordProgress, zegoDataRecordConfig, channel);
            });

        }

        [MonoPInvokeCallback(typeof(IExpressCustomVideoInternal.zego_on_custom_video_process_captured_unprocessed_raw_data))]
        public static void zego_on_custom_video_process_captured_unprocessed_raw_data(ref IntPtr data, ref uint data_length, zego_video_frame_param param, ulong reference_time_millisecond, ZegoPublishChannel channel, IntPtr user_context)
        {
            #if UNITY_STANDALONE_WIN
            if(enginePtr == null || enginePtr.onCapturedUnprocessedRawData == null) return;

            ZegoVideoFrameParam param_ = ChangeZegoVideoFrameStructToClass(param);
            enginePtr?.onCapturedUnprocessedRawData(ref data, ref data_length, param_, reference_time_millisecond, channel);
            #endif
        }

        [MonoPInvokeCallback(typeof(IExpressCustomVideoInternal.zego_on_custom_video_process_captured_unprocessed_cvpixelbuffer))]
        public static void zego_on_custom_video_process_captured_unprocessed_cvpixelbuffer(IntPtr buffer, ulong reference_time_millisecond, ZegoPublishChannel channel, IntPtr user_context)
        {
            #if UNITY_IOS || UNITY_STANDALONE_OSX
            if(enginePtr == null || enginePtr.onCapturedUnprocessedCVPixelBuffer == null) return;
            enginePtr?.onCapturedUnprocessedCVPixelBuffer(buffer, reference_time_millisecond, channel);
            #endif
        }

        [MonoPInvokeCallback(typeof(IExpressCustomVideoInternal.zego_on_custom_video_process_captured_unprocessed_texture_data))]
        public static void zego_on_custom_video_process_captured_unprocessed_texture_data(int texture_id, int width, int height, ulong reference_time_millisecond, ZegoPublishChannel channel, IntPtr user_context)
        {
            #if UNITY_ANDROID
            if(enginePtr == null || enginePtr.onCapturedUnprocessedTextureData == null) return;
            enginePtr?.onCapturedUnprocessedTextureData(texture_id, width, height, reference_time_millisecond, channel);
            #endif
        }

        [MonoPInvokeCallback(typeof(IExpressEngineInternal.zego_on_recv_experimental_api))]
        public static void zego_on_recv_experimental_api([In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string content, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onRecvExperimentalAPI == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onRecvExperimentalAPI?.Invoke(content);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressEngineInternal.zego_on_api_called_result))]
        public static void zego_on_api_called_result(int error_code, [In()][MarshalAs(UnmanagedType.LPStr)] string func_name, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string info, System.IntPtr user_context)
        {
            if (enginePtr == null || enginePtr.onApiCalledResult == null) return;
            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onApiCalledResult?.Invoke(error_code, func_name, info);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_token_will_expire))]
        public static void zego_on_room_token_will_expire([In()] [MarshalAs(UnmanagedType.LPStr)] string room_id, int remain_time_in_second, System.IntPtr user_context)
        {
            if(enginePtr == null || enginePtr.onRoomTokenWillExpire == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onRoomTokenWillExpire?.Invoke(room_id, remain_time_in_second);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_remote_speaker_state_update))]
        public static void zego_on_remote_speaker_state_update([In()][MarshalAs(UnmanagedType.LPStr)] string stream_id, ZegoRemoteDeviceState state, System.IntPtr user_context)
        {
            if(enginePtr == null || enginePtr.onRemoteSpeakerStateUpdate == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onRemoteSpeakerStateUpdate?.Invoke(stream_id, state);
            });    
        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_local_device_exception_occurred))]
        public static void zego_on_local_device_exception_occurred(ZegoDeviceExceptionType exception_type, ZegoDeviceType device_type, [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string device_id, System.IntPtr user_context)
        {
            if(enginePtr == null || enginePtr.onLocalDeviceExceptionOccurred == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onLocalDeviceExceptionOccurred?.Invoke(exception_type, device_type, device_id);
            }); 
        }

        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_render_video_first_frame))]
        public static void zego_on_publisher_render_video_first_frame(ZegoPublishChannel channel, System.IntPtr user_context)
        {
            if(enginePtr == null || enginePtr.onPublisherRenderVideoFirstFrame == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onPublisherRenderVideoFirstFrame?.Invoke(channel);
            }); 
        }

        [MonoPInvokeCallback(typeof(IExpressPlayerInternal.zego_on_player_take_snapshot_result))]
        public static void zego_on_player_take_snapshot_result(int error_code, [In()][MarshalAs(UnmanagedType.LPStr)] string stream_id, System.IntPtr image, System.IntPtr user_context)
        {
            if(enginePtr == null || onPlayerTakeSnapshotResultDics == null) return;

            OnPlayerTakeSnapshotResult callback;
            onPlayerTakeSnapshotResultDics.TryRemove(stream_id, out callback);

            // callback directly
            if(callback != null)
            {
                callback(error_code, image);
            }
        }

        [MonoPInvokeCallback(typeof(IExpressPublisherInternal.zego_on_publisher_take_snapshot_result))]
        public static void zego_on_publisher_take_snapshot_result(int error_code, ZegoPublishChannel channel, System.IntPtr image, System.IntPtr user_context)
        {
            if(enginePtr == null || onPublisherTakeSnapshotResultDics == null) return;

            var callback = GetCallbackFromSeq<OnPublisherTakeSnapshotResult>(onPublisherTakeSnapshotResultDics, (int)channel);

            // callback directly
            if(callback != null)
            {
                callback(error_code, image);
                onPublisherTakeSnapshotResultDics.TryRemove((int)channel, out _);
            }
        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_audio_route_change))]
        public static void zego_on_audio_route_change(ZegoAudioRoute audio_route, System.IntPtr user_context)
        {
            #if UNITY_ANDROID || UNITY_IOS
            if(enginePtr == null || enginePtr.onAudioRouteChange == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                enginePtr?.onAudioRouteChange?.Invoke(audio_route);
            }); 
            #endif
        }

        [MonoPInvokeCallback(typeof(IExpressEngineInternal.zego_on_network_quality))]
        public static void zego_on_network_quality([In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string userid, 
        ZegoStreamQualityLevel upstream_quality, 
        ZegoStreamQualityLevel downstream_quality, 
        IntPtr user_context)
        {
            if(enginePtr == null || enginePtr.onNetworkQuality == null) return;
            callBackQueue?.PostAsynAction(()=>{
                enginePtr?.onNetworkQuality?.Invoke(userid, upstream_quality, downstream_quality);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_login_result))]
        public static void zego_on_room_login_result(int error_code, 
        [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string extended_data, 
        [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string room_id, 
        int seq, 
        IntPtr user_context)
        {
            if(enginePtr == null || onRoomLoginResultDics == null) return;
            var callback = GetCallbackFromSeq<OnRoomLoginResult>(onRoomLoginResultDics, seq);
            if(callback == null) return;

            callBackQueue.PostAsynAction(() =>
            {
                callback?.Invoke(error_code, extended_data);
                onRoomLoginResultDics.TryRemove(seq, out _);
            }); 
        }

        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_logout_result))]
        public static void zego_on_room_logout_result(int error_code, 
        [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string extended_data, 
        [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))] string room_id, 
        int seq, 
        IntPtr user_context)
        {
            if(enginePtr == null || onRoomLogoutResultDics == null) return;

            var callback = GetCallbackFromSeq<OnRoomLogoutResult>(onRoomLogoutResultDics, seq);
            if(callback == null) return;

            callBackQueue.PostAsynAction(()=>
            {
                callback?.Invoke(error_code, extended_data);
                onRoomLogoutResultDics.TryRemove(seq, out _);
            });
        }


        [MonoPInvokeCallback(typeof(IExpressRoomInternal.zego_on_room_state_changed))]
        public static void zego_on_room_state_changed([In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string room_id, 
        ZegoRoomStateChangedReason reason, 
        int error_code, 
        [In()][MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ZegoUtil.UTF8StringMarshaler))]string extended_data, 
        IntPtr user_context)
        {
            if(enginePtr == null || enginePtr.onRoomStateChanged == null) return;

            callBackQueue.PostAsynAction(()=>
            {
                enginePtr?.onRoomStateChanged?.Invoke(room_id, reason, error_code, extended_data);
            });
        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_captured_sound_level_info_update))]
        public static void zego_on_captured_sound_level_info_update(IntPtr sound_level_info, IntPtr user_context)
        {
            if(enginePtr == null || enginePtr.onCapturedSoundLevelInfoUpdate == null) return;

            zego_sound_level_info info = ZegoUtil.GetStructByPtr<zego_sound_level_info>(sound_level_info);
            var soundLevelInfo = ChangeZegoSoundLevelInfoStructToClass(info);
            enginePtr?.onCapturedSoundLevelInfoUpdate?.Invoke(soundLevelInfo);
        }

        [MonoPInvokeCallback(typeof(IExpressDeviceInternal.zego_on_remote_sound_level_info_update))]
        public static void zego_on_remote_sound_level_info_update(IntPtr sound_level_info, uint info_count, IntPtr user_context)
        {
            if(enginePtr == null || enginePtr.onRemoteSoundLevelInfoUpdate == null) return;

            zego_sound_level_info[] infos = new zego_sound_level_info[info_count];
            ZegoUtil.GetStructListByPtr<zego_sound_level_info>(ref infos, sound_level_info, info_count);
            Dictionary<string, ZegoSoundLevelInfo> results = ChangeZegoSoundLevelInfoStructListToDictionaryAll(infos);
            enginePtr?.onRemoteSoundLevelInfoUpdate?.Invoke(results);
        }
    }
}
