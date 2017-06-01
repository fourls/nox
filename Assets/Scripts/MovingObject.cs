using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;
	public float maxDistance = 10f;

	private Rigidbody2D rb2D;
	private Collider2D coll;

	protected Coroutine currentMovement = null;
	protected Vector2 currentDirection = new Vector2();
	protected Vector2 lastDirection = new Vector2();

	protected bool moving = false;

	protected bool terminateMovement = false;

	protected virtual void Start () {
		rb2D = GetComponent<Rigidbody2D> ();
		coll = GetComponent<Collider2D> ();
	}
	
	protected bool Move (int xDir, int yDir, bool dontAddMoves, out RaycastHit2D hit) {
		Vector2 start = transform.position;
		Vector2 end = start + (new Vector2 (xDir, yDir) * maxDistance);
		float dist = Vector2.Distance (start, end);
		hit = Physics2D.Linecast (start + new Vector2(xDir,yDir), end, blockingLayer);
		Debug.DrawLine (start, end, Color.blue, 5f);

		if (hit.transform == null) {
			StartCoroutine (SmoothMovement (end, dist));
			OnCanMove (hit, dontAddMoves);
			return true;
		} else {
			if (xDir < 0)
				hit.point = new Vector2(Mathf.Floor (hit.point.x),hit.point.y);
			else if (xDir > 0)
				hit.point = new Vector2(Mathf.Ceil (hit.point.x),hit.point.y);
			if (yDir < 0)
				hit.point = new Vector2(hit.point.x,Mathf.Floor (hit.point.y));
			else if (yDir > 0)
				hit.point = new Vector2(hit.point.x,Mathf.Ceil (hit.point.y));
			if (hit.collider.gameObject.layer == LayerMask.NameToLayer ("Blocking")) {
				end = (Vector2)hit.point - (new Vector2 (xDir, yDir));
			} else {
				end = (Vector2)hit.point;
			}
			dist = Vector2.Distance (start, end);
			Debug.DrawLine ((Vector2)hit.point - new Vector2 (0.25f, 0.25f), (Vector2)hit.point + new Vector2 (0.25f, 0.25f), Color.red, 5f);
			Debug.DrawLine (end - new Vector2 (0.25f, 0.25f), end + new Vector2 (0.25f, 0.25f), Color.green, 5f);
			currentMovement = StartCoroutine (SmoothMovement (end, dist));
			currentDirection = new Vector2(xDir,yDir);
			if (Vector2.Distance (start, end) > 0.5f) {
				OnCanMove (hit,dontAddMoves);
				return true;
			}

			return false;
		}

	}

	protected bool MoveToPosition (Vector2 position, bool addMoves) {
		Vector2 start = transform.position;
		Vector2 end = position;
		float dist = Vector2.Distance (start, end);
		RaycastHit2D hit = Physics2D.Linecast (start, end, blockingLayer);
		OnCanMove (hit,addMoves);

		StartCoroutine (SmoothMovement (end, dist));
		return true;

	}

	protected IEnumerator SmoothMovement (Vector3 end, float dist) {
		moving = true;
		float remainingDist = (transform.position - end).sqrMagnitude;
		float invMoveTime = (dist / (moveTime * dist));
		while (remainingDist > float.Epsilon) {
			Vector3 newPos = Vector3.MoveTowards (transform.position, end, invMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPos);
			remainingDist = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		lastDirection = currentDirection;
		currentDirection = Vector2.zero;
		OnCompleteMove ();
		moving = false;
	}

	protected void Reevaluate() {
		StopCoroutine (currentMovement);
		RaycastHit2D hit;
		Move ((int)currentDirection.x, (int)currentDirection.y, true, out hit);
	}

	protected void Reevaluate(Vector2 newDir) {
		StopCoroutine (currentMovement);
		RaycastHit2D hit;
		Move ((int)newDir.x, (int)newDir.y, true, out hit);
	}

	protected abstract void OnCompleteMove ();

	protected abstract void OnCantMove ();

	protected abstract void OnCanMove (RaycastHit2D hit, bool dontAddMoves);
}
