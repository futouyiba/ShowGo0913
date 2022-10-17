using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ET
{
    public class ScaleReaction : MonoBehaviour
    {
        public float Gain;
        public float Duration;
        public List<Transform > Targets;
        public bool Punching = false;

        public bool SelfPunching = false;
        public float ReactGain;
        public bool Beat = false;

        public float SelfRotateVelocity=5f;
        public bool Boosting = false;
        private float BoostingTime = 0.8f;

        public int RotateDirection=1;

        public bool isSpeaker=false;

        void Start()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Targets.Add(this.transform.GetChild(i));
            }

        }

        void Update()
        {
            SelfScaleReact();
            if(!isSpeaker)SelfRotate(SelfRotateVelocity);//���첻ת 
        }

        public void SelfRotate(float velocity)
        {
            transform.rotation *= Quaternion.Euler(0f, velocity * Time.deltaTime* RotateDirection, 0f);
        }

        public void SelfRotateBoost()
        {
            if (!Boosting) { Boosting = true; SelfRotateVelocity *= 2f; Invoke("SelfRotateBoostEnded", BoostingTime); }
            else { SelfRotateVelocity *= 1.5f; }
        }

        private void SelfRotateBoostEnded()
        {
            SelfRotateVelocity = 5f;
            Boosting = false;
        }

        private void SelfScaleReact()
        {
            float v = AudioVisualizer.AudioSampler.instance.GetVolume();
            if (v <= 0) { return; }
            if (!SelfPunching)
            {
                SelfPunching = true;
                foreach (Transform t in Targets)
                {
                    if (!Beat)
                    {
                        if (v > 0.1f) v = 2f;
                        else if (v < 0.04f) v = 1f;
                        else v = 1f + v * 10f;
                        Vector3 ogscale=t.localScale;
                        if (!isSpeaker) t.transform.DOPunchScale(ogscale*-Gain*v, Duration, 12, 0).OnKill(() => { SelfPunching = false; });
                        else t.transform.DOPunchScale(ogscale * Gain * v, Duration, 6, 0).OnKill(() => { SelfPunching = false; });
                    }
                    else
                    {
                        if (v > 0.1f) v = 2f;
                        else if (v < 0.04f) v = 1f;
                        else v = 1f + v * 10f;
                        Vector3 ogscale = t.localScale;
                        if (!isSpeaker) t.transform.DOPunchScale(ogscale * -ReactGain * v, Duration, 12, 0.2f).OnKill(() => { SelfPunching = false; });
                        else t.transform.DOPunchScale(ogscale * ReactGain * v, Duration, 6, 0.2f).OnKill(() => { SelfPunching = false; });
                    }
                }
                Beat = false;
            }

        }
        public void OnBeat()
        {
            Beat = true;
        }
    }
}
