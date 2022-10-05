using UnityEngine;

namespace ZEGO
{
    public interface IVideoSource
    {
        bool LoadCaptureVideoDataToTexture(ZegoPublishChannel channelIndex, Texture2D tid, ref int w, ref int h,ref ZegoVideoFrameFormat format);//推流预览（渲染）
        bool LoadPlayVideoDataToTexture(string streamId, Texture2D tid, ref int w, ref int h,ref ZegoVideoFrameFormat format);//拉流渲染
    }
}
