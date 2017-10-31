using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Xml2CSharp;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class GameMessagesAccessor {

	public static Texts texts = InitializeTexts();

	// Use this for initialization
	void Start () {
		InitializeTexts ();
	}
	
	public static Texts InitializeTexts() {
		TextAsset temp = Resources.Load("GameTexts") as TextAsset;            
		XmlDocument _doc = new XmlDocument();
		var myreader = temp.text;
		byte[] byteArray = Encoding.UTF8.GetBytes(myreader);
		MemoryStream stream = new MemoryStream(byteArray);
		var serializer = new XmlSerializer(typeof(Texts));
		var defaults = (Texts)serializer.Deserialize(stream);
		return defaults;
	}

	public static int GetRandom () {
		int random = Random.Range (0, 6);
		return (random);
	}
		
	public static string GetStartText() {
		string startText = "Haha ! Si tu veux la sauver, tu dois gagner cette partie de dominos !";
		return (startText);
	}

	public static string GetEndText() {
		string endText = "Tu t'en tires bien, mais ne te réjouis pas trop vite...";
		return (endText);
	}

	public static string GetSelectDominoText() {
		string endText = "Eh ! Sélectionne un domino !";
		return (endText);
	}

	public static string GetAlreadyPlayedText() {
		string endText = "Tu as déjà joué Miller !";
		return (endText);
	}

	public static string GetNotPlayedText() {
		string endText = "Il faut que tu joues avant de te défausser Miller !";
		return (endText);
	}

	public static string GetGameOverText() {
		int random = GetRandom ();
		return (texts.GameOver.String[random]);
	}

	public static string GetInGameIsLosingText() {
		int random = GetRandom ();
		return (texts.InGameIsLosing.String[random]);
	}

	public static string GetInGameWinningText() {
		int random = GetRandom ();
		return (texts.InGameWinning.String[random]);
	}

	public static string GetInGameHasLooseText() {
		int random = GetRandom ();
		return (texts.HasLoose.String[random]);
	}

	public static string GetHasDrawText() {
		int random = GetRandom ();
		return (texts.HasDraw.String[random]);
	}
}
