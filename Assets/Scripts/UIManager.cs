using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class UIManager : MonoBehaviour {


	public void LoadLevel(string level){
        SceneManager.LoadScene(level);
	}

	public void exitGame()
	{
		Application.Quit();
	}
}
