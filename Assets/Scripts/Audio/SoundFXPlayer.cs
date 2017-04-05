using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundFXPlayer : MonoBehaviour
{
	
	public static SoundFXPlayer instance = null; // singleton

	[System.Serializable]
	public struct NamedSoundFX // for editor
	{
		public string name;
		public AudioClip soundFX;
	}

	[SerializeField]
	private NamedSoundFX[] soundEffectsArray; // for editor

	private Dictionary<string, AudioClip> soundEffectsDictionary; // hashtable containing the sfx during runtime
    private Stack<AudioSource> disabledAudioSources; // contains disabled AudioSources
    private Transform cachedTransform;
	

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
            cachedTransform = transform;
			GameObject.DontDestroyOnLoad(gameObject);
			soundEffectsDictionary = new Dictionary<string, AudioClip>();
            disabledAudioSources = new Stack<AudioSource>();
			fillSFXHashtable();
			soundEffectsArray = null; // empty the array because it's only used in the editor
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
	}


	// Basically a custom 2D version of AudioSource.PlayClipAtPoint().
	// Plays an AudioClip 'clip' with the desired volume at the (0, 0, 0) position; the position doesn't matter since it's 2D.
    // Returns a reference to the AudioSource component (can be null if clip not found).
    // After playing the audio deactivates the itself (the whole GameObject).
    // DO NOT ADD/REMOVE COMPONENTS TO/FROM THE GAMEOBJECT OR ENABLE/DISABLE ANYTHING
	public AudioSource play2DClipOnce( string clipName, float volume = 1f )
	{
		if ( soundEffectsDictionary.ContainsKey(clipName) )
		{
            AudioSource aSource = getAudioSourceObject();
			aSource.clip = soundEffectsDictionary[clipName];
			aSource.volume = volume;

			// Set the desired settings (no 3D):
			aSource.spread = 0;
			aSource.dopplerLevel = 0;

			aSource.playOnAwake = false; // for my own sanity
			aSource.Play();
            StartCoroutine( disableAfterPlaying(aSource) );
            return aSource;
		}
		else
		{
			Debug.Log("No sound with the name: " + clipName);
            return null;
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
            GameObject audioObj = new GameObject("Audio Shot 2D"); // create the audio game object
            DontDestroyOnLoad(audioObj);
            aSource = audioObj.AddComponent<AudioSource>(); // add an audio source component
            aSource.transform.position = cachedTransform.position;
        }
        return aSource;
    }

    // Disables the GameObject that the AudioSource component is attached to and puts it to disabledAudioSources container
    // after the AudioSource has done playing its AudioClip.
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
            aSource.clip = null;
            aSource.gameObject.SetActive(false);
            disabledAudioSources.Push(aSource);
        }
		Debug.Log("Disabled: " + disabledAudioSources.Count);
    }

    // Call on start; fills the hashtable with the values in soundEffectsArray
    private void fillSFXHashtable()
    {
        string sfxName;
        for (int i=0; i<soundEffectsArray.Length; ++i)
        {
            sfxName = soundEffectsArray[i].name;
            if ( !soundEffectsArray[i].soundFX || sfxName == "" ) // needs to have an actual sound and name
            {
                Debug.Log("soundEffectsArray in SoundFXPlayer contains an empty/incorrect entry on index " + i );
            }
            else if ( soundEffectsDictionary.ContainsKey( sfxName ) ) // sound with the same name already exists
            {
                Debug.Log("Two soundFXs have the name '" + sfxName + "' in soundEffectsArray in SoundFXPlayer class. See entry with index " + i);
            }
            else // successfully add new sound
            {
                soundEffectsDictionary.Add( sfxName, soundEffectsArray[i].soundFX );
            }
        }
    }

}
