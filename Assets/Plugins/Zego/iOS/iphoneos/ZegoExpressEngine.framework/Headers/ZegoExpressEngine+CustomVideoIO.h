//
//  ZegoExpressEngine+CustomVideoIO.h
//  ZegoExpressEngine
//
//  Copyright © 2019 Zego. All rights reserved.
//

#import "ZegoExpressEngine.h"

NS_ASSUME_NONNULL_BEGIN

@interface ZegoExpressEngine (CustomVideoIO)

/// Enables or disables custom video rendering.
///
/// Available since: 1.9.0
/// Description: When enable is `YES`,video custom rendering is enabled; if the value of `NO`, video custom rendering is disabled.
/// Use case: Use beauty features or apps that use a cross-platform interface framework (for example, Qt requires a complex hierarchical interface to achieve high-experience interaction) or game engines (for example, Unity3D, Cocos2d-x), etc.
/// Default value: Custom video rendering is turned off by default when this function is not called.
/// When to call: Must be set after [createEngine] before the engine starts, before calling [startPreview], [startPublishingStream],[startPlayingStream], [createRealTimeSequentialDataManager].The configuration can only be modified after the engine is stopped, that is, after [logoutRoom] is called.
/// Caution: Custom video rendering can be used in conjunction with custom video capture, but when both are enabled, the local capture frame callback for custom video rendering will no longer be triggered, and the developer should directly capture the captured video frame from the custom video capture source.
/// Related callbacks: When developers to open a custom rendering, by calling [setCustomVideoRenderHandler] can be set up to receive local and remote video data to be used for custom rendering. [onCapturedVideoFrameRawData] local bare preview video frame data correction, distal pull flow [onRemoteVideoFrameRawData] naked video frame data correction.
///
/// @param enable enable or disable
/// @param config custom video render config
- (void)enableCustomVideoRender:(BOOL)enable config:(nullable ZegoCustomVideoRenderConfig *)config;

/// Sets up the event callback handler for custom video rendering.
///
/// Available since: 1.9.0
/// Description: Custom video rendering, where the developer is responsible for rendering the video data onto the UI components. With the change callback set, a callback that throws video data to the developer will be triggered when the developer calls startPreview, startPublishingStream, and startPlayingStream.
/// Use case: Use the beauty function, or the App uses a cross-platform interface framework (e.g. Qt requires a complex hierarchical interface to achieve high-experience interaction) or a game engine (e.g. Unity3D, Cocos2D-X).
/// When to call: after [createEngine].
/// Related callbacks: [onCapturedVideoFrameRawData] local bare preview video frame data correction, distal pull flow [onRemoteVideoFrameRawData] naked video frame data correction.
///
/// @param handler Custom video render handler
- (void)setCustomVideoRenderHandler:(nullable id<ZegoCustomVideoRenderHandler>)handler;

/// Enables or disables custom video capture.
///
/// Available since: 1.9.0
/// Description: If the value of enable is YES, the video collection function is enabled. If the value of enable is NO, the video collection function is disabled.
/// Use case: The App developed by the developer uses the beauty SDK of a third-party beauty manufacturer to broadcast non-camera collected data.
/// Default value: When this function is not called, custom video collection is disabled by default.
/// When to call: After [createEngine], call [startPreview], [startPublishingStream], [createRealTimeSequentialDataManager], and call [logoutRoom] to modify the configuration.
/// Caution: Custom video rendering can be used in conjunction with custom video capture, but when both are enabled, the local capture frame callback for custom video rendering will no longer be triggered, and the developer should directly capture the captured video frame from the custom video capture source.
/// Related callbacks: When developers to open a custom collection, by calling [setCustomVideoCaptureHandler] can be set up to receive a custom collection start-stop event notification.
///
/// @param enable enable or disable
/// @param config custom video capture config
- (void)enableCustomVideoCapture:(BOOL)enable config:(nullable ZegoCustomVideoCaptureConfig *)config;

