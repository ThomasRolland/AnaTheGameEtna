using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
	Button	ButtonNote;
	Button	ButtonTranscripte;
	Button	ButtonRegles;
	Button	ButtonScore;
	Button	ButtonStart;

	void Start() {
		if (AppSupervisor.mapToLoad == null)
			AppSupervisor.InitializeGame ();
	}

	void Awake() {
		ButtonNote = GameObject.Find("ButtonNote").GetComponent<Button>();
		ButtonTranscripte = GameObject.Find("ButtonTranscripte").GetComponent<Button>();
		ButtonRegles = GameObject.Find("ButtonRegles").GetComponent<Button>();
		ButtonScore = GameObject.Find("ButtonScores").GetComponent<Button>();
		ButtonStart = GameObject.Find("ButtonStart").GetComponent<Button>();

		ButtonNote.onClick.AddListener( () => {
			ButtonNoteOnClickEvent();
		});
		ButtonTranscripte.onClick.AddListener( () => {
			ButtonTranscripteOnClickEvent();
		});
		ButtonRegles.onClick.AddListener( () => {
			ButtonReglesOnClickEvent();
		});
		ButtonScore.onClick.AddListener( () => {
			ButtonScoreOnClickEvent();
		});
		ButtonStart.onClick.AddListener( () => {
			ButtonStartOnClickEvent();
		});
	}

	void ButtonNoteOnClickEvent() {
		SceneManager.LoadScene ("ListeNotes");
	}

	//A changer : deplacer camera
	void ButtonTranscripteOnClickEvent() {
		SceneManager.LoadScene ("ListeTranscriptes");
	}

	void ButtonStartOnClickEvent() {
		SceneManager.LoadScene ("ChooseMode");
	}

	void ButtonReglesOnClickEvent() {
		SceneManager.LoadScene ("Regles");
	}

	void ButtonScoreOnClickEvent() {
		SceneManager.LoadScene ("Highscores");
	}
}
