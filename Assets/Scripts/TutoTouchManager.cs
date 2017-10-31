using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutoTouchManager : MonoBehaviour {
	Button			ButtonNext;
	Image			myImageComponent;
	public int		i;
	public Sprite[]	images;

	void Start () {
		if (AppSupervisor.mapToLoad == null) {
			AppSupervisor.InitializeGame ();
		}

		myImageComponent = GameObject.Find("ButtonNext").GetComponent<Image>();
	}

	void Awake () {
		ButtonNext = GameObject.Find("ButtonNext").GetComponent<Button>();
		ButtonNext.onClick.AddListener( () => {ButtonNextOnClickEvent();} );
	}

	void ButtonNextOnClickEvent() {
		if (i == 7) {
			Debug.Log ("Fin");
			AppSupervisor.TutoIsSeen ();
			AppSupervisor.GetCurrentMap();
			AppSupervisor.LoadMap ();
		}
		myImageComponent.sprite = images[i];
		i++;
	}
}