/// Enables or disables custom video capture (for the specified channel).
///
/// Available since: 1.9.0
/// Description: If the value of enable is YES, the video collection function is enabled. If the value of enable is NO, the video collection function is disabled.
/// Use case: The App developed by the developer uses the beauty SDK of a third-party beauty manufacturer to broadcast non-camera collected data.
/// Default value: When this function is not called, custom video collection is disabled by default.
/// When to call: After [createEngine], call [startPreview], [startPublishingStream], [createRealTimeSequentialDataManager], and call [logoutRoom] to modify the configuration.
/// Caution: Custom video rendering can be used in conjunction with custom video capture, but when both are enabled, the local capture frame callback for custom video rendering will no longer be triggered, and the developer should directly capture the captured video frame from the custom video capture source.
/// Related callbacks: When developers to open a custom collection, by calling [setCustomVideoCaptureHandler] can be set up to receive a custom collection start-stop event notification.
///
/// @param enable enable or disable
/// @param config custom video capture config
/// @param channel publish channel
- (void)enableCustomVideoCapture:(BOOL)enable config:(nullable ZegoCustomVideoCaptureConfig *)config channel:(ZegoPublishChannel)channel;

/// Sets the event callback handler for custom video capture.
///
/// Available since: 1.9.0
/// Description: Example Set the callback for custom video capture. That is, the developer is responsible for collecting video data, and sends the collected video data to the SDK for coding and pushing the video data to the ZEGO RTC server.
/// Use case: This feature is typically used by developers using third-party beauty features or livestreaming of games.
/// When to call: After [createEngine] .
/// Caution: In the case of customized video collection, the SDK will not start the camera to collect video data, and developers need to collect video data from the video collection source. A callback notifying the developer that it can start sending video data is triggered when the developer calls [startPreview] to start pushing the stream to [startPublishingStream]. A callback notifying the developer that it can stop sending video data is triggered when [stopPreview] and [stopPublishingStream].
/// Related callbacks: [onStart] Starts collection, and [onStop] stops collection.
///
/// @param handler Custom video capture handler
- (void)setCustomVideoCaptureHandler:(nullable id<ZegoCustomVideoCaptureHandler>)handler;

/// Sends the video frames (Texture Data) produced by custom video capture to the SDK.
///
/// Available since: 1.9.0
/// Description: Send custom captured video frame type GL_TEXTURE_2D data to the SDK.
/// When to call: After receiving the [onStart] notification, the developer starts the call after the collection logic starts and ends the call after the [onStop] notification.
/// Caution: Calling this interface must be [enableCustomVideoCapture] with an argument of type GL_TEXTURE_2D.
/// Related APIs: [enableCustomVideoCapture], [setCustomVideoCaptureHandler].
///
/// @param textureID texture ID
/// @param size Video frame width and height
/// @param timestamp Timestamp of this video frame
- (void)sendCustomVideoCaptureTextureData:(GLuint)textureID size:(CGSize)size timestamp:(CMTime)timestamp;

/// Sends the video frames (Texture Data) produced by custom video capture to the SDK (for the specified channel).
///
/// Available since: 1.9.0
/// Description: Send custom captured video frame type GL_TEXTURE_2D data to the SDK.
/// When to call: After receiving the [onStart] notification, the developer starts the call after the collection logic starts and ends the call after the [onStop] notification.
/// Caution: Calling this interface must be [enableCustomVideoCapture] with an argument of type GL_TEXTURE_2D.
/// Related APIs: [enableCustomVideoCapture], [setCustomVideoCaptureHandler].
///
/// @param textureID texture ID
/// @param size Video frame width and height
/// @param timestamp Timestamp of this video frame
/// @param channel Publishing stream channel
- (void)sendCustomVideoCaptureTextureData:(GLuint)textureID size:(CGSize)size timestamp:(CMTime)timestamp channel:(ZegoPublishChannel)channel;

/// Sends the video frames (CVPixelBuffer) produced by custom video capture to the SDK.
///
/// Available since: 1.9.0
/// Description: Send custom captured video frame type CVPixelBuffer data to the SDK.
/// When to call: After receiving the [onStart] notification, the developer starts the call after the collection logic starts and ends the call after the [onStop] notification.
/// Caution: Calling this interface must be [enableCustomVideoCapture] with an argument of type ZegoVideoBufferTypeCVPixelBuffer.
/// Related APIs: [enableCustomVideoCapture], [setCustomVideoCaptureHandler].
///
/// @param buffer Video frame data to send to the SDK
/// @param timestamp Timestamp of this video frame
- (void)sendCustomVideoCapturePixelBuffer:(CVPixelBufferRef)buffer timestamp:(CMTime)timestamp;

