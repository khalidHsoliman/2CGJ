using UnityEngine.SceneManagement; 
using UnityEngine;

// this script is just to make sure everything works properly 
public class StoryManagerScript : MonoBehaviour {
	public GameObject firstScene;
	public GameObject secondScene;
	public GameObject thirdScene;

	// Use this for initialization
	void Start () {
		hideGameObject (secondScene);
		hideGameObject (thirdScene);
		showGameObject (firstScene);

	}
	
	void hideGameObject(GameObject g)
	{
		if (g != null)
			g.SetActive (false);
	}

	void showGameObject(GameObject g)
	{
		if (g != null)
			g.SetActive (true);
	}

	//loads inputted level
	public void LoadLevel(string level){
        SceneManager.LoadScene(level); 
	}
}
