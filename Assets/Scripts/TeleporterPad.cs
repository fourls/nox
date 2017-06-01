using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeleporterPad : MonoBehaviour {

	public int connection;
	public int type; // 1=pad, 2=destination

	private List<TeleporterPad> toChangeBack = new List<TeleporterPad> ();

	// Use this for initialization
	void Start () {
		
	}

	public TeleporterPad GetDestination() {
		if (type == 1) {
			GameObject[] tps = GameObject.FindGameObjectsWithTag ("Teleporter");
			foreach (GameObject go in tps) {
				TeleporterPad tp = go.GetComponent<TeleporterPad> ();
				if (tp.connection == this.connection && tp.type == 2) {
					return tp;
				}
			}
		}

		return this;
	}

	public void Activate() {
		GetComponent<Animator> ().SetTrigger ("active");
	}

	public void ChangeColor (bool tored) {
		if (tored) {
			GetComponent<SpriteRenderer> ().color = Color.red;
		} else {
			GetComponent<SpriteRenderer> ().color = Color.white;
		}
	}

	void OnMouseEnter () {
		TeleporterPad[] connected = GameObject.FindObjectsOfType<TeleporterPad> ();
		toChangeBack.Clear ();

		foreach (TeleporterPad tp in connected) {
			if (tp.connection == connection) {
				tp.ChangeColor (true);
				toChangeBack.Add (tp);
			}
		}
	}

	void OnMouseExit () {
		foreach (TeleporterPad tp in toChangeBack) {
			tp.ChangeColor (false);
		}
		toChangeBack.Clear ();
	}
}
