using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Electronic : MonoBehaviour {

	public int connection;
	public int type;
	// 1 - switch, 2 - door
	public Sprite original;
	public Sprite alteration;
	private List<Electronic> toChangeBack = new List<Electronic>();
	public bool state = true;
	public int defaultLayer;
	public int altLayer;

	public void Swap() {
		if (state) {
			GetComponent<SpriteRenderer> ().sprite = alteration;
			gameObject.layer = altLayer;
			state = false;
		} else {
			GetComponent<SpriteRenderer> ().sprite = original;
			gameObject.layer = defaultLayer;
			state = true;
		}
	}

	public void ChangeColor (bool tored) {
		if (tored) {
			GetComponent<SpriteRenderer> ().color = Color.red;
		} else {
			GetComponent<SpriteRenderer> ().color = Color.white;
		}
	}

	void OnMouseEnter () {
		Electronic[] connected = GameObject.FindObjectsOfType<Electronic> ();
		toChangeBack.Clear ();

		foreach (Electronic el in connected) {
			if (el.connection == connection) {
				el.ChangeColor (true);
				toChangeBack.Add (el);
			}
		}
	}

	void OnMouseExit () {
		foreach (Electronic el in toChangeBack) {
			el.ChangeColor (false);
		}
		toChangeBack.Clear ();
	}
}
