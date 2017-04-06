using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*
 * Use this to load levels.
*/

public static class LevelManager
{
	
	// Loads level 'levelName'
	public static void loadLevel(string levelName)
	{
		SceneManager.LoadScene(levelName);
	}

	// Loads the next level
	public static void loadNextLevel()
	{
		// Load next scene if not in last scene already
		if ( SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings )
		{
			SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex + 1 );
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
