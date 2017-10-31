using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ListeNotesManager : MonoBehaviour {
	private int				noteDiscoved;
	private GameObject[]	respawns;
	Button					ButtonHome;

	void Start () {
		if (AppSupervisor.mapToLoad == null) {
			AppSupervisor.InitializeGame ();
		}

		ButtonHome = GameObject.Find("ButtonHome").GetComponent<Button>();
		ButtonHome.onClick.AddListener( () => {
			ButtonHomeOnClickEvent();
		});
		noteDiscoved = AppSupervisor.noteDiscovered;
		if (respawns == null) {
			respawns = GameObject.FindGameObjectsWithTag ("GoToNote");
		}
		for (int i = 0; i < respawns.Length; i++) {
			if (Convert.ToInt32(respawns[i].name) <= noteDiscoved) {
				int j = Convert.ToInt32 (respawns [i].name);
				respawns[i].GetComponent<Button> ().onClick.AddListener( () => {
					ButtonReadNoteOnClickEvent(j);
				});
			} else {
				respawns[i].GetComponent<Text> ().text = "????????";
			}
		}
	}

	void ButtonReadNoteOnClickEvent(int j) {
		AppSupervisor.noteToLoad = j-1;
		SceneManager.LoadScene ("Note");
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}
}
