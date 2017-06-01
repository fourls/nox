using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public GameObject playerPrefab;
	public GameObject wallPrefab;
	public GameObject exitPrefab;
	public GameObject breakablePrefab;
	public GameObject deadlyPrefab;
	public GameObject switchPrefab;
	public GameObject doorPrefab;
	public GameObject conveyorPrefab;
	public GameObject teleporterPadPrefab;
	public GameObject teleporterDestinationPrefab;

	public Transform boardHolder;
	public UIManager ui;

	public LevelInformation level;
	private bool levelWon = false;
	public bool levelPlaying = false;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (this);

		if (GameObject.Find ("Permanent") == null) {
			SceneManager.LoadScene ("Introduction");
			Destroy (gameObject);
		}
	}

	public void InitialiseLevel () {
		LoadLevel ();
		levelWon = false;
		levelPlaying = true;
		ui.graphics["LevelNameText"].GetComponent<UnityEngine.UI.Text>().text = "floor " + (level.index+1).ToString() + ": " + level.name;
		SoundManager.instance.PlayTrack ();
	}

	void Update () {
		if (levelWon && IsNextLevelAccessible(level)) {
			if ((Input.GetKeyUp (KeyCode.Return) || Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.N) ))
				NextLevel ();
		}
		if (Input.GetKeyUp (KeyCode.R) || Input.GetKeyUp (KeyCode.Backspace))
			RetryLevel ();
		if (Input.GetKeyUp (KeyCode.Escape) || Input.GetKeyUp (KeyCode.M))
			ReturnToMenu ();
	}

	void LoadLevel () {
	int x = 0;
	int y = 0;

		foreach(string line in level.map) {
			string[] blockIDs = line.Split ('|');

			for (int e = 1; e < blockIDs.Length - 1; e++) {
				int space = 0;
				blockIDs [e].Trim ();

				int connection = 0;
				Direction dir = Direction.Up;
				bool anti = false;

				if (blockIDs [e].Contains ("[")) {
					connection = 1;
					blockIDs[e] = blockIDs [e].Replace ("[", "");
				}
				if (blockIDs [e].Contains ("(")) {
					connection = 2;
					blockIDs[e] = blockIDs [e].Replace ("(", "");
				}
				if (blockIDs [e].Contains ("{")) {
					connection = 3;
					blockIDs[e] = blockIDs [e].Replace ("{", "");
				}
				if (blockIDs [e].Contains ("!")) {
					anti = true;
					blockIDs[e] = blockIDs [e].Replace ("!", "");
				}
				if (blockIDs [e].Contains ("d")) {
					dir = Direction.Down;
					blockIDs[e] = blockIDs [e].Replace ("d", "");
				}
				if (blockIDs [e].Contains ("u")) {
					dir = Direction.Up;
					blockIDs[e] = blockIDs [e].Replace ("u", "");
				}
				if (blockIDs [e].Contains ("l")) {
					dir = Direction.Left;
					blockIDs[e] = blockIDs [e].Replace ("l", "");
				}
				if (blockIDs [e].Contains ("r")) {
					dir = Direction.Right;
					blockIDs[e] = blockIDs [e].Replace ("r", "");
				}

				if (!blockIDs [e].Contains ("-")) {
					space = int.Parse (blockIDs [e]);
				}

				if (space != 0) {
					if (space == 1) {
						GameObject go = (GameObject)Instantiate (wallPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
					} else if (space == 2) {
						GameObject go = (GameObject)Instantiate (playerPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
					} else if (space == 3) {
						GameObject go = (GameObject)Instantiate (exitPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
					} else if (space == 4) {
						GameObject go = (GameObject)Instantiate (breakablePrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
					} else if (space == 5) {
						GameObject go = (GameObject)Instantiate (deadlyPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
					} else if (space == 6) {
						GameObject go = (GameObject)Instantiate (switchPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
						go.GetComponent<Electronic> ().connection = connection;
						if (anti)
							go.GetComponent<Electronic> ().Swap ();
					} else if (space == 7) {
						GameObject go = (GameObject)Instantiate (doorPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
						go.GetComponent<Electronic> ().connection = connection;
						if (anti)
							go.GetComponent<Electronic> ().Swap ();
					} else if (space == 8) {
						GameObject go = (GameObject)Instantiate (conveyorPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
						go.GetComponent<Conveyor> ().SetDirection (dir);
					} else if (space == 9) {
						GameObject go = (GameObject)Instantiate (teleporterPadPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
						go.GetComponent<TeleporterPad> ().connection = connection;
					} else if (space == 10) {
						GameObject go = (GameObject)Instantiate (teleporterDestinationPrefab, new Vector2 (x, y), Quaternion.identity);
						go.transform.SetParent (boardHolder);
						go.GetComponent<TeleporterPad> ().connection = connection;
					}
				}

				x++;
			}
			
			y++;
			x = 0;	
		}
	}

	public void UpdatePar (int moves) {
		if (moves > level.par) {
			ui.graphics["ParText"].GetComponent<UnityEngine.UI.Text>().text = "<color=\"red\">" + moves + "</color> / " + level.par;
		} else {
			ui.graphics["ParText"].GetComponent<UnityEngine.UI.Text>().text = moves + " / " + level.par;
		}
	}

	public void RetryLevel () {
		ResetScene ();
	}

	public void WinLevel (int moves) {
		levelWon = true;
		UIManager winUI = ui.graphics["WinMenu"].GetComponent<UIManager> ();
		winUI.graphics ["Container"].SetActive (true);
		winUI.graphics["LevelNameText"].GetComponent<UnityEngine.UI.Text>().text = level.name;
		if (moves > level.par) {
			winUI.graphics["ParText"].GetComponent<UnityEngine.UI.Text>().text = "<color=\"red\">" + moves + "</color> / " + level.par;
			SoundManager.instance.PlayTune (SoundManager.instance.failure);
		} else {
			winUI.graphics["ParText"].GetComponent<UnityEngine.UI.Text>().text = moves + " / " + level.par;
			SoundManager.instance.PlayTune (SoundManager.instance.success);
		}

		level.moves = moves;
		FileLoader.SaveLevelData (level);

		if (!IsNextLevelAccessible (level)) {
			winUI.graphics ["NextButton"].GetComponent<UnityEngine.UI.Button> ().interactable = false;
		} else {
			winUI.graphics ["NextButton"].GetComponent<UnityEngine.UI.Button> ().interactable = true;
		}
	}

	public void NextLevel () {
		if (!level.custom) {
			if (level.index + 1 < LevelDetails.RetrieveAllLevels ().Count) {
				level = LevelDetails.RetrieveLevelInformation (level.index + 1);
				ResetScene ();
			} else {
				ReturnToMenu ();
			}
		} else {
			if (level.index + 1 < LevelDetails.Custom.RetrieveAllLevels ().Count) {
				level = LevelDetails.Custom.RetrieveLevelInformation (level.index + 1);
				ResetScene ();
			} else {
				ReturnToMenu ();
			}
		}
	}

	void ResetScene() {
		for (int i = 0; i < boardHolder.childCount; i++) {
			Destroy(boardHolder.GetChild(i).gameObject);
		}

		StartCoroutine (TransitionManager.instance.TransitionScene (null,MiddleResetTransFn,EndResetTransFn));
	}

	void MiddleResetTransFn() {
		UIManager winUI = ui.graphics["WinMenu"].GetComponent<UIManager> ();
		winUI.graphics ["Container"].SetActive (false);
		InitialiseLevel ();
		levelPlaying = false;
	}

	void EndResetTransFn() {
		levelPlaying = true;
	}

	public void ReturnToMenu () {
		StartCoroutine (TransitionManager.instance.TransitionScene (null, MiddleMenuTransFn, EndMenuTransFn));
	}

	void MiddleMenuTransFn() {
		LevelSelectManager.showingCustomLevelSelect = false;
		SceneManager.LoadScene ("LevelSelect");
	}

	void EndMenuTransFn() {
		Destroy (gameObject);
	}

	public void ExitGame () {
		Application.Quit ();
		Debug.LogWarning ("The application has quit.");
	}

	public static bool IsLevelAccessible(LevelInformation target) {
		if (target.custom)
			return true;
		if (target.index < 1) {
			return true;
		} else {
			LevelInformation l = LevelDetails.RetrieveLevelInformation (target.index - 1);
			return IsNextLevelAccessible (l);
		}
	}	// IsLevelAccessible() is not working - IsNextLevelAccessible() is.

	public static bool IsNextLevelAccessible(LevelInformation level) {
		if (level.custom)
			return true;
		if (FileLoader.savegame.visitedLevels.Contains(level)) {
			if (FileLoader.savegame.visitedLevels [FileLoader.savegame.visitedLevels.IndexOf (level)].Complete ()) {
				return true;
			}
		}
		return false;
	}
}
