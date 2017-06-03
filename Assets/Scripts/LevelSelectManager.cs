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
	private LevelInformation waitingLevel = null;

	void Start () {
		if (GameObject.Find ("Permanent") == null) {
			SceneManager.LoadScene ("Introduction");
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
		FileLoader.LoadData ();
		SoundManager.instance.PlayTrack ();

		// normal level retrieval

		for (int i = 0; i < LevelDetails.RetrieveAllLevels().Count; i++) {
			LevelInformation li = LevelDetails.RetrieveLevelInformation (i);
			string levelname = li.name;
			int moves = -1;
			int par = li.par;
			if (FileLoader.savegame.visitedLevels.Contains (li)) {
				moves = FileLoader.savegame.visitedLevels [FileLoader.savegame.visitedLevels.IndexOf (li)].moves;
			}
			GameObject icon = (GameObject)Instantiate (levelCardPrefab);
			icon.transform.SetParent (cardHolder.transform, false);
			UIManager icui = icon.GetComponent<UIManager> ();
			icui.graphics["LevelNameText"].GetComponent<Text>().text = "FLOOR " + (i+1).ToString() + "\n" + levelname;
			if(moves == -1) {
				icui.graphics["ParText"].GetComponent<Text>().text = "-";
			} else {
				if (moves > par) {
					icui.graphics["ParText"].GetComponent<Text>().text = "<color=red>" + moves + "</color> / " + par; 
				} else {
					icui.graphics["ParText"].GetComponent<Text>().text = moves + " / " + par; 
				}
			}
			icon.GetComponent<Button> ().onClick.AddListener (() => OpenLevel(li));
			if (!GameManager.IsLevelAccessible(li)) {
				icon.GetComponent<Button> ().interactable = false;
			}
		}

		// custom level retrieval
		
		for (int i = 0; i < LevelDetails.Custom.RetrieveAllLevels().Count; i++) {
			LevelInformation li = LevelDetails.Custom.RetrieveLevelInformation (i);
			string levelname = li.name;
			int moves = -1;
			int par = li.par;
			Debug.Log (FileLoader.savegame.ToString ());
			Debug.Log (FileLoader.savegame.playerVisitedLevels.ToString ());
			if (FileLoader.savegame.playerVisitedLevels.Contains (li)) {
				moves = FileLoader.savegame.playerVisitedLevels [FileLoader.savegame.playerVisitedLevels.IndexOf (li)].moves;
			}

			GameObject icon = (GameObject)Instantiate (customLevelCardPrefab);
			icon.transform.SetParent (customCardHolder.transform, false);
			UIManager icui = icon.GetComponent<UIManager> ();
			icui.graphics["LevelNameText"].GetComponent<Text>().text = "FLOOR " + (i+1).ToString() + "\n" + levelname;
			if(moves == -1) {
				icui.graphics["ParText"].GetComponent<Text>().text = "-";
			} else {
				if (moves > par) {
					icui.graphics["ParText"].GetComponent<Text>().text = "<color=red>" + moves + "</color> / " + par; 
				} else {
					icui.graphics["ParText"].GetComponent<Text>().text = moves + " / " + par; 
				}
			}
			icon.GetComponent<Button> ().onClick.AddListener (() => OpenLevel(li));
			UIManager lcui = icon.GetComponent <UIManager>();

			lcui.graphics["EditButton"].GetComponent<Button>().onClick.AddListener (() => EditLevel(li));
			lcui.graphics["DeleteButton"].GetComponent<Button>().onClick.AddListener (() => DeleteLevel(li));
		}

		uiManager.graphics ["OptionsButton"].GetComponent<Button> ().onClick.AddListener (EnableOptions);
		uiManager.graphics ["QuitButton"].GetComponent<Button> ().onClick.AddListener (ExitGame);
		uiManager.graphics ["GuideButton"].GetComponent<Button> ().onClick.AddListener (OpenGuide);
		uiManager.graphics ["CreditsButton"].GetComponent<Button> ().onClick.AddListener (EnableCredits);
		uiManager.graphics ["CreatorButton"].GetComponent<Button> ().onClick.AddListener (EnableLevelCardsPanel);


		UIManager opui = uiManager.graphics["OptionsPanel"].GetComponent<UIManager>();

		opui.graphics["ResetGameButton"].GetComponent<Button>().onClick.AddListener (DestroyGameFile);
		opui.graphics["BackButton"].GetComponent<Button>().onClick.AddListener (DisableOptions);

		opui.graphics["FXVolumeSlider"].GetComponent<Slider>().onValueChanged.AddListener (delegate { ChangeSoundVolume (opui.graphics["FXVolumeSlider"].GetComponent<Slider>()); });
		opui.graphics["FXVolumeSlider"].GetComponent<Slider>().value = FileLoader.settings.fxVolume;
		opui.graphics["MusicVolumeSlider"].GetComponent<Slider>().onValueChanged.AddListener (delegate { ChangeMusicVolume (opui.graphics["MusicVolumeSlider"].GetComponent<Slider>()); });
		opui.graphics["MusicVolumeSlider"].GetComponent<Slider>().value = FileLoader.settings.musicVolume;

		UIManager crui = uiManager.graphics ["CreditsPanel"].GetComponent<UIManager> ();

		crui.graphics ["BackButton"].GetComponent<Button> ().onClick.AddListener (DisableCredits);

		UIManager ulcui = uiManager.graphics ["UserMadeLevelsPanel"].GetComponent<UIManager> ();

		ulcui.graphics ["NewLevelButton"].GetComponent<Button> ().onClick.AddListener (OpenLevelCreator);
		ulcui.graphics ["BackButton"].GetComponent<Button> ().onClick.AddListener (DisableLevelCardsPanel);


		if (showingCustomLevelSelect)
			MiddleEnableLevelCardsPanel ();
		else
			MiddleDisableLevelCardsPanel ();
	}

	void EditLevel (LevelInformation li) {
		OpenLevelCreator ();
		LevelCreatorManager.editing = li;
	}

	void DeleteLevel (LevelInformation li) {
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
		StartCoroutine(TransitionManager.instance.TransitionScene (null,MiddleEnableOptions,null));
	}

	void MiddleEnableOptions() {
		uiManager.graphics["OptionsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (true);
	}

	void DisableOptions () {
		StartCoroutine(TransitionManager.instance.TransitionScene (null,MiddleDisableOptions,null));
	}

	void MiddleDisableOptions() {
		uiManager.graphics["OptionsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (false);
	}

	// credits

	void EnableCredits () {
		StartCoroutine(TransitionManager.instance.TransitionScene (null,MiddleEnableCredits,null));
	}

	void MiddleEnableCredits() {
		uiManager.graphics["CreditsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (true);
	}

	void DisableCredits () {
		StartCoroutine(TransitionManager.instance.TransitionScene (null,MiddleDisableCredits,null));
	}

	void MiddleDisableCredits() {
		uiManager.graphics["CreditsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (false);
	}

	// level creator

	void EnableLevelCardsPanel () {
		StartCoroutine(TransitionManager.instance.TransitionScene (null,MiddleEnableLevelCardsPanel,null));
	}

	void MiddleEnableLevelCardsPanel() {
		uiManager.graphics["UserMadeLevelsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (true);
		LevelSelectManager.showingCustomLevelSelect = true;
	}

	void DisableLevelCardsPanel () {
		StartCoroutine(TransitionManager.instance.TransitionScene (null,MiddleDisableLevelCardsPanel,null));
	}

	void MiddleDisableLevelCardsPanel() {
		uiManager.graphics["UserMadeLevelsPanel"].GetComponent<UIManager>().graphics["Container"].SetActive (false);
		LevelSelectManager.showingCustomLevelSelect = false;
	}

	void DestroyGameFile () {
		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene(null,MiddleDestroyGameFile,null));
	}

	void MiddleDestroyGameFile() {
		FileLoader.EraseGame ();
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		Destroy (gameObject);
	}

	void ReloadCustomLevels () {
		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene(null,MiddleReloadCustomLevels,null));
	}

	void MiddleReloadCustomLevels() {
		showingCustomLevelSelect = true;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		Destroy (gameObject);
	}

	void OpenGuide () {
		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene(null,MiddleOpenGuide,null));
	}

	void MiddleOpenGuide() {
		SceneManager.LoadScene ("Guide");
		Destroy (gameObject);
	}

	void OpenLevelCreator () {
		LevelCreatorManager.editing = null;
		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene(null,MiddleOpenLevelCreator,null));
	}

	void MiddleOpenLevelCreator() {
		SceneManager.LoadScene ("LevelCreator");
		Destroy (gameObject);
	}

	void OpenLevel (LevelInformation i) {
		waitingLevel = i;
		StartCoroutine (TransitionManager.instance.TransitionScene (null,MiddleTransitionFn,EndTransitionFn));
	}

	void MiddleTransitionFn() {
		SceneManager.LoadScene ("Main");
		waitingForLevel = true;
	}

	void EndTransitionFn() {
		Destroy (gameObject);
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

	void OnEnable()
	{
	//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
	//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}
}
