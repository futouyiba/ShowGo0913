using System.Collections.Generic;
using UnityEngine;


namespace AudioVisualizer
{
    /// <summary>
    /// Samples the audio across multiple audio sources.
    /// All other AudioVisualizer classes rely on this class to sample the audio data.
    /// There should just be 1 instance of this in each scene.
    /// </summary>
    public class AudioSampler : MonoBehaviour
    {


        public static AudioSampler instance; //singleton static instance
        public bool debug = false; // if true, show audio data being sampled

        // used for drawing the debug chart.
        private Texture2D drawTexture;
        private Color startColor = Color.magenta;
        private Color endColor = Color.blue;
        private Gradient gradient;
        private float fMax;// = (float)AudioSettings.outputSampleRate/2;
        private List<string> debugLables = new List<string>() { "SubBass", "Bass", "LowMid", "Mid", "UpperMid", "High", "VeryHigh", "Decibal" };
        private int samplesToTake = 1024; // how many audio samples should we take?

        public float[] spectrumSamples;
        public float[] soundLevelSamples;
        private int SampleIndex = 0;

        //singleton logic
        void OnEnable()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        void OnDisable()
        {
            instance = null;
        }

        void Awake()
        {
            drawTexture = Texture2D.whiteTexture; // get an empty white texture
            gradient = PanelWaveform.GetColorGradient(startColor, endColor); // get a color gradient.
            spectrumSamples = new float[samplesToTake];
            soundLevelSamples = new float[samplesToTake];
        }

        void Start()
        {
            //get max frequency
            fMax = (float)AudioSettings.outputSampleRate / 2;
        }

        public float GetVolume()
        {
            float sum = 0;
            for(int i = 0;i< soundLevelSamples.Length; i++)
            {
                sum += soundLevelSamples[i];
            }
            return sum / soundLevelSamples.Length;
        }

        public void UpdateSample(float[] audioSpectrum)
        {
            //其实audioSpectrum.Length=64, samples长1024
            
            //for (int i = 0; i < audioSpectrum.Length; i++)//FFTWindow.BlackmanHarris
            //{
            //    audioSpectrum[i] = 0.35875f - (0.48829f * Mathf.Cos(1.0f * audioSpectrum[i] / audioSpectrum.Length)) + (0.14128f * Mathf.Cos(2.0f * audioSpectrum[i] / audioSpectrum.Length)) - (0.01168f * Mathf.Cos(3.0f * audioSpectrum[i] / audioSpectrum.Length));
            //}
            for (int i = 0; i <  audioSpectrum.Length; i++)//index不是按照时间分布而是按照频率分布的
            {
                
                if (audioSpectrum[i] / (2 ^ 30) > 1)
                {
                    for (int j = 0; j < spectrumSamples.Length / audioSpectrum.Length; j++)
                    {
                        spectrumSamples[i * spectrumSamples.Length / audioSpectrum.Length + j] = 1f;
                    }
                }
                else if (audioSpectrum[i] / (2 ^ 30) < 1E-09)
                {
                    for (int j = 0; j < spectrumSamples.Length / audioSpectrum.Length; j++)
                    {
                        spectrumSamples[i * spectrumSamples.Length / audioSpectrum.Length + j] = 0f;
                    }
                }
                else
                {
                    for (int j = 0; j < spectrumSamples.Length / audioSpectrum.Length; j++)
                    {
                        spectrumSamples[i * spectrumSamples.Length / audioSpectrum.Length + j] = audioSpectrum[i] / (2 ^ 30);
                    }
                }

            }
            
        }

        public void UpdateSoundLevel(float soundLevel)
        {
            if (SampleIndex >= samplesToTake)
            {
                SampleIndex = 0;
            }            
            soundLevelSamples[SampleIndex] = soundLevel;
            SampleIndex++;
        }

        //get an array of output data (decibals)
        public float[] GetAudioSamples(int audioSourceIndex)
        {
            //if (!audioSources[audioSourceIndex].mute) // if not muted
            {
                //normalize the samples
                float[] normSamples = NormalizeArray(soundLevelSamples);
                //multiply by volume.
                for (int i = 0; i < soundLevelSamples.Length; i++)
                {
                    normSamples[i] = normSamples[i] * 1;
                }
                return normSamples;
            }

            return new float[samplesToTake];
        }

