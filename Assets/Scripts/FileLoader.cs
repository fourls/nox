using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class FileLoader {

	public static GameData savegame = new GameData();
	public static GameSettings settings = new GameSettings();


	public static void SaveLevelData (LevelInformation level) {
		if (level.custom) {
			if (level.Complete ()) {
				savegame.CustomLevelCompletedBelowPar (level);
			} else {
				savegame.CustomLevelCompletedAbovePar (level);
			}
		} else {
			if (level.Complete ()) {
				savegame.LevelCompletedBelowPar (level);
			} else {
				savegame.LevelCompletedAbovePar (level);
			}
		}
		GameData data = savegame;
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/savedata.dat");
		bf.Serialize (file, data);
		file.Close ();
	}

	public static void SaveData () {
		GameData data = savegame;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/savedata.dat");
		bf.Serialize (file, data);
		file.Close ();

		GameSettings gdata = settings;

		FileStream gfile = File.Create (Application.persistentDataPath + "/settings.dat");
		bf.Serialize (gfile, gdata);
		gfile.Close ();
	}

	public static void LoadData () {
		if (File.Exists (Application.persistentDataPath + "/savedata.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/savedata.dat", FileMode.Open);
			try {
				savegame = (GameData)bf.Deserialize (file);
			} catch (System.Runtime.Serialization.SerializationException) {
				EraseGame ();
			}
			file.Close ();
			CheckSaveGameForUpdates ();
		} else {
			EraseGame ();
		}
		if (File.Exists (Application.persistentDataPath + "/settings.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/settings.dat", FileMode.Open);
			settings = (GameSettings)bf.Deserialize (file);
			file.Close ();
		} else {
			EraseSettings ();
		}
	}

	public static void EraseGame () {
		ResetSaveGame ();
		File.Delete (Application.persistentDataPath + "/savedata.dat");
		SaveData ();
	}

	public static void EraseSettings () {
		settings = new GameSettings ();
		File.Delete (Application.persistentDataPath + "/settings.dat");
		SaveData ();
	}

	public static void CheckSaveGameForUpdates () {
		List<LevelInformation> levelInfo = LevelDetails.RetrieveAllLevels ();

		foreach (LevelInformation li in savegame.visitedLevels) {
			if (levelInfo.Contains (li)) {
				li.index = levelInfo.IndexOf (li);
			}
		}
	}

	private static void ResetSaveGame() {
		savegame = new GameData ();
	}

	public static void DeleteCustomLevel(LevelInformation li) {
		if (File.Exists (Application.persistentDataPath + "/maps/map_" + li.name + ".txt"))
			File.Delete (Application.persistentDataPath + "/maps/map_" + li.name + ".txt");

		StreamReader sr = new StreamReader (Application.persistentDataPath + "/maps/index.txt");
		List<string> indexFile = new List<string> ();

		while (sr.Peek () != -1) {
			indexFile.Add (sr.ReadLine ());
		}

		for (int i = 0; i < indexFile.Count; i++) {
			if (indexFile [i] == li.name) {
				indexFile.Remove (li.name);
			}
		}

		sr.Close ();

		StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/maps/index.txt");

		for (int i = 0; i < indexFile.Count; i++) {
			sw.WriteLine (indexFile [i]);
		}

		sw.Close ();
	}

	/*
	private static List<LevelInformation> LIListtoPLDList() {
		List<LevelInformation> levelInformation = LevelDetails.RetrieveAllLevels();
		List<LevelInformation> playerLevelData = new List<LevelInformation> ();

		foreach (LevelInformation li in levelInformation) {
			playerLevelData.Add (new LevelInformation (li.name,-1));
		}

		return playerLevelData;
	}*/

	/*
	public static void EraseLevel (int levID, List<string> levelfiles) {
		string level = LoadLevelFile(levelfiles [levID])[0];
		string par = LoadLevelFile(levelfiles [levID])[1];
		if (savegame.levels.Contains (new GameLevel (level, 0))) {
			if (savegame.levels[savegame.levels.IndexOf (new GameLevel (level, 0))].moves <= int.Parse(par)) {
				savegame.levelsCompleted--;
			}
			savegame.levels.Remove (new GameLevel (level, 0));
			SaveData ();
		}
	}*/
}

[System.Serializable]
public class GameData {
	public List<LevelInformation> visitedLevels = new List<LevelInformation>();
	public List<LevelInformation> playerVisitedLevels = new List<LevelInformation>();

	public GameData () {
	}

	public void LevelCompletedBelowPar (LevelInformation gv) {
		LevelCompletedAbovePar (gv);
		if (!visitedLevels.Contains (gv)) {
			visitedLevels.Add (gv);
		} else {
			visitedLevels [visitedLevels.IndexOf (gv)].moves = gv.moves;
		}
	}

	public void LevelCompletedAbovePar (LevelInformation gv) {
		if (!visitedLevels.Contains (gv)) {
			visitedLevels.Add (gv);
		} else if (visitedLevels [visitedLevels.IndexOf (gv)].moves > gv.moves || visitedLevels[visitedLevels.IndexOf(gv)].moves == -1) {
			visitedLevels [visitedLevels.IndexOf (gv)] = gv;
		}
	}

	public void CustomLevelCompletedBelowPar (LevelInformation gv) {
		CustomLevelCompletedAbovePar (gv);
		if (!playerVisitedLevels.Contains (gv)) {
			playerVisitedLevels.Add (gv);
		} else {
			playerVisitedLevels [playerVisitedLevels.IndexOf (gv)].moves = gv.moves;
		}
	}

	public void CustomLevelCompletedAbovePar (LevelInformation gv) {
		if (!visitedLevels.Contains (gv)) {
			playerVisitedLevels.Add (gv);
		} else if (playerVisitedLevels [playerVisitedLevels.IndexOf (gv)].moves > gv.moves || playerVisitedLevels[playerVisitedLevels.IndexOf(gv)].moves == -1) {
			playerVisitedLevels [visitedLevels.IndexOf (gv)] = gv;
		}
	}
}

[System.Serializable]
public class GameSettings {
	public float musicVolume;
	public float fxVolume;

	public GameSettings () {
		musicVolume = 0.5f;
		fxVolume = 1f;
	}
	public GameSettings (float m, float f) {
		musicVolume = m;
		fxVolume = f;
	}
}

public enum Direction {Up,Down,Left,Right};