using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class AudioVisualizer : MonoBehaviour
    {

        public Transform[] audioSpectrumObjects;
        public float heightMultiplier;
        public int numberOfSamples = 1024;
        public float limit = 1f;
        public FFTWindow fftWindow;
        public float lerpTime = 0.05f;
        private AudioSource visualizeAudioSource;
        public int maxFreq = 44100;



        void Update()
        {
            if (VoiceRecognitionManager.Instance.IsNowRecording)
            {
                float[] spectrum = new float[numberOfSamples];

                if (visualizeAudioSource != null) visualizeAudioSource.GetSpectrumData(spectrum, 0, fftWindow);

                for (int i = 0; i < audioSpectrumObjects.Length; i++)
                {
                    // Debug.Log(spectrum[i] + ", " + Mathf.Sqrt(spectrum[i]*1000f) );
                    float intensity = Mathf.Min(Mathf.Sqrt(spectrum[i] * 1000f) * heightMultiplier, limit);
                    float lerpX = Mathf.Lerp(audioSpectrumObjects[i].localScale.x, intensity, lerpTime);
                    float lerpY = Mathf.Lerp(audioSpectrumObjects[i].localScale.y, intensity, lerpTime);
                    float lerpZ = Mathf.Lerp(audioSpectrumObjects[i].localScale.z, intensity, lerpTime);
                    Vector3 newScale = new Vector3(lerpX, lerpY, lerpZ);// audioSpectrumObjects[i].localScale.z);
                    audioSpectrumObjects[i].localScale = newScale;
                }
            }
        }

        public void VisualizeVoice()
        {
            visualizeAudioSource = GetComponent<AudioSource>();
            //visualizeAudioSource.clip = Microphone.Start(null, true, 100, 44100);
            //visualizeAudioSource.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { }
            // Debug.Log("start playing... position is " + Microphone.GetPosition(null));
            visualizeAudioSource.Play();
        }

    }
}

  