        //get an array of output data, averaged into 'numBins' bins
        public float[] GetAudioSamples(int audioSourceIndex, int numBins, bool absoluteVal)
        {
            //if (!audioSources[audioSourceIndex].mute) // if not muted
            {
                //normalize the samples
                float[] normSamples = new float[numBins];
                //multiply by volume.
                for (int i = 0; i < numBins; i++)
                {
                    if (absoluteVal)
                    {
                        normSamples[i] = Mathf.Abs(soundLevelSamples[i]) * 1;
                    }
                    else
                    {
                        normSamples[i] = soundLevelSamples[i] * 1;
                    }
                }

                return normSamples;
            }

            return new float[numBins];
        }


        //sample the audio, square each value, and sum them all to get instant energy (the current 'energy' in the audio)
        public float GetInstantEnergy(int audioSourceIndex)
        {
            //if (!audioSources[audioSourceIndex].mute)
            {
                float[] audioSamples = GetAudioSamples(audioSourceIndex);
                float sum = 0;
                //sum up the audio samples
                foreach (float f in audioSamples)
                {
                    sum += (f * f);
                }
                //return sum * audioSources[audioSourceIndex].volume;
                return sum * 1;
            }

            return 0;
        }

        //Get the RMS value of the audio (Root means squared) value 0-1
        //An average "noise" value of the audio at this point in time, using samplesToTake audio samples, and the passed in sensitivity.
        public float GetRMS(int audioSourceIndex)
        {
            //if (!audioSources[audioSourceIndex].mute)
            {
                //grab output data (decibals)
                //float[] normSamples = NormalizeArray(audioSamples);
                int i;
                float sum = 0;
                for (i = 0; i < samplesToTake; i++)
                {
                    sum += soundLevelSamples[i] * soundLevelSamples[i]; // sum squared samples
                }
                float rmsValue = Mathf.Sqrt(sum / samplesToTake) * 1; // rms = square root of average

                return rmsValue;
            }

            return 0;
        }

        //like GetAvg or GetRMS, but inside a given frequency range
        public float GetFrequencyVol(int audioSourceIndex, FrequencyRange freqRange)
        {
            //if (!audioSources[audioSourceIndex].mute) // if not muted
            {
                Vector2 range = GetFreqForRange(freqRange);
                float fLow = range.x;//Mathf.Clamp (range.x, 20, fMax); // limit low...
                float fHigh = range.y;//Mathf.Clamp (range.y, fLow, fMax); // and high frequencies
                // get spectrum
                float[] freqData = spectrumSamples;
                int n1 = (int)Mathf.Floor(fLow * samplesToTake / fMax);
                int n2 = (int)Mathf.Floor(fHigh * samplesToTake / fMax);
                float sum = 0;
                // Debug.Log("Smapling freq: " + n1 + "-" + n2);
                // average the volumes of frequencies fLow to fHigh
                for (int i = n1; i <= n2; i++)
                {
                    if (i < freqData.Length)
                        sum += Mathf.Abs(freqData[i]);
                }
                sum = sum * 1;
                return sum / (n2 - n1 + 1);
            }

            return 0;
        }

        //return the raw spectrum data i nthe given frequency range.
        public float[] GetFrequencyData(int audioSourceIndex, FrequencyRange freqRange)
        {
            //if (!audioSources[audioSourceIndex].mute) // if not muted
            {
                Vector2 range = GetFreqForRange(freqRange);
                float fLow = range.x;//Mathf.Clamp (range.x, 20, fMax); // limit low...
                float fHigh = range.y;//Mathf.Clamp (range.y, fLow, fMax); // and high frequencies
                // get spectrum
                float[] freqData = spectrumSamples;
                int n1 = (int)Mathf.Floor(fLow * samplesToTake / fMax);
                int n2 = (int)Mathf.Floor(fHigh * samplesToTake / fMax);
                float sum = 0;
                // Debug.Log("Smapling freq: " + n1 + "-" + n2);
                // average the volumes of frequencies fLow to fHigh

                List<float> validData = new List<float>();
                for (int i = n1; i <= n2; i++)
                {
                    validData.Add(freqData[i] * 1);
                }

                float[] normData = NormalizeArray(validData.ToArray());

                return normData;
            }

            Debug.LogWarning("warning: Audio Source: " + audioSourceIndex + " is muted");
            return new float[samplesToTake];
        }

