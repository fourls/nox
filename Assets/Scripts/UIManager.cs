using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	[SerializeField]
	GameObjectHolder[] inspectorGraphics;
	[HideInInspector]
	public Dictionary<string,GameObject> graphics;

	void Awake () {
		graphics = new Dictionary<string, GameObject>();
		FillGraphicsDictionary (); 
	}

	void FillGraphicsDictionary() {
		if (graphics == null)
			graphics = new Dictionary<string, GameObject> ();
		graphics.Clear ();
		foreach (GameObjectHolder gh in inspectorGraphics) {
			graphics.Add (gh.name, gh.gameobject);
		}
	}

	[System.Serializable]
	class GameObjectHolder {
		public string name;
		public GameObject gameobject;

		public GameObjectHolder(string i, GameObject g) {
			name = i;
			gameobject = g;
		}
	}
}
