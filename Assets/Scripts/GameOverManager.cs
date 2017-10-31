using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {
	Button	ButtonHome;
	Button	ButtonRetry;

	void Start () {
		if (AppSupervisor.mapToLoad == null) {
			AppSupervisor.InitializeGame ();
		}

		ButtonHome = GameObject.Find("ButtonHome").GetComponent<Button>();
		ButtonHome.onClick.AddListener( () => {
			ButtonHomeOnClickEvent();
		});
		ButtonRetry = GameObject.Find("Retry").GetComponent<Button>();
		ButtonRetry.onClick.AddListener( () => {
			ButtonRetryOnClickEvent();
		});
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}

	void ButtonRetryOnClickEvent() {
		AppSupervisor.LoadMap ();
	}
}
