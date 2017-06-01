using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class LevelCreatorButton : MonoBehaviour {

	public Text text;
	public string[] options;

	private int index = 0;

	// Use this for initialization
	void Start () {
		text.text = options [index];
	}

	public void Clicked() {
		index++;
		if (index >= options.Length)
			index = 0;
		
		text.text = options [index];
	}

	public void ChangeValueTo(int val) {
		text.text = options [val];
		index = val;
	}
}
