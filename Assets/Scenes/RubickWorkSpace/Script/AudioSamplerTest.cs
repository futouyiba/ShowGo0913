using UnityEngine;


namespace ET
{
    public class AudioSamplerTest : MonoBehaviour
    {
        private AudioSource audio;
        private float[] OutputData = new float[128];
        public float Volume;
        private float CheckMicroPhoneTimer = 0.5f;

        // Start is called before the first frame update
        void Start()
        {

            audio = GetComponents<AudioSource>()[0];

        }

        // Update is called once per frame
        

        void Update()
        {
            CheckMicroPhoneTimer -= Time.deltaTime;
            if (CheckMicroPhoneTimer <= 0)
            {
                GenerateImpulseBySpeakerVolume();
                CheckMicroPhoneTimer = 0.5f;
            }
        }

        

        private void GenerateImpulseBySpeakerVolume()
        {
            float sum = 0f;
            GetComponents<AudioSource>()[0].GetSpectrumData(OutputData, 0, FFTWindow.BlackmanHarris);
            foreach (var i in OutputData)
            {
                sum += i;
            }
            Volume = sum;


        }

        public float GetVolume()
        {
            return Volume;
        }
    }
}

