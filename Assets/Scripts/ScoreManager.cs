using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour {
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
		ButtonContinue.onClick.AddListener( () => {
			ButtonContinueOnClickEvent();
		});
		//Rempli les textes avec les scores :
		GameObject.Find("VictoryBonus").GetComponent<Text>().text = "Partie gagnee : " + AppSupervisor.infinitModeHasWon.ToString();
		GameObject.Find("Score1").GetComponent<Text>().text = "Dominos retournes : " + AppSupervisor.scoreReturned.ToString();
		GameObject.Find("Score2").GetComponent<Text>().text = "Dominos symetriques : " + AppSupervisor.scoreSymetric.ToString();
		GameObject.Find("Score3").GetComponent<Text>().text = "Combos : " + AppSupervisor.scoreCombo.ToString();
		GameObject.Find("Score4").GetComponent<Text>().text = "Bonus : " + AppSupervisor.score80.ToString();
		GameObject.Find("ScoreGame").GetComponent<Text>().text = AppSupervisor.score.ToString() + " points";
		GameObject.Find("ScoreTotal").GetComponent<Text>().text = AppSupervisor.scoreTotal.ToString() + " points";

		//Stocker le score
		AppSupervisor.highscores[AppSupervisor.highscoresCase] = AppSupervisor.scoreTotal;
		AppSupervisor.highscoresCase++;
		//Remet a 0 les scores bonus de la partie terminée pour la prochaine
		AppSupervisor.ResetInfinitModeGameScore ();
		//Si la partie est perdue, la progression totale de la session s'arette et est remise a zero
		//TODO : Stocker cette progression dans les highscores
		if (AppSupervisor.infinitModeHasWon == 0) {
			GameObject.Find("Title").GetComponent<Text>().text = "Défaite";
			AppSupervisor.ResetInfinitModeTotalSessionScore();
		}
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}

	void ButtonContinueOnClickEvent() {
		int map = Random.Range (1, 23);
		if (AppSupervisor.randomMap == map) {
			map = Random.Range (map, 23);
		}
		AppSupervisor.randomMap = map;
		AppSupervisor.origin = 2;
		AppSupervisor.GetOneMap(map);
		SceneManager.LoadScene ("InfiniteMode");
	}
}
