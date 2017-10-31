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

public class NoteManager : MonoBehaviour {
	Button				ButtonHome;
	Button				ButtonListeNote;
	public static Text	noteText;
	public static Text	noteTitle;
	private int			noteNumber;

	void Start () {
		if (AppSupervisor.mapToLoad == null) {
			AppSupervisor.InitializeGame ();
		}
		ButtonHome = GameObject.Find("ButtonHome").GetComponent<Button>();
		ButtonListeNote = GameObject.Find("ButtonListeNote").GetComponent<Button>();
		ButtonHome.onClick.AddListener( () => {
			ButtonHomeOnClickEvent();
		});
		ButtonListeNote.onClick.AddListener( () => {
			ButtonListeNoteOnClickEvent();
		});
		noteNumber = AppSupervisor.noteToLoad;
		InitializeView (noteNumber);
	}

	public static void InitializeView(int noteNumber) {
		noteText = GameObject.Find("NoteLabel").GetComponent<Text>();
		noteTitle = GameObject.Find("Title").GetComponent<Text>();

		Note note = InitializeNotes(noteNumber);
		noteText.text = GetNote (note, noteNumber);
		noteTitle.text = "Note numero " + (noteNumber + 1).ToString();
	}

	public static Note InitializeNotes(int noteNumber) {
		TextAsset temp = Resources.Load("Notes") as TextAsset;            
		XmlDocument _doc = new XmlDocument();
		var myreader = temp.text;
		byte[] byteArray = Encoding.UTF8.GetBytes(myreader);
		MemoryStream stream = new MemoryStream(byteArray);
		var serializer = new XmlSerializer(typeof(Notes));
		var defaults = (Notes)serializer.Deserialize(stream);
		return (defaults.Note[noteNumber]);
	}

	public static string GetNote(Note note, int id) {
		return (note.String.ToString());
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}

	void ButtonListeNoteOnClickEvent() {
		SceneManager.LoadScene ("ListeNotes");
	}

}
