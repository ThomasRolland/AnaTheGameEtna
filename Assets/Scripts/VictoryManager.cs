using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour {
	Button	ButtonHome;
	Button	ButtonContinue;

	void Start () {
		if (AppSupervisor.mapToLoad == null) {
			AppSupervisor.InitializeGame ();
		}

		ButtonHome = GameObject.Find("ButtonHome").GetComponent<Button>();
		ButtonHome.onClick.AddListener( () => {
			ButtonHomeOnClickEvent();
		});
		ButtonContinue = GameObject.Find("ButtonContinuer").GetComponent<Button>();
		ButtonContinue.onClick.AddListener( () => {ButtonContinueOnClickEvent();} );
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}

	void ButtonContinueOnClickEvent() {
		if (AppSupervisor.origin == 0) {
			SceneManager.LoadScene ("ListeTranscriptes");
		}
		else if (AppSupervisor.origin == 1) {
			SceneManager.LoadScene ("NoteHistoire");
		}
		else {
			SceneManager.LoadScene ("ShowScore");
		}
	}
}
