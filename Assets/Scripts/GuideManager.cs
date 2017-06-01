using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GuideManager : MonoBehaviour {

	public ObjectProfile[] profiles;
	public GameObject profilePrefab;
	public GameObject profileHolder;

	public Button backButton;

	void Start () {
		if (GameObject.Find ("Permanent") == null) {
			SceneManager.LoadScene ("Introduction");
			Destroy (gameObject);
		}

		foreach(ObjectProfile profile in profiles) {
			GameObject go = (GameObject)Instantiate (profilePrefab);
			go.GetComponent<UIManager> ().graphics["NameText"].GetComponent<Text>().text = profile.name;
			go.GetComponent<UIManager> ().graphics["DescriptionText"].GetComponent<Text>().text = profile.description;
			go.GetComponent<UIManager> ().graphics["Image"].GetComponent<Image>().sprite = profile.image;
			go.transform.SetParent (profileHolder.transform, false);
		}
		backButton.onClick.AddListener (ReturnToMenu);
	}

	void ReturnToMenu () {
		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene(null,MiddleReturnToMenu,null));
	}

	void MiddleReturnToMenu () {
		SceneManager.LoadScene ("LevelSelect");
	}
}

[System.Serializable]
public class ObjectProfile {
	public string name;
	public Sprite image;
	[TextArea(1,5)]
	public string description;

	public ObjectProfile (string n, Sprite i, string d) {
		name = n;
		image = i;
		description = d;
	}
}