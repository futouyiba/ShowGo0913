#pragma once

#include <string>

#include "./include/zego-express-errcode.h"
#include "./include/zego-express-engine.h"
#include "./include/zego-express-room.h"
#include "./include/zego-express-publisher.h"
#include "./include/zego-express-player.h"
#include "./include/zego-express-device.h"
#include "./include/zego-express-preprocess.h"
#include "./include/zego-express-im.h"
#include "./include/zego-express-mixer.h"
#include "./include/zego-express-mediaplayer.h"
#include "./include/zego-express-audio-effect-player.h"
#include "./include/zego-express-record.h"
#include "./include/zego-express-custom-video-io.h"
#include "./include/zego-express-custom-audio-io.h"
#include "./include/zego-express-utilities.h"
#include "./include/zego-express-range-audio.h"
#include "./include/zego-express-copyrighted-music.h"
#include "./include/zego-express-media-data-publisher.h"
#include "./ZegoInternalPrivate.h"

ZEGO_DISABLE_DEPRECATION_WARNINGS

namespace ZEGO
{
    namespace EXPRESS
    {

        class ZegoExpressEngineBridge
        {
            class ZegoExpressEngineBridgePri
            {
            public:
                void init()
                {
                    this->enableDebugErrorAlways = false;
                }
                bool enableDebugErrorAlways = false;
            };

        public:
            static ZegoExpressEngineBridge *GetInstance()
            {
                static ZegoExpressEngineBridge oInstance;
                return &oInstance;
            }

            const char *getVersion()
            {
                return zego_express_get_version();
            }

            void setDebugVerbose(bool enable, zego_language language)
            {
                zego_express_set_debug_verbose(enable, language);
            }

            void setAndroidEnv(void *jvm, void *ctx)
            {
                zego_express_set_android_env(jvm, ctx);
            }

            void setEngineConfig(zego_engine_config config)
            {
                std::string advanceConfig = config.advanced_config;

                // enable debug error when no error occurred if find "enable_debug_error_always=true" here
                std::string::size_type loc = advanceConfig.find("enable_debug_error_always=true", 0);
                pri->enableDebugErrorAlways = (loc != std::string::npos);

                zego_express_set_engine_config(config);
            }

            void setRoomMode(zego_room_mode mode)
            {
                zego_express_set_room_mode(mode);
            }

            void setLogConfig(zego_log_config config)
            {
                zego_express_set_log_config(config);
            }

            int init(unsigned int appID, const char *appSign, bool isTestEnvironment, zego_scenario scenario)
            {
                // reset the locals
                pri->init();

                // set platform
                zego_express_set_platform_language(zego_platform_language_cpp);

                int result = zego_express_engine_init(appID, appSign, isTestEnvironment, scenario);
                const char *function_name="createEngine";
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ENGINE, function_name, result);
                return result;
            }

            int init(const ZegoEngineProfile& profile)
            {
                // TODO: @kebo init
                // reset the locals
                pri->init();

                // set platform
                zego_express_set_platform_language(zego_platform_language_cpp);

                // do init
                zego_engine_profile config;
                config.app_id = profile.appID;
                if(!profile.appSign.empty())
                {
                    strncpy(config.app_sign, profile.appSign.c_str(), ZEGO_EXPRESS_MAX_APPSIGN_LEN);
                }
                else
                {
                    memset(config.app_sign, 0, ZEGO_EXPRESS_MAX_APPSIGN_LEN);
                }
                config.scenario = static_cast<zego_scenario>(profile.scenario);

                int result = zego_express_engine_init_with_profile(config);
                const char *function_name="createEngine";
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ENGINE, function_name, result);
                return result;
            }

