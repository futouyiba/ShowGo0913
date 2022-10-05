using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace ZEGO
{
    public class VideoDataRingBuffer
    {
        int cur_width_ = 0;
        int cur_height_ = 0;
        int pending_count_ = 0;

        int write_index_ = 0;
        int read_index_ = 0;
        ZegoVideoFrameFormat cur_format = ZegoVideoFrameFormat.BGRA32;
        const int MAX_BUFFER_COUNT = 2;

        private byte[][] video_data_buf_list_ = new byte[MAX_BUFFER_COUNT][];

        void ResetBuffer()
        {
            for (int i = 0; i < MAX_BUFFER_COUNT; i++)
            {
                video_data_buf_list_[i] = new byte[cur_width_ * cur_height_ * 4];
            }
            Interlocked.Exchange(ref write_index_, 0);
            Interlocked.Exchange(ref read_index_, 0);
            Interlocked.Exchange(ref pending_count_, 0);
        }

        public bool WriteBuffer(IntPtr ptr, int w, int h, int [] strides,ZegoVideoFrameFormat format)
        {
            
            if (cur_width_ != w || cur_height_ != h)
            {
                cur_width_ = w;
                cur_height_ = h;
                cur_format = format;
                ResetBuffer();
            }

            for(int i = 0;i < h; ++i)
            {
                Marshal.Copy(new IntPtr(ptr.ToInt64() + i*strides[0]), video_data_buf_list_[write_index_], i*w*4, w * 4);//w*4->强制内存对齐
            }

            Interlocked.Exchange(ref write_index_, Interlocked.Increment(ref write_index_) % MAX_BUFFER_COUNT);

            if (pending_count_ < MAX_BUFFER_COUNT)//仅当存储的数据数量小于设置的缓冲区长度，增加缓存数据，否则替换旧数据为最新的数据，并不会增加缓冲区占用的内存
            {
                Interlocked.Increment(ref pending_count_);
            }

            return true;

        }
        public bool WriteBuffer(byte[] data , int w, int h, int[] strides, ZegoVideoFrameFormat format)
        {

            if (cur_width_ != w || cur_height_ != h)
            {
                cur_width_ = w;
                cur_height_ = h;
                cur_format = format;
                ResetBuffer();
            }
            video_data_buf_list_[write_index_] = data;
            Interlocked.Exchange(ref write_index_, Interlocked.Increment(ref write_index_) % MAX_BUFFER_COUNT);

            if (pending_count_ < MAX_BUFFER_COUNT)//仅当存储的数据数量小于设置的缓冲区长度，增加缓存数据，否则替换旧数据为最新的数据，并不会增加缓冲区占用的内存
            {
                Interlocked.Increment(ref pending_count_);
            }

            return true;

        }

        public bool ReadBufferToTextureId(Texture2D tid, ref int w, ref int h,ref ZegoVideoFrameFormat format)
        {
            if (pending_count_ > 0)
            {
                if (w != cur_width_ || h != cur_height_)
                {
                    w = cur_width_;
                    h = cur_height_;
                    format = cur_format;
                    //减少这一帧延迟，丢失没关系
                    Interlocked.Exchange(ref read_index_, Interlocked.Increment(ref read_index_) % MAX_BUFFER_COUNT);

                    Interlocked.Decrement(ref pending_count_);

                    return false;
                }

                tid.LoadRawTextureData(video_data_buf_list_[read_index_]);

                Interlocked.Exchange(ref read_index_, Interlocked.Increment(ref read_index_) % MAX_BUFFER_COUNT);

                Interlocked.Decrement(ref pending_count_);

                return true;
            }
            return false;
        }

    }
}
