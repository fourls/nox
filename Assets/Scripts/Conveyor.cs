using UnityEngine;
using System.Collections;

public class Conveyor : MonoBehaviour {

	public Direction pointing;

	public static Vector3 upRot = new Vector3(0,0,0);
	public static Vector3 downRot = new Vector3(0,0,180);
	public static Vector3 leftRot = new Vector3(0,0,90);
	public static Vector3 rightRot = new Vector3(0,0,-90);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetDirection(Direction dir) {
		pointing = dir;
		if (pointing == Direction.Up)
			transform.eulerAngles = upRot;
		else if (pointing == Direction.Down)
			transform.eulerAngles = downRot;
		else if (pointing == Direction.Left)
			transform.eulerAngles = leftRot;
		else if (pointing == Direction.Right)
			transform.eulerAngles = rightRot;
	}
}
