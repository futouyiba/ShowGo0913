using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace ET
{
    public class BeatImpulseCamera : MonoBehaviour
    {
        public CinemachineImpulseSource impulseSource;
        [SerializeField] protected bool bRespondEnergy = false;
        [SerializeField] protected bool bRespondKick = true;
        [SerializeField] protected bool bRespondSnare = true;
        [SerializeField] protected bool bRespondHitHat = true;

        //信号强度由strength和音量大小一同控制
        [SerializeField, Range(0.1f, 3f)] protected float strength = 1f;

        [SerializeField] private float AverageGain;

        private float[] OutputData = new float[128];



        // Start is called before the first frame update
        void Start()
        {

            impulseSource = GetComponent<CinemachineImpulseSource>();
            
            //todo : 检查这里是否需要，用到了BeatDetection
            /*
            GetComponent<BeatDetection>().CallBackFunction = (info =>
            {
                //Debug.Log($"beat detected, info is {info}");
                //Debug.Log($"beat detected, info is {info.messageInfo}");
                switch (info.messageInfo)
                {
                    case BeatDetection.EventType.Energy:
                        if (bRespondEnergy)
                        {
                            impulseSource.GenerateImpulse(new Vector3(0,0,-AverageGain * strength));
                        }
                        break;
                    case BeatDetection.EventType.Kick:
                        if (bRespondKick)
                        {
                            impulseSource.GenerateImpulse(new Vector3(0, 0, -AverageGain * strength));
                            Debug.Log($"Kick strength is {AverageGain}");
                        }
                        break;
                    case BeatDetection.EventType.Snare:
                        if (bRespondSnare)
                        {
                            impulseSource.GenerateImpulse(new Vector3(0, 0, -AverageGain * strength));
                        }
                        break;
                    case BeatDetection.EventType.HitHat:
                        if (bRespondHitHat)
                        {
                            impulseSource.GenerateImpulse(new Vector3(0, 0, -AverageGain * strength));
                        }
                        break;
                    
                }
            });
            */
        }




        // Update is called once per frame
        void FixedUpdate()
        {
            //获取当前的音量大小
            GetComponent<AudioSource>().GetSpectrumData(OutputData, 0, FFTWindow.BlackmanHarris);
            float sum = 0f;
            foreach (var i in OutputData)
            {
                sum += i;
            }
            AverageGain = sum;

        }
    }
}
