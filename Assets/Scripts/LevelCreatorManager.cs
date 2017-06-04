using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LevelCreatorManager : MonoBehaviour {

	public static LevelCreatorManager instance = null;

	public UIManager ui;

	public Transform boardHolder;
	public GameObject objectPrefab;
	public GameObject toolbarButtonPrefab;
	public GameObject selectPrefab;
	public LayerMask blockingLayer;

	public Sprite[] objectSprites;
	public Sprite[] altObjectSprites;
	public string[] toolNames;

	public bool saved = true;

	public static LevelInformation editing = null;

	private int tool = 1;
	private bool anti = false;
	private Direction dir = Direction.Up;
	private int connection = 0;

	private float clickCooldown = 0.05f;
	private float lastClicked = 0f;

	private LevelCreatorObject selected;

	private List<GameObject> toolbarOptions;
	private GameObject toolbarSelect = null;

	private string lastTitleText = "";
	private string lastParText = "";

	void Start () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		lastClicked = Time.time;

		toolbarOptions = new List<GameObject>();
		toolbarOptions.Add(null);
		toolbarSelect = (GameObject)Instantiate(selectPrefab);

		int i = 0;
		foreach(Sprite sp in objectSprites) {
			if(i > 0) {
				GameObject icon = (GameObject)Instantiate (toolbarButtonPrefab);
				icon.transform.SetParent (ui.graphics["toolbar"].transform, false);
				int temp = i;
				icon.GetComponent<Button> ().onClick.AddListener (() => UpdateTool(temp,icon));
				icon.GetComponent<Image>().sprite = sp;
				toolbarOptions.Add(icon);

				if(i == 1) {
					toolbarSelect.transform.SetParent(icon.transform, false);
				}
			}
			i++;
		}


		UpdateToolWindow ();

		if (editing != null) {
			LoadSavedLevel ();
		}
	}

	void LoadSavedLevel () {
		int x = 0;
		int y = 0;

		LevelInformation level = editing;
		ui.graphics["titleField"].GetComponent<InputField>().text = level.name.ToString();
		ui.graphics["parField"].GetComponent<InputField>().text = level.par.ToString();

		foreach(string line in level.map) {
			string[] blockIDs = line.Split ('|');

			for (int e = 1; e < blockIDs.Length - 1; e++) {
				int space = -1;
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

				Vector2 roundedPos = new Vector2(x,y);
				if(space > -1) {
					GameObject go = (GameObject)Instantiate (objectPrefab, roundedPos, Quaternion.identity);
					LevelCreatorObject lco = go.GetComponent<LevelCreatorObject> ();
					// Vector2 p,int t, int c, Direction d, bool a
					lco.Initialise(roundedPos, space, connection, dir, anti);
				}

				x++;
			}
			
			y++;
			x = 0;	
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Tab)) {
			Deselected ();
			int wantedTool = tool + 1;
			if (wantedTool > 10) {
				wantedTool = 1;
			}
			UpdateTool (wantedTool,null);
		}
		if (ui.graphics ["titleField"].GetComponent<InputField> ().text != lastTitleText || ui.graphics ["parField"].GetComponent<InputField> ().text != lastParText) {
			saved = false;
		}
		if (selected != null) {
			GiveAttributesToObject (selected, selected.type, selected.position);
		}
		if (lastClicked + clickCooldown <= Time.time) {
			if (Input.GetMouseButton (0) && tool != 0) {
				lastClicked = Time.time;

				Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Collider2D[] colliders = Physics2D.OverlapPointAll (pos, blockingLayer);
				bool skip = false;

				foreach (Collider2D coll in colliders) {
					if (coll.CompareTag ("LevelCreatorObject")) {
						/*if (Input.GetKey (KeyCode.LeftShift)) {
							LevelCreatorObject lco = coll.GetComponent<LevelCreatorObject> ();
							selected = lco;
							lco.GetComponent<LevelCreatorObject> ().Selected ();
							ui.graphics["antiButton"].GetComponent<LevelCreatorButton> ().ChangeValueTo ((lco.anti) ? 1 : 0);
							if (lco.connection != 0)
								ui.graphics["connButton"].GetComponent<LevelCreatorButton> ().ChangeValueTo (lco.connection - 1);
					
							int d = 0; // up down left right
							switch (lco.dir) {
							case Direction.Up:
								d = 0;
								break;
							case Direction.Down:
								d = 1;
								break;
							case Direction.Left:
								d = 2;
								break;
							case Direction.Right:
								d = 3;
								break;
							}
							ui.graphics["dirButton"].GetComponent<LevelCreatorButton> ().ChangeValueTo (d);

							UpdateToolWindow ();
						}*/
						skip = true;
					}
				}

				Vector2 roundedPos = new Vector2 (Mathf.Floor (pos.x + 0.5f), Mathf.Floor (pos.y + 0.5f));

				if (roundedPos.x >= 0 && roundedPos.x < 10 && roundedPos.y >= 0 && roundedPos.y < 10) {
					if (selected && !skip) { 
						Deselected ();
					}
					if (selected == null && !Input.GetKey (KeyCode.LeftShift) && !skip) {
						saved = false;
						GameObject go = (GameObject)Instantiate (objectPrefab, roundedPos, Quaternion.identity);
						LevelCreatorObject lco = go.GetComponent<LevelCreatorObject> ();
						GiveAttributesToObject (lco, tool, roundedPos);
						go.transform.parent = boardHolder;
					}
				}
			} else if (Input.GetMouseButton (1)) {
				lastClicked = Time.time;
				if (selected == null) {
					Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					Collider2D[] colliders = Physics2D.OverlapPointAll (pos, blockingLayer);

					foreach (Collider2D coll in colliders) {
						if (coll.CompareTag ("LevelCreatorObject")) {
							coll.GetComponent<LevelCreatorObject> ().OnDestroy ();
							Destroy (coll.gameObject);
							saved = false;
						}
					}
				}
				Deselected ();
			}
		}
	}

	void GiveAttributesToObject(LevelCreatorObject lco, int type, Vector2 pos) {
		bool a = (ui.graphics["antiButton"].GetComponentInChildren<Text> ().text == "on") ? false : true;
		Direction d = Direction.Up;
		switch (ui.graphics["dirButton"].GetComponentInChildren<Text> ().text) {
		case "up":
			d = Direction.Up;
			break;
		case "down":
			d = Direction.Down;
			break;
		case "left":
			d = Direction.Left;
			break;
		case "right":
			d = Direction.Right;
			break;
		}
		int c = int.Parse (ui.graphics["connButton"].GetComponentInChildren<Text> ().text);

		lco.Initialise (pos, type, c, d, a);
		saved = false;
	}

	public void LevelCompleteClicked () {
		if (CreateLevelText ()) {
			QuitClicked ();
		}
	}

	public bool CreateLevelText() {
		string title = ui.graphics["titleField"].GetComponent<InputField>().text;
		string par = ui.graphics["parField"].GetComponent<InputField>().text;
		lastTitleText = title;
		lastParText = par;

		string[,] map = new string[10,10];

		LevelCreatorObject[] objects = GameObject.FindObjectsOfType<LevelCreatorObject>();

		bool hasPlayer = false;
		bool hasExit = false;

		foreach (LevelCreatorObject o in objects) {
			string s = "";

			if (o.type == 2)
				hasPlayer = true;
			if (o.type == 3)
				hasExit = true;

			if(o.type == 6 || o.type == 7 || o.type == 9 || o.type == 10) {
				switch(o.connection) {
				case 1:
					s += "[";
					break;
				case 2:
					s += "(";
					break;
				case 3:
					s += "{";
					break;
				}
			}

			if(o.type == 8) {
				switch(o.dir) {
				case Direction.Up:
					s += "u";
					break;
				case Direction.Down:
					s += "d";
					break;
				case Direction.Left:
					s += "l";
					break;
				case Direction.Right:
					s += "r";
					break;
				}
			}

			s += o.type.ToString();

			if(o.type == 7 && o.anti) {
				s += "!";
			}

			s = s.PadRight (3);

			map[(int)o.position.x,(int)o.position.y] = s;
		}

		int xd = 999;

		if (!hasPlayer || !hasExit || title == "" || int.TryParse (par, out xd) == false) {
			ChangeLevelCompleteText ();
			Invoke ("ChangeBackLevelCompleteText", 0.5f);
			return false;
		}

		List<string> levelText = new List<string> ();

		levelText.Add (title);
		levelText.Add (par);
		levelText.Add ("");

		List<string> mapText = new List<string>();


		for (int y = 0; y < 10; y++) {
			string text = "|";
			for (int x = 0; x < 10; x++) {
				if (map [x, y] != null) {
					text += map [x, y] + "|";
				} else {
					text += " - |";
				}
			}
			mapText.Add(text);
		}

		mapText.Reverse ();

		if (!Directory.Exists (Application.persistentDataPath + "/maps")) {
			Directory.CreateDirectory (Application.persistentDataPath + "/maps");
		}
		
		
		if(!File.Exists(Application.persistentDataPath + "/maps/map_" + title + ".txt")) {
			StreamWriter indexWriter = new StreamWriter (Application.persistentDataPath + "/maps/index.txt",true);
			indexWriter.WriteLine(title);
			indexWriter.Close ();
		}

		StreamWriter writer = new StreamWriter (Application.persistentDataPath + "/maps/map_" + title + ".txt");

		foreach (string l in levelText) {
			writer.WriteLine (l);
		}

		foreach (string l in mapText) {
			writer.WriteLine (l);
		}

		writer.Close ();

		saved = true;
		return true;
	}

	void ChangeLevelCompleteText () {
		ui.graphics ["CompleteButton"].GetComponentInChildren<Text> ().text = "make sure your level fulfils all requirements";
	}

	void ChangeBackLevelCompleteText () {
		ui.graphics ["CompleteButton"].GetComponentInChildren<Text> ().text = "this level is complete";
	}

	public void UpdateTool(int no, GameObject icon) {
		Debug.Log("UpdateTool called, no = " + no);
		if (no <= 10) {
			tool = no;
		}
		if(toolbarOptions.Count > tool) {
			toolbarSelect.transform.SetParent(toolbarOptions[tool].transform,false);
		}
		UpdateToolWindow ();
	}

	void Deselected () {
		if (selected != null) {
			selected.Deselected ();
			selected = null;
			UpdateToolWindow ();
		}
	}

	void UpdateToolWindow() {
		if (selected == null) {
			ui.graphics["toolText"].GetComponent<Text>().text = toolNames [tool];
		} else {
			ui.graphics["toolText"].GetComponent<Text>().text = "<color=grey>editing</color> " + toolNames [selected.type];
		}
		switch (tool) {
		case 0:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(false);
			break;
		case 1:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(false);
			break;
		case 2:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(false);
			break;
		case 3:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(false);
			break;
		case 4:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(false);
			break;
		case 5:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(false);
			break;
		case 6:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(true);
			break;
		case 7:
			ui.graphics["antiButton"].gameObject.SetActive(true);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(true);
			break;
		case 8:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(true);
			ui.graphics["connButton"].gameObject.SetActive(false);
			break;
		case 9:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(true);
			break;
		case 10:
			ui.graphics["antiButton"].gameObject.SetActive(false);
			ui.graphics["dirButton"].gameObject.SetActive(false);
			ui.graphics["connButton"].gameObject.SetActive(true);
			break;
		}
	}

	public void ReturnToMenu () {
		if (!saved) {
			ui.graphics ["Popup"].SetActive (true);
		} else {
			QuitClicked ();
		}
	}

	public void QuitClicked() {
		TransitionManager.instance.StartCoroutine (TransitionManager.instance.TransitionScene (null, MiddleMenuTransFn, null));
	}

	public void CancelClicked() {
		ui.graphics["Popup"].SetActive(false);
	}

	void MiddleMenuTransFn() {
		LevelSelectManager.showingCustomLevelSelect = true;
		UnityEngine.SceneManagement.SceneManager.LoadScene ("LevelSelect");
		Destroy (gameObject);
	}
}