/// Sends the video frames (CVPixelBuffer) produced by custom video capture to the SDK (for the specified channel).
///
/// Available since: 1.9.0
/// Description: Send custom captured video frame type CVPixelBuffer data to the SDK.
/// When to call: After receiving the [onStart] notification, the developer starts the call after the collection logic starts and ends the call after the [onStop] notification.
/// Caution: Calling this interface must be [enableCustomVideoCapture] with an argument of type ZegoVideoBufferTypeCVPixelBuffer.
/// Related APIs: [enableCustomVideoCapture], [setCustomVideoCaptureHandler].
///
/// @param buffer Video frame data to send to the SDK
/// @param timestamp Timestamp of this video frame
/// @param channel Publishing stream channel
- (void)sendCustomVideoCapturePixelBuffer:(CVPixelBufferRef)buffer timestamp:(CMTime)timestamp channel:(ZegoPublishChannel)channel;

/// Sends the video frames (Encoded Data) produced by custom video capture to the SDK.
///
/// Available since: 1.9.0
/// Description: Send custom captured video frame type CVPixelBuffer data to the SDK.
/// When to call: After receiving the [onStart] notification, the developer starts the call after the collection logic starts and ends the call after the [onStop] notification.
/// Caution: It is recommended to make a GOP every 2s, and each I frame must carry SPS and PPS and be placed at the top.  Only I and P frames are accepted, B frames are NOT accepted. Calling this interface must be [enableCustomVideoCapture] with an argument of type ZegoVideoBufferTypeEncodedData.
/// Related APIs: [enableCustomVideoCapture], [setCustomVideoCaptureHandler].
///
/// @param data video encoded frame data
/// @param params video encoded frame param
/// @param timestamp video frame reference time, UNIX timestamp.
- (void)sendCustomVideoCaptureEncodedData:(NSData *)data params:(ZegoVideoEncodedFrameParam *)params timestamp:(CMTime)timestamp;

/// Sends the video frames (Encoded Data) produced by custom video capture to the SDK (for the specified channel).
///
/// Available since: 1.9.0
/// Description: Send custom captured video frame type CVPixelBuffer data to the SDK.
/// When to call: After receiving the [onStart] notification, the developer starts the call after the collection logic starts and ends the call after the [onStop] notification.
/// Caution: It is recommended to make a GOP every 2s, and each I frame must carry SPS and PPS and be placed at the top.  Only I and P frames are accepted, B frames are NOT accepted. Calling this interface must be [enableCustomVideoCapture] with an argument of type ZegoVideoBufferTypeEncodedData.
/// Related APIs: [enableCustomVideoCapture], [setCustomVideoCaptureHandler].
///
/// @param data video frame encoded data
/// @param params video frame param
/// @param timestamp video frame reference time, UNIX timestamp.
/// @param channel Publishing stream channel
- (void)sendCustomVideoCaptureEncodedData:(NSData *)data params:(ZegoVideoEncodedFrameParam *)params timestamp:(CMTime)timestamp channel:(ZegoPublishChannel)channel;

/// Sets the video fill mode of custom video capture.
///
/// Available since: 1.10.0
/// Use cases: If the customized collection resolution and SDK coding resolution are inconsistent, THE SDK will process the video frame according to the fill mode and adjust it to the coding resolution.
/// Description: When the custom video collection function is enabled, you can use this interface to set the video zooming and filling mode.
/// When to call: Before [sendCustomVideoCaptureRawData], [sendCustomVideoCaptureTextureData], [sendCustomVideoCapturePixelBuffer], [sendCustomVideoCaptureEncodedData].
///
/// @param mode View mode
- (void)setCustomVideoCaptureFillMode:(ZegoViewMode)mode;

/// Sets the video fill mode of custom video capture (for the specified channel).
///
/// Available since: 1.10.0
/// Use cases: If the customized collection resolution and SDK coding resolution are inconsistent, THE SDK will process the video frame according to the fill mode and adjust it to the coding resolution.
/// Description: When the custom video collection function is enabled, you can use this interface to set the video zooming and filling mode.
/// When to call: Before [sendCustomVideoCaptureRawData], [sendCustomVideoCaptureTextureData], [sendCustomVideoCapturePixelBuffer], [sendCustomVideoCaptureEncodedData].
///
/// @param mode View mode
/// @param channel Publishing stream channel
- (void)setCustomVideoCaptureFillMode:(ZegoViewMode)mode channel:(ZegoPublishChannel)channel;

