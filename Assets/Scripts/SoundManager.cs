using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource musicSource;
	public AudioSource tuneSource;
	public AudioSource fxSource;

	public AudioClip enterlevel;
	public AudioClip locked;
	public AudioClip success;
	public AudioClip failure;
	public AudioClip death;
	public AudioClip breakable;
	public AudioClip switchsound;
	public AudioClip conveyor;
	public AudioClip teleporter;

	public float fxVol = 1f;
	public float musicVol = 0.5f;
	public float fxVolMultiplier = 1f;
	public float musicVolMultiplier = 0.1f;

	public static SoundManager instance = null;

	void Start () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			this.enabled = false;
	}

	void Update () {
		musicSource.volume = musicVol * musicVolMultiplier;
		fxSource.volume = fxVol * fxVolMultiplier;
		tuneSource.volume = fxVol * fxVolMultiplier;
	}

	public void PlayTrack () {
		musicSource.UnPause ();
		if (!musicSource.isPlaying) {
			musicSource.Play ();
		}
	}

	public void PauseTrack () {
		if (musicSource.isPlaying) {
			musicSource.Pause ();
		}
	}

	public void StopTrack () {
		musicSource.volume = musicVol;
		if (musicSource.isPlaying) {
			musicSource.Stop ();
		}
	}

	public void PlayTune (AudioClip clip) {
		tuneSource.pitch = 1f;
		tuneSource.PlayOneShot (clip);
	}

	public void PlaySound(AudioClip clip) {
		fxSource.pitch = 1f;
		fxSource.PlayOneShot (clip);
	}

	public void PlaySoundPitch(AudioClip clip) {
		fxSource.pitch = Random.Range (0.95f, 1.05f);
		fxSource.PlayOneShot (clip);
	}
}
