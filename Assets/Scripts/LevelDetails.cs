using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelDetails {
	public static LevelData RetrieveLevelInformation (int index) {
		List<string> lines = LoadLevelList ();

		string l = lines [index];

		List<string> fileLines = LoadLevelFile (l);
		List<string> m = fileLines.GetRange (3, 10);
		m.Reverse (); // fileLines[3] to fileLines[12]
		return new LevelData (false, index, fileLines [0], int.Parse (fileLines [1]), m);
	} 
		
	private static List<string> LoadLevelList () {
		TextAsset levelFile = (TextAsset)Resources.Load ("Levels/index", typeof(TextAsset));
		if (levelFile == null)
			Debug.LogError ("Cannot find index file.");
		List<string> lines = new List<string> (levelFile.text.Split ('\n'));
		return lines;
	}
		
	public static List<string> LoadLevelFile (string str) {
		TextAsset levelFile = (TextAsset)Resources.Load ("Levels/" + str, typeof(TextAsset));
		if (levelFile == null)
			Debug.LogError("Cannot find level file <" + str + ">");
		List<string> lines = new List<string> (levelFile.text.Split ('\n'));
		return lines;
	}

	public static List<LevelData> RetrieveAllLevels () {
		List<LevelData> levels = new List<LevelData>();

		List<string> lines = LoadLevelList ();
		for(int i = 0; i < lines.Count; i++) {
			string l = lines[i];
			List<string> fileLines = LoadLevelFile (l);
			List<string> m = fileLines.GetRange (3, 10); // fileLines[3] to fileLines[12]
			levels.Add (new LevelData (false,i,fileLines [0], int.Parse (fileLines [1]), m));
		}

		return levels;
	}


	// CUSTOM LEVELS
	public class Custom {
		public static LevelData RetrieveLevelInformation (int index) {
			List<string> lines = LoadLevelList ();

			string l = lines [index];

			List<string> fileLines = LoadLevelFile (l);
			List<string> m = fileLines.GetRange (3, 10);
			m.Reverse (); // fileLines[3] to fileLines[12]
			return new LevelData (true,index, fileLines [0], int.Parse (fileLines [1]), m);
		} 

		private static List<string> LoadLevelList () {
			if (!File.Exists (Application.persistentDataPath + "/maps/index.txt"))
				return new List<string> ();
			
			StreamReader sr = new StreamReader (Application.persistentDataPath + "/maps/index.txt");
			List<string> lines = new List<string> ();
			while (sr.Peek () != -1) {
				lines.Add (sr.ReadLine ());
			}

			sr.Close ();
			return lines;
		}

		public static List<string> LoadLevelFile (string str) {
			if (!File.Exists (Application.persistentDataPath + "/maps/index.txt"))
				Debug.LogError ("Level file does not exist");

			StreamReader sr = new StreamReader (Application.persistentDataPath + "/maps/map_" + str + ".txt");

			List<string> lines = new List<string> ();

			while (sr.Peek () != -1) {
				lines.Add (sr.ReadLine ());
			}

			sr.Close ();
			return lines;
		}

		public static List<LevelData> RetrieveAllLevels () {
			List<LevelData> levels = new List<LevelData>();

			List<string> lines = LoadLevelList ();
			for(int i = 0; i < lines.Count; i++) {
				string l = lines[i];
				List<string> fileLines = LoadLevelFile (l);
				List<string> m = fileLines.GetRange (3, 10); // fileLines[3] to fileLines[12]
				levels.Add (new LevelData (true,i,fileLines [0], int.Parse (fileLines [1]), m));
			}

			return levels;
		}
	}
}
