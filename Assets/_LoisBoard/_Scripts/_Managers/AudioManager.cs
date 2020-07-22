using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip[] clips;
    AudioSource audioSource;
    void Awake()
    {
        if (Instance == null) Instance = this;
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int clipNum)
    {
        StartCoroutine(CRPlaySound(clipNum));
    }

    IEnumerator CRPlaySound(int clipNum)
    {
        //audioSource.clip = clips[clipNum];
        audioSource.PlayOneShot(clips[clipNum]);
        yield return null;
    }
    /*
     Sound Track
     0. Main Scene
     1. End Scene
     2. Hover
     3. Open Menu
     4. Close Menu
     5. Start Record
     6. End Record
     7. Add Node
     8. Delete Node
     9. Elevator
     10. Recenter
     11. Camera
     12. Warning
     */
}
