using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelDetails {


	public static LevelInformation RetrieveLevelInformation (int index) {
		List<string> lines = LoadLevelList ();

		string l = lines [index];

		List<string> fileLines = LoadLevelFile (l);
		List<string> m = fileLines.GetRange (3, 10);
		m.Reverse (); // fileLines[3] to fileLines[12]
		return new LevelInformation (false, index, fileLines [0], int.Parse (fileLines [1]), m);
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
		List<string> lines = new List<string> (levelFile.text.Split ('\n'));
		return lines;
	}

	public static List<LevelInformation> RetrieveAllLevels () {
		List<LevelInformation> levels = new List<LevelInformation>();

		List<string> lines = LoadLevelList ();
		for(int i = 0; i < lines.Count; i++) {
			string l = lines[i];
			List<string> fileLines = LoadLevelFile (l);
			List<string> m = fileLines.GetRange (3, 10); // fileLines[3] to fileLines[12]
			levels.Add (new LevelInformation (false,i,fileLines [0], int.Parse (fileLines [1]), m));
		}

		return levels;
	}


	// CUSTOM LEVELS
	public class Custom {
		public static LevelInformation RetrieveLevelInformation (int index) {
			List<string> lines = LoadLevelList ();

			string l = lines [index];

			List<string> fileLines = LoadLevelFile (l);
			List<string> m = fileLines.GetRange (3, 10);
			m.Reverse (); // fileLines[3] to fileLines[12]
			return new LevelInformation (true,index, fileLines [0], int.Parse (fileLines [1]), m);
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

		public static List<LevelInformation> RetrieveAllLevels () {
			List<LevelInformation> levels = new List<LevelInformation>();

			List<string> lines = LoadLevelList ();
			for(int i = 0; i < lines.Count; i++) {
				string l = lines[i];
				List<string> fileLines = LoadLevelFile (l);
				List<string> m = fileLines.GetRange (3, 10); // fileLines[3] to fileLines[12]
				levels.Add (new LevelInformation (true,i,fileLines [0], int.Parse (fileLines [1]), m));
			}

			return levels;
		}
	}
}

[System.Serializable]
public class LevelInformation {
	public string name = "";
	public int moves = 0;
	public int par = -1;
	public int index;
	public bool custom = false;
	public List<string> map = new List<string> ();

	public LevelInformation(bool c, int i, string n, int p, List<string> m) {
		name = n;
		par = p;
		map = m;
		index = i;
		custom = c;
	}

	public LevelInformation(bool c, string n, int p, int m) {
		name = n;
		par = p;
		moves = m;
		custom = c;
	}


	public bool Complete() {
		if (moves <= par && (moves != 0 && par != -1)) {
			return true;
		}
		return false;
	}
		
	public override bool Equals(System.Object obj) {
		if (obj == null) {
			return false;
		}

		LevelInformation sv = (LevelInformation)obj;
		if ((System.Object)sv == null)
			return false;
		return name == sv.name && par == sv.par;
	}
	public bool Equals(LevelInformation sv) {
		if ((object)sv == null) {
			return false;
		}
		return name == sv.name && par == sv.par;
	}
	public override int GetHashCode () {
		return name.GetHashCode() ^ moves.GetHashCode();
	}
}
