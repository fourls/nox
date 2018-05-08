using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour {

	public GameObject levelCardPrefab;
	public GameObject customLevelCardPrefab;
	public GameObject cardHolder;
	public GameObject customCardHolder;
	public GameObject gameManagerPrefab;
	public UIManager uiManager;

	private bool waitingForLevel = false;
	public static bool showingCustomLevelSelect = false;
	private LevelData waitingLevel = null;

	void Start () {
		// if this scene has been played before the intro, load the intro scene (this should not happen)
		if (GameObject.Find ("Permanent") == null) {
			SceneManager.LoadScene ("Introduction");
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);

		FileLoader.LoadData ();
		
		SoundManager.instance.PlayTrack ();

		// normal level retrieval
		for (int i = 0; i < LevelDetails.RetrieveAllLevels().Count; i++) {
			LevelData thisLevel = LevelDetails.RetrieveLevelInformation (i);
			int moves = -1;

			// if the player has already visited this level, fill in the amount of moves the player has completed it in
			if (FileLoader.savegame.visitedLevels.Contains (thisLevel)) {
				moves = FileLoader.savegame.visitedLevels [FileLoader.savegame.visitedLevels.IndexOf (thisLevel)].moves;
			}

			// create clickable button
			GameObject icon = (GameObject)Instantiate (levelCardPrefab);
			icon.transform.SetParent (cardHolder.transform, false);
			
			UIManager iconUIElements = icon.GetComponent<UIManager> ();

			iconUIElements.graphics["LevelNameText"].GetComponent<Text>().text = "FLOOR " + (i+1).ToString() + "\n" + thisLevel.name;
			
			if(moves == -1) { // if the player has never visited this level
				iconUIElements.graphics["ParText"].GetComponent<Text>().text = "-";
			} else  if (moves > thisLevel.par) { // if the player is above par
				iconUIElements.graphics["ParText"].GetComponent<Text>().text = "<color=red>" + moves + "</color> / " + thisLevel.par; 
			} else { // if the player's moves are below or at par
				iconUIElements.graphics["ParText"].GetComponent<Text>().text = moves + " / " + thisLevel.par; 
			}
			
			// open level on click
			icon.GetComponent<Button> ().onClick.AddListener (() => OpenLevel(thisLevel));

			// if the player can't access this level yet, the button won't work
			if (!GameManager.IsLevelAccessible(thisLevel)) {
				icon.GetComponent<Button> ().interactable = false;
			}
		}

		// custom level retrieval
		for (int i = 0; i < LevelDetails.Custom.RetrieveAllLevels().Count; i++) {
			LevelData thisLevel = LevelDetails.Custom.RetrieveLevelInformation (i);
			int moves = -1;

			// if the player has already visited this level, fill in the amount of moves the player has completed it in
			if (FileLoader.savegame.playerVisitedLevels.Contains (thisLevel)) {
				moves = FileLoader.savegame.playerVisitedLevels [FileLoader.savegame.playerVisitedLevels.IndexOf (thisLevel)].moves;
			}

			// create clickable button
			GameObject icon = (GameObject)Instantiate (customLevelCardPrefab);
			icon.transform.SetParent (customCardHolder.transform, false);
			
			UIManager iconUIElements = icon.GetComponent<UIManager> ();

			iconUIElements.graphics["LevelNameText"].GetComponent<Text>().text = "FLOOR " + (i+1).ToString() + "\n" + thisLevel.name;
			
			if(moves == -1) { // if the player has never visited this level
				iconUIElements.graphics["ParText"].GetComponent<Text>().text = "-";
			} else  if (moves > thisLevel.par) { // if the player is above par
				iconUIElements.graphics["ParText"].GetComponent<Text>().text = "<color=red>" + moves + "</color> / " + thisLevel.par; 
			} else { // if the player's moves are below or at par
				iconUIElements.graphics["ParText"].GetComponent<Text>().text = moves + " / " + thisLevel.par; 
			}
			
			// open level on click
			icon.GetComponent<Button> ().onClick.AddListener (() => OpenLevel(thisLevel));
			UIManager lcui = icon.GetComponent <UIManager>();

			// edit / delete level on click
			lcui.graphics["EditButton"].GetComponent<Button>().onClick.AddListener (() => EditLevel(thisLevel));
			lcui.graphics["DeleteButton"].GetComponent<Button>().onClick.AddListener (() => DeleteLevel(thisLevel));
		}

		// add listeners for all the UI elements
		uiManager.graphics ["OptionsButton"].GetComponent<Button> ().onClick.AddListener (EnableOptions);
		uiManager.graphics ["QuitButton"].GetComponent<Button> ().onClick.AddListener (ExitGame);
		uiManager.graphics ["GuideButton"].GetComponent<Button> ().onClick.AddListener (OpenGuide);
		uiManager.graphics ["CreditsButton"].GetComponent<Button> ().onClick.AddListener (EnableCredits);
		uiManager.graphics ["CreatorButton"].GetComponent<Button> ().onClick.AddListener (EnableLevelCardsPanel);

		// add listeners for the options
		UIManager optionsUI = uiManager.graphics["OptionsPanel"].GetComponent<UIManager>();
		optionsUI.graphics["ResetGameButton"].GetComponent<Button>().onClick.AddListener (DestroyGameFile);
		optionsUI.graphics["BackButton"].GetComponent<Button>().onClick.AddListener (DisableOptions);

		optionsUI.graphics["FXVolumeSlider"].GetComponent<Slider>().onValueChanged.AddListener (delegate { ChangeSoundVolume (optionsUI.graphics["FXVolumeSlider"].GetComponent<Slider>()); });
		optionsUI.graphics["FXVolumeSlider"].GetComponent<Slider>().value = FileLoader.settings.fxVolume;
		optionsUI.graphics["MusicVolumeSlider"].GetComponent<Slider>().onValueChanged.AddListener (delegate { ChangeMusicVolume (optionsUI.graphics["MusicVolumeSlider"].GetComponent<Slider>()); });
		optionsUI.graphics["MusicVolumeSlider"].GetComponent<Slider>().value = FileLoader.settings.musicVolume;

		// credits back button
		UIManager creditsUI = uiManager.graphics ["CreditsPanel"].GetComponent<UIManager> ();
		creditsUI.graphics ["BackButton"].GetComponent<Button> ().onClick.AddListener (DisableCredits);

		// user made levels back/new level
		UIManager userLevelsUI = uiManager.graphics ["UserMadeLevelsPanel"].GetComponent<UIManager> ();

		userLevelsUI.graphics ["NewLevelButton"].GetComponent<Button> ().onClick.AddListener (OpenLevelCreator);
		userLevelsUI.graphics ["BackButton"].GetComponent<Button> ().onClick.AddListener (DisableLevelCardsPanel);
	}

	void EditLevel (LevelData li) {
		OpenLevelCreator ();
		LevelCreatorManager.editing = li;
	}

	void DeleteLevel (LevelData li) {
		FileLoader.DeleteCustomLevel (li);
		ReloadCustomLevels ();
	}

	void ChangeSoundVolume (Slider slider) {
		SoundManager.instance.fxVol = slider.value;
		FileLoader.settings.fxVolume = slider.value;
		FileLoader.SaveData ();
	}

	void ChangeMusicVolume(Slider slider) {
		SoundManager.instance.musicVol = slider.value;
		FileLoader.settings.musicVolume = slider.value;
		FileLoader.SaveData ();
	}

	// options

	void EnableOptions () {
		StartCoroutine(TransitionManager.instance.TransitionScene (
			null,
			delegate { uiManager.graphics["OptionsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (true); },
			null
		));
	}

	void DisableOptions () {
		StartCoroutine(TransitionManager.instance.TransitionScene (
			null,
			delegate { uiManager.graphics["OptionsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (false); },
			null
		));
	}


	// credits
	void EnableCredits () {
		StartCoroutine(TransitionManager.instance.TransitionScene (
			null,
			delegate { uiManager.graphics["CreditsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (true); },
			null
		));
	}

	void DisableCredits () {
		StartCoroutine(TransitionManager.instance.TransitionScene (
			null,
			delegate { uiManager.graphics["CreditsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (false); },
			null
		));
	}

	// level creator
	void EnableLevelCardsPanel () {
		StartCoroutine(TransitionManager.instance.TransitionScene (
			null,
			delegate { 
				uiManager.graphics["UserMadeLevelsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (true);
				uiManager.graphics["CreatorButton"].SetActive(false);
				LevelSelectManager.showingCustomLevelSelect = true; 
			},
			null
		));
	}

	void DisableLevelCardsPanel () {
		StartCoroutine(TransitionManager.instance.TransitionScene (
			null,
			delegate { 
				uiManager.graphics["UserMadeLevelsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (false);
				uiManager.graphics["CreatorButton"].SetActive(true);
				LevelSelectManager.showingCustomLevelSelect = false;
			},
			null
		));
	}

	void DestroyGameFile () {
		StartCoroutine(TransitionManager.instance.TransitionScene (
			null,
			delegate { 
				FileLoader.EraseGame ();
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				Destroy (gameObject);
			},
			null
		));
	}
	
	void ReloadCustomLevels () {
		StartCoroutine(TransitionManager.instance.TransitionScene (
			null,
			delegate { 
				showingCustomLevelSelect = true;
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				Destroy (gameObject);
			},
			null
		));
	}

	void OpenGuide () {
		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene(
			null,
			delegate {
				SceneManager.LoadScene ("Guide");
				Destroy (gameObject);
			}
			,null
		));
	}
	
	void OpenLevelCreator () {
		LevelCreatorManager.editing = null;
		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene(
			null,
			delegate {
				SceneManager.LoadScene ("LevelCreator");
				Destroy (gameObject);
			}
			,null
		));
	}

	void OpenLevel (LevelData levelInfo) {
		waitingLevel = levelInfo;
		LevelCreatorManager.editing = null;
		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene(
			null,
			delegate {
				SceneManager.LoadScene ("Main");
				waitingForLevel = true;
			}
			,delegate { Destroy(gameObject); }
		));
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
		if (waitingForLevel) {
			GameObject gm = GameObject.FindGameObjectWithTag ("GameManager");
			gm.GetComponent<GameManager> ().level = waitingLevel;
			gm.GetComponent<GameManager> ().InitialiseLevel ();
			waitingForLevel = false;
		}
	}

	void ExitGame() {
		Application.Quit();
	}

	void OnEnable() {
	//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable() {
	//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}
}
