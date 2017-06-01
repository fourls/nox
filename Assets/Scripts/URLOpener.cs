using UnityEngine;
using System.Collections;

public class URLOpener : MonoBehaviour {

	public void OpenURL(string url) {
		Application.OpenURL (url);
	}

}
