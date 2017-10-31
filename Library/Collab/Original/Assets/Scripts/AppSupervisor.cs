using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class AppSupervisor {
	/*Informations fixes*/
	static public int		level;
	static public int		tutoPassed;
	static public int		noteDiscovered;
	static public int		transcriptDiscovered;
	static public int		inifinitMode;
	static public int[]		highscores;
	static public int 		highscoresCase;

	/*Informations temporaires : passer les données entre les scenes*/
	static public int		noteToLoad;
	static public int		transcripteToLoad;
	static public string	mapToLoad;
	static public int		origin;
	/*Informations temportaires du mode infini : */
	static public int		randomMap;
	static public int		infinitModeHasWon;
	static public int		scoreReturned;
	static public int		scoreSymetric;
	static public int		scoreCombo;
	static public int		score80;
	static public int		score;
	static public int		scoreTotal;

	public static void InitializeGame() {
		//Initialiser les var temporaires a 0;
		AppSupervisor.noteToLoad = 0; 
		AppSupervisor.transcripteToLoad = 0;
		AppSupervisor.mapToLoad = "";
		AppSupervisor.origin = 0;
		AppSupervisor.randomMap = 0;
		AppSupervisor.infinitModeHasWon = 0;
		AppSupervisor.score80 = 0;
		AppSupervisor.scoreReturned = 0;
		AppSupervisor.scoreSymetric = 0;
		AppSupervisor.scoreCombo = 0;
		AppSupervisor.score = 0;
		AppSupervisor.scoreTotal = 0;
		AppSupervisor.highscores = new int[10];
		AppSupervisor.highscoresCase = 0;

		//Charger du fichier de sauvegarde les données :
		AppSupervisor.LoadData();
	}

	public static void StoreData() {
		PlayerPrefs.SetInt ("noteDiscovered", AppSupervisor.noteDiscovered);
		PlayerPrefs.SetInt ("transcriptDiscovered", AppSupervisor.transcriptDiscovered);
		PlayerPrefs.SetInt ("level", AppSupervisor.level);
		PlayerPrefs.SetInt ("tutoPassed", AppSupervisor.tutoPassed);
		PlayerPrefs.SetInt ("inifinitMode", AppSupervisor.inifinitMode);
	}

	public static void LoadData() {
		if (PlayerPrefs.HasKey ("noteDiscovered")) {
			AppSupervisor.noteDiscovered = PlayerPrefs.GetInt ("noteDiscovered");
		} else {
			AppSupervisor.noteDiscovered = 22;
		}
		if (PlayerPrefs.HasKey ("transcriptDiscovered")) {
			AppSupervisor.transcriptDiscovered = PlayerPrefs.GetInt ("transcriptDiscovered");
		} else {
			AppSupervisor.transcriptDiscovered = 22;
		}
		if (PlayerPrefs.HasKey ("level")) {
			AppSupervisor.level = PlayerPrefs.GetInt ("level");
		} else {
			AppSupervisor.level = 22;
		}
		if (PlayerPrefs.HasKey ("tutoPassed")) {
			AppSupervisor.tutoPassed = PlayerPrefs.GetInt ("tutoPassed");
		} else {
			AppSupervisor.tutoPassed = 1;
		}
		if (PlayerPrefs.HasKey ("inifinitMode")) {
			AppSupervisor.inifinitMode = PlayerPrefs.GetInt ("inifinitMode");
		} else {
			AppSupervisor.inifinitMode = 1;
		}
	}

	public static void UnlockNewLevel () {
		AppSupervisor.level++;
		AppSupervisor.StoreData ();
	}

	public static void UnlockNewStory () {
		AppSupervisor.noteDiscovered++;
		AppSupervisor.transcriptDiscovered++;
		AppSupervisor.StoreData ();
	}

	public static void TutoIsSeen () {
		AppSupervisor.tutoPassed++;
		AppSupervisor.StoreData ();
	}

	public static void ResetInfinitModeGameScore () {
		AppSupervisor.infinitModeHasWon = 0;
		AppSupervisor.score80 = 0;
		AppSupervisor.scoreReturned = 0;
		AppSupervisor.scoreSymetric = 0;
		AppSupervisor.scoreCombo = 0;
		AppSupervisor.score = 0;
	}

	public static void ResetInfinitModeTotalSessionScore () {
		AppSupervisor.scoreTotal = 0;
	}	

	public static void GetCurrentMap() {
		GetMap (AppSupervisor.level);
	}

	public static void GetOneMap(int i) {
		GetMap (i);
	}

	public static void GetNextMap() {
		GetMap (AppSupervisor.level + 1);
	}

	public static void GetMap(int map) {
		switch (map) {
		case 1:
			AppSupervisor.mapToLoad = "Lucie";
			break;
		case 2:
			AppSupervisor.mapToLoad = "Aline";
			break;
		case 3:
			AppSupervisor.mapToLoad = "Anna";
			break;
		case 4:
			AppSupervisor.mapToLoad = "Eva";
			break;
		case 5:
			AppSupervisor.mapToLoad = "Sarah";
			break;
		case 6:
			AppSupervisor.mapToLoad = "Marine";
			break;
		case 7:
			AppSupervisor.mapToLoad = "Margaux";
			break;
		case 8:
			AppSupervisor.mapToLoad = "Marie";
			break;
		case 9:
			AppSupervisor.mapToLoad = "Noemie";
			break;
		case 10:
			AppSupervisor.mapToLoad = "Manon";
			break;
		case 11:
			AppSupervisor.mapToLoad = "Sophie";
			break;
		case 12:
			AppSupervisor.mapToLoad = "Aurelie";
			break;
		case 13:
			AppSupervisor.mapToLoad = "Agathe";
			break;
		case 14:
			AppSupervisor.mapToLoad = "Charlene";
			break;
		case 15:
			AppSupervisor.mapToLoad = "Malika";
			break;
		case 16:
			AppSupervisor.mapToLoad = "Nausica";
			break;
		case 17:
			AppSupervisor.mapToLoad = "Vanessa";
			break;
		case 18:
			AppSupervisor.mapToLoad = "Chloe";
			break;
		case 19:
			AppSupervisor.mapToLoad = "Mathilde";
			break;
		case 20:
			AppSupervisor.mapToLoad = "Camille";
			break;
		case 21:
			AppSupervisor.mapToLoad = "Clémence";
			break;
		case 22:
			AppSupervisor.mapToLoad = "Louise";
			break;
		case 23:
			AppSupervisor.mapToLoad = "Louisa";
			break;
		case 24:
			AppSupervisor.mapToLoad = "End";
			break;
		default:
			Debug.Log ("Error unable to load good map");
			AppSupervisor.mapToLoad = "Tuto";
			break;
		}
	}

	public static void LoadMap() {
		if (AppSupervisor.mapToLoad == "Lucie") {
			if (AppSupervisor.tutoPassed == 0) {
				SceneManager.LoadScene ("Tuto"); //Launch d'abord le tuto, puis déverouille Lucie
			} else {
				SceneManager.LoadScene ("PlateaudeJeux");
			}
		} else if (AppSupervisor.mapToLoad == "End") {
			SceneManager.LoadScene ("End");
		} else {
			SceneManager.LoadScene ("PlateaudeJeux");
		}
	}
}