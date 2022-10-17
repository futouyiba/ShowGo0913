using UnityEngine;
using UnityEngine.UI;

namespace ZEGO
{
    public class IVideoSurface<T> : MonoBehaviour where T : Component
    {
        private int width = 0;
        private int height = 0;

        private Texture2D nativeTextureId = null;
        private static bool preIsFrontCamera = true;
        ZegoVideoFrameFormat previewFormat = ZegoVideoFrameFormat.BGRA32;
        void Update()
        {
            if (videoSource != null)
            {
                if (isLocalVideo)
                {

                    int w = width;
                    int h = height;
                    if (!videoSource.LoadCaptureVideoDataToTexture(isAuxChannel ? ZegoPublishChannel.Aux : ZegoPublishChannel.Main, nativeTextureId, ref w, ref h,ref previewFormat))
                    {
                        if (w != width || h != height || (preIsFrontCamera != CameraInformation.isFrontCamera))
                        {
                            preIsFrontCamera = CameraInformation.isFrontCamera;
                            width = w;
                            height = h;
                            nativeTextureId = GetTexture2D(width,height,previewFormat);
                            var render = GetComponent<T>();
                            if (render is Renderer)
                            {
                                GetComponent<Renderer>().material.mainTexture = nativeTextureId;
                                if (CameraInformation.isFrontCamera == false)
                                {
                                    GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, -1);
                                }
                                else
                                {
                                    GetComponent<Renderer>().material.mainTextureScale = new Vector2(-1, -1);
                                }

                            }
                            else if (render is RawImage)
                            {
                                GetComponent<RawImage>().texture = nativeTextureId;
                                if (CameraInformation.isFrontCamera == true)
                                {                                
                                    GetComponent<RawImage>().uvRect = new Rect(1, 1, -1, -1);
                                }
                                else
                                {                                   
                                    GetComponent<RawImage>().uvRect = new Rect(0, 0, 1, -1);
                                }
                            }
                        }
                    }
                    else
                    {
                        nativeTextureId.Apply();
                    }

                }
                else
                {
                    if (playStreamId != "")
                    {
                        int w = width;
                        int h = height;
                        ZegoVideoFrameFormat format = ZegoVideoFrameFormat.BGRA32;
                        if (!videoSource.LoadPlayVideoDataToTexture(playStreamId, nativeTextureId, ref w, ref h,ref format))
                        {
                            if (w != width || h != height)
                            {
                                width = w;
                                height = h;
                                nativeTextureId = GetTexture2D(width, height, format);
                                var render = GetComponent<T>();

                                if (render is Renderer)
                                {
                                    GetComponent<Renderer>().material.mainTexture = nativeTextureId;
                                    GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, -1);
                                }
                                else if (render is RawImage)
                                {
                                    GetComponent<RawImage>().texture = nativeTextureId;
                                    GetComponent<RawImage>().uvRect = new Rect(0, 0, 1, -1);

                                }
                            }
                        }
                        else
                        {
                            nativeTextureId.Apply();
                        }
                    }
                }
            }
        }

        private Texture2D GetTexture2D(int width, int height, ZegoVideoFrameFormat format)
        {
            TextureFormat textureFormat = TextureFormat.BGRA32;
            if (format == ZegoVideoFrameFormat.RGBA32)
            {
                textureFormat = TextureFormat.RGBA32;
            }
            else if(format== ZegoVideoFrameFormat.ARGB32)
            {
                textureFormat = TextureFormat.ARGB32;
            }else if (format == ZegoVideoFrameFormat.BGRA32)
            {
                textureFormat = TextureFormat.BGRA32;
            }else if (format == ZegoVideoFrameFormat.ABGR32)
            {
                textureFormat = TextureFormat.RGBA32;
            }
            return new Texture2D((int)width, (int)height, textureFormat, false);
        }

        public void SetCaptureVideoInfo(bool isAuxChannel = false)
        {
            isLocalVideo = true;
            this.isAuxChannel = isAuxChannel;
        }

        public void SetPlayVideoInfo(string streamId)
        {
            isLocalVideo = false;
            playStreamId = streamId;
        }

        public void SetVideoSource(IVideoSource videoSrc)
        {
            videoSource = videoSrc;
        }

        IVideoSource videoSource = null;

        private bool isLocalVideo = true;

        private bool isAuxChannel = false;

        private string playStreamId = "";

        public void ResetSDKDefaultState()//恢复到SDK渲染视频图像时,默认对mainTexture/uvRect的操作状态
        {
            width = 0;
            height = 0;
        }
        public void ResetSDKNoDealState()//恢复到SDK不对mainTexture/uvRect操作的状态，默认mainTextureScale恢复到(1,1),uvRect恢复到（0，0，1，1）;若用户需要恢复的不是这个数值，可以参考该方法在外层自行设置具体值。
        {
            var render = GetComponent<T>();

            if (render is Renderer)
            {
                GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, 1);
            }
            else if (render is RawImage)
            {
                GetComponent<RawImage>().uvRect = new Rect(0, 0, 1, 1);

            }
        }
    }

    public class RendererVideoSurface : IVideoSurface<Renderer> { }
    public class RawImageVideoSurface : IVideoSurface<RawImage> { }

}
