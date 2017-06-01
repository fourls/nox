using UnityEngine;
using System.Collections;

public class Permanent : MonoBehaviour {

	public static Permanent instance;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		
		DontDestroyOnLoad (gameObject);
	}
}
