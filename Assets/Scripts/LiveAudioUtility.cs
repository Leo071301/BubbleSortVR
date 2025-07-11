using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 
 *  LiveAudioUtility - is a class which impliments a co-routine behaviour for playing audio
 *                      the list of audioclips can be acessed and modified readily in 
 *                      the inspector tab
 *                      
 *  * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 */
public class LiveAudioUtility : MonoBehaviour
{
    // fields set in the inspector
    [SerializeField] public AudioSource audioSource = null;
    [SerializeField] public List<AudioClip> audioClips = null;

    void Start()
    {
        if (audioSource == null)
            Debug.Log(" Please Attach an AudioSource component! This script wont work without one" );

        if (audioClips == null)
            Debug.Log(" Please add audio clips to this script! Other-wise this script will break!" );
    }

    public IEnumerator playAndWait(int clip_number)
    {
        audioSource.clip = audioClips[clip_number];
        audioSource.Play();

        while (audioSource.isPlaying) 
            yield return null;  // keep stalling untill sound finished :)
    }
}
