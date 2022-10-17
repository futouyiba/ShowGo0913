
using System;
using System.Collections.Generic;
using UnityEngine;
using static ZEGO.IZegoEventHandler;
using static ZEGO.IZegoCustomVideoProcessHandler;
//current version is 2.19.0-video
namespace ZEGO
{

    public abstract class ZegoExpressEngine : IVideoSource
    {
        /**
         * Create ZegoExpressEngine singleton object and initialize SDK.
         *
         * Available since: 2.14.0
         * Description: Create ZegoExpressEngine singleton object and initialize SDK.
         * When to call: The engine needs to be created before calling other functions.
         * Restrictions: None.
         * Caution: The SDK only supports the creation of one instance of ZegoExpressEngine. Multiple calls to this function return the same object.
         *
         * @param profile The basic configuration information is used to create the engine.
         * @return engine singleton instance.
         */
        public static ZegoExpressEngine CreateEngine(ZegoEngineProfile profile)
        {
            return ZegoExpressEngineImpl.CreateEngine(profile);
        } 

        /**
         * Destroy the ZegoExpressEngine singleton object and deinitialize the SDK.
         *
         * Available since: 1.1.0
         * Description: Destroy the ZegoExpressEngine singleton object and deinitialize the SDK.
         * When to call: When the SDK is no longer used, the resources used by the SDK can be released through this interface
         * Restrictions: None.
         * Caution: After using [createEngine] to create a singleton, if the singleton object has not been created or has been destroyed, you will not receive related callbacks when calling this function.
         *
         * @param onDestroyCompletion Notification callback for destroy engine completion. Developers can listen to this callback to ensure that device hardware resources are released. If the developer only uses SDK to implement audio and video functions, this parameter can be passed [null].
         */
        public static void DestroyEngine(IZegoDestroyCompletionCallback onDestroyCompletion = null)
        {
            ZegoExpressEngineImpl.DestroyEngine(onDestroyCompletion);
        } 

        /**
         * Returns the singleton instance of ZegoExpressEngine.
         *
         * Available since: 1.1.0
         * Description: If the engine has not been created or has been destroyed, returns [null].
         * When to call: After creating the engine, before destroying the engine.
         * Restrictions: None.
         *
         * @return Engine singleton instance
         */
        public static ZegoExpressEngine GetEngine()
        {
            return ZegoExpressEngineImpl.GetEngine();
        } 

        /**
         * Set advanced engine configuration.
         *
         * Available since: 1.1.0
         * Description: Used to enable advanced functions.
         * When to call: Different configurations have different call timing requirements. For details, please consult ZEGO technical support.
         * Restrictions: None.
         *
         * @param config Advanced engine configuration
         */
        public static void SetEngineConfig(ZegoEngineConfig config)
        {
            ZegoExpressEngineImpl.SetEngineConfig(config);
        } 

        /**
         * Set log configuration.
         *
         * Available since: 2.3.0
         * Description: If you need to customize the log file size and path, please call this function to complete the configuration.
         * When to call: It must be set before calling [createEngine] to take effect. If it is set after [createEngine], it will take effect at the next [createEngine] after [destroyEngine].
         * Restrictions: None.
         * Caution: Once this interface is called, the method of setting log size and path via [setEngineConfig] will be invalid.Therefore, it is not recommended to use [setEngineConfig] to set the log size and path.
         *
         * @param config log configuration.
         */
        public static void SetLogConfig(ZegoLogConfig config)
        {
            ZegoExpressEngineImpl.SetLogConfig(config);
        } 

        /**
         * Set room mode.
         *
         * Available since: 2.9.0
         * Description: If you need to use the multi-room feature, please call this function to complete the configuration.
         * When to call: Must be set before calling [createEngine] to take effect, otherwise it will fail.
         * Restrictions: If you need to use the multi-room feature, please contact the instant technical support to configure the server support.
         * Caution: None.
         *
         * @param mode Room mode. Description: Used to set the room mode. Use cases: If you need to enter multiple rooms at the same time for publish-play stream, please turn on the multi-room mode through this interface. Required: True. Default value: ZEGO_ROOM_MODE_SINGLE_ROOM.
         */
        public static void SetRoomMode(ZegoRoomMode mode)
        {
            ZegoExpressEngineImpl.SetRoomMode(mode);
        } 

        /**
         * Gets the SDK's version number.
         *
         * Available since: 1.1.0
         * Description: If you encounter an abnormality during the running of the SDK, you can submit the problem, log and other information to the ZEGO technical staff to locate and troubleshoot. Developers can also collect current SDK version information through this API, which is convenient for App operation statistics and related issues.
         * When to call: Any time.
         * Restrictions: None.
         * Caution: None.
         *
         * @return SDK version.
         */
        public static string GetVersion()
        {
            return ZegoExpressEngineImpl.GetVersion();
        } 

        /**
         * Uploads logs to the ZEGO server.
         *
         * Available since: 1.1.0
         * Description: By default, SDK creates and prints log files in the App's default directory. Each log file defaults to a maximum of 5MB. Three log files are written over and over in a circular fashion. When calling this function, SDK will auto package and upload the log files to the ZEGO server.
         * Use cases: Developers can provide a business “feedback” channel in the App. When users feedback problems, they can call this function to upload the local log information of SDK to help locate user problems.
         * When to call: After [createEngine].
         * Restrictions: If you call this interface repeatedly within 10 minutes, only the last call will take effect.
         * Caution: After calling this interface to upload logs, if you call [destroyEngine] or exit the App too quickly, there may be a failure.It is recommended to wait a few seconds, and then call [destroyEngine] or exit the App after receiving the upload success callback.
         */
        public abstract void UploadLog();

        /**
         * Call the RTC experimental API.
         *
         * Available since: 2.7.0
         * Description: ZEGO provides some technical previews or special customization functions in RTC business through this API. If you need to get the use of the function or the details, please consult ZEGO technical support.
         * When to call: After [createEngine].
         *
         * @param param You need to pass in a parameter in the form of a JSON string, please consult ZEGO technical support for details.
         * @return Returns an argument in the format of a JSON string, please consult ZEGO technical support for details.
         */
        public abstract string CallExperimentalAPI(string param);

        public OnDebugError onDebugError;

        public OnApiCalledResult onApiCalledResult;

        public OnEngineStateUpdate onEngineStateUpdate;

        /**
         * Logs in to a room with advanced room configurations. You must log in to a room before publishing or playing streams.
         *
         * Available since: 1.1.0
         * Description: SDK uses the 'room' to organize users. After users log in to a room, they can use interface such as push stream [startPublishingStream], pull stream [startPlayingStream], send and receive broadcast messages [sendBroadcastMessage], etc. To prevent the app from being impersonated by a malicious user, you can add authentication before logging in to the room, that is, the [token] parameter in the ZegoRoomConfig object passed in by the [config] parameter.
         * Use cases: In the same room, users can conduct live broadcast, audio and video calls, etc.
         * When to call /Trigger: This interface is called after [createEngine] initializes the SDK.
         * Restrictions: For restrictions on the use of this function, please refer to https://docs.zegocloud.com/article/7611 or contact ZEGO technical support.
         * Caution:
         *   1. Apps that use different appIDs cannot intercommunication with each other.
         *   2. SDK supports startPlayingStream audio and video streams from different rooms under the same appID, that is, startPlayingStream audio and video streams across rooms. Since ZegoExpressEngine's room related callback notifications are based on the same room, when developers want to startPlayingStream streams across rooms, developers need to maintain related messages and signaling notifications by themselves.
         *   3. It is strongly recommended that userID corresponds to the user ID of the business APP, that is, a userID and a real user are fixed and unique, and should not be passed to the SDK in a random userID. Because the unique and fixed userID allows ZEGO technicians to quickly locate online problems.
         *   4. After the first login failure due to network reasons or the room is disconnected, the default time of SDK reconnection is 20min.
         *   5. After the user has successfully logged in to the room, if the application exits abnormally, after restarting the application, the developer needs to call the logoutRoom interface to log out of the room, and then call the loginRoom interface to log in to the room again.
         * Privacy reminder: Please do not fill in sensitive user information in this interface, including but not limited to mobile phone number, ID number, passport number, real name, etc.
         * Related callbacks:
         *   1. When the user starts to log in to the room, the room is successfully logged in, or the room fails to log in, the [onRoomStateChanged] (Not supported before 2.18.0, please use [onRoomStateUpdate]) callback will be triggered to notify the developer of the status of the current user connected to the room.
         *   2. Different users who log in to the same room can get room related notifications in the same room (eg [onRoomUserUpdate], [onRoomStreamUpdate], etc.), and users in one room cannot receive room signaling notifications in another room.
         *   3. If the network is temporarily interrupted due to network quality reasons, the SDK will automatically reconnect internally. You can get the current connection status of the local room by listening to the [onRoomStateChanged] (Not supported before 2.18.0, please use [onRoomStateUpdate]) callback method, and other users in the same room will receive [onRoomUserUpdate] callback notification.
         *   4. Messages sent in one room (e.g. [setStreamExtraInfo], [sendBroadcastMessage], [sendBarrageMessage], [sendCustomCommand], etc.) cannot be received callback ((eg [onRoomStreamExtraInfoUpdate], [onIMRecvBroadcastMessage], [onIMRecvBarrageMessage], [onIMRecvCustomCommand], etc) in other rooms. Currently, SDK does not provide the ability to send messages across rooms. Developers can integrate the SDK of third-party IM to achieve.
         * Related APIs:
         *   1. Users can call [logoutRoom] to log out. In the case that a user has successfully logged in and has not logged out, if the login interface is called again, the console will report an error and print the error code 1002001.
         *   2. SDK supports multi-room login, please call [setRoomMode] function to select multi-room mode before engine initialization, and then call [loginRoom] to log in to multi-room.
         *   3. Calling [destroyEngine] will also automatically log out.
         *
         * @param roomID Room ID, a string of up to 128 bytes in length. Only support numbers, English characters and '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '`', ';', '’', ',', '.', '<', '>', '/', '\'.
         * @param user User object instance, configure userID, userName. Note that the userID needs to be globally unique with the same appID, otherwise the user who logs in later will kick out the user who logged in first.
         * @param config Advanced room configuration.
         */
        public abstract void LoginRoom(string roomID, ZegoUser user, ZegoRoomConfig config = null);

        /**
         * Log in to the room by configuring advanced properties, and return the login result through the callback parameter. You must log in to the room before pushing or pulling the stream.
         *
         * Available since: 2.18.0
         * Description: SDK uses the 'room' to organize users. After users log in to a room, they can use interface such as push stream [startPublishingStream], pull stream [startPlayingStream], send and receive broadcast messages [sendBroadcastMessage], etc. To prevent the app from being impersonated by a malicious user, you can add authentication before logging in to the room, that is, the [token] parameter in the ZegoRoomConfig object passed in by the [config] parameter.
         * Use cases: In the same room, users can conduct live broadcast, audio and video calls, etc.
         * When to call /Trigger: This interface is called after [createEngine] initializes the SDK.
         * Restrictions: For restrictions on the use of this function, please refer to https://docs.zegocloud.com/article/7611 or contact ZEGO technical support.
         * Caution:
         *   1. Apps that use different appIDs cannot intercommunication with each other.
         *   2. SDK supports startPlayingStream audio and video streams from different rooms under the same appID, that is, startPlayingStream audio and video streams across rooms. Since ZegoExpressEngine's room related callback notifications are based on the same room, when developers want to startPlayingStream streams across rooms, developers need to maintain related messages and signaling notifications by themselves.
         *   3. It is strongly recommended that userID corresponds to the user ID of the business APP, that is, a userID and a real user are fixed and unique, and should not be passed to the SDK in a random userID. Because the unique and fixed userID allows ZEGO technicians to quickly locate online problems.
         *   4. After the first login failure due to network reasons or the room is disconnected, the default time of SDK reconnection is 20min.
         *   5. After the user has successfully logged in to the room, if the application exits abnormally, after restarting the application, the developer needs to call the logoutRoom interface to log out of the room, and then call the loginRoom interface to log in to the room again.
         * Privacy reminder: Please do not fill in sensitive user information in this interface, including but not limited to mobile phone number, ID number, passport number, real name, etc.
         * Related callbacks:
         *   1. When the user starts to log in to the room, the room is successfully logged in, or the room fails to log in, the [onRoomStateChanged] (Not supported before 2.18.0, please use [onRoomStateUpdate]) callback will be triggered to notify the developer of the status of the current user connected to the room.
         *   2. Different users who log in to the same room can get room related notifications in the same room (eg [onRoomUserUpdate], [onRoomStreamUpdate], etc.), and users in one room cannot receive room signaling notifications in another room.
         *   3. If the network is temporarily interrupted due to network quality reasons, the SDK will automatically reconnect internally. You can get the current connection status of the local room by listening to the [onRoomStateChanged] (Not supported before 2.18.0, please use [onRoomStateUpdate]) callback method, and other users in the same room will receive [onRoomUserUpdate] callback notification.
         *   4. Messages sent in one room (e.g. [setStreamExtraInfo], [sendBroadcastMessage], [sendBarrageMessage], [sendCustomCommand], etc.) cannot be received callback ((eg [onRoomStreamExtraInfoUpdate], [onIMRecvBroadcastMessage], [onIMRecvBarrageMessage], [onIMRecvCustomCommand], etc) in other rooms. Currently, SDK does not provide the ability to send messages across rooms. Developers can integrate the SDK of third-party IM to achieve.
         * Related APIs:
         *   1. Users can call [logoutRoom] to log out. In the case that a user has successfully logged in and has not logged out, if the login interface is called again, the console will report an error and print the error code 1002001.
         *   2. SDK supports multi-room login, please call [setRoomMode] function to select multi-room mode before engine initialization, and then call [loginRoom] to log in to multi-room.
         *   3. Calling [destroyEngine] will also automatically log out.
         *
         * @param roomID Room ID, a string of up to 128 bytes in length. Only support numbers, English characters and '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '`', ';', '’', ',', '.', '<', '>', '/', '\'.
         * @param user User object instance, configure userID, userName. Note that the userID needs to be globally unique with the same appID, otherwise the user who logs in later will kick out the user who logged in first.
         * @param config Advanced room configuration.
         * @param callback The callback of this login result, if you need detailed room status, please pay attention to the [onRoomStateChanged] callback. Required: No. Default value: null.Caution: If the connection is retried multiple times due to network problems, the retry status will not be thrown by this callback.
         */
        public abstract void LoginRoom(string roomID, ZegoUser user, ZegoRoomConfig config, OnRoomLoginResult callback = null);

        /**
         * Logs out of a room.
         *
         * Available since: 2.9.0
         * Description: This API will log out the current user has logged in the room, if user logs in more than one room, all the rooms will be logged out.
         * Use cases: In the same room, users can conduct live broadcast, audio and video calls, etc.
         * When to call /Trigger: After successfully logging in to the room, if the room is no longer used, the user can call the function [logoutRoom].
         * Restrictions: None.
         * Caution: 1. Exiting the room will stop all publishing and playing streams for user, and inner audio and video engine will stop, and then SDK will auto stop local preview UI. If you want to keep the preview ability when switching rooms, please use the [switchRoom] method. 2. If the user is not logged in to the room, calling this function will also return success.
         * Related callbacks: After calling this function, you will receive [onRoomStateChanged] (Not supported before 2.18.0, please use [onRoomStateUpdate]) callback notification successfully exits the room, while other users in the same room will receive the [onRoomUserUpdate] callback notification(On the premise of enabling isUserStatusNotify configuration).
         * Related APIs: Users can use [loginRoom], [switchRoom] functions to log in or switch rooms.
         *
         * @param callback The callback of this logout room result, if you need detailed room status, please pay attention to the [onRoomStateChanged] callback.. Required: No. Default value: null.
         */
        public abstract void LogoutRoom(OnRoomLogoutResult callback = null);

