using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * If you need to use a MonoBehaviour instance to call the LevelManager (for buttons etc).
*/
public class LevelManagerWrapper : MonoBehaviour
{
	public void loadLevelWrapper(string levelName)
	{
		LevelManager.loadLevel(levelName);
	}

	public void loadNextLevelWrapper()
	{
		LevelManager.loadNextLevel();
	}

	public void reloadCurrentLevelWrapper()
	{
		LevelManager.reloadCurrentLevel();
	}

	public void quitRequestWrapper()
	{
		LevelManager.quitRequest();
	}

	public void loadStartWrapper()
	{
		LevelManager.loadStart();
	}

	public void loadWinWrapper()
	{
		LevelManager.loadWin();
	}

	public void loadLoseWrapper()
	{
		LevelManager.loadLose();
	}

}
