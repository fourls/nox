using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCreatorObject : MonoBehaviour {

	public int type;
	public int connection = 0;
	public Direction dir = Direction.Up;
	public bool anti = false;
	public Vector2 position = Vector2.zero;

	private List<LevelCreatorObject> toChangeBack = new List<LevelCreatorObject>();
	private bool selected = false;


	public void Initialise (Vector2 p,int t, int c, Direction d, bool a) {
		type = t;
		connection = c;
		dir = d;
		anti = a;
		position = p;

		GetComponent<SpriteRenderer> ().sprite = LevelCreatorManager.instance.objectSprites [t];
		SetDirection (dir);
		if (anti) {
			GetComponent<SpriteRenderer> ().sprite = LevelCreatorManager.instance.altObjectSprites[t];
		}
	}

	public void Selected () {
		selected = true;
		ChangeColor (true);
	}

	public void Deselected () {
		selected = false;
		ChangeColor (false);
	}

	void SetDirection(Direction dir) {
		if (dir == Direction.Up)
			transform.eulerAngles = Conveyor.upRot;
		else if (dir == Direction.Down)
			transform.eulerAngles = Conveyor.downRot;
		else if (dir == Direction.Left)
			transform.eulerAngles = Conveyor.leftRot;
		else if (dir == Direction.Right)
			transform.eulerAngles = Conveyor.rightRot;
	}



	public void ChangeColor (bool tored) {
		if (tored) {
			if (!selected) {
				GetComponent<SpriteRenderer> ().color = Color.red;
			} else {
				GetComponent<SpriteRenderer> ().color = Color.cyan;
			}
		} else {
			if (!selected) {
				GetComponent<SpriteRenderer> ().color = Color.white;
			} else {
				GetComponent<SpriteRenderer> ().color = Color.cyan;
			}
		}
	}

	void OnMouseEnter () {
		LevelCreatorObject[] connected = GameObject.FindObjectsOfType<LevelCreatorObject> ();
		toChangeBack.Clear ();

		foreach (LevelCreatorObject el in connected) {
			if (el.connection == connection) {
				if ((type == 6 || type == 7) && (el.type == 6 || el.type == 7)) {
					el.ChangeColor (true);
					toChangeBack.Add (el);
				} else if ((type == 9 || type == 10) && (el.type == 9 || el.type == 10)) {
					el.ChangeColor (true);
					toChangeBack.Add (el);
				}
			}
		}
	}

	void OnMouseExit () {
		foreach (LevelCreatorObject el in toChangeBack) {
			el.ChangeColor (false);
		}
		toChangeBack.Clear ();
	}

	public void OnDestroy() {
		foreach (LevelCreatorObject el in toChangeBack) {
			el.ChangeColor (false);
		}
		toChangeBack.Clear ();
	}
}
