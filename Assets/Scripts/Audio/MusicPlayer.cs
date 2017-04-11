using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Plays music. Has a few different ways to play.
 * Supports fade in/fade out and changing the volume.
 * Doesn't support layered music.
*/

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
	enum PlayingState { NotPlaying, Playing, FadeIn, FadeOut };

	public static MusicPlayer instance = null; // singleton

	[System.Serializable]
	public struct NamedMusicTrack
	{
		public string name;
		public AudioClip musicTrack;
	}

	[SerializeField]
	private NamedMusicTrack[] musicTrackArray;

	private AudioSource audioSource;
	private float maxVolume;
	private Dictionary<string, AudioClip> musicTrackDictionary;

	private string lastPlayingTrackName;
	private float lastPlayingTrackTime;
	private bool lastLoopStatus;

	private PlayingState state;
	private IEnumerator fadeInCoroutine;
	private IEnumerator fadeOutCoroutine;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
			fillMusicHashtable();
			initVariables();
			musicTrackArray = null;
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		playMusicOnLoop("track_name", 0f); // TODO delete
	}


	// Changes the current volume of music to 'volume' (0...1). 
	// If not playing anything, the volume will still change 
	// and it will remain changed when starting to play music the next time.
	public void changeVolume(float volume)
	{
		if (volume > 1.0f)
		{
			volume = 1.0f;
		}
		else if (volume <= 0f)
		{
			volume = 0f;
			Debug.Log("Muted music player.");
		}
		audioSource.volume = volume;
		maxVolume = volume;
	}

	// Plays music on loop. If the same music is already playing,
	// just continues playing it (doesn't reset).
	public void playMusicOnLoop( string musicName, float fadeInTime = 1.0f, float fadeOutTime = 1.0f )
	{
		if ( musicTrackDictionary.ContainsKey(musicName) &&  audioSource.clip != null )
		{
			if ( lastPlayingTrackName != musicName || !audioSource.isPlaying ) // different track or current one isn't playing
			{
				stopPlayingMusic(fadeOutTime);
				startFadeIn(fadeInTime, musicName, true, 0f);
			}
		}
		else if ( musicTrackDictionary.ContainsKey(musicName) && audioSource.clip == null  )
		{
			startFadeIn(fadeInTime, musicName, true, 0f);
		}
		else
		{
			Debug.Log("No music track with the name: " + musicName);
		}
	}

	// Plays music on loop. If the same music is already playing,
	// starts playing it again from start.
	public void playMusicOnLoopFromStart( string musicName, float fadeInTime = 1.0f, float fadeOutTime = 1.0f )
	{
		if ( musicTrackDictionary.ContainsKey(musicName) )
		{
			stopPlayingMusic(fadeOutTime);
			startFadeIn(fadeInTime, musicName, true, 0f);
		}
		else
		{
			Debug.Log("No music track with the name: " + musicName);
		}
	}

	// Plays music non-looped
	public void playMusicNoLoop( string musicName, float fadeInTime = 1.0f, float fadeOutTime = 1.0f )
	{
		if (lastPlayingTrackName == musicName && audioSource.isPlaying)
		{
			audioSource.loop = false;
		}
		else if ( musicTrackDictionary.ContainsKey(musicName) )
		{
			stopPlayingMusic(fadeOutTime);
			startFadeIn(fadeInTime, musicName, false, 0f);
		}
		else
		{
			Debug.Log("No music track with the name: " + musicName);
		}
	}

	// Stops any music that is playing
	public void stopPlayingMusic(float fadeOutTime = 1.0f)
	{
		if (audioSource.clip != null || state != PlayingState.NotPlaying || state != PlayingState.FadeOut)
		{
			if ( fadeInCoroutine != null && state == PlayingState.FadeIn )
			{
				StopCoroutine( fadeInCoroutine );
				state = PlayingState.NotPlaying;
			}
			if ( fadeOutCoroutine != null && state == PlayingState.FadeOut )
			{
				return;
			}
			fadeOutCoroutine = audioFadeOut(fadeOutTime);
			StartCoroutine( fadeOutCoroutine );
		}
	}

    // Continues playing music (after stopping)
	public void continuePlayingMusic(float fadeInTime = 1.0f)
	{
		if ( audioSource.isPlaying || state == PlayingState.Playing || !musicTrackDictionary.ContainsKey(lastPlayingTrackName) )
		{
			return;
		}
		startFadeIn(fadeInTime, lastPlayingTrackName, lastLoopStatus, lastPlayingTrackTime);
	}


	// musicName must be valid!
	private void startFadeIn(float fadeInTime, string musicName, bool loop, float startPlayTime)
	{
		if (fadeInCoroutine != null)
		{
			StopCoroutine( fadeInCoroutine );
		}
		fadeInCoroutine = audioFadeIn(fadeInTime, musicName, loop, startPlayTime);
		StartCoroutine( fadeInCoroutine );
	}


	private IEnumerator audioFadeIn(float fadeTime, string musicName, bool loop, float startPlayTime)
	{
		while (state == PlayingState.FadeIn || state == PlayingState.FadeOut)
		{
			yield return null;
		}
		audioSource.clip = musicTrackDictionary[musicName];
		audioSource.time = startPlayTime;
		audioSource.loop = loop;
		lastLoopStatus = loop;
		lastPlayingTrackName = musicName;
		if (fadeTime > 0)
		{
			state = PlayingState.FadeIn;
			float timer = 0f;
			audioSource.volume = 0f;
			audioSource.Play();
			while ( audioSource.volume < maxVolume )
			{
				audioSource.volume = maxVolume*timer/fadeTime;
				timer += Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			audioSource.Play();
		}
		state = PlayingState.Playing;
		audioSource.volume = maxVolume;
	}

	private IEnumerator audioFadeOut(float fadeTime)
	{
		while (state == PlayingState.FadeIn || state == PlayingState.FadeOut)
		{
			yield return null;
		}
		if (fadeTime > 0)
		{
			state = PlayingState.FadeOut;
			float timer = fadeTime;
			float startVolume = audioSource.volume;
			while ( audioSource.volume > 0 )
			{
				audioSource.volume = startVolume*timer/fadeTime;
				timer -= Time.deltaTime;
				if (timer < 0)
				{
					timer = 0;
				}
				yield return null;
			}
			audioSource.volume = startVolume;
		}
		lastPlayingTrackTime = audioSource.time;
		audioSource.Stop();
		state = PlayingState.NotPlaying;
	}



	// Call this on start; creates the hashtable and fills it with the values in soundEffectsArray
	private void fillMusicHashtable()
	{
		musicTrackDictionary = new Dictionary<string, AudioClip>();
		string musicName;
		for (int i=0; i<musicTrackArray.Length; ++i)
		{
			musicName = musicTrackArray[i].name;
			if ( !musicTrackArray[i].musicTrack || musicName == "" ) // needs to have an actual sound and name
			{
				Debug.Log("musicTrackArray in MusicPlayer contains an empty/incorrect entry on index " + i );
			}
			else if ( musicTrackDictionary.ContainsKey( musicName ) ) // sound with the same name already exists
			{
				Debug.Log("Two music tracks have the name '" + musicName + "' in musicTrackArray in MusicPlayer class. See entry with index " + i);
			}
			else // successfully add new sound
			{
				musicTrackDictionary.Add( musicName, musicTrackArray[i].musicTrack );
			}
		}
	}

	// Call this on start.
	private void initVariables()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 1.0f;
            audioSource.dopplerLevel = 0;
            audioSource.spread = 0;
        }
		maxVolume = audioSource.volume;
		lastPlayingTrackName = "";
		lastPlayingTrackTime = 0f;
		state = PlayingState.NotPlaying;
	}

}