        /**
         * Logs out of a room.
         *
         * Available since: 1.1.0
         * Description: This API will log out the room named roomID.
         * Use cases: In the same room, users can conduct live broadcast, audio and video calls, etc.
         * When to call /Trigger: After successfully logging in to the room, if the room is no longer used, the user can call the function [logoutRoom].
         * Restrictions: None.
         * Caution: 1. Exiting the room will stop all publishing and playing streams for user, and inner audio and video engine will stop, and then SDK will auto stop local preview UI. If you want to keep the preview ability when switching rooms, please use the [switchRoom] method. 2. If the user logs in to the room, but the incoming 'roomID' is different from the logged-in room name, SDK will return failure.
         * Related callbacks: After calling this function, you will receive [onRoomStateChanged] (Not supported before 2.18.0, please use [onRoomStateUpdate]) callback notification successfully exits the room, while other users in the same room will receive the [onRoomUserUpdate] callback notification(On the premise of enabling isUserStatusNotify configuration).
         * Related APIs: Users can use [loginRoom], [switchRoom] functions to log in or switch rooms.
         *
         * @param roomID Room ID, a string of up to 128 bytes in length. Only support numbers, English characters and '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '`', ';', '’', ',', '.', '<', '>', '/', '\'.
         * @param callback The callback of this logout room result, if you need detailed room status, please pay attention to the [onRoomStateChanged] callback.. Required: No. Default value: null.
         */
        public abstract void LogoutRoom(string roomID, OnRoomLogoutResult callback = null);

        /**
         * Switch the room with advanced room configurations.
         *
         * Available since: 1.15.0
         * Description: Using this interface allows users to quickly switch from one room to another room.
         * Use cases: if you need to quickly switch to the next room, you can call this function.
         * When to call /Trigger: After successfully login room.
         * Restrictions: None.
         * Caution:
         *   1. When this function is called, all streams currently publishing or playing will stop (but the local preview will not stop).
         *   2. To prevent the app from being impersonated by a malicious user, you can add authentication before logging in to the room, that is, the [token] parameter in the ZegoRoomConfig object passed in by the [config] parameter. This parameter configuration affects the room to be switched over. 3. When the function [setRoomMode] is used to set ZegoRoomMode to ZEGO_ROOM_MODE_MULTI_ROOM, this function is not available.
         * Privacy reminder: Please do not fill in sensitive user information in this interface, including but not limited to mobile phone number, ID number, passport number, real name, etc.
         * Related callbacks: When the user call the [switchRoom] function, the [onRoomStateChanged] (Not supported before 2.18.0, please use [onRoomStateUpdate]) callback will be triggered to notify the developer of the status of the current user connected to the room.
         * Related APIs: Users can use the [logoutRoom] function to log out of the room.
         *
         * @param fromRoomID Current roomID.
         * @param toRoomID The next roomID.
         * @param config Advanced room configuration.
         */
        public abstract void SwitchRoom(string fromRoomID, string toRoomID, ZegoRoomConfig config = null);

        /**
         * Renew token.
         *
         * Available since: 2.8.0
         * Description: After the developer receives [onRoomTokenWillExpire], they can use this API to update the token to ensure that the subsequent RTC functions are normal.
         * Use cases: Used when the token is about to expire.
         * When to call /Trigger: After the developer receives [onRoomTokenWillExpire].
         * Restrictions: None.
         * Caution: The token contains important information such as the user's room permissions, publish stream permissions, and effective time, please refer to https://docs.zegocloud.com/article/11649.
         * Related callbacks: None.
         * Related APIs: None.
         *
         * @param roomID Room ID.
         * @param token The token that needs to be renew.
         */
        public abstract void RenewToken(string roomID, string token);

        /**
         * Set room extra information.
         *
         * Available since: 1.13.0
         * Description: The user can call this function to set the extra info of the room.
         * Use cases: You can set some room-related business attributes, such as whether someone is Co-hosting.
         * When to call /Trigger: After logging in the room successful.
         * Restrictions: For restrictions on the use of this function, please refer to https://docs.zegocloud.com/article/7611 or contact ZEGO technical support.
         * Caution: 'key' is non null. The length of key and value is limited, please refer to Restrictions. The newly set key and value will overwrite the old setting.
         * Related callbacks: Other users in the same room will be notified through the [onRoomExtraInfoUpdate] callback function.
         * Related APIs: None.
         *
         * @param roomID Room ID.
         * @param key key of the extra info.
         * @param value value if the extra info.
         * @param onRoomSetRoomExtraInfoResult Callback for setting room extra information.
         */
        public abstract void SetRoomExtraInfo(string roomID, string key, string value, OnRoomSetRoomExtraInfoResult onRoomSetRoomExtraInfoResult);

        public OnRoomStateUpdate onRoomStateUpdate;

        public OnRoomStateChanged onRoomStateChanged;

        public OnRoomUserUpdate onRoomUserUpdate;

        public OnRoomOnlineUserCountUpdate onRoomOnlineUserCountUpdate;

        public OnRoomStreamUpdate onRoomStreamUpdate;

        public OnRoomStreamExtraInfoUpdate onRoomStreamExtraInfoUpdate;

        public OnRoomExtraInfoUpdate onRoomExtraInfoUpdate;

        public OnRoomTokenWillExpire onRoomTokenWillExpire;

