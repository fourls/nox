using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LevelData {
	public string name = "";
	public int moves = 0;
	public int par = -1;
	public int index;
	public bool custom = false;
	public List<string> map = new List<string> ();

	public LevelData(bool c, int i, string n, int p, List<string> m) {
		name = n;
		par = p;
		map = m;
		index = i;
		custom = c;
	}

	public LevelData(bool c, string n, int p, int m) {
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

		LevelData sv = (LevelData)obj;
		if ((System.Object)sv == null)
			return false;
		return name == sv.name && par == sv.par;
	}
	public bool Equals(LevelData sv) {
		if ((object)sv == null) {
			return false;
		}
		return name == sv.name && par == sv.par;
	}
	public override int GetHashCode () {
		return name.GetHashCode() ^ moves.GetHashCode();
	}
}
