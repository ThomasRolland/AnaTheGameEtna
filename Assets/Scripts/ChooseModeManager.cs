using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseModeManager : MonoBehaviour {

	Button	ButtonHome;
	Button	ButtonHistory;
	Button	ButtonInfini;

	void Start () {
		if (AppSupervisor.mapToLoad == null) {
			AppSupervisor.InitializeGame ();
		}

		ButtonHome = GameObject.Find("ButtonHome").GetComponent<Button>();
		ButtonHome.onClick.AddListener( () => {
			ButtonHomeOnClickEvent();
		});
		ButtonHistory = GameObject.Find("ButtonHistory").GetComponent<Button>();
		ButtonHistory.onClick.AddListener( () => {
			ButtonHistoryOnClickEvent();
		});
		ButtonInfini = GameObject.Find("ButtonInfini").GetComponent<Button>();

		if (AppSupervisor.inifinitMode == 0) {
			ButtonInfini.GetComponent<Button> ().interactable = false;
			GameObject.Find("ButtonInfini/GameObject/Text").GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.2f);
		} else {
			ButtonInfini.GetComponent<Button> ().interactable =  true;
			GameObject.Find("ButtonInfini/GameObject/Text").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
			ButtonInfini.onClick.AddListener( () => {ButtonInfiniOnClickEvent();} );
		}
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}

	void ButtonHistoryOnClickEvent() {
		SceneManager.LoadScene ("NoteHistoire");
	}

	void ButtonInfiniOnClickEvent() {
		int map = Random.Range (1, 23);
		AppSupervisor.randomMap = map;
		AppSupervisor.GetOneMap(map);
		AppSupervisor.origin = 2;
		SceneManager.LoadScene ("InfiniteMode");
	}
}