        /**
         * Starts publishing a stream (for the specified channel). You can call this function to publish a second stream.
         *
         * Available since: 1.1.0
         * Description: Users push their local audio and video streams to the ZEGO RTC server or CDN, and other users in the same room can pull the audio and video streams to watch through the `streamID` or CDN pull stream address.
         * Use cases: It can be used to publish streams in real-time connecting wheat, live broadcast and other scenarios.
         * When to call: After [loginRoom].
         * Restrictions: None.
         * Caution:
         *   1. Before start to publish the stream, the user can choose to call [setVideoConfig] to set the relevant video parameters, and call [startPreview] to preview the video.
         *   2. Other users in the same room can get the streamID by monitoring the [onRoomStreamUpdate] event callback after the local user publishing stream successfully.
         *   3. In the case of poor network quality, user publish may be interrupted, and the SDK will attempt to reconnect. You can learn about the current state and error information of the stream published by monitoring the [onPublisherStateUpdate] event.
         *
         * @param streamID Stream ID, a string of up to 256 characters, needs to be globally unique within the entire AppID. If in the same AppID, different users publish each stream and the stream ID is the same, which will cause the user to publish the stream failure. You cannot include URL keywords, otherwise publishing stream and playing stream will fails. Only support numbers, English characters and '~', '!', '@', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '`', ';', '’', ',', '.', '<', '>', '/', '\'.
         * @param channel Publish stream channel.
         */
        public abstract void StartPublishingStream(string streamID, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Starts publishing a stream. Support multi-room mode.
         *
         * Available since: 1.1.0
         * Description: Users push their local audio and video streams to the ZEGO RTC server or CDN, and other users in the same room can pull the audio and video streams to watch through the `streamID` or CDN pull stream address.
         * Use cases: It can be used to publish streams in real-time connecting wheat, live broadcast and other scenarios.
         * When to call: After [loginRoom].
         * Restrictions: None.
         * Caution:
         *   1. Before start to publish the stream, the user can choose to call [setVideoConfig] to set the relevant video parameters, and call [startPreview] to preview the video.
         *   2. Other users in the same room can get the streamID by monitoring the [onRoomStreamUpdate] event callback after the local user publishing stream successfully.
         *   3. In the case of poor network quality, user publish may be interrupted, and the SDK will attempt to reconnect. You can learn about the current state and error information of the stream published by monitoring the [onPublisherStateUpdate] event.
         *   4. To call [SetRoomMode] function to select multiple rooms, the room ID must be specified explicitly.
         *
         * @param streamID Stream ID, a string of up to 256 characters, needs to be globally unique within the entire AppID. If in the same AppID, different users publish each stream and the stream ID is the same, which will cause the user to publish the stream failure. You cannot include URL keywords, otherwise publishing stream and playing stream will fails. Only support numbers, English characters and '~', '!', '@', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '`', ';', '’', ',', '.', '<', '>', '/', '\'.
         * @param config Advanced publish configuration.
         * @param channel Publish stream channel.
         */
        public abstract void StartPublishingStream(string streamID, ZegoPublisherConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Stops publishing a stream (for the specified channel).
         *
         * Available since: 1.1.0
         * Description: The user stops sending local audio and video streams, and other users in the room will receive a stream deletion notification.
         * Use cases: It can be used to stop publish streams in real-time connecting wheat, live broadcast and other scenarios.
         * When to call: After [startPublishingStream].
         * Restrictions: None.
         * Caution:
         *   1. After stopping the streaming, other users in the same room can receive the delete notification of the stream by listening to the [onRoomStreamUpdate] callback.
         *   2. If the user has initiated publish flow, this function must be called to stop the publish of the current stream before publishing the new stream (new streamID), otherwise the new stream publish will return a failure.
         *   3. After stopping streaming, the developer should stop the local preview based on whether the business situation requires it.
         *
         * @param channel Publish stream channel.
         */
        public abstract void StopPublishingStream(ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sets the extra information of the stream being published for the specified publish channel.
         *
         * Available since: 1.1.0
         * Description: Use this function to set the extra info of the stream. The stream extra information is an extra information identifier of the stream ID. Unlike the stream ID, which cannot be modified during the publishing process, the stream extra information can be modified midway through the stream corresponding to the stream ID. Developers can synchronize variable content related to stream IDs based on stream additional information.
         * When to call: After the engine is created [createEngine], Called before and after [startPublishingStream] can both take effect.
         * Restrictions: None.
         * Related callbacks: Users can obtain the execution result of the function through [ZegoPublisherSetStreamExtraInfoCallback] callback.
         *
         * @param extraInfo Stream extra information, a string of up to 1024 characters.
         * @param onPublisherSetStreamExtraInfoResult Set stream extra information execution result notification.
         * @param channel Publish stream channel.
         */
        public abstract void SetStreamExtraInfo(string extraInfo, OnPublisherSetStreamExtraInfoResult onPublisherSetStreamExtraInfoResult, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Starts/Updates the local video preview (for the specified channel).
         *
         * Available since: 1.1.0
         * Description: The user can see his own local image by calling this function.
         * Use cases: It can be used for local preview in real-time connecting wheat, live broadcast and other scenarios.
         * When to call: After [createEngine].
         * Restrictions: None.
         * Caution: 1. The preview function does not require you to log in to the room or publish the stream first. But after exiting the room, SDK internally actively stops previewing by default. 2. Local view and preview modes can be updated by calling this function again. The user can only preview on one view. If you call [startPreview] again to pass in a new view, the preview screen will only be displayed in the new view. 3. You can set the mirror mode of the preview by calling the [setVideoMirrorMode] function. The default preview setting is image mirrored. 4. When this function is called, the audio and video engine module inside SDK will start really, and it will start to try to collect audio and video..
         *
         * @param channel Publish stream channel
         */
        public abstract void StartPreview(ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Stops the local video preview (for the specified channel).
         *
         * Available since: 1.1.0
         * Description: This function can be called to stop previewing when there is no need to see the preview locally.
         *
         * @param channel Publish stream channel
         */
        public abstract void StopPreview(ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sets up the video configurations (for the specified channel).
         *
         * Available since: 1.1.0
         * Description: Set the video frame rate, bit rate, video capture resolution, and video encoding output resolution.
         * Default value: The default video capture resolution is 360p, the video encoding output resolution is 360p, the bit rate is 600 kbps, and the frame rate is 15 fps.
         * When to call: After [createEngine].
         * Restrictions: It is necessary to set the relevant video configuration before publishing the stream or startPreview, and only support the modification of the encoding resolution and the bit rate after publishing the stream.
         * Caution: Developers should note that the wide and high resolution of the mobile end is opposite to the wide and high resolution of the PC. For example, in the case of 360p, the resolution of the mobile end is 360x640, and the resolution of the PC end is 640x360.
         *
         * @param config Video configuration, the SDK provides a common setting combination of resolution, frame rate and bit rate, they also can be customized.
         * @param channel Publish stream channel.
         */
        public abstract void SetVideoConfig(ZegoVideoConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Gets the current video configurations (for the specified channel).
         *
         * This function can be used to get the specified publish channel's current video frame rate, bit rate, video capture resolution, and video encoding output resolution.
         *
         * @param channel Publish stream channel
         * @return Video configuration object
         */
        public abstract ZegoVideoConfig GetVideoConfig(ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sets the video mirroring mode (for the specified channel).
         *
         * Available since: 1.1.0
         * Description: Set whether the local preview video and the published video have mirror mode enabled. For specific mirroring mode.
         * When to call: After [createEngine].
         * Restrictions: None.
         *
         * @param mirrorMode Mirror mode for previewing or publishing the stream.
         * @param channel Publish stream channel.
         */
        public abstract void SetVideoMirrorMode(ZegoVideoMirrorMode mirrorMode, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sets the video orientation (for the specified channel).
         *
         * Available since: 1.1.0
         * Description: Set the video orientation, please refer to https://doc-zh.zego.im/article/10365.
         * Use cases: When users use mobile devices to conduct live broadcasts or video calls, they can set different video directions according to the scene.
         * When to call: After [createEngine].
         * Restrictions: None.
         *
         * @param orientation Video orientation.
         * @param channel Publish stream channel.
         */
        public abstract void SetAppOrientation(ZegoOrientation orientation, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sets up the audio configurations for the specified publish channel.
         *
         * Available since: 1.3.4
         * Description: You can set the combined value of the audio codec, bit rate, and audio channel through this function. If the preset value cannot meet the developer's scenario, the developer can set the parameters according to the business requirements.
         * Default value: The default audio config refers to the default value of [ZegoAudioConfig]. 
         * When to call: After the engine is created [createEngine], and before publishing [startPublishingStream].
         * Restrictions: None.
         * Related APIs: [getAudioConfig].
         *
         * @param config Audio config.
         * @param channel Publish stream channel.
         */
        public abstract void SetAudioConfig(ZegoAudioConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Gets the current audio configurations.
         *
         * Available since: 1.8.0
         * Description: You can get the current audio codec, bit rate, and audio channel through this function.
         * When to call: After the engine is created [createEngine].
         * Restrictions: None.
         * Caution: Act on the main publish channel ZegoPublishChannel.Main.
         * Related APIs: [setAudioConfig].
         *
         * @return Audio config.
         */
        public abstract ZegoAudioConfig GetAudioConfig();

        /**
         * Take a snapshot of the publishing stream for the specified publish channel.
         *
         * Available since: 1.17.0
         * Description: Take a snapshot of the publishing stream.
         * When to call: Called this function after calling [startPublishingStream] or [startPreview].
         * Restrictions: None.
         * Caution: The resolution of the snapshot is the encoding resolution set in [setVideoConfig]. If you need to change it to capture resolution, please call [setCapturePipelineScaleMode] to change the capture pipeline scale mode to [Post].
         * Related callbacks: The screenshot result will be called back through [ZegoPublisherTakeSnapshotCallback].
         * Related APIs: [takePlayStreamSnapshot].
         *
         * @param callback Results of take publish stream snapshot.
         * @param channel Publish stream channel.
         */
        public abstract void TakePublishStreamSnapshot(OnPublisherTakeSnapshotResult callback, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Stops or resumes sending the audio part of a stream for the specified channel.
         *
         * Available since: 1.1.0
         * Description: This function can be called when publishing the stream to realize not publishing the audio data stream. The SDK still collects and processes the audio, but does not send the audio data to the network.
         * When to call: Called after the engine is created [createEngine] can take effect.
         * Restrictions: None.
         * Related callbacks: If you stop sending audio streams, the remote user that play stream of local user publishing stream can receive `Mute` status change notification by monitoring [onRemoteMicStateUpdate] callbacks.
         * Related APIs: [mutePublishStreamVideo].
         *
         * @param mute Whether to stop sending audio streams, true means not to send audio stream, and false means sending audio stream. The default is false.
         * @param channel Publish stream channel.
         */
        public abstract void MutePublishStreamAudio(bool mute, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Stops or resumes sending the video part of a stream for the specified channel.
         *
         * Available since: 1.1.0
         * Description: This function can be called when publishing the stream to realize not publishing the video stream. The local camera can still work normally, can capture, preview and process video images normally, but does not send the video data to the network.
         * When to call: Called after the engine is created [createEngine] can take effect.
         * Restrictions: None.
         * Related callbacks: If you stop sending video streams locally, the remote user that play stream of local user publishing stream can receive `Mute` status change notification by monitoring [onRemoteCameraStateUpdate] callbacks.
         * Related APIs: [mutePublishStreamAudio].
         *
         * @param mute Whether to stop sending video streams, true means not to send video stream, and false means sending video stream. The default is false.
         * @param channel Publish stream channel.
         */
        public abstract void MutePublishStreamVideo(bool mute, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Enable or disable the stream mixing precision alignment function.
         *
         * Available since: 2.11.0.
         * Description: Use this interface to enable stream alignment, the SDK will attach network time information to the stream when publishing it for accurate stream alignment.
         * Use case: Generally used in scenarios such as KTV where stream mixing alignment is required.
         * When to call: After the engine is created [createEngine].
         * Caution: If mixed flow need time for alignment, the flow through the network push flow need to call startPublishingStream and ZegoPublisherConfig.forceSynchronousNetworkTime = 1, for open network time synchronization.
         * Related APIs: [startMixerTask], [startAutoMixerTask]
         *
         * @param alignment Whether to enable the stream mixing precision alignment function.
         * @param channel Publish stream channel
         */
        public abstract void SetStreamAlignmentProperty(int alignment, ZegoPublishChannel channel);

        /**
         * Enables or disables traffic control.
         *
         * Available since: 1.5.0
         * Description: Enabling traffic control allows the SDK to adjust the audio and video streaming bitrate according to the current upstream network environment conditions, or according to the counterpart's downstream network environment conditions in a one-to-one interactive scenario, to ensure smooth results. At the same time, you can further specify the attributes of traffic control to adjust the corresponding control strategy.
         * Default value: Enable.
         * When to call: After the engine is created [createEngine], Called before [startPublishingStream] can take effect.
         * Restrictions: Only support RTC publish.
         * Caution: Act on the main publish channel ZegoPublishChannel.Main.
         *
         * @param enable Whether to enable traffic control. The default is ture.
         * @param property Adjustable property of traffic control, bitmask OR format. Should be one or the combinations of [ZegoTrafficControlProperty] enumeration. [AdaptiveFPS] as default.
         */
        public abstract void EnableTrafficControl(bool enable, int property);

        /**
         * Set the minimum video bitrate threshold for traffic control.
         *
         * Available since: 1.1.0
         * Description: Set the control strategy when the video bitrate reaches the lowest threshold during flow control. When the bitrate is lower than the minimum threshold, you can choose not to send video data or send it at a very low frame bitrate.
         * Default value: There is no control effect of the lowest threshold of video bitrate.
         * When to call: After the engine is created [createEngine], Called before [startPublishingStream] can take effect.
         * Restrictions: The traffic control must be turned on [enableTrafficControl].
         * Caution: Act on the main publish channel ZegoPublishChannel.Main.
         * Related APIs: [enableTrafficControl].
         *
         * @param bitrate Minimum video bitrate threshold for traffic control(kbps).
         * @param mode Video sending mode below the minimum bitrate.
         */
        public abstract void SetMinVideoBitrateForTrafficControl(int bitrate, ZegoTrafficControlMinVideoBitrateMode mode);

        /**
         * Set the factors of concern that trigger traffic control for the specified publish channel.
         *
         * Available since: 2.4.0
         * Description: Use this interface to control whether to start traffic control due to poor remote network conditions.
         * Default value: Default is disable.
         * When to call: After the engine is created [createEngine], Called before [startPublishingStream] can take effect.
         * Restrictions: The traffic control must be turned on [enableTrafficControl].
         * Related APIs: [enableTrafficControl.
         *
         * @param mode When LOCAL_ONLY is selected, only the local network status is concerned. When choosing REMOTE, also take into account the remote network.
         * @param channel Publish stream channel.
         */
        public abstract void SetTrafficControlFocusOn(ZegoTrafficControlFocusOnMode mode, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sets the audio recording volume for stream publishing.
         *
         * Available since: 1.13.0
         * Description: This function is used to perform gain processing based on the device's collected volume. The local user can control the sound level of the audio stream sent to the remote end.
         * Default value: Default is 100.
         * When to call: After creating the engine [createEngine].
         * Restrictions: The capture volume can be dynamically set during publishing.
         * Related APIs: Set the playing stream volume [setPlayVolume].
         *
         * @param volume The volume gain percentage, the range is 0 ~ 200, and the default value is 100, which means 100% of the original collection volume of the device.
         */
        public abstract void SetCaptureVolume(int volume);

        /**
         * Set audio capture stereo mode.
         *
         * Available since: 1.15.0 (iOS/Android/Windows); support macOS since 2.16.0
         * Description: This function is used to set the audio capture channel mode. When the developer turns on the stereo capture, using a special stereo capture device, the stereo audio data can be captured and streamed.
         * Use cases: In some professional scenes, users are particularly sensitive to sound effects, such as voice radio and musical instrument performance. At this time, support for stereo and high-quality sound is required.
         * Default value: The default is None, which means mono capture.
         * When to call: It needs to be called after [createEngine]， before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager].
         * Restrictions: If you need to enable stereo capture, you also need to meet the following conditions: For iOS/Android, you need to connect an external audio device that supports stereo capture and be at the media volume. For macOS, it needs to be at the media volume. For Windows, an external audio device that supports stereo capture is required.
         * Related APIs: When streaming, you need to enable the stereo audio encoding function through the [setAudioConfig] interface at the same time.
         *
         * @param mode Audio stereo capture mode.
         */
        public abstract void SetAudioCaptureStereoMode(ZegoAudioCaptureStereoMode mode);

        /**
         * Adds a target CDN URL to which the stream will be relayed from ZEGO RTC server.
         *
         * Available since: 1.1.0
         * Description: Forward audio and video streams from ZEGO RTC servers to custom CDN content distribution networks with high latency but support for high concurrent pull streams.
         * Use cases: 1. It is often used in large-scale live broadcast scenes that do not have particularly high requirements for delay. 2. Since ZEGO RTC server itself can be configured to support CDN(content distribution networks), this function is mainly used by developers who have CDN content distribution services themselves. 3. This function supports dynamic relay to the CDN content distribution network, so developers can use this function as a disaster recovery solution for CDN content distribution services.
         * When to call: After calling the [createEngine] function to create the engine.
         * Restrictions: When the [enablePublishDirectToCDN] function is set to true to publish the stream straight to the CDN, then calling this function will have no effect.
         * Caution: Removing URLs retweeted to CDN requires calling [removePublishCdnUrl], calling [stopPublishingStream] will not remove URLs publish to CDN.
         * Related APIs: Remove URLs that are re-pushed to the CDN [removePublishCdnUrl].
         *
         * @param streamID Stream ID.
         * @param targetURL CDN relay address, supported address format is rtmp, rtmps.
         * @param onPublisherUpdateCdnUrlResult The execution result of update the relay CDN operation.
         */
        public abstract void AddPublishCdnUrl(string streamID, string targetURL, OnPublisherUpdateCdnUrlResult onPublisherUpdateCdnUrlResult);

        /**
         * Deletes the specified CDN URL, which is used for relaying streams from ZEGO RTC server to CDN.
         *
         * Available since: 1.1.0
         * Description: When a CDN forwarding address has been added via [addPublishCdnUrl], this function is called when the stream needs to be stopped.
         * When to call: After calling the [createEngine] function to create the engine, When you don't need to continue publish to the CDN.
         * Restrictions: When the [enablePublishDirectToCDN] function is set to true to publish the stream straight to the CDN, then calling this function will have no effect.
         * Caution: This function does not stop publishing audio and video stream to the ZEGO ZEGO RTC server.
         * Related APIs: Add URLs that are re-pushed to the CDN [addPublishCdnUrl].
         *
         * @param streamID Stream ID.
         * @param targetURL CDN relay address, supported address format rtmp.
         * @param onPublisherUpdateCdnUrlResult The execution result of update the relay CDN operation.
         */
        public abstract void RemovePublishCdnUrl(string streamID, string targetURL, OnPublisherUpdateCdnUrlResult onPublisherUpdateCdnUrlResult);

        /**
         * Whether to directly push to CDN (without going through the ZEGO RTC server), for the specified channel.
         *
         * Available since: 1.5.0
         * Description: Whether to publish streams directly from the client to CDN without passing through Zego RTC server.
         * Use cases: It is often used in large-scale live broadcast scenes that do not have particularly high requirements for delay.
         * Default value: The default is false, and direct push is not enabled.
         * When to call: After creating the engine [createEngine], before starting to push the stream [startPublishingStream].
         * Caution: The Direct Push CDN feature does not pass through the ZEGO Real-Time Audio and Video Cloud during network transmission, so you cannot use ZEGO's ultra-low latency audio and video services.
         * Related APIs: Dynamic re-push to CDN function [addPublishCdnUrl], [removePublishCdnUrl].
         *
         * @param enable Whether to enable direct publish CDN, true: enable direct publish CDN, false: disable direct publish CDN.
         * @param config CDN configuration, if null, use Zego's background default configuration.
         * @param channel Publish stream channel.
         */
        public abstract void EnablePublishDirectToCDN(bool enable, ZegoCDNConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sends Supplemental Enhancement Information.
         *
         * Available since: 1.1.0
         * Description: While pushing the stream to transmit the audio and video stream data, the stream media enhancement supplementary information is sent to synchronize some other additional information.
         * Use cases: Generally used in scenes such as synchronizing music lyrics or precise video layout, you can choose to send SEI.
         * When to call: After starting to push the stream [startPublishingStream].
         * Restrictions: Do not exceed 30 times per second, and the SEI data length is limited to 4096 bytes.
         * Caution: Since the SEI information follows the video frame, there may be frame loss due to network problems, so the SEI information may also be lost. In order to solve this situation, it should be sent several times within the restricted frequency.
         * Related APIs: After the pusher sends the SEI, the puller can obtain the SEI content by monitoring the callback of [onPlayerRecvSEI].
         *
         * @param data SEI data.
         * @param channel Publish stream channel.
         */
        public abstract void SendSEI(byte[] data, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Enables or disables hardware encoding.
         *
         * Available since: 1.1.0
         * Description: Whether to use the hardware encoding function when publishing the stream, the GPU is used to encode the stream and to reduce the CPU usage.
         * When to call: The setting can take effect before the stream published. If it is set after the stream published, the stream should be stopped first before it takes effect.
         * Caution: Because hard-coded support is not particularly good for a few models, SDK uses software encoding by default. If the developer finds that the device is hot when publishing a high-resolution audio and video stream during testing of some models, you can consider calling this function to enable hard coding.
         *
         * @param enable Whether to enable hardware encoding, true: enable hardware encoding, false: disable hardware encoding.
         */
        public abstract void EnableHardwareEncoder(bool enable);

        /**
         * Sets the timing of video scaling in the video capture workflow. You can choose to do video scaling right after video capture (the default value) or before encoding.
         *
         * Available since: 1.1.0
         * When to call: This function needs to be set before call [startPreview] or [startPublishingStream].
         * Caution: The main effect is Whether the local preview is affected when the acquisition resolution is different from the encoding resolution.
         *
         * @param mode The capture scale timing mode.
         */
        public abstract void SetCapturePipelineScaleMode(ZegoCapturePipelineScaleMode mode);

        public OnPublisherStateUpdate onPublisherStateUpdate;

        public OnPublisherQualityUpdate onPublisherQualityUpdate;

        public OnPublisherCapturedAudioFirstFrame onPublisherCapturedAudioFirstFrame;

        public OnPublisherCapturedVideoFirstFrame onPublisherCapturedVideoFirstFrame;

        public OnPublisherRenderVideoFirstFrame onPublisherRenderVideoFirstFrame;

        public OnPublisherVideoSizeChanged onPublisherVideoSizeChanged;

        public OnPublisherRelayCDNStateUpdate onPublisherRelayCDNStateUpdate;

        /**
         * Starts playing a stream from ZEGO RTC server or from third-party CDN. Support multi-room mode.
         *
         * Available since: 1.1.0
         * Description: Play audio and video streams from the ZEGO RTC server or CDN.
         * Use cases: In real-time or live broadcast scenarios, developers can listen to the [onRoomStreamUpdate] event callback to obtain the new stream information in the room where they are located, and call this interface to pass in streamID for play streams.
         * When to call: After [loginRoom].
         * Restrictions: None.
         * Caution: 1. The developer can update the player canvas by calling this function again (the streamID must be the same). 2. After the first play stream failure due to network reasons or the play stream is interrupted, the default time for SDK reconnection is 20min. 3. In the case of poor network quality, user play may be interrupted, the SDK will try to reconnect, and the current play status and error information can be obtained by listening to the [onPlayerStateUpdate] event. please refer to https://docs.zegocloud.com/faq/reconnect. 4. Playing the stream ID that does not exist, the SDK continues to try to play after calling this function. After the stream ID is successfully published, the audio and video stream can be actually played.
         *
         * @param streamID Stream ID, a string of up to 256 characters. You cannot include URL keywords, otherwise publishing stream and playing stream will fails. Only support numbers, English characters and '~', '!', '@', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '`', ';', '’', ',', '.', '<', '>', '/', '\'.
         * @param config Advanced player configuration.
         */
        public abstract void StartPlayingStream(string streamID, ZegoPlayerConfig config = null);

        /**
         * Stops playing a stream.
         *
         * Available since: 1.1.0
         * Description: Play audio and video streams from the ZEGO RTC server.
         * Use cases: In the real-time scenario, developers can listen to the [onRoomStreamUpdate] event callback to obtain the delete stream information in the room where they are located, and call this interface to pass in streamID for stop play streams.
         * When to call: After [loginRoom].
         * Restrictions: None.
         * Caution: When stopped, the attributes set for this stream previously, such as [setPlayVolume], [mutePlayStreamAudio], [mutePlayStreamVideo], etc., will be invalid and need to be reset when playing the the stream next time.
         *
         * @param streamID Stream ID.
         */
        public abstract void StopPlayingStream(string streamID);

        /**
         * Take a snapshot of the playing stream.
         *
         * Available since: 1.17.0
         * Description: Take a screenshot of the specified stream ID.
         * When to call: after called [startPlayingStream].
         * Restrictions: None.
         * Related callbacks: [onPlayerTakeSnapshotResult] Screenshot data callback.
         *
         * @param streamID Stream ID to be snapshot.
         * @param callback Results of take play stream snapshot.
         */
        public abstract void TakePlayStreamSnapshot(string streamID, OnPlayerTakeSnapshotResult callback);

        /**
         * Sets the stream playback volume.
         *
         * Available since: 1.16.0
         * Description: Set the sound size of the stream, the local user can control the playback volume of the audio stream.
         * When to call: after called [startPlayingStream].
         * Restrictions: None.
         * Related APIs: [setAllPlayStreamVolume] Set all stream volume.
         * Caution: You need to reset after [stopPlayingStream] and [startPlayingStream]. This function and the [setAllPlayStreamVolume] function overwrite each other, and the last call takes effect.
         *
         * @param streamID Stream ID.
         * @param volume Volume percentage. The value ranges from 0 to 200, and the default value is 100.
         */
        public abstract void SetPlayVolume(string streamID, int volume);

        /**
         * Sets the all stream playback volume.
         *
         * Available since: 2.3.0
         * Description: Set the sound size of the stream, the local user can control the playback volume of the audio stream.
         * When to call: after called [startPlayingStream].
         * Restrictions: None.
         * Related APIs: [setPlayVolume] Set the specified streaming volume.
         * Caution: You need to reset after [stopPlayingStream] and [startPlayingStream]. Set the specified streaming volume and [setAllPlayStreamVolume] interface to override each other, and the last call takes effect.
         *
         * @param volume Volume percentage. The value ranges from 0 to 200, and the default value is 100.
         */
        public abstract void SetAllPlayStreamVolume(int volume);

        /**
         * Set play video stream type.
         *
         * Available since: 2.3.0
         * Description: When the publish stream sets the codecID to SVC through [setVideoConfig], the puller can dynamically set and select different stream types (small resolution is one-half of the standard layer).
         * Use cases: In general, when the network is weak or the rendered UI window is small, you can choose to pull videos with small resolutions to save bandwidth.
         * When to call: before or after called [startPlayingStream].
         * Restrictions: None.
         *
         * @param streamID Stream ID.
         * @param streamType Video stream type.
         */
        public abstract void SetPlayStreamVideoType(string streamID, ZegoVideoStreamType streamType);

        /**
         * Set the adaptive adjustment interval range of the buffer for playing stream.
         *
         * Available since: 2.1.0
         * Description: Set the range of adaptive adjustment of the internal buffer of the sdk when streaming is 0-4000ms.
         * Use cases: Generally, in the case of a poor network environment, adjusting and increasing the playback buffer of the pull stream will significantly reduce the audio and video freezes, but will increase the delay.
         * When to call: after called [createEngine].
         * Restrictions: None.
         * Caution: When the upper limit of the cache interval set by the developer exceeds 4000ms, the value will be 4000ms. When the upper limit of the cache interval set by the developer is less than the lower limit of the cache interval, the upper limit will be automatically set as the lower limit.
         *
         * @param streamID Stream ID.
         * @param minBufferInterval The lower limit of the buffer adaptation interval, in milliseconds. The default value is 0ms.
         * @param maxBufferInterval The upper limit of the buffer adaptation interval, in milliseconds. The default value is 4000ms.
         */
        public abstract void SetPlayStreamBufferIntervalRange(string streamID, uint minBufferInterval, uint maxBufferInterval);

        /**
         * Set the weight of the pull stream priority.
         *
         * Available since: 1.1.0
         * Description: Set the weight of the streaming priority.
         * Use cases: This interface can be used when developers need to prioritize the quality of a stream in business. For example: in class scene, if students pull multiple streams, you can set high priority for teacher stream.
         * When to call: after called [startPlayingStream].
         * Restrictions: None.
         * Caution: By default, all streams have the same weight. Only one stream can be set with high priority, whichever is set last. After the flow is stopped, the initial state is automatically restored, and all flows have the same weight.When the local network is not good, while ensuring the focus flow, other stalls may be caused more.
         *
         * @param streamID Stream ID.
         */
        public abstract void SetPlayStreamFocusOn(string streamID);

        /**
         * Whether the pull stream can receive the specified audio data.
         *
         * Available since: 1.1.0
         * Description: In the process of real-time audio and video interaction, local users can use this function to control whether to receive audio data from designated remote users when pulling streams as needed. When the developer does not receive the audio receipt, the hardware and network overhead can be reduced.
         * Use cases: Call this function when developers need to quickly close and restore remote audio. Compared to re-flow, it can greatly reduce the time and improve the interactive experience.
         * When to call: This function can be called after calling [createEngine].
         * Caution: This function is valid only when the [muteAllPlayStreamAudio] function is set to `false`.
         * Related APIs: You can call the [muteAllPlayStreamAudio] function to control whether to receive all audio data. When the two functions [muteAllPlayStreamAudio] and [mutePlayStreamAudio] are set to `false` at the same time, the local user can receive the audio data of the remote user when the stream is pulled: 1. When the [muteAllPlayStreamAudio(true)] function is called, it is globally effective, that is, local users will be prohibited from receiving all remote users' audio data. At this time, the [mutePlayStreamAudio] function will not take effect whether it is called before or after [muteAllPlayStreamAudio].2. When the [muteAllPlayStreamAudio(false)] function is called, the local user can receive the audio data of all remote users. At this time, the [mutePlayStreamAudio] function can be used to control whether to receive a single audio data. Calling the [mutePlayStreamAudio(true, streamID)] function allows the local user to receive audio data other than the `streamID`; calling the [mutePlayStreamAudio(false, streamID)] function allows the local user to receive all audio data.
         *
         * @param streamID Stream ID.
         * @param mute Whether it can receive the audio data of the specified remote user when streaming, "true" means prohibition, "false" means receiving, the default value is "false".
         */
        public abstract void MutePlayStreamAudio(string streamID, bool mute);

        /**
         * Whether the pull stream can receive the specified video data.
         *
         * Available since: 1.1.0
         * Description: In the process of real-time video and video interaction, local users can use this function to control whether to receive video data from designated remote users when pulling streams as needed. When the developer does not receive the audio receipt, the hardware and network overhead can be reduced.
         * Use cases: This function can be called when developers need to quickly close and resume watching remote video. Compared to re-flow, it can greatly reduce the time and improve the interactive experience.
         * When to call: This function can be called after calling [createEngine].
         * Caution: This function is valid only when the [muteAllPlayStreamVideo] function is set to `false`.
         * Related APIs: You can call the [muteAllPlayStreamVideo] function to control whether to receive all video data. When the two functions [muteAllPlayStreamVideo] and [mutePlayStreamVideo] are set to `false` at the same time, the local user can receive the video data of the remote user when the stream is pulled: 1. When the [muteAllPlayStreamVideo(true)] function is called, it will take effect globally, that is, local users will be prohibited from receiving all remote users' video data. At this time, the [mutePlayStreamVideo] function will not take effect whether it is called before or after [muteAllPlayStreamVideo]. 2. When the [muteAllPlayStreamVideo(false)] function is called, the local user can receive the video data of all remote users. At this time, the [mutePlayStreamVideo] function can be used to control whether to receive a single video data. Call the [mutePlayStreamVideo(true, streamID)] function, the local user can receive other video data other than the `streamID`; call the [mutePlayStreamVideo(false, streamID)] function, the local user can receive all the video data.
         *
         * @param streamID Stream ID.
         * @param mute Whether it is possible to receive the video data of the specified remote user when streaming, "true" means prohibition, "false" means receiving, the default value is "false".
         */
        public abstract void MutePlayStreamVideo(string streamID, bool mute);

        /**
         * Can the pull stream receive all audio data.
         *
         * Available since: 2.4.0
         * Description: In the process of real-time audio and video interaction, local users can use this function to control whether to receive audio data from all remote users when pulling streams (including the audio streams pushed by users who have newly joined the room after calling this function). By default, users can receive audio data pushed by all remote users after joining the room. When the developer does not receive the audio receipt, the hardware and network overhead can be reduced.
         * Use cases: Call this function when developers need to quickly close and restore remote audio. Compared to re-flow, it can greatly reduce the time and improve the interactive experience.
         * When to call: This function can be called after calling [createEngine].
         * Related APIs: You can call the [mutePlayStreamAudio] function to control whether to receive a single piece of audio data. When the two functions [muteAllPlayStreamAudio] and [mutePlayStreamAudio] are set to `false` at the same time, the local user can receive the audio data of the remote user when the stream is pulled: 1. When the [muteAllPlayStreamAudio(true)] function is called, it takes effect globally, that is, local users will be prohibited from receiving audio data from all remote users. At this time, the [mutePlayStreamAudio] function will not take effect no matter if the [mutePlayStreamAudio] function is called before or after [muteAllPlayStreamAudio]. 2. When the [muteAllPlayStreamAudio(false)] function is called, the local user can receive the audio data of all remote users. At this time, the [mutePlayStreamAudio] function can be used to control whether to receive a single audio data. Calling the [mutePlayStreamAudio(true, streamID)] function allows the local user to receive audio data other than the `streamID`; calling the [mutePlayStreamAudio(false, streamID)] function allows the local user to receive all audio data.
         *
         * @param mute Whether it is possible to receive audio data from all remote users when streaming, "true" means prohibition, "false" means receiving, and the default value is "false".
         */
        public abstract void MuteAllPlayStreamAudio(bool mute);

        /**
         * Can the pull stream receive all video data.
         *
         * Available since: 2.4.0
         * Description: In the process of real-time video and video interaction, local users can use this function to control whether to receive all remote users' video data when pulling the stream (including the video stream pushed by the new user who joins the room after calling this function). By default, users can receive video data pushed by all remote users after joining the room. When the developer does not receive the audio receipt, the hardware and network overhead can be reduced.
         * Use cases: This function can be called when developers need to quickly close and resume watching remote video. Compared to re-flow, it can greatly reduce the time and improve the interactive experience.
         * When to call: This function can be called after calling [createEngine].
         * Related APIs: You can call the [mutePlayStreamVideo] function to control whether to receive a single piece of video data. When the two functions [muteAllPlayStreamVideo] and [mutePlayStreamVideo] are set to `false` at the same time, the local user can receive the video data of the remote user when the stream is pulled: 1. When the [muteAllPlayStreamVideo(true)] function is called, it will take effect globally, that is, the local user will be prohibited from receiving all remote users' video data. At this time, the [mutePlayStreamVideo] function will not take effect whether it is called before or after [muteAllPlayStreamVideo]. 2. When the [muteAllPlayStreamVideo(false)] function is called, the local user can receive the video data of all remote users. At this time, the [mutePlayStreamVideo] function can be used to control whether to receive a single video data. Call the [mutePlayStreamVideo(true, streamID)] function, the local user can receive other video data other than the `streamID`; call the [mutePlayStreamVideo(false, streamID)] function, the local user can receive all the video data.
         *
         * @param mute Whether it is possible to receive all remote users' video data when streaming, "true" means prohibition, "false" means receiving, and the default value is "false".
         */
        public abstract void MuteAllPlayStreamVideo(bool mute);

        /**
         * Enables or disables hardware decoding.
         *
         * Available since: 1.1.0
         * Description: Control whether hardware decoding is used when playing streams, with hardware decoding enabled the SDK will use the GPU for decoding, reducing CPU usage.
         * Use cases: If developers find that the device heats up badly when playing large resolution audio and video streams during testing on some models, consider calling this function to enable hardware decoding.
         * Default value: Hardware decoding is disabled by default when this interface is not called.
         * When to call: This function needs to be called after [createEngine] creates an instance.
         * Restrictions: None.
         * Caution: Need to be called before calling [startPlayingStream], if called after playing the stream, it will only take effect after stopping the stream and re-playing it. Once this configuration has taken effect, it will remain in force until the next call takes effect.
         *
         * @param enable Whether to turn on hardware decoding switch, true: enable hardware decoding, false: disable hardware decoding.
         */
        public abstract void EnableHardwareDecoder(bool enable);

        /**
         * Enables or disables frame order detection.
         *
         * Available since: 1.1.0
         * Description: Control whether to turn on frame order detection, on to not support B frames, off to support B frames.
         * Use cases: Turning on frame order detection when pulling cdn's stream will prevent splash screens.
         * Default value: Turn on frame order detection by default when this interface is not called.
         * When to call: This function needs to be called after [createEngine] creates an instance.
         * Restrictions: None.
         * Caution: Turn off frame order detection during playing stream may result in a brief splash screen.
         *
         * @param enable Whether to turn on frame order detection, true: enable check poc,not support B frames, false: disable check poc, support B frames.
         */
        public abstract void EnableCheckPoc(bool enable);

        public OnPlayerStateUpdate onPlayerStateUpdate;

        public OnPlayerQualityUpdate onPlayerQualityUpdate;

        public OnPlayerMediaEvent onPlayerMediaEvent;

        public OnPlayerRecvAudioFirstFrame onPlayerRecvAudioFirstFrame;

        public OnPlayerRecvVideoFirstFrame onPlayerRecvVideoFirstFrame;

        public OnPlayerRenderVideoFirstFrame onPlayerRenderVideoFirstFrame;

        public OnPlayerVideoSizeChanged onPlayerVideoSizeChanged;

        public OnPlayerRecvSEI onPlayerRecvSEI;

        /**
         * Starts a stream mixing task.
         *
         * Available since: 1.2.1
         * Description: Initiate a mixing stream request to the ZEGO RTC server, the server will look for the stream currently being pushed, and mix the layers according to the parameters of the mixing stream task requested by the SDK. When you need to update a mixing task, that is, when the input stream increases or decreases, you need to update the input stream list. At this time, you can update the field of the [ZegoMixerTask] object inputList and call this function again to pass in the same [ZegoMixerTask] object to update the mixing task.
         * Use cases: It is often used when multiple video images are required to synthesize a video using mixed streaming, such as education, live broadcast of teacher and student images.
         * When to call: After calling [loginRoom] to log in to the room.
         * Restrictions: None.
         * Caution: Due to the performance considerations of the client device, the SDK muxing is to start the mixing task on the ZEGO RTC server for mixing. If an exception occurs when the mixing task is requested to start, for example, the most common mixing input stream does not exist, the error code will be given from the callback callback. If a certain input stream does not exist in the middle, the muxing task will automatically retry to pull this input stream for 90 seconds, and will not retry after 90 seconds. If all input streams no longer exist, the server will automatically stop the mixing task after 90 seconds.
         * Related callbacks: [OnMixerRelayCDNStateUpdate] can be used to obtain the CDN status update notification of the mixed stream repost, and the sound update notification of each single stream in the mixed stream can be obtained through [onMixerSoundLevelUpdate].
         * Related APIs: the mixing task can be stopped by the [stopMixerTask] function.
         *
         * @param task Mixing task object. Required: Yes.
         * @param onMixerStartResult Start notification of mixing task results.Required: No. Caution: Passing [null] means not receiving callback notifications.
         */
        public abstract void StartMixerTask(ZegoMixerTask task, OnMixerStartResult onMixerStartResult);

        /**
         * Stops a stream mixing task.
         *
         * Available since: 1.2.1
         * Description: Initiate a request to end the mixing task to the ZEGO RTC server.
         * Use cases: It is often used when multiple video images are required to synthesize a video using mixed streaming, such as education, live broadcast of teacher and student images.
         * When to call: After calling [startMixerTask] to start mixing.
         * Restrictions: None.
         * Caution: If the developer starts the next mixing task without stopping the previous mixing task, the previous mixing task will not automatically stop until the input stream of the previous mixing task does not exist for 90 seconds. Before starting the next mixing task, you should stop the previous mixing task, so that when an anchor has already started the next mixing task to mix with other anchors, the audience is still pulling the output stream of the previous mixing task.
         * Related APIs: You can start mixing by using the [startMixerTask] function.
         *
         * @param task Mixing task object. Required: Yes.
         * @param onMixerStopResult Stop stream mixing task result callback notification.Required: No. Caution: Passing [null] means not receiving callback notifications.
         */
        public abstract void StopMixerTask(ZegoMixerTask task, OnMixerStopResult onMixerStopResult);

        /**
         * Starts an auto stream mixing task.
         *
         * Available since: 2.10.0
         * Description: Local users can use this function to start an auto stream mixing task, all streams in a room wil be mixed. Currently, only audio streams can be mixed. When auto stream mixing is enabled, the audio of all streams in the room is automatically mixed, and any further pushed streams in the room are automatically mixed into the final output stream.
         * Use cases: Often used in voice chat room scenarios.
         * When to call: After the engine is created, user can call this function to enable auto stream mixing in the target room if the target room has already been created
         * Restrictions: Please refer to https://docs.zegocloud.com/article/7611 or contact ZEGO technical support.
         * Caution: Before starting the next auto stream mixing task in the same room, call [stopAutoMixerTask] function to end the last auto stream mixing task. Otherwise, when one anchor has started the next auto stream mixing task and other anchors have started the next auto stream mixing task, the audience still pulls the output stream of the first auto stream mixing task. If the user does not end the current auto stream mixing task, the task will automatically end after the room does not exist or the input streams do not exist for 90 seconds.
         * Related callbacks: Users can obtain the execution result of the function through [ZegoMixerStartCallback] callback.Users can get automatic mixed flow information through [onAutoMixerSoundLevelUpdate] callback.
         * Related APIs: Users can call [stopAutoMixerTask] function to stop an auto stream mixing task.
         *
         * @param task Auto mix stream task object
         * @param callback Start auto mix stream task result callback notification
         */
        public abstract void StartAutoMixerTask(ZegoAutoMixerTask task, OnMixerStartResult callback);

        /**
         * Stops an auto stream mixing task.
         *
         * Available since: 2.10.0
         * Description: Local users can use this function to stop an auto stream mixing task.
         * Use cases: Often used in voice chat room scenarios.
         * When to call: Call this function after [startAutoMixerTask] function is called to start an auto stream mixing task.
         * Caution: Before calling [startAutoMixerTask] function to start the next auto stream mixing task in the same room, call this function to end the last auto stream mixing task. Otherwise, when one anchor has started the next auto stream mixing task and other anchors have started the next auto stream mixing task, the audience still pulls the output stream of the first auto stream mixing task. If the user does not end the current auto stream mixing task, the task will automatically end after the room does not exist or the input streams do not exist for 90 seconds.
         * Related callbacks: Users can obtain the execution result of the function through [ZegoMixerStopCallback] callback.
         * Related APIs: Users can call [startAutoMixerTask] function to start an auto stream mixing task.
         *
         * @param task Auto mix stream task object
         * @param callback Stop auto mix stream task result callback notification
         */
        public abstract void StopAutoMixerTask(ZegoAutoMixerTask task, OnMixerStopResult callback);

        public OnMixerRelayCDNStateUpdate onMixerRelayCDNStateUpdate;

        public OnMixerSoundLevelUpdate onMixerSoundLevelUpdate;

        public OnAutoMixerSoundLevelUpdate onAutoMixerSoundLevelUpdate;

        /**
         * Mutes or unmutes the microphone.
         *
         * Available since: 1.1.0
         * Description: This function is used to control whether to use the collected audio data. Mute (turn off the microphone) will use the muted data to replace the audio data collected by the device for streaming. At this time, the microphone device will still be occupied.
         * Default value: The default is `false`, which means no muting.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         * Related APIs: Developers who want to control whether to use microphone on the UI should use this function to avoid unnecessary performance overhead by using the [enableAudioCaptureDevice]. You can use [isMicrophoneMuted] to check if the microphone is muted.
         *
         * @param mute Whether to mute (disable) the microphone, `true`: mute (disable) microphone, `false`: enable microphone.
         */
        public abstract void MuteMicrophone(bool mute);

        /**
         * Checks whether the microphone is muted.
         *
         * Available since: 1.1.0
         * Description: Used to determine whether the microphone is set to mute.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         * Related APIs: [muteMicrophone].
         *
         * @return Whether the microphone is muted; true: the microphone is muted; `false`: the microphone is enable (not muted).
         */
        public abstract bool IsMicrophoneMuted();

        /**
         * Mutes or unmutes the audio output speaker.
         *
         * Available since: 1.1.0
         * Description: After mute speaker, all the SDK sounds will not play, including playing stream, mediaplayer, etc. But the SDK will still occupy the output device.
         * Default value: The default is `false`, which means no muting.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         *
         * @param mute Whether to mute (disable) speaker audio output, `true`: mute (disable) speaker audio output, `false`: enable speaker audio output.
         */
        public abstract void MuteSpeaker(bool mute);

        /**
         * Checks whether the audio output speaker is muted.
         *
         * Available since: 1.1.0
         * Description: Used to determine whether the audio output is muted.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         * Related APIs: [muteSpeaker].
         *
         * @return Whether the speaker is muted; `true`: the speaker is muted; `false`: the speaker is enable (not muted).
         */
        public abstract bool IsSpeakerMuted();

        /**
         * Gets a list of audio devices.
         *
         * Only supports Windows and Mac.
         *
         * @param deviceType Audio device type
         * @return Audo device List
         */
        public abstract ZegoDeviceInfo[] GetAudioDeviceList(ZegoAudioDeviceType deviceType);

        /**
         * Chooses to use the specified audio device.
         *
         * Available since: 1.0.0
         * Description: Chooses to use the specified audio device.
         * When to call: After creating the engine [createEngine] and before call [startPublishingStream] or [startPlayingStream].
         * Restrictions: Only supports Windows/macOS.
         *
         * @param deviceType Audio device type
         * @param deviceID ID of a device obtained by [getAudioDeviceList]
         */
        public abstract void UseAudioDevice(ZegoAudioDeviceType deviceType, string deviceID);

        /**
         * Enables or disables the audio capture device.
         *
         * Available since: 1.1.0
         * Description: This function is used to control whether to use the audio collection device. When the audio collection device is turned off, the SDK will no longer occupy the audio device. Of course, if the stream is being published at this time, there is no audio data.
         * Use cases: When the user never needs to use the audio, you can call this function to close the audio collection.
         * Default value: The default is `true`.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         * Related APIs: Turning off or turning on the microphone on the hardware is a time-consuming operation, and there is a certain performance overhead when the user performs frequent operations. [muteMicrophone] is generally recommended.
         *
         * @param enable Whether to enable the audio capture device, `true`: enable audio capture device, `false`: disable audio capture device.
         */
        public abstract void EnableAudioCaptureDevice(bool enable);

#if UNITY_ANDROID || UNITY_IOS
        /**
         * get current audio route type.
         *
         * Available since: 1.1.0
         * Description: Audio routing refers to the audio output device that an app uses to play audio, and common audio routes are: speakers, handsets, headphones, Bluetooth devices, and so on.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         * Related APIs: Set audio route to speaker [setAudioRouteToSpeaker].
         */
        public abstract ZegoAudioRoute GetAudioRouteType();
#endif

#if UNITY_ANDROID || UNITY_IOS
        /**
         * Whether to use the built-in speaker to play audio.
         *
         * Available since: 1.1.0
         * Description: Whether to use the speaker to play audio, when you choose not to use the built-in speaker to play the sound, the SDK will select the audio output device with the highest current priority to play the sound according to the system schedule, and common audio routes are: handsets, headphones, Bluetooth devices, and so on.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         * Related APIs: Get the current audio route [getAudioRouteType].
         *
         * @param defaultToSpeaker Whether to use the built-in speaker to play sound, `true`: use the built-in speaker to play sound, `false`: use the highest priority audio output device scheduled by the current system to play sound
         */
        public abstract void SetAudioRouteToSpeaker(bool defaultToSpeaker);
#endif

        /**
         * Turns on/off the camera (for the specified channel).
         *
         * Available since: 1.1.0
         * Description: This function is used to control whether to start the capture of the camera. After the camera is turned off, the video capture will not be performed. At this time, there will be no video data for local preview and push streaming.
         * Default value: The default is `true` which means the camera is turned on.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         * Caution: In the case of using the custom video capture function [enableCustomVideoCapture], since the developer has taken over the video data capture, the SDK is no longer responsible for the video data capture, but this function still affects whether to encode or not. Therefore, when developers use custom video capture, please ensure that the value of this function is `true`.
         *
         * @param enable Whether to turn on the camera, `true`: turn on camera, `false`: turn off camera
         * @param channel Publishing stream channel
         */
        public abstract void EnableCamera(bool enable, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Switches to the front or the rear camera (for the specified channel).
         *
         * Available since: 1.1.0
         * Description: This function is used to control the use of the front camera or the rear camera (only supported by Android and iOS).
         * Default value: The default is `true` which means the front camera is used.
         * When to call: After creating the engine [createEngine].
         * Restrictions: None.
         * Caution: When the custom video capture function [enableCustomVideoCapture] is turned on, since the developer has taken over the video data capture, the SDK is no longer responsible for the video data capture, and this function is no longer valid.
         *
         * @param enable Whether to use the front camera, `true`: use the front camera, `false`: use the the rear camera.
         * @param channel Publishing stream channel
         */
        public abstract void UseFrontCamera(bool enable, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Chooses to use the specified video device (for the specified channel).
         *
         * Only for Windows/macOS.
         *
         * @param deviceID ID of a device obtained by getVideoDeviceList
         * @param channel Publishing stream channel
         */
        public abstract void UseVideoDevice(string deviceID, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Gets a list of video devices.
         *
         * Only for Windows/macOS.
         *
         * @return Video device List
         */
        public abstract ZegoDeviceInfo[] GetVideoDeviceList();

        /**
         * Starts sound level monitoring. Support setting the listening interval.
         *
         * Available since: 1.15.0
         * Description: After starting monitoring, you can receive local audio sound level via [onCapturedSoundLevelUpdate] callback, and receive remote audio sound level via [onRemoteSoundLevelUpdate] callback. Before entering the room, you can call [startPreview] with this function and combine it with [onCapturedSoundLevelUpdate] callback to determine whether the audio device is working properly.
         * Use cases: During the publishing and playing process, determine who is talking on the wheat and do a UI presentation.
         * When to call: After the engine is created [createEngine].
         * Caution:
         *   1. [onCapturedSoundLevelUpdate] and [onRemoteSoundLevelUpdate] callback notification period is the value set by the parameter. If you want to use advanced feature of sound level, please use the function of the same name (the parameter type is ZegoSoundLevelConfig) instead.
         *   2. After the sound monitoring is started, even if the local audio capture is not started, [onCapturedSoundLevelUpdate] will have a callback, and the sound level is 0.
         *
         * @param millisecond Monitoring time period of the sound level, in milliseconds, has a value range of [100, 3000]. Default is 100 ms.
         */
        public abstract void StartSoundLevelMonitor(uint millisecond = 100);

        /**
         * Starts sound level monitoring. Support enable some advanced feature.
         *
         * Available since: 2.10.0
         * Description: After starting monitoring, you can receive local audio sound level via [onCapturedSoundLevelUpdate] callback, and receive remote audio sound level via [onRemoteSoundLevelUpdate] callback. Before entering the room, you can call [startPreview] with this function and combine it with [onCapturedSoundLevelUpdate] callback to determine whether the audio device is working properly.
         * Use cases: During the publishing and playing process, determine who is talking on the wheat and do a UI presentation.
         * When to call: After the engine is created [createEngine].
         * Caution:
         *   1. [onCapturedSoundLevelUpdate] and [onRemoteSoundLevelUpdate] callback notification period is the value set by the parameter.
         *   2. After the sound monitoring is started, even if the local audio capture is not started, [onCapturedSoundLevelUpdate] will have a callback, and the sound level is 0.
         *
         * @param config Configuration for starts the sound level monitor.
         */
        public abstract void StartSoundLevelMonitor(ZegoSoundLevelConfig config);

        /**
         * Stops sound level monitoring.
         *
         * Available since: 1.1.0
         * Description: After the monitoring is stopped, the callback of the local/remote audio sound level will be stopped.
         * When to call: After the engine is created [createEngine].
         * Related APIs: Soundwave monitoring can be initiated via [startSoundLevelMonitor].
         */
        public abstract void StopSoundLevelMonitor();

        /**
         * Starts audio spectrum monitoring. Support setting the listening interval.
         *
         * Available since: 1.15.0
         * Description: After starting monitoring, you can receive local audio spectrum via [onCapturedAudioSpectrumUpdate] callback, and receive remote audio spectrum via [onRemoteAudioSpectrumUpdate] callback.
         * Use cases: In the host K song scene, has been published or played under the premise that the host or audience to see the tone and volume change animation.
         * When to call: After the engine is created [createEngine].
         * Caution: [onCapturedAudioSpectrumUpdate] and [onRemoteAudioSpectrumUpdate] callback notification period is the value set by the parameter.
         *
         * @param millisecond Monitoring time period of the audio spectrum, in milliseconds, has a value range of [100, 3000]. Default is 100 ms.
         */
        public abstract void StartAudioSpectrumMonitor(uint millisecond = 100);

        /**
         * Stops audio spectrum monitoring.
         *
         * Available since: 1.1.0
         * Description: After the monitoring is stopped, the callback of the local/remote audio spectrum will be stopped.
         * When to call: After the engine is created [createEngine].
         * Related APIs: Audio spectrum monitoring can be initiated via [startAudioSpectrumMonitor].
         */
        public abstract void StopAudioSpectrumMonitor();

        /**
         * Enables or disables headphone monitoring.
         *
         * Available since: 1.9.0
         * Description: Enable/Disable headphone monitor, and users hear their own voices as they use the microphone to capture sounds.
         * When to call: After the engine is created [createEngine].
         * Default value: Disable.
         * Caution: This setting does not actually take effect until both the headset and microphone are connected.
         *
         * @param enable Whether to use headphone monitor, true: enable, false: disable
         */
        public abstract void EnableHeadphoneMonitor(bool enable);

        /**
         * Sets the headphone monitor volume.
         *
         * Available since: 1.9.0
         * Description: set headphone monitor volume.
         * When to call: After the engine is created [createEngine].
         * Caution: This setting does not actually take effect until both the headset and microphone are connected.
         * Related APIs: Enables or disables headphone monitoring via [enableHeadphoneMonitor].
         *
         * @param volume headphone monitor volume, range from 0 to 200, 60 as default.
         */
        public abstract void SetHeadphoneMonitorVolume(int volume);

        public OnCapturedSoundLevelUpdate onCapturedSoundLevelUpdate;

        public OnCapturedSoundLevelInfoUpdate onCapturedSoundLevelInfoUpdate;

        public OnRemoteSoundLevelUpdate onRemoteSoundLevelUpdate;

        public OnRemoteSoundLevelInfoUpdate onRemoteSoundLevelInfoUpdate;

        public OnCapturedAudioSpectrumUpdate onCapturedAudioSpectrumUpdate;

        public OnRemoteAudioSpectrumUpdate onRemoteAudioSpectrumUpdate;

        public OnLocalDeviceExceptionOccurred onLocalDeviceExceptionOccurred;

        public OnRemoteCameraStateUpdate onRemoteCameraStateUpdate;

        public OnRemoteMicStateUpdate onRemoteMicStateUpdate;

        public OnRemoteSpeakerStateUpdate onRemoteSpeakerStateUpdate;

#if UNITY_IOS || UNITY_ANDROID
        public OnAudioRouteChange onAudioRouteChange;
#endif

        /**
         * Whether to enable acoustic echo cancellation (AEC).
         *
         * Available since: 1.1.0
         * Description: Turning on echo cancellation, the SDK filters the collected audio data to reduce the echo component in the audio.
         * Use case: When you need to reduce the echo to improve the call quality and user experience, you can turn on this feature.
         * Default value: When this function is not called, iOS turns off echo cancellation by default and other platforms turn on echo cancellation by default
         * When to call: It needs to be called after [createEngine], before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager].
         * Caution: The AEC function only supports the processing of sounds playbacked through the SDK, such as sounds played by the playing stream, media player, audio effect player, etc.
         * Restrictions: None.
         * Related APIs: Developers can use [enableHeadphoneAEC] to set whether to enable AEC when using headphones, and use [setAECMode] to set the echo cancellation mode.
         *
         * @param enable Whether to enable echo cancellation, true: enable, false: disable
         */
        public abstract void EnableAEC(bool enable);

        /**
         * Whether to turn on acoustic echo cancellation (AEC) when using the headphone.
         *
         * Available since: 1.1.0
         * Description: When [enableAEC] is used to turn on echo cancellation, it is only turned on when the speaker is used for mobile terminal equipment. Call this function if you need to turn echo cancellation on or off when using the headset.
         * Use case: It is common when the mobile device is connected to a external sound card as the audio output source. In order to eliminate the echo in this case, you need to call this function to turn on the echo cancellation.
         * Default value: Android is off by default and iOS is on by default.
         * When to call: It needs to be called after [createEngine], before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager].
         * Caution: Turning on echo cancellation will increase the ear return delay. On the iOS platform, the SDK cannot distinguish between the headset and the external sound card. If you use this function to turn off the system echo cancellation when using the headset, the sound played by the external sound card will be collected when the user accesses the external sound card.
         * Restrictions: None.
         * Related APIs: When the headset is not used, you can set whether the SDK turns on echo cancellation through [enableAEC].
         * Platform differences: Only supports iOS and Android.
         *
         * @param enable Whether to enable, true: enable, false: disable
         */
        public abstract void EnableHeadphoneAEC(bool enable);

        /**
         * Sets the acoustic echo cancellation (AEC) mode.
         *
         * Available since: 1.1.0
         * Description: When [enableAEC] is used to enable echo cancellation, this function can be used to switch between different echo cancellation modes to control the degree of echo cancellation.
         * Use case: When the default echo cancellation effect does not meet expectations, this function can be used to adjust the echo cancellation mode.
         * Default value: When this function is not called, the default echo cancellation mode is [Aggressive].
         * When to call: It needs to be called after [createEngine], before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager].
         * Restrictions: The value set by this function is valid only after the echo cancellation function is turned on.
         *
         * @param mode Echo cancellation mode
         */
        public abstract void SetAECMode(ZegoAECMode mode);

        /**
         * Enables or disables automatic gain control (AGC).
         *
         * Available since: 1.1.0
         * Description: After turning on this function, the SDK can automatically adjust the microphone volume to adapt to near and far sound pickups and keep the volume stable.
         * Use case: When you need to ensure volume stability to improve call quality and user experience, you can turn on this feature.
         * Default value: When this function is not called, AGC is enabled by default.
         * When to call: It needs to be called after [createEngine] and before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager]. Note that the Mac needs to be called after [startPreview] and before [startPublishingStream].
         * Restrictions: None.
         *
         * @param enable Whether to enable automatic gain control, true: enable, false: disable
         */
        public abstract void EnableAGC(bool enable);

        /**
         * Enables or disables active noise suppression (ANS, aka ANC).
         *
         * Available since: 1.1.0
         * Description: Enable the noise suppression can reduce the noise in the audio data and make the human voice clearer.
         * Use case: When you need to suppress noise to improve call quality and user experience, you can turn on this feature.
         * Default value: When this function is not called, ANS is enabled by default.
         * When to call: It needs to be called after [createEngine], before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager].
         * Related APIs: This function has a better suppression effect on continuous noise (such as the sound of rain, white noise). If you need to turn on transient noise suppression, please use [enableTransientANS]. And the noise suppression mode can be set by [setANSMode].
         * Restrictions: None.
         *
         * @param enable Whether to enable noise suppression, true: enable, false: disable
         */
        public abstract void EnableANS(bool enable);

        /**
         * Enables or disables transient noise suppression.
         *
         * Available since: 1.17.0
         * Description: Enable the transient noise suppression can suppress the noises such as keyboard and desk knocks.
         * Use case: When you need to suppress transient noise to improve call quality and user experience, you can turn on this feature.
         * Default value: When this function is not called, this is disabled by default.
         * When to call: It needs to be called after [createEngine], before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager].
         * Related APIs: This function will not suppress normal noise after it is turned on. If you need to turn on normal noise suppression, please use [enableANS].
         * Restrictions: None.
         *
         * @param enable Whether to enable transient noise suppression, true: enable, false: disable
         */
        public abstract void EnableTransientANS(bool enable);

        /**
         * Sets the automatic noise suppression (ANS) mode.
         *
         * Available since: 1.1.0
         * Description: When [enableANS] is used to enable noise suppression, this function can be used to switch between different noise suppression modes to control the degree of noise suppression.
         * Use case: When the default noise suppression effect does not meet expectations, this function can be used to adjust the noise suppression mode.
         * Default value: When this function is not called, the default echo cancellation mode is [Medium].
         * When to call: It needs to be called after [createEngine], before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager].
         * Restrictions: The value set by this function is valid only after the noise suppression function is turned on.
         *
         * @param mode Audio Noise Suppression mode
         */
        public abstract void SetANSMode(ZegoANSMode mode);

        /**
         * Setting up the voice changer via preset enumeration.
         *
         * Available since: 1.17.0
         * Description: Call this function to use preset voice changer effect.
         * Use cases: Often used in live broadcasting, voice chatroom and KTV.
         * When to call: It needs to be called after [createEngine].
         * Restrictions: Voice changer effect is only effective for SDK captured sound.
         * Related APIs:
         * If you need advanced voice changer effect, please use [setVoiceChangerParam].
         * This function is mutually exclusive with [setReverbPreset]. If used at the same time, it will produce undefined effect.
         * Using ANDROID/ETHEREAL preset voice changer effect will modify reverberation or reverberation echo parameters. Calling [setVoiceChangerParam], [setReverbAdvancedParam], [setReverbEchoParam] may affect the voice changer effect after use these preset voice changer effect.
         * If you need advanced reverb/echo/electronic effects/voice changer effect, please use [setReverbAdvancedParam], [setReverbEchoParam], [setElectronicEffects], [setVoiceChangerParam] together.
         *
         * @param preset The voice changer preset enumeration.
         */
        public abstract void SetVoiceChangerPreset(ZegoVoiceChangerPreset preset);

        /**
         * Setting up the specific voice changer parameters.
         *
         * Available since: 1.10.0
         * Description: Call this function to set custom voice changer effect.
         * Use cases: Often used in live broadcasting, voice chatroom and KTV.
         * When to call: It needs to be called after [createEngine].
         * Restrictions: Voice changer effect is only effective for SDK captured sound.
         * Related APIs:
         * [setVoiceChangerPreset] provide a set of preset voice changer effects.
         * If you need advanced reverb/echo/voice changer effect, please use [setReverbAdvancedParam], [setReverbEchoParam], [setVoiceChangerParam] together.
         *
         * @param param Voice changer parameters.
         */
        public abstract void SetVoiceChangerParam(ZegoVoiceChangerParam param);

        /**
         * Setting up the reverberation via preset enumeration.
         *
         * Available since: 1.17.0
         * Description: Call this function to set preset reverb effect.
         * Use cases: Often used in live broadcasting, voice chatroom and KTV.
         * When to call: It needs to be called after [createEngine]. Support call this function to change preset reverb effect during publishing stream.
         * Restrictions: Reverb effect is only effective for SDK captured sound.
         * Related APIs:
         * If you need advanced reverb effect, please use [setReverbAdvancedParam].
         * This function is mutually exclusive with [setVoiceChangerPreset]. If used at the same time, it will produce undefined effects.
         * If you need advanced reverb/echo/voice changer effect, please use [setReverbAdvancedParam], [setReverbEchoParam], [setVoiceChangerParam] together.
         *
         * @param preset The reverberation preset enumeration.
         */
        public abstract void SetReverbPreset(ZegoReverbPreset preset);

        /**
         * Setting up the specific reverberation parameters.
         *
         * Available since: 1.10.0
         * Description: Call this function to set preset reverb effect.
         * Use cases: Often used in live broadcasting, voice chatroom and KTV.
         * When to call: It needs to be called after [createEngine].
         * Restrictions: Reverb effect is only effective for SDK captured sound.
         * Caution: Different values dynamically set during publishing stream will take effect. When all parameters are set to 0, the reverberation is turned off.
         * Related APIs:
         * [setReverbPreset] provide a set of preset reverb effects.
         * If you need advanced reverb/echo/voice changer effect, please use [setReverbAdvancedParam], [setReverbEchoParam], [setVoiceChangerParam] together.
         *
         * @param param Reverb advanced parameter.
         */
        public abstract void SetReverbAdvancedParam(ZegoReverbAdvancedParam param);

        /**
         * Setting up the specific reverberation echo parameters.
         *
         * Available since: 1.17.0
         * Description: Call this function to set reverb echo effect. This function can be used with voice changer and reverb to achieve a variety of custom sound effects.
         * Use cases: Often used in live broadcasting, voice chatroom and KTV.
         * When to call: It needs to be called after [createEngine].
         * Restrictions: Reverb echo effect is only effective for SDK captured sound.
         * Related APIs: If you need advanced reverb/echo/voice changer effect, please use [setReverbAdvancedParam], [setReverbEchoParam], [setVoiceChangerParam] together.
         *
         * @param param The reverberation echo parameter.
         */
        public abstract void SetReverbEchoParam(ZegoReverbEchoParam param);

        /**
         * Enable or disable the virtual stereo effect when publishing stream..
         *
         * Available since: 1.10.0; Note: Starting from 2.15.0, the angle parameter supports setting -1 to present a all round virtual stereo effect.
         * Description: Call this function to enable / disable the virtual stereo effect when publishing stream.
         * Use cases: Often used in live broadcasting, voice chatroom and KTV.
         * When to call: It needs to be called after [createEngine].
         * Restrictions: Virtual stereo effect is only effective for SDK captured sound.
         * Caution: You need to set up a dual channel with [setAudioConfig] for the virtual stereo to take effect.
         *
         * @param enable true to turn on the virtual stereo, false to turn off the virtual stereo.
         * @param angle The angle of the sound source in virtual stereo in the range of -1 ~ 360, with 90 being directly in front, 0 / 180 / 270 corresponding to the rightmost and leftmost respectively. In particular, when set to -1, it is all round virtual stereo effects.
         */
        public abstract void EnableVirtualStereo(bool enable, int angle);

        /**
         * Enable or disable the virtual stereo effect when playing stream.
         *
         * Available since: 2.8.0
         * Description: Call this function to enable/disable the virtual stereo effect when playing stream.
         * Use cases: Often used in live broadcasting, voice chatroom and KTV.
         * When to call: It needs to be called after [createEngine].
         * Caution: It can dynamically switch and set angle parameters before and after playing stream. After stopping playing stream, it will automatically reset and disable.
         *
         * @param enable true to turn on the virtual stereo, false to turn off the virtual stereo.
         * @param angle The angle of the sound source in virtual stereo in the range of 0 ~ 360, with 90 being directly in front, 0 / 180 / 270 corresponding to the rightmost and leftmost respectively.
         * @param streamID Stream ID.
         */
        public abstract void EnablePlayStreamVirtualStereo(bool enable, int angle, string streamID);

        /**
         * Sends a Broadcast Message.
         *
         * Available since: 1.2.1
         * Description: Send a broadcast message to the room, users who have entered the same room can receive the message, and the message is reliable.
         * Use cases: Generally used when the number of people in the live room does not exceed 500.
         * When to call: After calling [loginRoom] to log in to the room.
         * Restrictions: It is not supported when the number of people online in the room exceeds 500. If you need to increase the limit, please contact ZEGO technical support to apply for evaluation. The frequency of sending broadcast messages in the same room cannot be higher than 10 messages/s. The maximum QPS for a single user calling this interface from the client side is 2. For restrictions on the use of this function, please contact ZEGO technical support.
         * Related callbacks: The room broadcast message can be received through [onIMRecvBroadcastMessage].
         * Related APIs: Barrage messages can be sent through the [sendBarrageMessage] function, and custom command can be sent through the [sendCustomCommand] function.
         *
         * @param roomID Room ID. Required: Yes. Value range: The maximum length is 128 bytes. Caution: The room ID is in string format and only supports numbers, English characters and'~','!','@','#','$','%','^','&', ' *','(',')','_','+','=','-','`',';',''',',','.','<' ,'>','/','\'.
         * @param message The content of the message. Required: Yes. Value range: The length does not exceed 1024 bytes.
         * @param onIMSendBroadcastMessageResult Send a notification of the result of a broadcast message. Required: No. Caution: Passing [null] means not receiving callback notifications.
         */
        public abstract void SendBroadcastMessage(string roomID, string message, OnIMSendBroadcastMessageResult onIMSendBroadcastMessageResult);

        /**
         * Sends a Barrage Message (bullet screen) to all users in the same room, without guaranteeing the delivery.
         *
         * Available since: 1.5.0
         * Description: Send a barrage message to the room, users who have logged in to the same room can receive the message, the message is unreliable.
         * Use cases: Generally used in scenarios where there is a large number of messages sent and received in the room and the reliability of the messages is not required, such as live barrage.
         * When to call: After calling [loginRoom] to log in to the room.
         * Restrictions: The frequency of sending barrage messages in the same room cannot be higher than 20 messages/s. For restrictions on the use of this function, please contact ZEGO technical support.
         * Related callbacks: The room barrage message can be received through [onIMRecvBarrageMessage].
         * Related APIs: Broadcast messages can be sent through the [sendBroadcastMessage] function, and custom command can be sent through the [sendCustomCommand] function.
         *
         * @param roomID Room ID. Required: Yes. Value range: The maximum length is 128 bytes. Caution: The room ID is in string format and only supports numbers, English characters and'~','!','@','#','$','%','^','&', ' *','(',')','_','+','=','-','`',';',''',',','.','<' ,'>','/','\'.
         * @param message The content of the message. Required: Yes. Value range: The length does not exceed 1024 bytes.
         * @param onIMSendBarrageMessageResult Send barrage message result callback.Required: No. Caution: Passing [null] means not receiving callback notifications.
         */
        public abstract void SendBarrageMessage(string roomID, string message, OnIMSendBarrageMessageResult onIMSendBarrageMessageResult);

        /**
         * Sends a Custom Command to the specified users in the same room.
         *
         * Available since: 1.2.1
         * Description: After calling this function, users in the same room who have entered the room can receive the message, the message is unreliable.
         * Use cases: Generally used in scenarios where there is a large number of messages sent and received in the room and the reliability of the messages is not required, such as live barrage.
         * When to call: After calling [loginRoom] to log in to the room.
         * Restrictions: Generally used when the number of people in the live room does not exceed 500.The frequency of sending barrage messages in the same room cannot be higher than 20 messages/s. For restrictions on the use of this function, please contact ZEGO technical support.
         * Related callbacks: The room custom command can be received through [onIMRecvCustomCommand].
         * Related APIs: Broadcast messages can be sent through the [sendBroadcastMessage] function, and barrage messages can be sent through the [sendBarrageMessage] function.
         * Privacy reminder: Please do not fill in sensitive user information in this interface, including but not limited to mobile phone number, ID number, passport number, real name, etc.
         *
         * @param roomID Room ID. Required: Yes. Value range: The maximum length is 128 bytes. Caution: The room ID is in string format and only supports numbers, English characters and'~','!','@','#','$','%','^','&', ' *','(',')','_','+','=','-','`',';',''',',','.','<' ,'>','/','\'.
         * @param command Custom command content. Required: Yes. Value range: The maximum length is 1024 bytes. Caution: To protect privacy, please do not fill in sensitive user information in this interface, including but not limited to mobile phone number, ID number, passport number, real name, etc.
         * @param toUserList List of recipients of signaling. Required: Yes. Value range: user list or [null]. Caution: When it is [null], the SDK will send custom signaling back to all users in the room
         * @param onIMSendCustomCommandResult Send a notification of the signaling result. Required: No. Caution: Passing [null] means not receiving callback notifications.
         */
        public abstract void SendCustomCommand(string roomID, string command, List<ZegoUser> toUserList, OnIMSendCustomCommandResult onIMSendCustomCommandResult);

        public OnIMRecvBroadcastMessage onIMRecvBroadcastMessage;

        public OnIMRecvBarrageMessage onIMRecvBarrageMessage;

        public OnIMRecvCustomCommand onIMRecvCustomCommand;

        /**
         * Creates a media player instance.
         *
         * Available since: 2.1.0
         * Description: Creates a media player instance.
         * Use case: It is often used to play media resource scenes, For example, play video files, push the video of media resources in combination with custom video acquisition, and the remote end can pull the stream for viewing.
         * When to call: It can be called after the SDK by [createEngine] has been initialized.
         * Restrictions: Currently, a maximum of 4 instances can be created, after which it will return null.
         * Caution: The more instances of a media player, the greater the performance overhead on the device.
         * Related APIs: User can call [destroyMediaPlayer] function to destroy a media player instance.
         *
         * @return Media player instance, null will be returned when the maximum number is exceeded.
         */
        public abstract ZegoMediaPlayer CreateMediaPlayer();

        /**
         * Destroys a media player instance.
         *
         * Available since: 2.1.0
         * Description: Destroys a media player instance.
         * Related APIs: User can call [createMediaPlayer] function to create a media player instance.
         *
         * @param mediaPlayer The media player instance object to be destroyed.
         */
        public abstract void DestroyMediaPlayer(ZegoMediaPlayer mediaPlayer);

        /**
         * Creates a audio effect player instance.
         *
         * Available since: 1.16.0
         * Description: Creates a audio effect player instance.
         * Use cases: When you need to play short sound effects, such as applause, cheers, etc., you can use audioEffectPlayer to achieve.
         * When to call: It can be called after [createEngine].
         * Restrictions: Currently, a maximum of 1 instances can be created, after which it will return null.
         * Related APIs: [destroyAudioEffectPlayer].
         *
         * @return audio effect player instance, null will be returned when the maximum number is exceeded.
         */
        public abstract ZegoAudioEffectPlayer CreateAudioEffectPlayer();

        /**
         * Destroys a audio effect player instance.
         *
         * Available since: 1.16.0
         * Description: Destroys the specified audio effect player instance.
         * When to call: It can be called after [createAudioEffectPlayer].
         * Restrictions: None.
         * Related APIs: [createAudioEffectPlayer].
         *
         * @param audioEffectPlayer The audio effect player instance object to be destroyed.
         */
        public abstract void DestroyAudioEffectPlayer(ZegoAudioEffectPlayer audioEffectPlayer);

        /**
         * Starts to record and directly save the data to a file.
         *
         * Available since: 1.10.0
         * Description: Starts to record locally captured audio or video and directly save the data to a file, The recorded data will be the same as the data publishing through the specified channel.
         * When to call: This function needs to be called after the success of [startPreview] or [startPublishingStream] to be effective.
         * Restrictions: None.
         * Caution: Developers should not [stopPreview] or [stopPublishingStream] during recording, otherwise the SDK will end the current recording task. The data of the media player needs to be mixed into the publishing stream to be recorded.
         * Related callbacks: Developers will receive the [onCapturedDataRecordStateUpdate] and the [onCapturedDataRecordProgressUpdate] callback after start recording.
         *
         * @param config Record config.
         * @param channel Publishing stream channel.
         */
        public abstract void StartRecordingCapturedData(ZegoDataRecordConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Stops recording locally captured audio or video.
         *
         * Available since: 1.10.0
         * Description: Stops recording locally captured audio or video.
         * When to call: After [startRecordingCapturedData].
         * Restrictions: None.
         *
         * @param channel Publishing stream channel.
         */
        public abstract void StopRecordingCapturedData(ZegoPublishChannel channel = ZegoPublishChannel.Main);

        public OnCapturedDataRecordStateUpdate onCapturedDataRecordStateUpdate;

        public OnCapturedDataRecordProgressUpdate onCapturedDataRecordProgressUpdate;

        /**
         * Obtain synchronization network time information.
         *
         * Available since: 2.9.0
         * Description: Obtain synchronization network time(NTP), including timestamp and maximum deviation.
         * Use cases: When performing multi-terminal synchronization behaviors, network time synchronization is required.
         * When to call: It needs to be called after [createEngine].
         * Restrictions: None.
         */
        public abstract ZegoNetworkTimeInfo GetNetworkTimeInfo();

        public OnRecvExperimentalAPI onRecvExperimentalAPI;

        public OnNetworkQuality onNetworkQuality;

        /**
         * Enables or disables custom video capture (for the specified channel).
         *
         * Available since: 1.9.0
         * Description: If the value of enable is true, the video collection function is enabled. If the value of enable is false, the video collection function is disabled.
         * Use case: The App developed by the developer uses the beauty SDK of a third-party beauty manufacturer to broadcast non-camera collected data.
         * Default value: When this function is not called, custom video collection is disabled by default.
         * When to call: After [createEngine], call [startPreview], [startPublishingStream], [createRealTimeSequentialDataManager], and call [logoutRoom] to modify the configuration.
         * Caution: Custom video rendering can be used in conjunction with custom video capture, but when both are enabled, the local capture frame callback for custom video rendering will no longer be triggered, and the developer should directly capture the captured video frame from the custom video capture source.
         * Related callbacks: When developers to open a custom collection, by calling [setCustomVideoCaptureHandler] can be set up to receive a custom collection start-stop event notification.
         *
         * @param enable enable or disable
         * @param config custom video capture config
         * @param channel publish channel
         */
        public abstract void EnableCustomVideoCapture(bool enable, ZegoCustomVideoCaptureConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sends the video frames (Raw Data) produced by custom video capture to the SDK (for the specified channel).
         *
         * Available since: 1.9.0
         * Description: Sends customized raw video frame data to the SDK.
         * When to call: After receiving the [onStart] notification, the developer starts the call after the collection logic starts and ends the call after the [onStop] notification.
         * Caution: This interface must be called with [enableCustomVideoCapture] passing the parameter type RAW_DATA.
         * Related APIs: [enableCustomVideoCapture], [setCustomVideoCaptureHandler].
         *
         * @param data video frame data
         * @param dataLength video frame data length
         * @param param video frame param
         * @param referenceTimeMillisecond video frame reference time, UNIX timestamp, in milliseconds.
         * @param channel Publishing stream channel
         */
        public abstract void SendCustomVideoCaptureRawData(byte[] data, uint dataLength, ZegoVideoFrameParam param, ulong referenceTimeMillisecond, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Sends the video frames (Raw Data) produced by custom video capture to the SDK (for the specified channel).
         *
         * Available since: 1.9.0
         * Description: Sends customized raw video frame data to the SDK.
         * When to call: After receiving the [onStart] notification, the developer starts the call after the collection logic starts and ends the call after the [onStop] notification.
         * Caution: This interface must be called with [enableCustomVideoCapture] passing the parameter type RAW_DATA.
         * Related APIs: [enableCustomVideoCapture], [setCustomVideoCaptureHandler].
         *
         * @param data video frame data
         * @param dataLength video frame data length
         * @param param video frame param
         * @param referenceTimeMillisecond video frame reference time, UNIX timestamp, in milliseconds.
         * @param channel Publishing stream channel
         */
        public abstract void SendCustomVideoCaptureRawData(IntPtr data, uint dataLength, ZegoVideoFrameParam param, ulong referenceTimeMillisecond, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        public OnCustomVideoCaptureStart onCustomVideoCaptureStart;

        public OnCustomVideoCaptureStop onCustomVideoCaptureStop;

        /**
         * Enables or disables custom video processing.
         *
         * Available since: 2.2.0 (Android/iOS/macOS native), 2.4.0 (Windows/macOS C++).
         * Description: When the developer opens custom pre-processing, by calling [setCustomVideoProcessHandler] you can set the custom video pre-processing callback.
         * Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
         * Default value: Off by default
         * When to call: Must be set before calling [startPreview], [startPublishingStream], [createRealTimeSequentialDataManager]. If you need to modify the configuration, please call [logoutRoom] to log out of the room first, otherwise it will not take effect.
         * Restrictions: None.
         * Related APIs: Call the [setCustomVideoProcessHandler] function to set the callback before custom video processing.
         *
         * @param enable enable or disable. Required: Yes.
         * @param config custom video processing configuration. Required: Yes.Caution: If NULL is passed, the platform default value is used.
         * @param channel Publishing stream channel.Required: No.Default value: Main publish channel.
         */
        public abstract void EnableCustomVideoProcessing(bool enable, ZegoCustomVideoProcessConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main);

#if UNITY_STANDALONE_WIN
        /**
         * Send the original video data after the pre-processing of the custom video to the SDK, and support other channels to push the stream.
         *
         * Available since: 2.4.0
         * Description: When the developer opens the custom pre-processing, by calling [setCustomVideoProcessHandler], you can set the custom video pre-processing callback to obtain the original video data.
         * Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
         * When to call: Must be called in the [onCapturedUnprocessedCVPixelbuffer] callback.
         * Restrictions: None.
         * Platform differences: Only valid on Windows platform.
         *
         * @param data Raw video data. RGB format data storage location is data[0], YUV format data storage location is Y component：data[0], U component：data[1], V component：data[2].
         * @param dataLength Raw video data length. RGB format data length storage location is dataLength[0], YUV format data storage location respectively Y component length：dataLength[0], U component length：dataLength[1], V component length：dataLength[2].
         * @param param video frame param.
         * @param referenceTimeMillisecond video frame reference time, UNIX timestamp, in milliseconds.
         * @param channel Publishing stream channel.Required: No.Default value: Main publish channel.
         */
        public abstract void SendCustomVideoProcessedRawData(ref IntPtr data, ref uint dataLength, ZegoVideoFrameParam param, ulong referenceTimeMillisecond, ZegoPublishChannel channel = ZegoPublishChannel.Main);
#endif

#if UNITY_STANDALONE_OSX || UNITY_IOS
        /**
         * Send the [CVPixelBuffer] type video data after the custom video processing to the SDK (for the specified channel).
         *
         * Available since: 2.2.0 (iOS native), 2.4.0 (macOS C++).
         * Description: When the custom video pre-processing is turned on, the [CVPixelBuffer] format video data after the custom video pre-processing is sent to the SDK, and other channels are supported.
         * Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
         * When to call: Must be called in the [onCapturedUnprocessedCVPixelbuffer] callback.
         * Restrictions: This interface takes effect when [enableCustomVideoProcessing] is called to enable custom video pre-processing and the bufferType of config is passed in [ZegoVideoBufferTypeCVPixelBuffer].
         * Platform differences: Only valid on Windows platform.
         *
         * @param buffer CVPixelBuffer type video frame data to be sent to the SDK.
         * @param timestamp Timestamp of this video frame.
         * @param channel Publishing stream channel.
         */
        public abstract void SendCustomVideoProcessedCVPixelBuffer(IntPtr buffer, ulong timestamp, ZegoPublishChannel channel = ZegoPublishChannel.Main);
#endif

#if UNITY_ANDROID
        /**
         * Send the [Texture] type video data after the pre-processing of the custom video to the SDK (for the specified channel).
         *
         * Available since: 2.2.0
         * Description: When the custom video pre-processing is turned on, the [Texture] format video data after the custom video pre-processing is sent to the SDK, and other channels are supported.
         * Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
         * When to call: Must be called in the [onCapturedUnprocessedTextureData] callback.
         * Restrictions: This interface takes effect when [enableCustomVideoProcessing] is called to enable custom video pre-processing and the bufferType of config is passed in [ZegoVideoBufferTypeGLTexture2D].
         * Platform differences: Only valid on Android platform.
         *
         * @param textureID texture ID.
         * @param width Texture width.
         * @param height Texture height.
         * @param referenceTimeMillisecond video frame reference time, UNIX timestamp, in milliseconds.
         * @param channel Publishing stream channel.
         */
        public abstract void SendCustomVideoProcessedTextureData(int textureID, int width, int height, ulong referenceTimeMillisecond, ZegoPublishChannel channel = ZegoPublishChannel.Main);
#endif

#if UNITY_STANDALONE_WIN
        public OnCapturedUnprocessedRawData onCapturedUnprocessedRawData;
#endif

#if UNITY_IOS || UNITY_STANDALONE_OSX
        public OnCapturedUnprocessedCVPixelBuffer onCapturedUnprocessedCVPixelBuffer;
#endif

#if UNITY_ANDROID
        public OnCapturedUnprocessedTextureData onCapturedUnprocessedTextureData;
#endif

        /**
         * Enables the custom audio I/O function (for the specified channel), support PCM, AAC format data.
         *
         * Available since: 1.10.0
         * Description: Enable custom audio IO function, support PCM, AAC format data.
         * Use cases: If the developer wants to implement special functions (such as voice change, bel canto, etc.) through custom processing after the audio data is collected or before the remote audio data is drawn for rendering.
         * When to call: It needs to be called before [startPublishingStream], [startPlayingStream], [startPreview], [createMediaPlayer], [createAudioEffectPlayer] and [createRealTimeSequentialDataManager] to be effective.
         * Restrictions: None.
         *
         * @param enable Whether to enable custom audio IO, default is false.
         * @param config Custom audio IO config.
         * @param channel Specify the publish channel to enable custom audio IO.
         */
        public abstract void EnableCustomAudioIO(bool enable, ZegoCustomAudioConfig config, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        public OnCapturedAudioData onCapturedAudioData;

        public OnPlaybackAudioData onPlaybackAudioData;

        public OnMixedAudioData onMixedAudioData;

        public OnPlayerAudioData onPlayerAudioData;

        /**
         * Creates a range audio instance.
         *
         * Available since: 2.11.0
         * Description: Creates a range audio instance.
         * Use case: Often used in game voice scenarios, users can use related functions by creating range audio instance objects.
         * When to call: It can be called after the engine by [createEngine] has been initialized.
         * Restrictions: Currently, a maximum of 1 instances can be created, after which it will return null.
         * Impacts on other APIs: If you use the range audio module, you cannot use the basic push-pull stream [startPublishingStream], [startPlayingStream] interfaces and related callbacks.
         *
         * @return range audio instance, null will be returned when the maximum number is exceeded.
         */
        public abstract ZegoRangeAudio CreateRangeAudio();

        /**
         * Destroys a range audio instance.
         *
         * Available since: 2.11.0
         * Description: Destroys a range audio instance.
         *
         * @param rangeAudio The range audio instance object to be destroyed.
         */
        public abstract void DestroyRangeAudio(ZegoRangeAudio rangeAudio);

        /**
         * Creates a copyrighted music instance.
         *
         * Available since: 2.13.0
         * Description: Creates a copyrighted music instance.
         * Use case: Often used in online KTV chorus scenarios, users can use related functions by creating copyrighted music instance objects.
         * When to call: It can be called after the engine by [createEngine] has been initialized.
         * Restrictions: The SDK only supports the creation of one instance of CopyrightedMusic. Multiple calls to this function return the same object.
         *
         * @return copyrighted music instance, multiple calls to this function return the same object.
         */
        public abstract ZegoCopyrightedMusic CreateCopyrightedMusic();

        /**
         * Destroys a copyrighted music instance.
         *
         * Available since: 2.13.0
         * Description: Destroys a copyrighted music instance.
         * When to call: It can be called before the engine by [destroyEngine]
         *
         * @param copyrightedMusic The copyrighted music instance object to be destroyed.
         */
        public abstract void DestroyCopyrightedMusic(ZegoCopyrightedMusic copyrightedMusic);

        /**
         * Enables or disables the beauty features for the specified publish channel.
         *
         * Available since: 1.1.0
         * Description: When developers do not have much need for beauty features, they can use this function to set some very simple beauty effects.
         * When to call: It needs to be called after [createEngine].
         * Default value: When this function is not called, the beauty feature is not enabled by default.
         * Related APIs: After turning on the beauty features, you can call the [setBeautifyOption] function to adjust the beauty parameters.
         * Caution: This beauty feature is very simple and may not meet the developer’s expectations. It is recommended to use the custom video processing function [enableCustomVideoProcessing] or the custom video capture function [enableCustomVideoCapture] to connect the [ZegoEffects] AI SDK https://docs.zegocloud.com/article/9896 for best results.
         * Restrictions: In the case of using the custom video capture function, since the developer has handle the video data capturing, the SDK is no longer responsible for the video data capturing, so this function is no longer valid. It is also invalid when using the custom video processing function.
         *
         * @deprecated Deprecated since 2.16.0, please use the [enableEffectsBeauty] function instead.
         * @param featureBitmask Beauty features, bitmask format, you can choose to enable several features in [ZegoBeautifyFeature] at the same time
         * @param channel Publishing stream channel
         */
        [Obsolete("Deprecated since 2.16.0, please use the [enableEffectsBeauty] function instead.",false)]
        public abstract void EnableBeautify(int featureBitmask, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * Set beautify option.
         *
         * Available since: 1.1.0
         * Description: set beautify option for specified stream publish channel.
         * Use cases: Often used in video call, live broadcasting.
         * When to call: It needs to be called after [createEngine].
         * Restrictions: None.
         * Caution: In the case of using a custom video capture function, because the developer has taken over the video data capturing, the SDK is no longer responsible for the video data capturing, call this function will not take effect. When using custom video processing, the video data collected by the SDK will be handed over to the business for further processing, call this function will not take effect either.
         *
         * @deprecated Deprecated since 2.16.0, please use the [setEffectsBeautyParam] function instead.
         * @param option Beautify option.
         * @param channel stream publish channel.
         */
        [Obsolete("Deprecated since 2.16.0, please use the [setEffectsBeautyParam] function instead.",false)]
        public abstract void SetBeautifyOption(ZegoBeautifyOption option, ZegoPublishChannel channel = ZegoPublishChannel.Main);

        /**
         * [Deprecated] Create ZegoExpressEngine singleton object and initialize SDK.
         *
         * Available: 1.1.0 ~ 2.13.1, deprecated since 2.14.0, please use the method with the same name without [isTestEnv] parameter instead
         * Description: Create ZegoExpressEngine singleton object and initialize SDK.
         * When to call: The engine needs to be created before calling other functions.
         * Restrictions: None.
         * Caution: The SDK only supports the creation of one instance of ZegoExpressEngine. Multiple calls to this function return the same object.
         *
         * @deprecated Deprecated since 2.14.0, please use the method with the same name without [isTestEnv] parameter instead.
         * @param appID Application ID issued by ZEGO for developers, please apply from the ZEGO Admin Console https://console.zegocloud.com The value ranges from 0 to 4294967295.
         * @param appSign Application signature for each AppID, please apply from the ZEGO Admin Console. Application signature is a 64 character string. Each character has a range of '0' ~ '9', 'a' ~ 'z'. AppSign 2.17.0 and later allows null or no transmission. If the token is passed empty or not passed, the token must be entered in the [ZegoRoomConfig] parameter for authentication when the [loginRoom] interface is called to login to the room.
         * @param isTestEnv Choose to use a test environment or a formal commercial environment, the formal environment needs to submit work order configuration in the ZEGO management console. The test environment is for test development, with a limit of 10 rooms and 50 users. Official environment App is officially launched. ZEGO will provide corresponding server resources according to the configuration records submitted by the developer in the management console. The test environment and the official environment are two sets of environments and cannot be interconnected.
         * @param scenario The application scenario. Developers can choose one of ZegoScenario based on the scenario of the app they are developing, and the engine will preset a more general setting for specific scenarios based on the set scenario. After setting specific scenarios, developers can still call specific functions to set specific parameters if they have customized parameter settings.
         * @return Engine singleton instance.
         */
        [Obsolete("Deprecated since 2.14.0, please use the method with the same name without [isTestEnv] parameter instead.",false)]
        public static ZegoExpressEngine CreateEngine(uint appID, string appSign, bool isTestEnv, ZegoScenario scenario)
        {
            return ZegoExpressEngineImpl.CreateEngine(appID, appSign, isTestEnv, scenario);
        } 

        /**
         * [Deprecated] Turns on/off verbose debugging and sets up the log language. This function is deprecated in version 2.3.0, please use [enableDebugAssistant] to achieve the original function.
         *
         * This feature is disabled by default and the language of debug information is English.
         *
         * @deprecated This function is deprecated in version 2.3.0, please use [enableDebugAssistant] to achieve the original function.
         * @param enable Detailed debugging information switch
         * @param language Debugging information language. Note that Chinese is deprecated, if you need Chinese info, please refer to the document https://docs.zegocloud.com/en/5548.html
         */
        [Obsolete("This function is deprecated in version 2.3.0, please use [enableDebugAssistant] to achieve the original function.",false)]
        public abstract void SetDebugVerbose(bool enable, ZegoLanguage language);

        /**
         * [Deprecated] Logs in multi room. This method has been deprecated after version 2.9.0 If you want to access the multi-room feature, Please set [setRoomMode] to select multi-room mode before the engine started, and then call [loginRoom] to use multi-room. If you call [loginRoom] function to log in to multiple rooms, please make sure to pass in the same user information.
         *
         * This method has been deprecated after version 2.9.0 If you want to access the multi-room feature, Please set [setRoomMode] to select multi-room mode before the engine started, and then call [loginRoom] to use multi-room. If you call [loginRoom] function to log in to multiple rooms, please make sure to pass in the same user information.
         * You must log in the main room with [loginRoom] before invoke this function to logging in to multi room.
         * Currently supports logging into 1 main room and 1 multi room at the same time.
         * When logging out, you must log out of the multi room before logging out of the main room.
         * User can only publish the stream in the main room, but can play the stream in the main room and multi room at the same time, and can receive the signaling and callback in each room.
         * The advantage of multi room is that you can login another room without leaving the current room, receive signaling and callback from another room, and play streams from another room.
         * To prevent the app from being impersonated by a malicious user, you can add authentication before logging in to the room, that is, the [token] parameter in the ZegoRoomConfig object passed in by the [config] parameter.
         * Different users who log in to the same room can get room related notifications in the same room (eg [onRoomUserUpdate], [onRoomStreamUpdate], etc.), and users in one room cannot receive room signaling notifications in another room.
         * Messages sent in one room (e.g. [setStreamExtraInfo], [sendBroadcastMessage], [sendBarrageMessage], [sendCustomCommand], etc.) cannot be received callback ((eg [onRoomStreamExtraInfoUpdate], [onIMRecvBroadcastMessage], [onIMRecvBarrageMessage], [onIMRecvCustomCommand], etc) in other rooms. Currently, SDK does not provide the ability to send messages across rooms. Developers can integrate the SDK of third-party IM to achieve.
         * SDK supports startPlayingStream audio and video streams from different rooms under the same appID, that is, startPlayingStream audio and video streams across rooms. Since ZegoExpressEngine's room related callback notifications are based on the same room, when developers want to startPlayingStream streams across rooms, developers need to maintain related messages and signaling notifications by themselves.
         * If the network is temporarily interrupted due to network quality reasons, the SDK will automatically reconnect internally. You can get the current connection status of the local room by listening to the [onRoomStateUpdate] callback method, and other users in the same room will receive [onRoomUserUpdate] callback notification.
         * It is strongly recommended that userID corresponds to the user ID of the business APP, that is, a userID and a real user are fixed and unique, and should not be passed to the SDK in a random userID. Because the unique and fixed userID allows ZEGO technicians to quickly locate online problems.
         *
         * @deprecated This method has been deprecated after version 2.9.0 If you want to access the multi-room feature, Please set [setRoomMode] to select multi-room mode before the engine started, and then call [loginRoom] to use multi-room. If you call [loginRoom] function to log in to multiple rooms, please make sure to pass in the same user information.
         * @param roomID Room ID, a string of up to 128 bytes in length. Only support numbers, English characters and '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '-', '`', ';', '’', ',', '.', '<', '>', '/', '\'.
         * @param config Advanced room configuration.
         */
        [Obsolete("This method has been deprecated after version 2.9.0 If you want to access the multi-room feature, Please set [setRoomMode] to select multi-room mode before the engine started, and then call [loginRoom] to use multi-room. If you call [loginRoom] function to log in to multiple rooms, please make sure to pass in the same user information.",false)]
        public abstract void LoginMultiRoom(string roomID, ZegoRoomConfig config = null);

        /**
         * [Deprecated] Set the selected video layer of playing stream. This function has been deprecated since version 2.3.0, Please use [setPlayStreamVideoType] instead.
         *
         * Available: 1.19.0 to 2.3.0, deprecated.
         * This function has been deprecated since version 2.3.0, Please use [setPlayStreamVideoType] instead.
         * When the publisher has set the codecID to SVC through [setVideoConfig], the player can dynamically set whether to use the standard layer or the base layer (the resolution of the base layer is one-half of the standard layer)
         * Under normal circumstances, when the network is weak or the rendered UI form is small, you can choose to use the video that plays the base layer to save bandwidth.
         * It can be set before and after playing stream.
         *
         * @deprecated This function has been deprecated since version 2.3.0, Please use [setPlayStreamVideoType] instead.
         * @param streamID Stream ID.
         * @param videoLayer Video layer of playing stream. AUTO by default.
         */
        [Obsolete("This function has been deprecated since version 2.3.0, Please use [setPlayStreamVideoType] instead.",false)]
        public abstract void SetPlayStreamVideoLayer(string streamID, ZegoPlayerVideoLayer videoLayer);

        /**
         * [Deprecated] Enables the callback for receiving audio data. This function has been deprecated since version 2.7.0, please use [startAudioDataObserver] and [stopAudioDataObserver] instead.
         *
         * This function has been deprecated since version 2.7.0, please use [startAudioDataObserver] and [stopAudioDataObserver] instead.
         * The callback to the corresponding setting of [setAudioDataHandler] is triggered when this function is called and at publishing stream state or playing stream state. If you want to enable the [onPlayerAudioData] callback, the sample rate passed in by calling the [enableAudioDataCallback] function does not support 8000Hz, 22050Hz and 24000Hz.
         *
         * @deprecated This function has been deprecated since version 2.7.0, please use [startAudioDataObserver] and [stopAudioDataObserver] instead.
         * @param enable Whether to enable audio data callback
         * @param callbackBitMask The callback function bitmask marker for receive audio data, refer to enum [ZegoAudioDataCallbackBitMask], when this param converted to binary, 0b01 that means 1 << 0 for triggering [onCapturedAudioData], 0x10 that means 1 << 1 for triggering [onPlaybackAudioData], 0x100 that means 1 << 2 for triggering [onMixedAudioData], 0x1000 that means 1 << 3 for triggering [onPlayerAudioData]. The masks can be combined to allow different callbacks to be triggered simultaneously.
         * @param param param of audio frame
         */
        [Obsolete("This function has been deprecated since version 2.7.0, please use [startAudioDataObserver] and [stopAudioDataObserver] instead.",false)]
        public abstract void EnableAudioDataCallback(bool enable, uint callbackBitMask, ZegoAudioFrameParam param);

        /**
         * [Deprecated] Whether to use the built-in speaker to play audio.This function has been deprecated since version 2.3.0 Please use [setAudioRouteToSpeaker] instead.
         *
         * This function has been deprecated since version 2.3.0 Please use [setAudioRouteToSpeaker] instead.
         * When you choose not to use the built-in speaker to play sound, that is, set to false, the SDK will select the currently highest priority audio output device to play the sound according to the system schedule
         *
         * @deprecated This function has been deprecated since version 2.3.0 Please use [setAudioRouteToSpeaker] instead.
         * @param enable Whether to use the built-in speaker to play sound, true: use the built-in speaker to play sound, false: use the highest priority audio output device scheduled by the current system to play sound
         */
        [Obsolete("This function has been deprecated since version 2.3.0 Please use [setAudioRouteToSpeaker] instead.",false)]
        public abstract void SetBuiltInSpeakerOn(bool enable);

        [Obsolete("Deprecated since 2.15.0, please use [onLocalDeviceExceptionOccurred] instead.",false)]
        public OnDeviceError onDeviceError;


        public abstract bool LoadCaptureVideoDataToTexture(ZegoPublishChannel channelIndex, Texture2D tid, ref int w, ref int h, ref ZegoVideoFrameFormat format);
        public abstract bool LoadPlayVideoDataToTexture(string streamId, Texture2D tid, ref int w, ref int h, ref ZegoVideoFrameFormat format);   
    }

}