            void uninitAsync()
            {
                int result = zego_express_engine_uninit_async();
                const char *function_name="destroyEngine";
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ENGINE, function_name, result);
            }

            void printDebugInfoAndPopupWhenAlreadyInit(){
                const char *function_name="createEngine";
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ENGINE, function_name, 0);
            }

            const char* callExperimentalAPI(const std::string& params) {
                return zego_express_call_experimental_api(params.c_str());
            }

            void setDummyCaptureImagePath(const std::string& filePath, ZegoPublishChannel channel = ZEGO_PUBLISH_CHANNEL_MAIN) {
                const char *function_name="setDummyCaptureImagePath";
                zego_express_set_dummy_capture_image_path(filePath.c_str(), (zego_publish_channel)channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ENGINE, function_name, 0);
            }

            int uploadLog()
            {
                return zego_express_upload_log();
            }

            void enableDebugAssistant(bool enable)
            {
                zego_express_enable_debug_assistant(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ENGINE, __FUNCTION__, 0);
            }

            void loginRoom(const char *room_id, zego_user user, zego_room_config *room_config)
            {
                int result = zego_express_login_room(room_id, user, room_config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ROOM, __FUNCTION__, result);
            }

            int loginRoomWithCallback(const char *room_id, zego_user user, zego_room_config *room_config)
            {
                int seq = zego_express_login_room_with_callback(room_id, user, room_config);
                return seq;
            }

            void loginMultiRoom(const char *room_id, zego_room_config *room_config)
            {
                int result = zego_express_login_multi_room(room_id, room_config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ROOM, __FUNCTION__, result);
            }

            void logoutRoom()
            {
                int result = zego_express_logout_all_room();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ROOM, __FUNCTION__, result);
            }

            void logoutRoom(const char *room_id)
            {
                int result = zego_express_logout_room(room_id);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ROOM, __FUNCTION__, result);
            }

            int logoutRoomWithCallback()
            {
                int seq = zego_express_logout_all_room_with_callback();
                return seq;
            }

            int logoutRoomWithCallback(const char *room_id)
            {
                int seq = zego_express_logout_room_with_callback(room_id);
                return seq;
            }

            void switchRoom(const char *from_room_id, const char *to_room_id, zego_room_config *room_config)
            {
                int result = zego_express_switch_room(from_room_id, to_room_id, room_config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ROOM, __FUNCTION__, result);
            }

            void renewToken(const char* room_id, const char* token)
            {
                int result = zego_express_renew_token(room_id, token);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_ROOM, __FUNCTION__, result);
            }

            int setRoomExtraInfo(const char *extraInfo, const char *key, const char *value)
            {
                int seq = zego_express_set_room_extra_info(extraInfo, key, value);
                return seq;
            }

            void setVideoMirrorMode(zego_video_mirror_mode mirrorMode, zego_publish_channel channel)
            {
                int result = zego_express_set_video_mirror_mode(mirrorMode, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void startPreview(zego_canvas *canvas, zego_publish_channel channel)
            {
                int result = zego_express_start_preview(canvas, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void stopPreview(zego_publish_channel channel)
            {
                int result = zego_express_stop_preview(channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setAudioConfig(zego_audio_config audioConfig, zego_publish_channel channel)
            {
                int result = zego_express_set_audio_config_by_channel(audioConfig, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            zego_audio_config getAudioConfig(zego_publish_channel channel)
            {
                return zego_express_get_audio_config_by_channel(channel);
            }

            void setVideoConfig(zego_video_config videoConfig, zego_publish_channel channel)
            {
                int result = zego_express_set_video_config(videoConfig, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            zego_video_config getVideoConfig(zego_publish_channel channel)
            {
                return zego_express_get_video_config(channel);
            }

            void startPublishingStream(const char *streamID, zego_publish_channel channel)
            {
                int result = zego_express_start_publishing_stream(streamID, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void startPublishingStreamWithConfig(const char *streamID, zego_publish_channel channel, zego_publisher_config config)
            {
                int result = zego_express_start_publishing_stream_with_config(streamID, config, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void stopPublishingStream(zego_publish_channel channel)
            {
                int result = zego_express_stop_publishing_stream(channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            int setStreamExtraInfo(const char *extraInfo, zego_publish_channel channel)
            {
                int seq = zego_express_set_stream_extra_info(extraInfo, channel);
                return seq;
            }

            void setPublishStreamEncryptionKey(const char *key, zego_publish_channel channel)
            {
                int result = zego_express_set_publish_stream_encryption_key(key, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void takePublishStreamSnapshot(zego_publish_channel channel)
            {
                int result = zego_express_take_publish_stream_snapshot(channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void mutePublishStreamAudio(bool mute, zego_publish_channel channel)
            {
                int result = zego_express_mute_publish_stream_audio(mute, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void mutePublishStreamVideo(bool mute, zego_publish_channel channel)
            {
                int result = zego_express_mute_publish_stream_video(mute, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setStreamAlignmentProperty(int alignment, zego_publish_channel channel)
            {
                int result = zego_express_set_stream_alignment_property(alignment, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setCaptureVolume(int volume)
            {
                int result = zego_express_set_capture_volume(volume);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setAudioCaptureStereoMode(zego_audio_capture_stereo_mode mode)
            {
                int result = zego_express_set_audio_capture_stereo_mode(mode);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            int addPublishCdnUrl(const char *streamID, const char *targetURL)
            {
                int seq = zego_express_add_publish_cdn_url(streamID, targetURL);
                return seq;
            }

            int removePublishCdnUrl(const char *streamID, const char *targetURL)
            {
                int seq = zego_express_remove_publish_cdn_url(streamID, targetURL);
                return seq;
            }

            void enablePublishDirectToCDN(bool enable, zego_cdn_config *config, zego_publish_channel channel)
            {
                int result = zego_express_enable_publish_direct_to_cdn(enable, config, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setPublishWatermark(zego_watermark *watermark, bool isPreviewVisible, zego_publish_channel channel)
            {
                int result = zego_express_set_publish_watermark(isPreviewVisible, watermark, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void enableHardwareEncoder(bool enable)
            {
                int result = zego_express_enable_hardware_encoder(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setCapturePipelineScaleMode(zego_capture_pipeline_scale_mode mode)
            {
                int result = zego_express_set_capture_pipeline_scale_mode(mode);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void enableH265EncodeFallback(bool enable)
            {
                int result = zego_express_enable_h_265_encode_fallback(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            bool isVideoEncoderSupported(zego_video_codec_id codecID)
            {
                return zego_express_is_video_encoder_supported(codecID);
            }

            void setSEIConfig(zego_sei_config config)
            {
                int result = zego_express_set_sei_config(config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void sendSEI(const unsigned char *buffer, unsigned int buffer_length, zego_publish_channel channel)
            {
                int result = zego_express_send_sei(buffer, buffer_length, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void sendSEISyncWithCustomVideo(const unsigned char* data, unsigned int dataLength, unsigned long long timeStampNs, zego_publish_channel channel)
            {
                int result = zego_express_send_sei_sync_with_custom_video(data, dataLength, timeStampNs, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void sendAudioSideInfo(const unsigned char* data, unsigned int dataLength, double timeStampMs, zego_publish_channel channel)
            {
                int result = zego_express_send_audio_side_info(data, dataLength, timeStampMs, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void enableTrafficControl(bool enable, int property, zego_publish_channel channel)
            {
                int result = zego_express_enable_traffic_control_by_channel(enable, property, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setMinVideoBitrateForTrafficControl(int bitrate, zego_traffic_control_min_video_bitrate_mode mode, zego_publish_channel channel)
            {
                int result = zego_express_set_min_video_bitrate_for_traffic_control_by_channel(bitrate, mode, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setMinVideoFpsForTrafficControl(int fps, zego_publish_channel channel)
            {
                int result = zego_express_set_min_video_fps_for_traffic_control_by_channel(fps, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setMinVideoResolutionForTrafficControl(int width, int height, zego_publish_channel channel)
            {
                int result = zego_express_set_min_video_resolution_for_traffic_control_by_channel(width, height, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setTrafficControlFocusOn(zego_traffic_control_focus_on_mode mode, zego_publish_channel channel)
            {
                int result = zego_express_set_traffic_control_focus_on_by_channel(mode, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void setAppOrientation(zego_orientation orientation, zego_publish_channel channel)
            {
                int result = zego_express_set_app_orientation(orientation, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            void startPlayingStream(const char *streamID, zego_canvas *canvas)
            {
                int result = zego_express_start_playing_stream(streamID, canvas);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            // void setPlayStreamView(const char *streamID, zego_canvas *canvas)
            // {
            //     int result = zego_express_set_play_stream_view(streamID, canvas);
            //     printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            // }

            void startPlayingStreamWithConfig(const char *streamID, zego_canvas *canvas, zego_player_config config)
            {
                int result = zego_express_start_playing_stream_with_config(streamID, canvas, config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void stopPlayingStream(const char *streamID)
            {
                int result = zego_express_stop_playing_stream(streamID);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void setPlayStreamDecryptionKey(const char *streamID, const char *key)
            {
                int result = zego_express_set_play_stream_decryption_key(streamID, key);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void setPlayStreamCrossAppInfo(const char *streamID, struct zego_cross_app_info info)
            {
                int result = zego_express_set_play_stream_cross_app_info(streamID, info);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void takePlayStreamSnapshot(const char *streamID)
            {
                int result = zego_express_take_play_stream_snapshot(streamID);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void setPlayVolume(const char *streamID, int volume)
            {
                int result = zego_express_set_play_volume(streamID, volume);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void setAllPlayStreamVolume(int volume)
            {
                int result = zego_express_set_all_play_stream_volume(volume);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void setPlayStreamVideoLayer(const char *streamID, zego_player_video_layer video_layer)
            {
                int result = zego_express_set_play_stream_video_layer(streamID, video_layer);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void setPlayStreamVideoType(const char *streamID, zego_video_stream_type type)
            {
                int result = zego_express_set_play_stream_video_type(streamID, type);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void setPlayStreamBufferIntervalRange(const char *streamID, unsigned int minBufferInterval, unsigned int maxBufferInterval)
            {
                int result = zego_express_set_play_stream_buffer_interval_range(streamID, minBufferInterval, maxBufferInterval);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void setPlayStreamFocusOn(const char *streamID)
            {
                int result = zego_express_set_play_stream_focus_on(streamID);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void mutePlayStreamAudio(const char *streamID, bool mute)
            {
                int result = zego_express_mute_play_stream_audio(streamID, mute);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void mutePlayStreamVideo(const char *streamID, bool mute)
            {
                int result = zego_express_mute_play_stream_video(streamID, mute);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void muteAllPlayStreamAudio(bool mute)
            {
                int result = zego_express_mute_all_play_stream_audio(mute);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void muteAllPlayStreamVideo(bool mute)
            {
                int result = zego_express_mute_all_play_stream_video(mute);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void enableHardwareDecoder(bool enable)
            {
                int result = zego_express_enable_hardware_decoder(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void enableCheckPoc(bool enable)
            {
                int result = zego_express_enable_check_poc(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PUBLISHER, __FUNCTION__, result);
            }

            bool isVideoDecoderSupported(zego_video_codec_id codecID)
            {
                return zego_express_is_video_decoder_supported(codecID);
            }

            void setPlayStreamsAlignmentProperty(zego_stream_alignment_mode mode)
            {
                int result = zego_express_set_play_streams_alignment_property(mode);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PLAYER, __FUNCTION__, result);
            }

            void muteMicrophone(bool mute)
            {
                int result = zego_express_mute_microphone(mute);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            bool isMicrophoneMuted()
            {
                bool result = zego_express_is_microphone_muted();
                return result;
            }

            void muteSpeaker(bool mute)
            {
                int result = zego_express_mute_speaker(mute);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            bool isSpeakerMuted()
            {
                bool result = zego_express_is_speaker_muted();
                return result;
            }

            void enableCamera(bool enable, zego_publish_channel channel)
            {
                int result = zego_express_enable_camera(enable, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void enableAudioCaptureDevice(bool enable)
            {
                int result = zego_express_enable_audio_capture_device(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            zego_audio_route getAudioRouteType()
            {
                zego_audio_route result = zego_express_get_audio_route_type();
                return result;
            }

            void setAudioRouteToSpeaker(bool defaultToSpeaker)
            {
                int result = zego_express_set_audio_route_to_speaker(defaultToSpeaker);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void setBuiltInSpeakerOn(bool enable)
            {
                int result = zego_express_set_built_in_speaker_on(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void useFrontCamera(bool enable, zego_publish_channel channel)
            {
                int result = zego_express_use_front_camera(enable, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            bool isCameraFocusSupported(zego_publish_channel channel)
            {
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, 0);
                return zego_express_is_camera_focus_supported(channel);
            }

            void setCameraFocusMode(zego_camera_focus_mode mode, zego_publish_channel channel)
            {
                int result = zego_express_set_camera_focus_mode(mode,  channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void setCameraFocusPointInPreview(float x, float y, zego_publish_channel channel)
            {
                int result = zego_express_set_camera_focus_point_in_preview(x, y, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void setCameraExposureMode(zego_camera_exposure_mode mode, zego_publish_channel channel)
            {
                int result = zego_express_set_camera_exposure_mode(mode, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void setCameraExposurePointInPreview(float x, float y, zego_publish_channel channel)
            {
                int result = zego_express_set_camera_exposure_point_in_preview(x, y, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void setCameraExposureCompensation(float value, zego_publish_channel channel)
            {
                int result = zego_express_set_camera_exposure_compensation(value, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void setCameraZoomFactor(float factor, zego_publish_channel channel)
            {
                int result = zego_express_set_camera_zoom_factor(factor, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            float getCameraMaxZoomFactor(zego_publish_channel channel)
            {
                float factor = zego_express_get_camera_max_zoom_factor(channel);
                return factor;
            }

            void startSoundLevelMonitor(struct zego_sound_level_config config)
            {
                int result = zego_express_start_sound_level_monitor_with_config(config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void stopSoundLevelMonitor()
            {
                int result = zego_express_stop_sound_level_monitor();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void startAudioSpectrumMonitor(unsigned int millisecond)
            {
                int result = zego_express_start_audio_spectrum_monitor(millisecond);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void stopAudioSpectrumMonitor()
            {
                int result = zego_express_stop_audio_spectrum_monitor();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void startPerformanceMonitor(unsigned int millisecond)
            {
                int result = zego_express_start_performance_monitor(millisecond);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void stopPerformanceMonitor()
            {
                int result = zego_express_stop_performance_monitor();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void enableHeadphoneMonitor(bool enable)
            {
                int result = zego_express_enable_headphone_monitor(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void setHeadphoneMonitorVolume(int volume)
            {
                int result = zego_express_set_headphone_monitor_volume(volume);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void enableMixSystemPlayout(bool enable)
            {
                int result = zego_express_enable_mix_system_playout(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void setMixSystemPlayoutVolume(int volume)
            {
                int result = zego_express_set_mix_system_playout_volume(volume);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void startAudioVADStableStateMonitor(ZegoAudioVADStableStateMonitorType type, int millisecond)
            {
                zego_audio_vad_stable_state_monitor_type monitor_type =
                        static_cast<zego_audio_vad_stable_state_monitor_type>(type);
                int result = zego_express_start_audio_vad_stable_state_monitor(monitor_type, millisecond);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void stopAudioVADStableStateMonitor(ZegoAudioVADStableStateMonitorType type)
            {
                zego_audio_vad_stable_state_monitor_type monitor_type =
                        static_cast<zego_audio_vad_stable_state_monitor_type>(type);
                int result = zego_express_stop_audio_vad_stable_state_monitor(monitor_type);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void enableMixEnginePlayout(bool enable)
            {
                int result = zego_express_enable_mix_engine_playout(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void useAudioDevice(zego_audio_device_type deviceType, const char *deviceID)
            {
                int result = zego_express_use_audio_device(deviceType, deviceID);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            int getAudioDeviceVolume(zego_audio_device_type deviceType, const char *deviceID)
            {
                int volume = zego_express_get_audio_device_volume(deviceType, deviceID);
                return volume;
            }

            void setAudioDeviceVolume(zego_audio_device_type deviceType, const char *deviceID, int volume)
            {
                int result = zego_express_set_audio_device_volume(deviceType, deviceID, volume);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void startAudioDeviceVolumeMonitor(enum zego_audio_device_type device_type, const char * device_id)
            {
                int result = zego_express_start_audio_device_volume_monitor(device_type, device_id);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void stopAudioDeviceVolumeMonitor(enum zego_audio_device_type device_type, const char * device_id)
            {
                int result = zego_express_stop_audio_device_volume_monitor(device_type, device_id);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            void muteAudioDevice(enum zego_audio_device_type device_type, const char * device_id, bool mute)
            {
                int result = zego_express_mute_audio_device(device_type, device_id, mute);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            bool isAudioDeviceMuted(enum zego_audio_device_type device_type, const char * device_id)
            {
                bool muted = zego_express_is_audio_device_muted(device_type, device_id);
                return muted;
            }

            zego_device_info *getAudioDeviceList(zego_audio_device_type device_type, int *device_count)
            {
                zego_device_info *result = zego_express_get_audio_device_list(device_type, device_count);
                return result;
            }

            void freeAudioDeviceList(zego_device_info *device_list)
            {
                int result = zego_express_free_audio_device_list(device_list);
                (void)result;
            }

            const char *getDefaultAudioDeviceID(zego_audio_device_type device_type)
            {
                return zego_express_get_default_audio_device_id(device_type);
            }

            const char *getDefaultVideoDeviceID()
            {
                return zego_express_get_default_video_device_id();
            }

            zego_device_info getCurrentAudioDevice(zego_audio_device_type deviceType)
            {
                return zego_express_get_current_audio_device(deviceType);
            }

            void useVideoDevice(const char *deviceID, zego_publish_channel channel)
            {
                int result = zego_express_use_video_device(deviceID, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_DEVICE, __FUNCTION__, result);
            }

            zego_device_info *getVideoDeviceList(int *device_count)
            {
                zego_device_info *result = zego_express_get_video_device_list(device_count);
                return result;
            }

            void freeVideoDeviceList(zego_device_info *device_list)
            {
                int result = zego_express_free_video_device_list(device_list);
                (void)result;
            }

            void enableAEC(bool enable)
            {
                int result = zego_express_enable_aec(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enableHeadphoneAEC(bool enable)
            {
                int result = zego_express_enable_headphone_aec(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setAECMode(zego_aec_mode mode)
            {
                int result = zego_express_set_aec_mode(mode);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enableAGC(bool enable)
            {
                int result = zego_express_enable_agc(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setANSMode(zego_ans_mode mode)
            {
                int result = zego_express_set_ans_mode(mode);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enableANS(bool enable)
            {
                int result = zego_express_enable_ans(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enableTransientANS(bool enable)
            {
                int result = zego_express_enable_transient_ans(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enableAudioMixing(bool enable)
            {
                int result = zego_express_enable_audio_mixing(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void muteLocalAudioMixing(bool mute)
            {
                int result = zego_express_mute_local_audio_mixing(mute);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setAudioMixingVolume(int volume, zego_volume_type type)
            {
                int result = zego_express_set_audio_mixing_volume_with_type(volume, type);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setAudioEqualizerGain(int bandIndex, float bandGain)
            {
                int result = zego_express_set_audio_equalizer_gain(bandIndex, bandGain);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enableBeautify(int feature, zego_publish_channel channel)
            {
                int result = zego_express_enable_beautify(feature, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setBeautifyOption(zego_beautify_option option, zego_publish_channel channel)
            {
                int result = zego_express_set_beautify_option(option, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void startEffectsEnv()
            {
                int result = zego_express_start_effects_env();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void stopEffectsEnv()
            {
                int result = zego_express_stop_effects_env();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enableEffectsBeauty(bool enable)
            {
                int result = zego_express_enable_effects_beauty(enable);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setEffectsBeautyParam(zego_effects_beauty_param param)
            {
                int result = zego_express_set_effects_beauty_param(param);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setVoiceChangerPreset(zego_voice_changer_preset preset)
            {
                int result = zego_express_set_voice_changer_preset(preset);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setVoiceChangerParam(float param)
            {
                int result = zego_express_set_voice_changer_param(param);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enableVirtualStereo(bool enable, int angle)
            {
                int result = zego_express_enable_virtual_stereo(enable, angle);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void enablePlayStreamVirtualStereo(bool enable, int angle, const char* stream_id)
            {
                int result = zego_express_enable_play_stream_virtual_stereo(enable, angle, stream_id);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setElectronicEffects(bool enable, zego_electronic_effects_mode mode, int tonal)
            {
                int result = zego_express_set_electronic_effects(enable, mode, tonal);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setReverbPreset(zego_reverb_preset preset)
            {
                int result = zego_express_set_reverb_preset(preset);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setReverbParam(struct zego_reverb_param param)
            {
                int result = zego_express_set_reverb_param(param);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setReverbParam(struct zego_reverb_advanced_param param)
            {
                int result = zego_express_set_reverb_advanced_param(param);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            void setReverbEchoParam(struct zego_reverb_echo_param param)
            {
                int result = zego_express_set_reverb_echo_param(param);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_PREPROCESS, __FUNCTION__, result);
            }

            int createRealTimeSequentialDataManager(const char *room_id)
            {
                int instanceIndex = zego_express_create_real_time_sequential_data_manager(room_id);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_IM, __FUNCTION__, instanceIndex == -1 ? ZEGO_ERRCODE_IM_MANAGER_CREATION_FAILED : ZEGO_ERRCODE_COMMON_SUCCESS);
                return instanceIndex;
            }

            int destroyRealTimeSequentialDataManager(int instance_index)
            {
                int result = zego_express_destroy_real_time_sequential_data_manager(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_IM, __FUNCTION__, result);
                return result;
            }

            int realTimeSequentialDataStartBroadcasting(const char * stream_id, int instance_index)
            {
                int result = zego_express_real_time_sequential_data_start_broadcasting(stream_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_IM, __FUNCTION__, result);
                return result;
            }

            int realTimeSequentialDataStopBroadcasting(const char * stream_id, int instance_index)
            {
                int result = zego_express_real_time_sequential_data_stop_broadcasting(stream_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_IM, __FUNCTION__, result);
                return result;
            }

            int realTimeSequentialDataSendData(const unsigned char* data, unsigned int data_length, const char * stream_id, int instance_index)
            {
                int seq = zego_express_send_real_time_sequential_data(data, data_length, stream_id, instance_index);
                return seq;
            }

            int realTimeSequentialDataStartSubscribing(const char * stream_id, int_fast16_t instance_index)
            {
                int result = zego_express_real_time_sequential_data_start_subscribing(stream_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_IM, __FUNCTION__, result);
                return result;
            }

            int realTimeSequentialDataStopSubscribing(const char * stream_id, int instance_index)
            {
                int result = zego_express_real_time_sequential_data_stop_subscribing(stream_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_IM, __FUNCTION__, result);
                return result;
            }

            int sendBroadcastMessage(const char *room_id, const char *content)
            {
                int seq = zego_express_send_broadcast_message(room_id, content);
                return seq;
            }

            int sendBarrageMessage(const char *room_id, const char *content)
            {
                int seq = zego_express_send_barrage_message(room_id, content);
                return seq;
            }

            int sendCustomCommand(const char *room_id, const char *content, struct zego_user *to_user_list, unsigned int to_user_count)
            {
                int seq = zego_express_send_custom_command(room_id, content, to_user_list, to_user_count);
                return seq;
            }

            int startMixerTask(zego_mixer_task task)
            {
                int seq = zego_express_start_mixer_task(task);
                return seq;
            }

            int stopMixerTask(zego_mixer_task task)
            {
                int seq = zego_express_stop_mixer_task(task);
                return seq;
            }

            int startAutoMixerTask(zego_auto_mixer_task task)
            {
                int seq = zego_express_start_auto_mixer_task(task);
                return seq;
            }

            int stopAutoMixerTask(zego_auto_mixer_task task)
            {
                int seq = zego_express_stop_auto_mixer_task(task);
                return seq;
            }

            zego_media_player_instance_index createMediaPlayer()
            {
                zego_media_player_instance_index instanceIndex = zego_express_create_media_player();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, instanceIndex == zego_media_player_instance_index_null ? ZEGO_ERRCODE_MEDIAPLAYER_EXCEED_MAX_COUNT : ZEGO_ERRCODE_COMMON_SUCCESS);
                return instanceIndex;
            }

            int destroyMediaPlayer(zego_media_player_instance_index instance_index)
            {
                int result = zego_express_destroy_media_player(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerStart(zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_start(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerStop(zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_stop(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerPause(zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_pause(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerResume(zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_resume(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerPreload(const char *path, zego_media_player_instance_index instance_index)
            {
                int seq = zego_express_media_player_load_resource(path, instance_index);
                return seq;
            }

            int mediaPlayerPreloadWithPosition(const char *path, unsigned long long start_position, zego_media_player_instance_index instance_index)
            {
                int seq = zego_express_media_player_load_resource_with_position(path, start_position, instance_index);
                return seq;
            }

            int mediaPlayerPreload(unsigned char* media_data, int media_data_length, unsigned long long start_position, zego_media_player_instance_index instance_index)
            {
                int seq = zego_express_media_player_load_resource_from_media_data(media_data, media_data_length, start_position, instance_index);
                return seq;
            }

            int mediaPlayerPreloadCopyrightedMusicResourceWithPosition(const char* recource_id, unsigned long long start_position, zego_media_player_instance_index instance_index)
            {
                int seq = zego_express_media_player_load_copyrighted_music_resource_with_position(recource_id, start_position, instance_index);
                return seq;
            }

            int mediaPlayerSeekTo(unsigned long long millisecond, zego_media_player_instance_index instance_index)
            {
                int seq = zego_express_media_player_seek_to(millisecond, instance_index);
                return seq;
            }

            int mediaPlayerSetVolume(int volume, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_volume(volume, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetPlayVolume(int volume, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_play_volume(volume, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetPublishVolume(int volume, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_publish_volume(volume, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            unsigned long long mediaPlayerGetTotalDuration(zego_media_player_instance_index instance_index)
            {
                unsigned long long totalDuration = zego_express_media_player_get_total_duration(instance_index);
                return totalDuration;
            }

            unsigned long long mediaPlayerGetCurrentProgress(zego_media_player_instance_index instance_index)
            {
                unsigned long long currentProgress = zego_express_media_player_get_current_progress(instance_index);
                return currentProgress;
            }

            int mediaPlayerGetPublishVolume(zego_media_player_instance_index instance_index)
            {
                int volume = zego_express_media_player_get_publish_volume(instance_index);
                return volume;
            }

            int mediaPlayerGetPlayVolume(zego_media_player_instance_index instance_index)
            {
                int volume = zego_express_media_player_get_play_volume(instance_index);
                return volume;
            }

            zego_media_player_state mediaPlayerGetCurrentState(zego_media_player_instance_index instance_index)
            {
                zego_media_player_state state = zego_express_media_player_get_current_state(instance_index);
                return state;
            }

            unsigned int mediaPlayerGetAudioTrackCount(zego_media_player_instance_index instance_index)
            {
                unsigned int count = zego_express_media_player_get_audio_track_count(instance_index);
                return count;
            }

            int mediaPlayerSetAudioTrackIndex(unsigned int index, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_audio_track_index(index, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetVoiceChangerParam(zego_media_player_audio_channel audio_channel, float pitch, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_voice_changer_param(audio_channel, pitch, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int takeSnapshot(zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_take_snapshot(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerEnableAccurateSeek(bool enable, struct zego_accurate_seek_config* config, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_enable_accurate_seek(enable, config, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetNetWorkResourceMaxCache(unsigned int time, unsigned int size, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_network_resource_max_cache(time, size, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }
            int mediaPlayerGetNetWorkResourceCache(struct zego_network_resource_cache* cache, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_get_network_resource_cache(cache, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetNetWorkBufferThreshold(unsigned int threshold, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_network_buffer_threshold(threshold, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerMuteLocal(bool mute, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_mute_local_audio(mute, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetPlayerCanvas(struct zego_canvas *canvas, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_player_canvas(canvas, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerEnableAUX(bool enable, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_enable_aux(enable, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetPlayLoopCount(int count, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_play_loop_count(count, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetPlaySpeed(float speed, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_play_speed(speed, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerEnableRepeat(bool enable, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_enable_repeat(enable, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerSetProgressInterval(unsigned long long millisecond, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_set_progress_interval(millisecond, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerEnableAudioData(bool enable, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_enable_audio_data(enable, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerEnableVideoData(bool enable, zego_video_frame_format format, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_enable_video_data(enable, format, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerEnableSoundLevelMonitor(bool enable, unsigned int millisecond, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_enable_sound_level_monitor(enable, millisecond, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            int mediaPlayerEnableFrequencySpectrumMonitor(bool enable, unsigned int millisecond, zego_media_player_instance_index instance_index)
            {
                int result = zego_express_media_player_enable_frequency_spectrum_monitor(enable, millisecond, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_MEDIAPLAYER, __FUNCTION__, result);
                return result;
            }

            zego_audio_effect_player_instance_index createAudioEffectPlayer()
            {
                zego_audio_effect_player_instance_index instanceIndex = zego_express_create_audio_effect_player();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, instanceIndex == zego_audio_effect_player_instance_index_null ? ZEGO_ERRCODE_AUDIOEFFECTPLAYER_EXCEED_MAX_COUNT : ZEGO_ERRCODE_COMMON_SUCCESS);
                return instanceIndex;
            }

            int destroyAudioEffectPlayer(zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_destroy_audio_effect_player(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerLoadResource(unsigned int audio_effect_id, const char *path, zego_audio_effect_player_instance_index instance_index)
            {
                int seq = zego_express_audio_effect_player_load_resource(audio_effect_id, path, instance_index);
                return seq;
            }

            int audioEffectPlayerUnloadResource(unsigned int audio_effect_id, zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_unload_resource(audio_effect_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerStart(unsigned int audio_effect_id, const char *path, struct zego_audio_effect_play_config *config, zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_start(audio_effect_id, path, config, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            };

            int audioEffectPlayerStop(unsigned int audio_effect_id, zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_stop(audio_effect_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerPause(unsigned int audio_effect_id, zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_pause(audio_effect_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerResume(unsigned int audio_effect_id, zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_resume(audio_effect_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerStopAll(zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_stop_all(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerPauseAll(zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_pause_all(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerResumeAll(zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_resume_all(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerSeekTo(unsigned int audio_effect_id, unsigned long long millisecond, zego_audio_effect_player_instance_index instance_index)
            {
                int seq = zego_express_audio_effect_player_seek_to(audio_effect_id, millisecond, instance_index);
                return seq;
            }

            int audioEffectPlayerSetVolume(unsigned int audio_effect_id, int volume, zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_set_volume(audio_effect_id, volume, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerSetVolumeAll(int volume, zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_set_volume_all(volume, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            int audioEffectPlayerSetPlaySpeed(unsigned int audio_effect_id, float speed, zego_audio_effect_player_instance_index instance_index)
            {
                int result = zego_express_audio_effect_player_set_play_speed(audio_effect_id, speed, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_AUDIOEFFECTPLAYER, __FUNCTION__, result);
                return result;
            }

            unsigned long long audioEffectPlayerGetTotalDuration(unsigned int audio_effect_id, zego_audio_effect_player_instance_index instance_index)
            {
                unsigned long long result = zego_express_audio_effect_player_get_total_duration(audio_effect_id, instance_index);
                return result;
            }

            unsigned long long audioEffectPlayerGetCurrentProgress(unsigned int audio_effect_id, zego_audio_effect_player_instance_index instance_index)
            {
                unsigned long long result = zego_express_audio_effect_player_get_current_progress(audio_effect_id, instance_index);
                return result;
            }

            void enableCustomVideoRender(bool enable, struct zego_custom_video_render_config *config)
            {
                int result = zego_express_enable_custom_video_render(enable, config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
            }

            void enableCustomVideoCapture(bool enable, zego_custom_video_capture_config *config, zego_publish_channel channel)
            {
                int result = zego_express_enable_custom_video_capture(enable, config, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
            }

            void customVideoCaptureSetFillMode(zego_view_mode mode, zego_publish_channel channel)
            {
                int result = zego_express_set_custom_video_capture_fill_mode(mode, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
            }

            void setCustomVideoCaptureDeviceState(bool isEnable, zego_remote_device_state state, zego_publish_channel channel)
            {
                int result = zego_express_set_custom_video_capture_device_state(isEnable, state, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
            }

            void customVideoCaptureRegionOfInterest(zego_roi_rect *rects, unsigned int rects_count, zego_publish_channel channel)
            {
                int result = zego_express_set_custom_video_capture_region_of_interest(rects, rects_count, channel);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
                }
            }

            void customVideoCaptureSendRawData(const unsigned char *data, unsigned int data_length, const struct zego_video_frame_param param, unsigned long long reference_time_millisecond, zego_publish_channel channel)
            {
                int result = zego_express_send_custom_video_capture_raw_data(data, data_length, param, reference_time_millisecond, 1000, channel);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
                }
            }

            void customVideoCaptureSendEncodedFrameData(const unsigned char *data, unsigned int data_length, const struct zego_video_encoded_frame_param param, unsigned long long reference_time_millisecond, zego_publish_channel channel)
            {
                int result = zego_express_send_custom_video_capture_encoded_data(data, data_length, param, double(reference_time_millisecond), channel);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
                }
            }

            void enableCustomVideoProcessing(bool enable, zego_custom_video_process_config *config, zego_publish_channel channel)
            {
                int result = zego_express_enable_custom_video_processing(enable, config, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
            }

            void sendCustomVideoProcessedRawData(const unsigned char **data, unsigned int *data_length, const struct zego_video_frame_param param, unsigned long long reference_time_millisecond, zego_publish_channel channel)
            {
                int result = zego_express_send_custom_video_processed_raw_data(data, data_length, param, reference_time_millisecond, channel);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
                }
            }

            void sendCustomVideoProcessedCVPixelBuffer(void * buffer, unsigned long long reference_time_millisecond, zego_publish_channel channel)
            {
                int result = zego_express_send_custom_video_processed_cv_pixel_buffer(buffer, reference_time_millisecond, channel);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_CUSTOMVIDEOIO, __FUNCTION__, result);
                }
            }

            void enableCustomAudioCaptureProcessing(bool enable, zego_custom_audio_process_config *config)
            {
                int result = zego_express_enable_custom_audio_capture_processing(enable, config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
            }

            void enableCustomAudioCaptureProcessingAfterHeadphoneMonitor(bool enable, zego_custom_audio_process_config *config)
            {
                int result = zego_express_enable_custom_audio_capture_processing_after_headphone_monitor(enable, config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
            }

            void enableCustomAudioRemoteProcessing(bool enable, zego_custom_audio_process_config *config)
            {
                int result = zego_express_enable_custom_audio_remote_processing(enable, config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
            }

            void enableCustomAudioPlaybackProcessing(bool enable, zego_custom_audio_process_config *config)
            {
                int result = zego_express_enable_custom_audio_playback_processing(enable, config);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
            }

            void enableAudioDataCallback(bool enable, unsigned int callback_bitmask, zego_audio_frame_param param)
            {
                int result = zego_express_enable_audio_data_callback(enable, callback_bitmask, param);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
            }

            void startAudioDataObserver(unsigned int observer_bitmask, zego_audio_frame_param param)
            {
                int result = zego_express_start_audio_data_observer(observer_bitmask, param);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
            }

            void stopAudioDataObserver()
            {
                int result = zego_express_stop_audio_data_observer();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
            }

            void startRecordingCapturedData(zego_data_record_config config, zego_publish_channel channel)
            {
                int result = zego_express_start_recording_captured_data(config, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RECORD, __FUNCTION__, result);
            }

            void stopRecordingCapturedData(zego_publish_channel channel)
            {
                int result = zego_express_stop_recording_captured_data(channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RECORD, __FUNCTION__, result);
            }

            void enableCustomAudioIO(bool enable, zego_custom_audio_config *config, zego_publish_channel channel)
            {
                int result = zego_express_enable_custom_audio_io(enable, config, channel);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RECORD, __FUNCTION__, result);
            }

            void sendCustomAudioCaptureAACData(unsigned char *data, unsigned int dataLength, unsigned int configLength, unsigned long long referenceTimeMillisecond, zego_audio_frame_param param, zego_publish_channel channel)
            {
                int result = zego_express_send_custom_audio_capture_aac_data(data, dataLength, configLength, referenceTimeMillisecond, param, channel);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
                }
            }

            void sendCustomAudioCapturePCMData(unsigned char *data, unsigned int dataLength, zego_audio_frame_param param, zego_publish_channel channel)
            {
                int result = zego_express_send_custom_audio_capture_pcm_data(data, dataLength, param, channel);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
                }
            }

            void fetchCustomAudioRenderPCMData(unsigned char *data, unsigned int dataLength, zego_audio_frame_param param)
            {
                int result = zego_express_fetch_custom_audio_render_pcm_data(data, dataLength, param);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_CUSTOMAUDIOIO, __FUNCTION__, result);
                }
            }

            int testNetworkConnectivity()
            {
                int seq = zego_express_test_network_connectivity();
                return seq;
            }

            void startNetworkSpeedTest(zego_network_speed_test_config config, unsigned int interval)
            {
                int result = zego_express_start_network_speed_test(config, interval);
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_UTILITIES, __FUNCTION__, result);
                }
            }

            void stopNetworkSpeedTest()
            {
                int result = zego_express_stop_network_speed_test();
                if (result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_UTILITIES, __FUNCTION__, result);
                }
            }

            zego_network_time_info getNetworkTimeInfo()
            {
                zego_network_time_info timeInfo;
                int result = zego_express_get_network_time_info(&timeInfo);
                if(result != 0)
                {
                    printDebugInfo(ZEGO_EXPRESS_MODULE_UTILITIES, __FUNCTION__, result);
                }

                return timeInfo;
            }

			int startNetworkProbe(zego_network_probe_config config)
			{
				int seq = zego_express_start_network_probe(config);
				return seq;
			}

			void stopNetworkProbe()
			{
				int result = zego_express_stop_network_probe();
				if (result != 0)
				{
					printDebugInfo(ZEGO_EXPRESS_MODULE_UTILITIES, __FUNCTION__, result);
				}
			}

            zego_range_audio_instance_index createRangeAudio()
            {
                zego_range_audio_instance_index instanceIndex = zego_express_create_range_audio();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, instanceIndex == zego_range_audio_instance_index_null ? ZEGO_ERRCODE_RANGEAUDIO_EXCEED_MAX_COUNT : ZEGO_ERRCODE_COMMON_SUCCESS);
                return instanceIndex;
            }

            int destroyRangeAudio(zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_destroy_range_audio(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
                return result;
            }

            void setRangeAudioMode(zego_range_audio_mode mode, zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_set_range_audio_mode(mode, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void rangeAudioSetTeamID(const char * team_id, zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_range_audio_set_team_id(team_id, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void rangeAudioSetAudioReceiveRange(float range, zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_range_audio_set_audio_receive_range(range, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void rangeAudioEnableMicrophone(bool enable, zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_range_audio_enable_microphone(enable, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void rangeAudioEnableSpeaker(bool enable, zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_range_audio_enable_speaker(enable, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void rangeAudioEnableSpatializer(bool enable, zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_range_audio_enable_spatializer(enable, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void rangeAudioUpdateAudioSource(const char *user_id, float position[3], zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_range_audio_update_audio_source(user_id, position, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void rangeAudioUpdateSelfPosition(float position[3], float axisForward[3], float axisRight[3], float axisUp[3], zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_range_audio_update_self_position(position, axisForward, axisRight, axisUp, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void rangeAudioMuteUser(const std::string& userID, bool mute, zego_range_audio_instance_index instance_index)
            {
                int result = zego_express_range_audio_mute_user(userID.c_str(), mute, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void createCopyrightedMusic()
            {
                zego_express_create_copyrighted_music();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC, __FUNCTION__, 0);
            }

            void destroyCopyrightedMusic()
            {
                int result = zego_express_destroy_copyrighted_music();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC, __FUNCTION__, result);
            }

            int copyrightedMusicInitCopyrightedMusic(zego_copyrighted_music_config config)
            {
                int seq = zego_express_copyrighted_music_init(config);
                return seq;
            }

            unsigned long long copyrightedMusicGetCacheSize()
            {
                unsigned long long size = zego_express_copyrighted_music_get_cache_size();
                return size;
            }

            void copyrightedMusicClearCache()
            {
                int result = zego_express_copyrighted_music_clear_cache();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_COPYRIGHTEDMUSIC, __FUNCTION__, result);
            }

            int copyrightedMusicSendExtendedRequest(const char* command, const char* params)
            {
                int seq = zego_express_copyrighted_music_send_extended_request(command, params);
                return seq;
            }

            int copyrightedMusicGetLrcLyric(const char* song_id)
            {
                int seq = zego_express_copyrighted_music_get_lrc_lyric(song_id);
                return seq;
            }

            int copyrightedMusicGetKrcLyricByToken(const char* krc_token)
            {
                int seq = zego_express_copyrighted_music_get_krc_lyric_by_token(krc_token);
                return seq;
            }

            int copyrightedMusicRequestSong(zego_copyrighted_music_request_config config)
            {
                int seq = zego_express_copyrighted_music_request_song(config);
                return seq;
            }

            int copyrightedMusiRequestAccompaniment(zego_copyrighted_music_request_config config)
            {
                int seq = zego_express_copyrighted_music_request_accompaniment(config);
                return seq;
            }

            int copyrightedMusiRequestAccompanimentClip(zego_copyrighted_music_request_config config)
            {
                int seq = zego_express_copyrighted_music_request_accompaniment_clip(config);
                return seq;
            }

            int copyrightedMusicGetMusicByToken(const char* song_token)
            {
                int seq = zego_express_copyrighted_music_get_music_by_token(song_token);
                return seq;
            }

            int copyrightedMusicDownload(const char* resource_id)
            {
                int seq = zego_express_copyrighted_music_download(resource_id);
                return seq;
            }

            bool copyrightedMusicQueryCache(const char* song_id, zego_copyrighted_music_type type)
            {
                bool result = zego_express_copyrighted_music_query_cache(song_id, type);
                return result;
            }

            unsigned long long copyrightedMusicGetDuration(const char* resource_id)
            {
                unsigned long long totalDuration = zego_express_copyrighted_music_get_duration(resource_id);
                return totalDuration;
            }

            int copyrightedMusicStartScore(const char* resource_id, int pitchValueInterval)
            {
                int ret = zego_express_copyrighted_music_start_score(resource_id, pitchValueInterval);
                return ret;
            }

            int copyrightedMusicPauseScore(const char* resource_id)
            {
                int ret = zego_express_copyrighted_music_pause_score(resource_id);
                return ret;
            }

            int copyrightedMusicResumeScore(const char* resource_id)
            {
                int ret = zego_express_copyrighted_music_resume_score(resource_id);
                return ret;
            }

            int copyrightedMusicStopScore(const char* resource_id)
            {
                int ret = zego_express_copyrighted_music_stop_score(resource_id);
                return ret;
            }


            int copyrightedMusicResetScore(const char* resource_id)
            {
                int ret = zego_express_copyrighted_music_reset_score(resource_id);
                return ret;
            }

            int copyrightedMusicGetPreviousScore(const char* resource_id)
            {
                int ret = zego_express_copyrighted_music_get_previous_score(resource_id);
                return ret;
            }

            int copyrightedMusicGetAverageScore(const char* resource_id)
            {
                int ret = zego_express_copyrighted_music_get_average_score(resource_id);
                return ret;
            }

            int copyrightedMusicGetTotalScore(const char* resource_id)
            {
                int ret = zego_express_copyrighted_music_get_total_score(resource_id);
                return ret;
            }

            int copyrightedMusicGetCurrentPitch(const char* resource_id)
            {
                int ret = zego_express_copyrighted_music_get_current_pitch(resource_id);
                return ret;
            }

            int copyrightedMusicGetStandardPitch(const char* resource_id)
            {
                int seq = zego_express_copyrighted_music_get_standard_pitch(resource_id);
                return seq;
            }

            zego_instance createMediaDataPublisher()
            {
                zego_instance instanceIndex = zego_express_create_media_data_publisher();
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, instanceIndex < 0 ? ZEGO_ERROR_CODE_MEDIA_DATA_PUBLISHER_EXCEED_MAX_COUNT : ZEGO_ERRCODE_COMMON_SUCCESS);
                return instanceIndex;
            }

            void destroyMediaDataPublisher(zego_instance instance_index)
            {
                zego_express_destroy_media_data_publisher(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, ZEGO_ERRCODE_COMMON_SUCCESS);
            }

            void mediaDataPublisherAddMediaFilePath(const std::string& path, bool is_clear, zego_instance instance_index)
            {
                int result = zego_express_media_data_publisher_add_media_file_path(path.c_str(), is_clear, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void mediaDataPublisherReset(zego_instance instance_index)
            {
                int result = zego_express_media_data_publisher_reset(instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void mediaDataPublisherSetVideoSendDelayTime(int delayTime, zego_instance instance_index)
            {
                int result = zego_express_media_data_publisher_set_video_send_delay_time(delayTime, instance_index);
                printDebugInfoAndPopupWhenError(ZEGO_EXPRESS_MODULE_RANGEAUDIO, __FUNCTION__, result);
            }

            void mediaDataPublisherSeekTo(unsigned long long millisecond, zego_instance instance_index)
            {
                zego_express_media_data_publisher_seek_to(millisecond, instance_index);
            }

            unsigned long long mediaDataPublisherGetTotalDuration(zego_instance instance_index)
            {
                return zego_express_media_data_publisher_get_total_duration(instance_index);
            }

            unsigned long long mediaDataPublisherGetCurrentDuration(zego_instance instance_index)
            {
                return zego_express_media_data_publisher_get_current_duration(instance_index);
            }

            void printDebugInfoAndPopupWhenError(int module, const char *function_name, int error_code)
            {
                auto detail = zego_express_get_print_debug_info(module, function_name, error_code);
                zego_express_trigger_on_api_called_result(error_code,function_name,detail);

                if (pri->enableDebugErrorAlways || error_code != 0)
                {
                    zego_express_trigger_on_debug_error(error_code, function_name, detail);
                }

                zego_express_show_debug_message_box(error_code, function_name, detail);
            }

            void printDebugInfo(int module, const char *function_name, int error_code)
            {
                if (pri->enableDebugErrorAlways || error_code != 0)
                {
                    auto detail = zego_express_get_print_debug_info(module, function_name, error_code);
                    zego_express_trigger_on_debug_error(error_code, function_name, detail);
                }
            }

            void postWindowsMessage(void *message)
            {
                zego_express_post_windows_message(message);
            }

            // eventHandler
            void registerRecvWindowsMessageCallback(void *callback, void *user_context)
            {
                zego_register_recv_windows_message_callback(callback, user_context);
            }

            void registerEngineStateCallback(void *callback_func, void *user_context)
            {
                zego_register_engine_state_update_callback(zego_on_engine_state_update(callback_func), user_context);
            }

            void registerUploadLogResultCallback(void *callback_func, void *user_context)
            {
                zego_register_upload_log_result_callback(zego_on_upload_log_result(callback_func), user_context);
            }

            void registerEngineUninitCallback(void *callback_func, void *user_context)
            {
                zego_register_engine_uninit_callback(zego_on_engine_uninit(callback_func), user_context);
            }

            void registerOnCallErrorCallback(void *callback, void *context)
            {
                zego_register_debug_error_callback(zego_on_debug_error(callback), context);
            }
            void registerOnApiCalledResultCallback(void *callback, void *context)
            {
                zego_register_api_called_result_callback(zego_on_api_called_result(callback), nullptr);
            }

            void registerRoomStreamUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_room_stream_update_callback(zego_on_room_stream_update(callback_func), user_context);
            }

            void registerRoomStreamExtraInfoUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_room_stream_extra_info_update_callback(zego_on_room_stream_extra_info_update(callback_func), user_context);
            }

            void registerRoomStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_room_state_update_callback(zego_on_room_state_update(callback_func), user_context);
            }

            void registerRoomUserStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_room_user_update_callback(zego_on_room_user_update(callback_func), user_context);
            }

            void registerRoomOnlineUserCountUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_room_online_user_count_update_callback(zego_on_room_online_user_count_update(callback_func), user_context);
            }

            void registerRoomExtraInfoUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_room_extra_info_update_callback(zego_on_room_extra_info_update(callback_func), user_context);
            }

            void registerRoomSetRoomExtraInfoResultCallback(void *callback_func, void *user_context)
            {
                zego_register_room_set_room_extra_info_result_callback(zego_on_room_set_room_extra_info_result(callback_func), user_context);
            }

            void registerRoomTokenWillExpireCallback(void *callback_func, void *user_context)
            {
                zego_register_room_token_will_expire_callback(zego_on_room_token_will_expire(callback_func), user_context);
            }

            void registerRoomLoginCallback(void *callback_func, void *user_context)
            {
                zego_register_room_login_result_callback(zego_on_room_login_result(callback_func), user_context);
            }

            void registerRoomLogoutCallback(void *callback_func, void *user_context)
            {
                zego_register_room_logout_result_callback(zego_on_room_logout_result(callback_func), user_context);
            }

            void registerRoomStateChangedCallback(void *callback_func, void *user_context)
            {
                zego_register_room_state_changed_callback(zego_on_room_state_changed(callback_func), user_context);
            }

            void registerPublisherStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_state_update_callback(zego_on_publisher_state_update(callback_func), user_context);
            }

            void registerPublisherQualityUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_quality_update_callback(zego_on_publisher_quality_update(callback_func), user_context);
            }

            void registerPublisherRecvAudioCapturedFirstFrameCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_captured_audio_first_frame_callback(zego_on_publisher_captured_audio_first_frame(callback_func), user_context);
            }

            void registerPublisherRecvVideoCapturedFirstFrameCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_captured_video_first_frame_callback(zego_on_publisher_captured_video_first_frame(callback_func), user_context);
            }

            void registerPublisherRenderVideoFirstFrameCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_render_video_first_frame_callback(zego_on_publisher_render_video_first_frame(callback_func), user_context);
            }

            void registerPublisherVideoSizeChangedCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_video_size_changed_callback(zego_on_publisher_video_size_changed(callback_func), user_context);
            }

            void registerPublisherRelayCDNStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_relay_cdn_state_update_callback(zego_on_publisher_relay_cdn_state_update(callback_func), user_context);
            }

            void registerPublisherVideoEncoderChangedCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_video_encoder_changed_callback(zego_on_publisher_video_encoder_changed(callback_func), user_context);
            }

            void registerPublisherUpdatePublishCdnUrlCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_update_cdn_url_result_callback(zego_on_publisher_update_cdn_url_result(callback_func), user_context);
            }

            void registerPublisherUpdateStreamExtraInfoResultCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_update_stream_extra_info_result_callback(zego_on_publisher_update_stream_extra_info_result(callback_func), user_context);
            }

            void registerPublisherTakeSnapshotResultCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_take_snapshot_result_callback(zego_on_publisher_take_snapshot_result(callback_func), user_context);
            }

            void registerPublisherStreamEventCallback(void *callback_func, void *user_context)
            {
                zego_register_publisher_stream_event_callback(zego_on_publisher_stream_event(callback_func), user_context);
            }

            void registerPlayerStreamEventCallback(void *callback_func, void *user_context)
            {
                zego_register_player_stream_event_callback(zego_on_player_stream_event(callback_func), user_context);
            }

            void registerPlayerStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_player_state_update_callback(zego_on_player_state_update(callback_func), user_context);
            }

            void registerPlayerQualityUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_player_quality_update_callback(zego_on_player_quality_update(callback_func), user_context);
            }

            void registerPlayerMediaEventCallback(void *callback_func, void *user_context)
            {
                zego_register_player_media_event_callback(zego_on_player_media_event(callback_func), user_context);
            }

            void registerPlayerRecvAudioFirstFrameCallback(void *callback_func, void *user_context)
            {
                zego_register_player_recv_audio_first_frame_callback(zego_on_player_recv_audio_first_frame(callback_func), user_context);
            }

            void registerPlayerRecvVideoFirstFrameCallback(void *callback_func, void *user_context)
            {
                zego_register_player_recv_video_first_frame_callback(zego_on_player_recv_video_first_frame(callback_func), user_context);
            }

            void registerPlayerRenderVideoFirstFrameCallback(void *callback_func, void *user_context)
            {
                zego_register_player_render_video_first_frame_callback(zego_on_player_render_video_first_frame(callback_func), user_context);
            }

            void registerPlayerVideoSizeChangedCallback(void *callback_func, void *user_context)
            {
                zego_register_player_video_size_changed_callback(zego_on_player_video_size_changed(callback_func), user_context);
            }

            void registerPlayerRecvSEICallback(void *callback_func, void *user_context)
            {
                zego_register_player_recv_sei_callback(zego_on_player_recv_sei(callback_func), user_context);
            }

            void registerPlayerRecvAudioSideInfoCallback(void *callback_func, void *user_context)
            {
                zego_register_player_recv_audio_side_info_callback(zego_on_player_recv_audio_side_info(callback_func), user_context);
            }

            void registerPlayerTakeSnapshotResultCallback(void *callback_func, void *user_context)
            {
                zego_register_player_take_snapshot_result_callback(zego_on_player_take_snapshot_result(callback_func), user_context);
            }

            void registerPlayerLowFpsWarningCallback(void *callback_func, void *user_context)
            {
                zego_register_player_low_fps_warning_callback(zego_on_player_low_fps_warning(callback_func), user_context);
            }

            void registerCapturedSoundLevelUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_captured_sound_level_update_callback(zego_on_captured_sound_level_update(callback_func), user_context);
            }

            void registerRemoteSoundLevelUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_remote_sound_level_update_callback(zego_on_remote_sound_level_update(callback_func), user_context);
            }

            void registerCapturedSoundLevelInfoUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_captured_sound_level_info_update_callback(zego_on_captured_sound_level_info_update(callback_func), user_context);
            }

            void registerRemoteSoundLevelInfoUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_remote_sound_level_info_update_callback(zego_on_remote_sound_level_info_update(callback_func), user_context);
            }

            void registerAudioVADStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_audio_vad_state_update_callback(zego_on_audio_vad_state_update(callback_func), user_context);
            }

            void registerCapturedAudioSpectrumUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_captured_audio_spectrum_update_callback(zego_on_captured_audio_spectrum_update(callback_func), user_context);
            }

            void registerRemoteAudioSpectrumUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_remote_audio_spectrum_update_callback(zego_on_remote_audio_spectrum_update(callback_func), user_context);
            }

            void registerAudioDeviceStateChangedCallback(void *callback_func, void *user_context)
            {
                zego_register_audio_device_state_changed_callback(zego_on_audio_device_state_changed(callback_func), user_context);
            }

            void registerAudioDeviceVolumeChangedCallback(void *callback_func, void *user_context)
            {
                zego_register_audio_device_volume_changed_callback(zego_on_audio_device_volume_changed(callback_func), user_context);
            }

            void registerVideoDeviceStateChangedCallback(void *callback_func, void *user_context)
            {
                zego_register_video_device_state_changed_callback(zego_on_video_device_state_changed(callback_func), user_context);
            }

            void registerLocalDeviceExceptionOccurredCallback(void *callback_func, void *user_context)
            {
                zego_register_local_device_exception_occurred_callback(zego_on_local_device_exception_occurred(callback_func), user_context);
            }

            void registerDeviceErrorCallback(void *callback_func, void *user_context)
            {
                zego_register_device_error_callback(zego_on_device_error(callback_func), user_context);
            }

            void registerAudioRouteChangeCallback(void *callback_func, void *user_context)
            {
                zego_register_audio_route_change_callback(zego_on_audio_route_change(callback_func), user_context);
            }

            void registerRemoteCameraStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_remote_camera_state_update_callback(zego_on_remote_camera_state_update(callback_func), user_context);
            }

            void registerRemoteMicStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_remote_mic_state_update_callback(zego_on_remote_mic_state_update(callback_func), user_context);
            }

            void registerRemoteSpeakerStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_remote_speaker_state_update_callback(zego_on_remote_speaker_state_update(callback_func), user_context);
            }

            void registerRealTimeSequentialDataSentCallback(void *callback_func, void *user_context)
            {
                zego_register_real_time_sequential_data_sent_callback(zego_on_real_time_sequential_data_sent(callback_func), user_context);
            }

            void registerReceiveRealTimeSequentialDataCallback(void *callback_func, void *user_context)
            {
                zego_register_receive_real_time_sequential_data_callback(zego_on_receive_real_time_sequential_data(callback_func), user_context);
            }

            void registerIMSendBroadcastMessageResultCallback(void *callback_func, void *user_context)
            {
                zego_register_im_send_broadcast_message_result_callback(zego_on_im_send_broadcast_message_result(callback_func), user_context);
            }

            void registerIMRecvBroadcasMessageCallback(void *callback_func, void *user_context)
            {
                zego_register_im_recv_broadcast_message_callback(zego_on_im_recv_broadcast_message(callback_func), user_context);
            }

            void registerIMSendBarrageMessageResultCallback(void *callback_func, void *user_context)
            {
                zego_register_im_send_barrage_message_result_callback(zego_on_im_send_barrage_message_result(callback_func), user_context);
            }

            void registerIMRecvBarrageMessageResultCallback(void *callback_func, void *user_context)
            {
                zego_register_im_recv_barrage_message_callback(zego_on_im_recv_barrage_message(callback_func), user_context);
            }

            void registerIMSendCustomCommandResultCallback(void *callback_func, void *user_context)
            {
                zego_register_im_send_custom_command_result_callback(zego_on_im_send_custom_command_result(callback_func), user_context);
            }

            void registerIMRecvCustomCommandCallback(void *callback_func, void *user_context)
            {
                zego_register_im_recv_custom_command_callback(zego_on_im_recv_custom_command(callback_func), user_context);
            }

            void registerMixerStartResultCallback(void *callback_func, void *user_context)
            {
                zego_register_mixer_start_result_callback(zego_on_mixer_start_result(callback_func), user_context);
                zego_register_auto_mixer_start_result_callback(zego_on_auto_mixer_start_result(callback_func), user_context);
            }

            void registerMixerStopResultCallback(void *callback_func, void *user_context)
            {
                zego_register_mixer_stop_result_callback(zego_on_mixer_stop_result(callback_func), user_context);
                zego_register_auto_mixer_stop_result_callback(zego_on_auto_mixer_stop_result(callback_func), user_context);
            }

            void registerMixerSoundLevelUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_mixer_sound_level_update_callback(zego_on_mixer_sound_level_update(callback_func), user_context);
            }

            void registerAutoMixerSoundLevelUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_auto_mixer_sound_level_update_callback(zego_on_auto_mixer_sound_level_update(callback_func), user_context);
            }

            void registerMixerRelayCDNStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_mixer_relay_cdn_state_update_callback(zego_on_mixer_relay_cdn_state_update(callback_func), user_context);
            }

            void registerMediaPlayerStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_state_update_callback(zego_on_media_player_state_update(callback_func), user_context);
            }

            void registerMediaPlayerNetworkEventCallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_network_event_callback(zego_on_media_player_network_event(callback_func), user_context);
            }

            void registerMediaPlayerPlayingProgressCallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_playing_progress_callback(zego_on_media_player_playing_progress(callback_func), user_context);
            }

            void registerMediaPlayerRecvSEICallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_recv_sei_callback(zego_on_media_player_recv_sei(callback_func), user_context);
            }

            void registerMediaPlayerSoundLevelUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_sound_level_update_callback(zego_on_media_player_sound_level_update(callback_func), user_context);
            }

            void registerMediaPlayerFrequencySpectrumUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_frequency_spectrum_update_callback(zego_on_media_player_frequency_spectrum_update(callback_func), user_context);
            }

            void registerMediaPlayerSeekToResult(void *callback_func, void *user_context)
            {
                zego_register_media_player_seek_to_callback(zego_on_media_player_seek_to(callback_func), user_context);
            }

            void registerMediaPlayerLoadFileResult(void *callback_func, void *user_context)
            {
                zego_register_media_player_load_resource_callback(zego_on_media_player_load_resource(callback_func), user_context);
            }

            void registerMediaPlayerAudioDataCallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_audio_frame_callback(zego_on_media_player_audio_frame(callback_func), user_context);
            }

            void registerMediaPlayerVideoDataCallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_video_frame_callback(zego_on_media_player_video_frame(callback_func), user_context);
            }

             void registerMediaPlayerTakeSnapshotResultCallback(void *callback_func, void *user_context)
            {
                zego_register_media_player_take_snapshot_result_callback(zego_on_media_player_take_snapshot_result(callback_func), user_context);
            }

            void registerAudioEffectPlayStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_audio_effect_play_state_update_callback(zego_on_audio_effect_play_state_update(callback_func), user_context);
            }

            void registerAudioEffectPlayerLoadResourceCallback(void *callback_func, void *user_context)
            {
                zego_register_audio_effect_player_load_resource_callback(zego_on_audio_effect_player_load_resource(callback_func), user_context);
            }

            void registerAudioEffectPlayerSeekToCallback(void *callback_func, void *user_context)
            {
                zego_register_audio_effect_player_seek_to_callback(zego_on_audio_effect_player_seek_to(callback_func), user_context);
            }

            void registerRangeAudioMicrophoneStateUpdate(void *callback_func, void *user_context)
            {
                zego_register_range_audio_microphone_state_update_callback(zego_on_range_audio_microphone_state_update(callback_func), user_context);
            }

            void registerCustomVideoRenderLocalFrameDataCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_render_captured_frame_data_callback(zego_on_custom_video_render_captured_frame_data(callback_func), user_context);
            }

            void registerCustomVideoRenderRemoteFrameDataCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_render_remote_frame_data_callback(zego_on_custom_video_render_remote_frame_data(callback_func), user_context);
            }

            void registerCustomVideoRenderLocalFrameEncodedDataCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_render_captured_frame_encoded_data_callback(zego_on_custom_video_render_captured_frame_encoded_data(callback_func), user_context);
            }

            void registerCustomVideoRenderRemoteFrameEncodedDataCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_render_remote_frame_encoded_data_callback(zego_on_custom_video_render_remote_frame_encoded_data(callback_func), user_context);
            }

            void registerCustomVideoCaptureStartCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_capture_start_callback(zego_on_custom_video_capture_start(callback_func), user_context);
            }

            void registerCustomVideoCaptureStopCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_capture_stop_callback(zego_on_custom_video_capture_stop(callback_func), user_context);
            }

            void registerCustomVideoCaptureEncodedDataTrafficControlCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_capture_encoded_data_traffic_control_callback(zego_on_custom_video_capture_encoded_data_traffic_control(callback_func), user_context);
            }

            void registerCustomVideoProcessCapturedRawDataCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_process_captured_unprocessed_raw_data_callback(zego_on_custom_video_process_captured_unprocessed_raw_data(callback_func), user_context);
            }

            void registerCustomVideoProcessCapturedCVPixelBufferCallback(void *callback_func, void *user_context)
            {
                zego_register_custom_video_process_captured_unprocessed_cvpixelbuffer_callback(zego_on_custom_video_process_captured_unprocessed_cvpixelbuffer(callback_func), user_context);
            }

            void registerAudioMixingCopyDataCallback(void *callback_func, void *user_context)
            {
                zego_register_copy_audio_mixing_data_callback(zego_on_copy_audio_mixing_data(callback_func), user_context);
            }

            void registerOnCapturedAudioDataCallback(void *callback_func, void *user_context)
            {
                zego_register_captured_audio_data_callback(zego_on_captured_audio_data(callback_func), user_context);
            }

            void registerOnPlaybackAudioDataCallback(void *callback_func, void *user_context)
            {
                zego_register_playback_audio_data_callback(zego_on_playback_audio_data(callback_func), user_context);
            }

            void registerOnMixedAudioDataCallback(void *callback_func, void *user_context)
            {
                zego_register_mixed_audio_data_callback(zego_on_mixed_audio_data(callback_func), user_context);
            }

            void registerOnPlayerAudioDataCallback(void *callback_func, void *user_context)
            {
                zego_register_player_audio_data_callback(zego_on_player_audio_data(callback_func), user_context);
            }

            void registerRecordingCapturedDataStateUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_captured_data_record_state_update_callback(zego_on_captured_data_record_state_update(callback_func), user_context);
            }

            void registerRecordingCapturedDataProgressUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_captured_data_record_progress_update_callback(zego_on_captured_data_record_progress_update(callback_func), user_context);
            }

            void registerProcessCapturedAudioDataCallback(void *callback_func, void *user_context)
            {
                zego_register_process_captured_audio_data_callback(zego_on_process_captured_audio_data(callback_func), user_context);
            }
            void registerProcessCapturedAudioDataAfterUsedHeadphoneMonitorCallback(void *callback_func, void *user_context)
            {
                zego_register_process_captured_audio_data_after_used_headphone_monitor_callback(zego_on_process_captured_audio_data_after_used_headphone_monitor(callback_func), user_context);
            }
            void registerProcessRemoteAudioDataCallback(void *callback_func, void *user_context)
            {
                zego_register_process_remote_audio_data_callback(zego_on_process_remote_audio_data(callback_func), user_context);
            }

            void registerProcessPlaybackAudioDataCallback(void *callback_func, void *user_context)
            {
                zego_register_process_playback_audio_data_callback(zego_on_process_playback_audio_data(callback_func), user_context);
            }

            void registerPerformanceStatueUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_performance_status_update_callback(zego_on_performance_status_update(callback_func), user_context);
            }

            void registerNetworkModeChangedCallback(void *callback_func, void *user_context)
            {
                zego_register_network_mode_changed_callback(zego_on_network_mode_changed(callback_func), user_context);
            }

            void registerTestNetworkConnectivityCallback(void *callback_func, void *user_context)
            {
                zego_register_test_network_connectivity_callback(zego_on_test_network_connectivity(callback_func), user_context);
            }

            void registerNetworkSpeedTestQualityUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_network_speed_test_quality_update_callback(zego_on_network_speed_test_quality_update(callback_func), user_context);
            }

            void registerNetworkSpeedTestErrorCallback(void *callback_func, void *user_context)
            {
                zego_register_network_speed_test_error_callback(zego_on_network_speed_test_error(callback_func), user_context);
            }

			void registerNetworkProbeResultCallback(void *callback_func, void *user_context)
			{
				zego_register_network_probe_result_callback(zego_on_network_probe_result(callback_func), user_context);
			}

            void registerRecvExperimentalAPICallback(void *callback_func, void *user_context)
            {
                zego_register_recv_experimental_api_callback(zego_on_recv_experimental_api(callback_func), user_context);
            }

            void registerNetworkQualityCallback(void *callback_func, void *user_context)
            {
                zego_register_network_quality_callback(zego_on_network_quality(callback_func), user_context);
            }

            void registerCopyrightedMusicDownloadProcessUpdateCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_download_progress_update_callback(zego_on_copyrighted_music_download_progress_update(callback_func), user_context);
            }

            void registerCopyrightedMusicCurrentPitchValueUpdateCallback(void* callback_func, void* user_context)
            {
                zego_register_copyrighted_music_current_pitch_value_update_callback(zego_on_copyrighted_music_current_pitch_value_update(callback_func), user_context);
            }

            void registerCopyrightedMusicInitCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_init_callback(zego_on_copyrighted_music_init(callback_func), user_context);
            }

            void registerCopyrightedMusicSendExtendedRequestCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_send_extended_request_callback(zego_on_copyrighted_music_send_extended_request(callback_func), user_context);
            }

            void registerCopyrightedMusicGetLrcLyricCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_get_lrc_lyric_callback(zego_on_copyrighted_music_get_lrc_lyric(callback_func), user_context);
            }

            void registerCopyrightedMusicGetKrcLyricByTokenCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_get_krc_lyric_by_token_callback(zego_on_copyrighted_music_get_krc_lyric_by_token(callback_func), user_context);
            }

            void registerCopyrightedMusicRequestSongCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_request_song_callback(zego_on_copyrighted_music_request_song(callback_func), user_context);
            }

            void registerCopyrightedMusicRequestAccompanimentCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_request_accompaniment_callback(zego_on_copyrighted_music_request_accompaniment(callback_func), user_context);
            }

            void registerCopyrightedMusicRequestAccompanimentClipCallback(void* callback_func, void* user_context)
            {
                zego_register_copyrighted_music_request_accompaniment_clip_callback(zego_on_copyrighted_music_request_accompaniment(callback_func), user_context);
            }

            void registerCopyrightedMusicGetMusicByTokenCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_get_music_by_token_callback(zego_on_copyrighted_music_get_music_by_token(callback_func), user_context);
            }

            void registerCopyrightedMusicDownloadCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_download_callback(zego_on_copyrighted_music_download(callback_func), user_context);
            }

            void registerCopyrightedMusicGetStandardPitchCallback(void *callback_func, void *user_context)
            {
                zego_register_copyrighted_music_get_standard_pitch_callback(zego_on_copyrighted_music_get_standard_pitch(callback_func), user_context);
            }

            void registerNetworkTimeSynchronizedCallback(void *callback_func, void *user_context)
            {
                zego_register_network_time_synchronized_callback(zego_on_network_time_synchronized(callback_func), user_context);
            }

            void registerMediaDataPublisherFileOpenCallback(void *callback_func, void *user_context)
            {
                zego_register_media_data_publisher_file_open_callback(zego_on_media_data_publisher_file_open(callback_func), user_context);
            }

            void registerMediaDataPublisherFileCloseCallback(void *callback_func, void *user_context)
            {
                zego_register_media_data_publisher_file_close_callback(zego_on_media_data_publisher_file_close(callback_func), user_context);
            }

            void registerMediaDataPublisherFileDataBeginCallback(void *callback_func, void *user_context)
            {
                zego_register_media_data_publisher_file_data_begin_callback(zego_on_media_data_publisher_file_data_begin(callback_func), user_context);
            }

        private:
            ZegoExpressEngineBridge()
            {
                pri = new ZegoExpressEngineBridgePri();
            }

            ~ZegoExpressEngineBridge()
            {
                delete pri;
            }

            ZegoExpressEngineBridgePri *pri = nullptr;
        };
#define oInternalOriginBridge ZegoExpressEngineBridge::GetInstance()

    } // namespace EXPRESS
} // namespace ZEGO

ZEGO_ENABLE_DEPRECATION_WARNINGS
