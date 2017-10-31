using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour 
{
	private static MusicManager instance = null;

	public static MusicManager Instance {
		get { return instance; }
	}

	void Awake() 
	{
		if (instance != null && instance != this) 
		{
			if(instance.GetComponent<AudioSource>().clip != gameObject.GetComponent<AudioSource>().clip)
			{
				instance.GetComponent<AudioSource>().clip = gameObject.GetComponent<AudioSource>().clip;
				instance.GetComponent<AudioSource>().volume = gameObject.GetComponent<AudioSource>().volume;
				instance.GetComponent<AudioSource>().Play();
			}

			Destroy(this.gameObject);
			return;
		} 
		instance = this;
		gameObject.GetComponent<AudioSource>().Play ();
		DontDestroyOnLoad(this.gameObject);
	}
}