        //return the raw spectrum data i nthe given frequency range, using the specified number of bins
        public float[] GetFrequencyData(int audioSourceIndex, FrequencyRange freqRange, int numBins, bool abs)
        {
            //if (!audioSources[audioSourceIndex].mute) // if not muted
            {
                Vector2 range = GetFreqForRange(freqRange);
                float fLow = range.x;//Mathf.Clamp (range.x, 20, fMax); // limit low...
                float fHigh = range.y;//Mathf.Clamp (range.y, fLow, fMax); // and high frequencies
                // get spectrum
                float[] freqData = spectrumSamples;
                int n1 = (int)Mathf.Floor(fLow * samplesToTake / fMax);
                int n2 = (int)Mathf.Floor(fHigh * samplesToTake / fMax);
                float sum = 0;
                // Debug.Log("Smapling freq: " + n1 + "-" + n2);
                // average the volumes of frequencies fLow to fHigh

                //Debug.Log("Valid Freq Data: (" + n1 + "-" + n2 + ")/" + samplesToTake);
                List<float> validData = new List<float>();
                for (int i = n1; i <= n2; i++)
                {
                    float frequency = freqData[i];
                    if (abs)
                    {
                        frequency = Mathf.Abs(freqData[i]);
                    }

                    validData.Add(frequency * 1);
                }

                float[] binnedArray = GetBinnedArray(validData.ToArray(), numBins);
                float[] normalizedArray = NormalizeArray(binnedArray);
                return normalizedArray;
            }

            Debug.LogWarning("warning: Audio Source: " + audioSourceIndex + " is muted");
            return new float[numBins];
        }

        //take an array, and bin the values. 
        // if numBins is > intput.Length, duplicate input values
        // if numBins is < input.Length, average input values
        float[] GetBinnedArray(float[] input, int numBins)
        {
            float[] output = new float[numBins];

            if (numBins == input.Length)
            {
                return input;
            }
            // if numBins is > intput.Length, duplicate input values
            if (numBins > input.Length)
            {
                int binsPerInput = numBins / input.Length;
                for (int b = 0; b < numBins; b++)
                {
                    int inputIndex = (b + 1) % binsPerInput;
                    output[b] = input[inputIndex];
                }
            }

            // if numBins is < input.Length, average input values
            if (numBins < input.Length)
            {
                int inputsPerBin = input.Length / numBins;
                for (int b = 0; b < numBins; b++)
                {
                    float avg = 0;
                    for (int i = 0; i < inputsPerBin; i++)
                    {
                        int index = b * inputsPerBin + i;
                        avg += input[index];
                    }
                    avg = avg / inputsPerBin;

                    output[b] = avg;
                }
            }

            return output;
        }

        //normalize array values to be in the range 0-1
        float[] NormalizeArray(float[] input)
        {
            float[] output = new float[input.Length];
            float max = -Mathf.Infinity;
            //get the max value in the array
            for (int i = 0; i < input.Length; i++)
            {
                max = Mathf.Max(max, Mathf.Abs(input[i]));
            }

            //divide everything by the max value
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i] / max;
            }

            return output;
        }

        public static Vector2 GetFreqForRange(FrequencyRange freqRange)
        {
            switch (freqRange)
            {
                case FrequencyRange.SubBase:
                    return new Vector2(20, 60);
                    break;
                case FrequencyRange.Bass:
                    return new Vector2(60, 250);
                    break;
                case FrequencyRange.LowMidrange:
                    return new Vector2(250, 500);
                    break;
                case FrequencyRange.Midrange:
                    return new Vector2(500, 2000);
                    break;
                case FrequencyRange.UpperMidrange:
                    return new Vector2(2000, 4000);
                    break;
                case FrequencyRange.High:
                    return new Vector2(4000, 6000);
                    break;
                case FrequencyRange.VeryHigh:
                    return new Vector2(6000, 20000);
                    break;
                case FrequencyRange.Decibal:
                    return new Vector2(0, 20000);
                default:
                    break;
            }

            return Vector2.zero;
        }
    }

    public enum FrequencyRange
    {
        SubBase, // 20-60 Hz
        Bass, // 60-250 Hz
        LowMidrange, //250-500 Hz
        Midrange, //500-2,000 Hz
        UpperMidrange, //2,000-4,000 Hz
        High, //4,000-6000 Hz
        VeryHigh, //6,000-20,000 Hz
        Decibal // use output data instead of frequency data
    };



}

