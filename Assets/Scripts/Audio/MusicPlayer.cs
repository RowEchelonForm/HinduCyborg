using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/*
 * Plays music. Has a few different ways to play.
 * Supports fade in/fade out and changing the volume.
 * Doesn't support layered music. Plays the AudioClip parameter given to each function.
*/

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
	enum PlayingState { NotPlaying, Playing, FadeIn, FadeOut };

	public static MusicPlayer instance = null; // singleton
    
    [System.Serializable]
    public class LevelMusic
    {
        public string levelName;
        public AudioClip levelMusicClip;
        [Range(0f, 1f)]
        public float trackVolume = 1f;
        public bool loop = true;
    }
    
    [SerializeField]
    private List<LevelMusic> levelMusicList = new List<LevelMusic>();
    
    private Dictionary<string, LevelMusic> levelMusicHTable = new Dictionary<string, LevelMusic>();

	private AudioSource audioSource;
	private float maxVolume;

	private AudioClip lastPlayingTrack;
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
			initVariables();
            fillLevelMusicHTable();
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
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
	public void playMusicOnLoop( AudioClip musicClip, float fadeInTime = 1.0f, float fadeOutTime = 1.0f )
	{
		if ( musicClip != null &&  audioSource.clip != null )
		{
			if ( lastPlayingTrack != musicClip || !audioSource.isPlaying ) // different track or current one isn't playing
			{
				stopPlayingMusic(fadeOutTime);
				startFadeIn(fadeInTime, musicClip, true, 0f);
			}
		}
		else if ( musicClip != null && audioSource.clip == null  )
		{
			startFadeIn(fadeInTime, musicClip, true, 0f);
		}
		else
		{
			Debug.LogError("Music AudioClip is null");
		}
	}

	// Plays music on loop. If the same music is already playing,
	// starts playing it again from start.
	public void playMusicOnLoopFromStart( AudioClip musicClip, float fadeInTime = 1.0f, float fadeOutTime = 1.0f )
	{
		if ( musicClip != null )
		{
			stopPlayingMusic(fadeOutTime);
			startFadeIn(fadeInTime, musicClip, true, 0f);
		}
		else
		{
			Debug.LogError("Music AudioClip is null");
		}
	}

	// Plays music non-looped
	public void playMusicNoLoop( AudioClip musicClip, float fadeInTime = 1.0f, float fadeOutTime = 1.0f )
	{
		if (lastPlayingTrack == musicClip && audioSource.isPlaying)
		{
			audioSource.loop = false;
		}
		else if ( musicClip != null )
		{
			stopPlayingMusic(fadeOutTime);
			startFadeIn(fadeInTime, musicClip, false, 0f);
		}
		else
		{
			Debug.LogError("Music AudioClip is null");
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
		if ( audioSource.isPlaying || state == PlayingState.Playing || lastPlayingTrack == null )
		{
			return;
		}
		startFadeIn(fadeInTime, lastPlayingTrack, lastLoopStatus, lastPlayingTrackTime);
	}



	private void startFadeIn(float fadeInTime, AudioClip musicClip, bool loop, float startPlayTime)
	{
		if (fadeInCoroutine != null)
		{
			StopCoroutine( fadeInCoroutine );
		}
		fadeInCoroutine = audioFadeIn(fadeInTime, musicClip, loop, startPlayTime);
		StartCoroutine( fadeInCoroutine );
	}


	private IEnumerator audioFadeIn(float fadeTime, AudioClip musicClip, bool loop, float startPlayTime)
	{
		while (state == PlayingState.FadeIn || state == PlayingState.FadeOut)
		{
			yield return null;
		}
		audioSource.clip = musicClip;
		audioSource.time = startPlayTime;
		audioSource.loop = loop;
		lastLoopStatus = loop;
		lastPlayingTrack = musicClip;
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
		lastPlayingTrack = null;
		lastPlayingTrackTime = 0f;
		state = PlayingState.NotPlaying;
	}
    
    private void fillLevelMusicHTable()
    {
        for (int i = 0; i < levelMusicList.Count; ++i)
        {
            if (levelMusicList[i].levelName == "")
            {
                Debug.LogError("Error: levelName is invalid at the index " + i + " in levelMusicList in MusicPlayer class.");
                continue;
            }
            if (levelMusicList[i].levelMusicClip == null)
            {
                Debug.LogError("Error: levelMusicClip is invalid at the index " + i + " with the name " +
                               levelMusicList[i].levelName + " in levelMusicList in MusicPlayer class.");
                continue;
            }
            if (levelMusicList[i].trackVolume <= 0 || levelMusicList[i].trackVolume > 1)
            {
                Debug.LogWarning("trackVolume of a level music should be [0...1]. (Index " + i + " with the name " +
                               levelMusicList[i].levelName + " in levelMusicList in MusicPlayer class.)");
            }
            levelMusicHTable.Add(levelMusicList[i].levelName, levelMusicList[i]);
        }
        levelMusicList.Clear();
    }
    
    
    //////////////////////////////////
    // Scene loading related stuff: //
    //////////////////////////////////
    
    private void playLevelMusic(string levelName)
    {
        if (!levelMusicHTable.ContainsKey(levelName))
        {
            Debug.LogWarning("No music for the level " + levelName);
            return;
        }
        
        LevelMusic lvlm = levelMusicHTable[levelName];
        if (lvlm.loop)
        {
            playMusicOnLoop(lvlm.levelMusicClip);
        }
        else
        {
            playMusicNoLoop(lvlm.levelMusicClip);
        }
        changeVolume(lvlm.trackVolume);
    }
    
    void sceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        playLevelMusic(scene.name);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += sceneWasLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= sceneWasLoaded;
    }

}
