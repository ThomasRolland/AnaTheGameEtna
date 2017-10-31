using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ListeTranscriptesManager : MonoBehaviour {
	private int				transcripteDiscoved;
	private GameObject[]	respawns;
	Button					ButtonHome;

	// Use this for initialization
	void Start () {
		if (AppSupervisor.mapToLoad == null)
			AppSupervisor.InitializeGame ();

		ButtonHome = GameObject.Find ("ButtonHome").GetComponent<Button> ();
		ButtonHome.onClick.AddListener (() => {
			ButtonHomeOnClickEvent ();
		});

		if (respawns == null)
			respawns = GameObject.FindGameObjectsWithTag ("GoToTranscripte");

		for (int i = 0; i < respawns.Length; i++) {
			if (Convert.ToInt32 (respawns [i].name) <= AppSupervisor.level) {
				int j = Convert.ToInt32 (respawns [i].name);
				respawns [i].GetComponent<Button> ().onClick.AddListener (() => {
					ButtonReadTranscripteOnClickEvent (j);
				});
			} else {
				respawns [i].transform.GetChild (0).GetComponent<Text> ().text = "????????";
			}
		}
	}

	void ButtonReadTranscripteOnClickEvent(int j) {
		AppSupervisor.transcripteToLoad = j + 1;
		SceneManager.LoadScene ("Transcripte");
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}
}