/// Sets the video flip mode of custom video capture (for the specified channel). This function takes effect only if the custom video buffer type is Texture2D.
///
/// Available since: 1.10.0
/// Description: When the custom video collection function is enabled, you can use this interface to set the video flipping mode.
/// When to call: Before [sendCustomVideoCaptureRawData], [sendCustomVideoCaptureTextureData], [sendCustomVideoCapturePixelBuffer], [sendCustomVideoCaptureEncodedData].
/// Caution: This function takes effect only if the custom collection type is Texture2D.
///
/// @param mode Flip mode
- (void)setCustomVideoCaptureFlipMode:(ZegoVideoFlipMode)mode;

/// Sets the video flip mode of custom video capture (for the specified channel). This function takes effect only if the custom video buffer type is Texture2D.
///
/// Available since: 1.10.0
/// Description: When the custom video collection function is enabled, you can use this interface to set the video flipping mode.
/// When to call: Before [sendCustomVideoCaptureRawData], [sendCustomVideoCaptureTextureData], [sendCustomVideoCapturePixelBuffer], [sendCustomVideoCaptureEncodedData].
/// Caution: This function takes effect only if the custom collection type is Texture2D.
///
/// @param mode Flip mode
/// @param channel Publishing stream channel
- (void)setCustomVideoCaptureFlipMode:(ZegoVideoFlipMode)mode channel:(ZegoPublishChannel)channel;

/// Sets the video rotation of custom video capture (for the specified channel).
///
/// Available since: 2.14.0.
/// Description: You can set the video rotation of custom video capture (for the specified channel) through this function.
/// When to call: After the callback [onStart] is received.
/// Restrictions: This function takes effect when the custom acquisition type is CVPixelBuffer or Texture2D.
///
/// @param rotation Clockwise angle.
/// @param channel Publishing stream channel
- (void)setCustomVideoCaptureRotation:(int)rotation channel:(ZegoPublishChannel)channel;

/// Sets device state of custom video capture (for the specified channel).
///
/// Available since: 2.15.0.
/// Description: User can set the video device state of custom video capture (for the specified channel) through this function.
/// When to call: After the callback [onStart] is received.
/// Restrictions: None.
/// Related Callbacks: The remote user that play stream of local user publishing stream can receive state update notification by monitoring [onRemoteCameraStateUpdate] callbacks.
///
/// @param isEnable Whether the device is enable.
/// @param state The device state.
/// @param channel Publishing stream channel
- (void)setCustomVideoCaptureDeviceState:(BOOL)isEnable state:(ZegoRemoteDeviceState)state channel:(ZegoPublishChannel)channel;

/// Set the region of interest (ROI) for encoder of custom video capture (for the specified channel).
///
/// Available since: 2.16.0.
/// Description: You can set the video encoder's region of interest rectangle areas of custom video capture (for the specified channel) through this function.
/// When to call: After the callback [onStart] is received.
/// Restrictions: Currently, only certain video encoders support this function, please contact ZEGO technical support before using it.
/// Caution: This function is currently an experimental feature, please contact ZEGO technical support before using it.
///
/// @param rectList ROI rectangle area list, currently supports up to 6 areas.
/// @param channel Publish channel
- (void)setCustomVideoCaptureRegionOfInterest:(NSArray<ZegoRoiRect *> *)rectList channel:(ZegoPublishChannel)channel;

/// Enables or disables custom video processing.
///
/// Available since: 2.2.0 (Android/iOS/macOS native), 2.4.0 (Windows/macOS C++).
/// Description: When the developer opens custom pre-processing, by calling [setCustomVideoProcessHandler] you can set the custom video pre-processing callback.
/// Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
/// Default value: Off by default
/// When to call: Must be set before calling [startPreview], [startPublishingStream], [createRealTimeSequentialDataManager]. If you need to modify the configuration, please call [logoutRoom] to log out of the room first, otherwise it will not take effect.
/// Restrictions: None.
/// Related APIs: Call the [setCustomVideoProcessHandler] function to set the callback before custom video processing.
///
/// @param enable enable or disable. Required: Yes.
/// @param config custom video processing configuration. Required: Yes.Caution: If nil is passed, the platform default value is used.
- (void)enableCustomVideoProcessing:(BOOL)enable config:(nullable ZegoCustomVideoProcessConfig *)config;

