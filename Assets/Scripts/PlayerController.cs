using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MovingObject {

	public LayerMask invisiLayer; // the layer switch doors go on when inactive
	public float timeBeforeNextKeyPress = 0.1f; // the time before input axes are read again
	private float timeFinishedMove = -1f; // the time the last move was finished
	private bool freedom = true; // is the player free
	private int moves = 0; // the amount of moves the player has made

	private GameObject lastHit = null; // the last hit object, used in conveyor belts
	private GameObject tobedestroyed = null; // the object to be destroyed at the end of a move
	private TeleporterPad toTeleportTo = null;

	// Initialise
	protected override void Start () {
		GameManager.instance.UpdatePar (moves);
		base.Start ();
	}

	// Check for input
	void Update () {
		if (freedom && !moving && (Time.time - timeFinishedMove) > timeBeforeNextKeyPress && GameManager.instance.levelPlaying) {
			int moveVert = (int)Input.GetAxisRaw ("Vertical");
			int moveHoriz = (int)Input.GetAxisRaw ("Horizontal");

			if (moveHoriz != 0) {
				moveVert = 0;
			}

			MoveIfPossible (moveHoriz, moveVert,false);

		}
	}

	// Test if moving is possible, then move
	void MoveIfPossible (int moveHoriz, int moveVert, bool dontAddMoves) {
		if (moveHoriz != 0 || moveVert != 0) {
			LockPlayer ();
			RaycastHit2D hit;
			Move (moveHoriz, moveVert, dontAddMoves, out hit);
		}
	}

	// When the move is completed
	protected override void OnCompleteMove() {
		FreePlayer ();
		timeFinishedMove = Time.time;
		Collider2D hit = Physics2D.OverlapPoint(transform.position,blockingLayer);
		if (hit != null) {
			if (hit.gameObject != lastHit) {
				lastHit = hit.gameObject;
				if (hit.CompareTag ("Conveyor")) {
					Debug.Log ("Player has hit a conveyor belt.");
					Conveyor con = hit.GetComponent<Conveyor> ();
					Vector2 dir = Vector2.zero;
					if (con.pointing == Direction.Down)
						dir = Vector2.down;
					else if (con.pointing == Direction.Up)
						dir = Vector2.up;
					else if (con.pointing == Direction.Left)
						dir = Vector2.left;
					else if (con.pointing == Direction.Right)
						dir = Vector2.right;
					MoveIfPossible ((int)dir.x, (int)dir.y,true);
					SoundManager.instance.PlaySoundPitch (SoundManager.instance.conveyor);
				} else if (hit.CompareTag ("Deadly")) {
					LockPlayer ();
					GetComponent<SpriteRenderer> ().enabled = false;
					hit.gameObject.GetComponent<SpriteRenderer> ().color = Color.red;
					StopAllCoroutines ();
					Invoke ("RetryLevel", 0.5f);
					SoundManager.instance.PlayTune (SoundManager.instance.death);
				} else if (hit.CompareTag ("Exit")) {
					LockPlayer ();
					GameManager.instance.WinLevel (moves);
				} else if (hit.CompareTag("Teleporter")) {
					lastHit = null;
					LockPlayer ();
					TeleporterPad tp = hit.GetComponent<TeleporterPad> ();
					toTeleportTo = tp;
					tp.Activate ();
					SoundManager.instance.PlaySoundPitch (SoundManager.instance.teleporter);
					//GetComponent<SpriteRenderer> ().color = Color.clear;
					Invoke ("Teleport", 0.1f);
				} else if (hit.CompareTag ("Electronic")) {
					Electronic el = hit.GetComponent<Electronic> ();
					if (el.state) {
						if (el.type == 1) {
							SoundManager.instance.PlaySoundPitch (SoundManager.instance.switchsound);
							GameObject[] elecs = GameObject.FindGameObjectsWithTag ("Electronic");
							foreach (GameObject door in elecs) {
								Electronic del = door.GetComponent<Electronic> ();
								if (del.connection == el.connection && del.type == 2) {
									del.Swap ();
								}
							}
						}
					}
					MoveInLastDirection ();
				}
			}
		} else {
			lastHit = null;
		}
		if (tobedestroyed != null) {
			Destroy (tobedestroyed);
			tobedestroyed = null;
			SoundManager.instance.PlaySoundPitch (SoundManager.instance.breakable);
		}
	}

	private void Teleport() {
		TeleporterPad dest = toTeleportTo.GetDestination();
		transform.position = dest.transform.position;
		dest.Activate ();
		//GetComponent<SpriteRenderer> ().color = Color.white;
		Invoke ("MoveInLastDirection", 0.1f);
	}

	private void MoveInLastDirection () {
		MoveIfPossible ((int)lastDirection.x, (int)lastDirection.y,true);
	}

	protected override void OnCantMove() {
		FreePlayer ();
	}

	protected override void OnCanMove (RaycastHit2D hit, bool dontAddMoves) {
		if (!dontAddMoves) {
			moves++;
		}
		if (hit.transform) {
			if (hit.transform.CompareTag ("Breakable")) {
				tobedestroyed = hit.transform.gameObject;
			}
		}
		GameManager.instance.UpdatePar (moves);
	}

	public void FreePlayer () {
		freedom = true;
	}

	public void LockPlayer () {
		freedom = false;
	}


	void RetryLevel () {
		GameManager.instance.RetryLevel ();
	}
}
