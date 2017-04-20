using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Plays sound effects and handles GameObjects with AudioSource component and Audio.
 * Use whenever you need to play a sound that isn't music.
*/
public class SoundFXPlayer : MonoBehaviour
{
	
	public static SoundFXPlayer instance = null; // singleton
    
    [SerializeField]
    private string audioObjectTag = "SFXObject"; // the tag for all the AudioSource GameObjects (remember to create this tag in Unity)

	private Stack<AudioSource> disabledAudioSources; // contains disabled AudioSources
	private Transform cachedTransform;
	
	
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			cachedTransform = transform;
			GameObject.DontDestroyOnLoad(gameObject);
            disabledAudioSources = new Stack<AudioSource>();
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
	}

    void Start()
    {
        verifyTag(ref audioObjectTag);
    }


    /* 
     * Plays an AudioClip 'clip' with the desired volume once. Default behavior is playing a 2D sound
     * Returns a reference to the AudioSource component (null if clip is invalid).
     * Will play the audioclip once and then disable the whole GameObject.
     * DISABLING/ENABLING IS HANDLED AUTOMATICALLY, DO NOT ENABLE/DISABLE MANUALLY.
     * levelOf3D controls the spatial blend (0==2D, 1==3D), position only matters, if levelOf3D > 0.
     * The rest of the parameters control the corresponding settings of AudioSource.
    */
    public AudioSource playClipOnce(AudioClip clip, float volume = 1f, float levelOf3D = 0, Vector3 position =  default(Vector3),
                                    float dopplerLevel = 0f, float spread = 0f, float minDistance = 1, float maxDistance = 500,
                                    AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic)
	{
        AudioSource aSource = playClip(clip, false, volume, levelOf3D, position, dopplerLevel, spread, minDistance, maxDistance, rolloffMode);
        if (aSource != null)
        {
            StartCoroutine(disableAfterPlaying(aSource));
        }
        return aSource;
	}

    /* 
     * Plays an AudioClip 'clip' with the desired volume. Default behavior is playing a 2D sound
     * Returns a reference to the AudioSource component (null if clip is invalid).
     * Will continue playing the audio over and over, can be paused manually.
     * Disabling/enabling should be handled by calling recycleAudioSource or completely manually.
     * levelOf3D controls the spatial blend (0==2D, 1==3D), position only matters, if levelOf3D > 0.
     * The rest of the settings control the corresponding settings of AudioSource.
    */
    public AudioSource playClipContinuosly(AudioClip clip, float volume = 1f, float levelOf3D = 0, Vector3 position =  default(Vector3),
                                           float dopplerLevel = 0f, float spread = 0f, float minDistance = 1, float maxDistance = 500,
                                           AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic)
    {
        return playClip(clip, true, volume, levelOf3D, position, dopplerLevel, spread, minDistance, maxDistance, rolloffMode);
    }

    // Call to manually recycle and disable a GameObject with an AudioSource component.
    // Call this when you want to completely stop playing a continuous AudioSource.
    // DO NOT TRY TO USE THE AUDIOSOURCE OR THE GAMEOBJECT IT IS ATTACHED TO AFTER CALLING THIS.
    public void recycleAudioSource(AudioSource aSource)
    {
        if (aSource.CompareTag(audioObjectTag))
        {
            aSource.Stop();
            aSource.clip = null;
            aSource.gameObject.SetActive(false);
            disabledAudioSources.Push(aSource);
        }
        else
        {
            Debug.LogWarning("recycleAudioSource was called for an AudioSource GameObject that isn't tagged with " + audioObjectTag +
                             ". Please only use AudioSources created by the SoundFXPlayer as parameters for recycleAudioSource function.");
        }
    }

    // Destroys the percentage (0-100) of disabled AudioSource GameObjects entirely.
    // Only call if you think there are too many disabled AudioSources in the memory.
    public void destroyDisabledAudioObjects(float percentage)
    {
        if (percentage <= 0)
        {
            return;
        }
        else if (percentage > 100)
        {
            percentage = 100;
        }

        // the aSource with the lowest index to be destroyed
        int destroyCount = (int)( ((float)disabledAudioSources.Count) * (percentage / 100f) );
        for (int i = 0; i < destroyCount; ++i)
        {
            AudioSource aSource = disabledAudioSources.Pop();
            Destroy(aSource.gameObject);
        }
        Debug.Log("Destroyed " + destroyCount + " AudioSource objects in SoundFXPlayer. " + disabledAudioSources.Count + " disabled AudioSource objects remain.");
    }


    // Plays the AudioClip 'clip' with the desired parameters
    private AudioSource playClip(AudioClip clip, bool loop, float volume = 1f, float levelOf3D = 0, Vector3 position =  default(Vector3),
                                 float dopplerLevel = 0f, float spread = 0f, float minDistance = 1, float maxDistance = 500,
                                 AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic)
    {
        if (clip == null)
        {
            Debug.LogError("Error: The audio clip is invalid or missing.");
            return null;
        }
        AudioSource aSource = getAudioSourceObject();
        aSource.clip = clip;

        // Set the desired settings:
        aSource.volume = volume;
        aSource.spatialBlend = levelOf3D;
        aSource.spread = spread;
        aSource.dopplerLevel = dopplerLevel;
        aSource.transform.position = position;
        aSource.minDistance = minDistance;
        aSource.maxDistance = maxDistance;
        aSource.rolloffMode = rolloffMode;
        aSource.loop = loop;

        aSource.playOnAwake = false; // for my own sanity
        aSource.Play();
        return aSource;
    }

    // Returns an AudioSource component of an otherwise empty GameObject.
    private AudioSource getAudioSourceObject()
    {
        AudioSource aSource;
        if (disabledAudioSources.Count > 0)
        {
            aSource = disabledAudioSources.Pop();
            aSource.clip = null;
            aSource.gameObject.SetActive(true);
        }
        else
        {
            GameObject audioObj = new GameObject("AudioObject"); // create the audio game object
            audioObj.tag = audioObjectTag;
            DontDestroyOnLoad(audioObj);
            aSource = audioObj.AddComponent<AudioSource>(); // add an audio source component
            aSource.transform.position = cachedTransform.position;
        }
        return aSource;
    }

    // Disables the GameObject that the AudioSource component is attached to and 
    // puts it to disabledAudioSources container after the AudioSource has done playing its AudioClip.
    private IEnumerator disableAfterPlaying(AudioSource aSource)
    {
        if (aSource.clip == null)
        {
            aSource.gameObject.SetActive(false);
            disabledAudioSources.Push(aSource);
        }
        else
        {
            float waitTime = aSource.clip.length;
            yield return new WaitForSeconds(waitTime);
            if (aSource.gameObject.activeSelf)
            {
                aSource.Stop();
                aSource.clip = null;
                aSource.gameObject.SetActive(false);
                disabledAudioSources.Push(aSource);
            }
        }
    }

    private void verifyTag(ref string tag)
    {
        try
        {
            gameObject.CompareTag(tag);
        }
        catch (UnityException ex)
        {
            Debug.LogError("Error: In " + this.GetType().ToString() + ": Tag  " + audioObjectTag + "  does not exist. Please create it in Unity.");
            tag = "Untagged";
            throw ex;
        }
    }

}
