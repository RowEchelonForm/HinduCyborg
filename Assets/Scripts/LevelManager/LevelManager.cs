using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*
 * Use this to load levels.
*/

public static class LevelManager
{

	public static string currentLevelName
	{ 
		get 		{ return _currentLevelName;  }
		private set	{ _currentLevelName = value; }
	}
	public static int currentLevelIndex
	{ 
		get 		{ return _currentLevelIndex;  }
		private set { _currentLevelIndex = value; }
	}

	private static string _currentLevelName = SceneManager.GetActiveScene().name;
	private static int _currentLevelIndex = SceneManager.GetActiveScene().buildIndex;


	// Loads level 'levelName'
	public static void loadLevel(string levelName)
	{
		Scene newScene = SceneManager.GetSceneByName(levelName);
		SceneManager.LoadScene(levelName);
		currentLevelName = levelName;
		currentLevelIndex = newScene.buildIndex;
	}

	// Loads the next level
	public static void loadNextLevel()
	{
		// Load next scene if not in last scene already
		if ( currentLevelIndex + 1 < SceneManager.sceneCountInBuildSettings )
		{
			Scene newScene = SceneManager.GetSceneAt(currentLevelIndex + 1);
			SceneManager.LoadScene( currentLevelIndex + 1 );
			currentLevelName = newScene.name;
			currentLevelIndex = newScene.buildIndex;
		}
		else
		{
			Debug.Log("Already on last scene");
		}
	}

	public static void reloadCurrentLevel()
	{
		loadLevel( SceneManager.GetActiveScene().name );
	}

	// Quits from the game
	public static void quitRequest()
	{
		Debug.Log("Quit request");
		Application.Quit();
	}

	public static void loadStart()
	{
		loadLevel("Start");
	}

	public static void loadWin()
	{
		loadLevel("Win");
	}

	public static void loadLose()
	{
		loadLevel("Lose");
	}

}
