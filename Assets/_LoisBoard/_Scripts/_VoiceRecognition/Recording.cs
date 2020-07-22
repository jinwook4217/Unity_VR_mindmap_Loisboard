using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Recording : MonoBehaviour {

    struct ClipData
    {
        public int samples;
    }

    const int HEADER_SIZE = 44;

    private int minFreq;
    private int maxFreq;

    private bool micConnected = false;

    //A handle to the attached AudioSource
    private AudioSource goAudioSource;
    void Start () {
        //Check if there is at least one microphone connected
        if (Microphone.devices.Length <= 0)
        {
            //Throw a warning message at the console if there isn't
            Debug.LogWarning("Microphone not connected!");
        }
        else //At least one microphone is present
        {
            //Set 'micConnected' to true
            micConnected = true;

            //Get the default microphone recording capabilities
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...
            if (minFreq == 0 && maxFreq == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate
                maxFreq = 44100;
            }

            //Get the attached AudioSource component
            goAudioSource = this.GetComponent<AudioSource>();
        }
    }

    public void SaveWave(AudioClip clip)
    {
        float filenameRand = 1f; //UnityEngine.Random.Range(0.0f, 10.0f);
        string filename = "testing" + filenameRand;
        Microphone.End(null); //Stop the audio recording
        // Debug.Log("Recording Stopped");
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }
        var filePath = Path.Combine("testing/", filename);
        filePath = Path.Combine(Application.persistentDataPath, filePath);
        //Debug.Log("Created filepath string: " + filePath);
        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        SavWav.Save(filePath, clip); //Save a temporary Wav File
        //Debug.Log("Saving @ " + filePath);
    }

}
