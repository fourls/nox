using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuitManager : MonoBehaviour {

	public void ReturnToMenu () {
		SceneManager.LoadScene ("LevelSelect");
		Destroy (gameObject);
	}

	public void ExitGame () {
		Application.Quit ();
		//UnityEditor.EditorApplication.isPlaying = false;
	}
}