/// Enables or disables custom video processing.
///
/// Available since: 2.2.0 (Android/iOS/macOS native), 2.4.0 (Windows/macOS C++).
/// Description: When the developer opens custom pre-processing, by calling [setCustomVideoProcessHandler] you can set the custom video pre-processing callback.
/// Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
/// Default value: Off by default
/// When to call: Must be set before calling [startPreview], [startPublishingStream], [createRealTimeSequentialDataManager]. If you need to modify the configuration, please call [logoutRoom] to log out of the room first, otherwise it will not take effect.
/// Restrictions: None.
/// Related APIs: Call the [setCustomVideoProcessHandler] function to set the callback before custom video processing.
///
/// @param enable enable or disable. Required: Yes.
/// @param config custom video processing configuration. Required: Yes.Caution: If nil is passed, the platform default value is used.
/// @param channel Publishing stream channel.Required: No.Default value: Main publish channel.
- (void)enableCustomVideoProcessing:(BOOL)enable config:(nullable ZegoCustomVideoProcessConfig *)config channel:(ZegoPublishChannel)channel;

/// Sets up the event callback handler for custom video processing.
///
/// Available since: 2.2.0 (Android/iOS/macOS native), 2.4.0 (Windows/macOS C++).
/// Description: When the developer opens the custom pre-processing, by calling [setCustomVideoProcessHandler], you can set the custom video pre-processing callback to obtain the original video data.
/// Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
/// When to call: Called before [startPreview], [startPublishingStream] and [createRealTimeSequentialDataManager], otherwise it may cause the timing of obtaining video data to be too slow.
/// Restrictions: None.
/// Related APIs: Call [enableCustomVideoProcessing] function to enable custom video pre-processing callback.
///
/// @param handler Custom video process handler.Required: Yes.
- (void)setCustomVideoProcessHandler:(nullable id<ZegoCustomVideoProcessHandler>)handler;

/// Send the [CVPixelBuffer] type video data after the custom video processing to the SDK (for the specified channel).
///
/// Available since: 2.2.0 (iOS native), 2.4.0 (macOS C++).
/// Description: When the custom video pre-processing is turned on, the [CVPixelBuffer] format video data after the custom video pre-processing is sent to the SDK, and other channels are supported.
/// Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
/// When to call: Must be called in the [onCapturedUnprocessedCVPixelbuffer] callback.
/// Restrictions: This interface takes effect when [enableCustomVideoProcessing] is called to enable custom video pre-processing and the bufferType of config is passed in [ZegoVideoBufferTypeCVPixelBuffer].
/// Platform differences: Only valid on Windows platform.
///
/// @param buffer CVPixelBuffer type video frame data to be sent to the SDK.Required: Yes.
/// @param timestamp Timestamp of this video frame.Required: Yes.
- (void)sendCustomVideoProcessedCVPixelBuffer:(CVPixelBufferRef)buffer timestamp:(CMTime)timestamp;

/// Send the [CVPixelBuffer] type video data after the custom video processing to the SDK (for the specified channel).
///
/// Available since: 2.2.0 (iOS native), 2.4.0 (macOS C++).
/// Description: When the custom video pre-processing is turned on, the [CVPixelBuffer] format video data after the custom video pre-processing is sent to the SDK, and other channels are supported.
/// Use cases: After the developer collects the video data by himself or obtains the video data collected by the SDK, if the basic beauty and watermark functions of the SDK cannot meet the needs of the developer (for example, the beauty effect cannot meet the expectations), the ZegoEffects SDK can be used to perform the video Some special processing, such as beautifying, adding pendants, etc., this process is the pre-processing of custom video.
/// When to call: Must be called in the [onCapturedUnprocessedCVPixelbuffer] callback.
/// Restrictions: This interface takes effect when [enableCustomVideoProcessing] is called to enable custom video pre-processing and the bufferType of config is passed in [ZegoVideoBufferTypeCVPixelBuffer].
/// Platform differences: Only valid on Windows platform.
///
/// @param buffer CVPixelBuffer type video frame data to be sent to the SDK.
/// @param timestamp Timestamp of this video frame.
/// @param channel Publishing stream channel.
- (void)sendCustomVideoProcessedCVPixelBuffer:(CVPixelBufferRef)buffer timestamp:(CMTime)timestamp channel:(ZegoPublishChannel)channel;

@end

NS_ASSUME_NONNULL_END
