using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighscoresManager : MonoBehaviour {
	Button	ButtonHome;

	void Start () {
		if (AppSupervisor.mapToLoad == null) {
			AppSupervisor.InitializeGame ();
		}

		ButtonHome = GameObject.Find("ButtonHome").GetComponent<Button>();
		ButtonHome.onClick.AddListener( () => {
			ButtonHomeOnClickEvent();
		});
		InitializeScores ();
	}

	public void InitializeScores() {
		string score;
		string player;
		for (int i = 1; i <= 10; i++) {
			if (AppSupervisor.highscores[i-1] != 0) {
				score = AppSupervisor.highscores [i-1].ToString();
				player = "Vous";
			} else  {
				score = "??????";
				player = "Aucun";
			}
			GameObject.Find("Score" + i).GetComponent<Text>().text = player;
			GameObject.Find("Score" + i + " (1)").GetComponent<Text>().text = score;
		}
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}
}
