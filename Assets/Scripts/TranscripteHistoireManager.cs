using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Xml2CSharp;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;

public class TranscripteHistoireManager : MonoBehaviour {
	Button					ButtonHome;
	public static Text		TranscripteText;
	public static Text		TranscripteTitle;
	public static Button	ButtonStartGame;

	void Start () {
		if (AppSupervisor.mapToLoad == null) {
			AppSupervisor.InitializeGame ();
		}

		InitializeView (AppSupervisor.transcripteToLoad);
		ButtonHome = GameObject.Find("ButtonHome").GetComponent<Button>();
		ButtonHome.onClick.AddListener( () => {
			ButtonHomeOnClickEvent();
		});
		ButtonStartGame.onClick.AddListener( () => {
			ButtonStartGameOnClickEvent();
		});
	}

	public static void InitializeView(int TranscripteNumber) {
		Transcripts transcripts = InitializeTranscriptes();
		Group Conversation = transcripts.Group [TranscripteNumber];
		GenerateTranscripte (Conversation);
	}

	public static Transcripts InitializeTranscriptes() {
		TextAsset temp = Resources.Load("Transcripts") as TextAsset;            
		XmlDocument _doc = new XmlDocument();
		var myreader = temp.text;
		byte[] byteArray = Encoding.UTF8.GetBytes(myreader);
		MemoryStream stream = new MemoryStream(byteArray);
		var serializer = new XmlSerializer(typeof(Transcripts));
		var defaults = (Transcripts)serializer.Deserialize(stream);
		return (defaults);
	}

	public static void GenerateTranscripte(Group Conversation) {
		string previousPerson = "";
		string prefab = "";
		int space = -50;
		int i = 0;

		foreach (var Dialog in Conversation.String) {

			if (Dialog.Category == "tueur") {
				if (previousPerson == "" || previousPerson == "miller") {
					prefab = "Prefab/BulleMessageTueurWithIcon";
					space -= 400;
				} else {
					prefab = "Prefab/BulleMessageTueur";
					space -= 300;
				}
				previousPerson = "tueur";
			} else {
				if (previousPerson == "" || previousPerson == "tueur") {
					prefab = "Prefab/BulleMessageMillerWithIcon";
					space -= 350;
				} else {
					prefab = "Prefab/BulleMessageMiller";
					space -= 300;
				}
				previousPerson = "miller";
			}
			GameObject go = Instantiate(Resources.Load(prefab) as GameObject);
			go.transform.SetParent(GameObject.Find("Content").transform);
			go.transform.position = new Vector3 (0, space, 0);
			go.name = "Bulle" + i;
			Text goText = GameObject.Find("Bulle" + i + "/Dialog").GetComponent<Text>();
			goText.text = Dialog.Text;
			i++;
		}
		createMessageStartGame (space);
	}

	public static void createMessageStartGame(int space) {
		space -= 350;

		//Transcriptes du début : pas de start Game scene
		if (AppSupervisor.level == 0) {
			GameObject go = Instantiate(Resources.Load("Prefab/BulleMessageStart") as GameObject);
			go.transform.SetParent(GameObject.Find("Content").transform);
			go.transform.position = new Vector3 (0, space, 0);
			ButtonStartGame = GameObject.Find("BulleMessageStart(Clone)").GetComponent<Button>();
		} else { //Sinon, afficher le bouton de début de Game scene
			GameObject go = Instantiate(Resources.Load("Prefab/BulleMessageStart") as GameObject);
			go.transform.SetParent(GameObject.Find("Content").transform);
			go.transform.position = new Vector3 (0, space, 0);
			ButtonStartGame = GameObject.Find("BulleMessageStart(Clone)").GetComponent<Button>();
		}
	}


	public void ButtonStartGameOnClickEvent() {
		if (AppSupervisor.level == 0) {
			if (AppSupervisor.noteDiscovered == 1) {
				AppSupervisor.UnlockNewLevel ();
			}
			AppSupervisor.UnlockNewStory ();
			AppSupervisor.noteToLoad = AppSupervisor.noteDiscovered;
			SceneManager.LoadScene ("NoteHistoire");
		} else {
			AppSupervisor.origin = 1;
			AppSupervisor.GetCurrentMap ();
			AppSupervisor.LoadMap ();
		}
	}


	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}
}