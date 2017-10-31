using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayerType {
	Computer,
	Real
}

public class Player : MonoBehaviour {

	private	static	PlayerType	whoIsPlaying; // Either IA or player
	private	List<DominoFaces>	dominos; // Player's deck, 112 dominos
	private List<GameObject>	bonusDominos;
	private	GameObject[]		handPlayer; // Player's hand, max 3
	private	GameObject[]		handIA;

	private	static	Map			map;


	private	bool				hasDiscarded;
	private	bool				hasPlayed;
	private	bool				hasPlayedBonus;
	private bool				hasSelectedBonus;

	private	bool				hasSelectedDomino;
	private	GameObject			selectedDomino;
	private	List<GameObject>	handContainer;

	private	static IA			enemy;

	public void Start() {
		map = gameObject.GetComponent<Map> ();
		enemy = gameObject.GetComponent<IA> ();
		handContainer = new List<GameObject>();
		handPlayer = new GameObject[3];
		handIA = new GameObject[3];
		for (int i = 0; i < 3; i++) {
			handPlayer[i] = null;
			handIA[i] = null;
		}
		InitDominos ();
		InitBonusDominos ();
		GameObject h = GameObject.Find ("Hand");
		for (int i = 0; i < h.transform.childCount; i++) {
			handContainer.Add (h.transform.GetChild (i).gameObject);
		}
		Draw (handPlayer, PlayerType.Real);
		Draw (handIA, PlayerType.Computer);
		SelectWhoStart ();
		GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetStartText ();
	}

	void Update () {
		if (whoIsPlaying == PlayerType.Real) {
			GetUserInputTouch ();
		} else if (whoIsPlaying == PlayerType.Computer) {
			Domino useDomino = enemy.CheckWhereToPlay(handIA);
			handIA = enemy.PutDominos (useDomino, handIA);
			enemy.DominoToTrash (handIA);
			ChangeTurn ();
		}
	}

	private void GetUserInputTouch() {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit != null) {
				if (hit.name == "HelpBonus") {
					Camera.main.transform.position = new Vector3 (-5.73f, 0f);
				} else if (hasSelectedDomino == false) {
					if (hit.tag == "UsableDomino" && (hasPlayed == false || hasDiscarded == false)) {
						hasSelectedDomino = true;
						selectedDomino = hit.gameObject;
						if (selectedDomino.GetComponent<Domino>().GetDominoType() != DominoType.Simple) {
							hasSelectedBonus = true;
						}
					} else if (hit.name == "EndTurn" && hasPlayed == true && hasDiscarded == true) {
						ChangeTurn ();
					} else if (hit.tag == "FreeBoard") {
						GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetSelectDominoText ();
					}
				} else if (hasSelectedDomino == true) {
					if (hit.tag == "FreeBoard" && hasPlayed == false) {
						hit.tag = "Untagged";
						hasSelectedDomino = false;
						hasPlayed = true;
						Domino d = hit.gameObject.GetComponent<Domino> ();
						d.SetDominoColor (selectedDomino.GetComponent<Domino>().GetDominoColor());
						d.SetDominoType (selectedDomino.GetComponent<Domino>().GetDominoType());
						d.SetDominoFaces (selectedDomino.GetComponent<Domino> ().GetDominoFaces ());
						Domino.CreateDominoTexture (hit.gameObject);
						enemy.AddToRange (hit.gameObject);
						handPlayer[System.Array.IndexOf(handPlayer, selectedDomino)] = null;
						Destroy (selectedDomino);
						selectedDomino = null;
						SymetricalSum (d.GetPosition (), d.GetDominoColor ());
						ReturnClose (d.GetPosition(), d.GetDominoColor());
					} else if (hit.name == "Trash" && hasDiscarded == false && hasPlayed == true) {
						hasSelectedDomino = false;
						handPlayer[System.Array.IndexOf(handPlayer, selectedDomino)] = null;
						Destroy (selectedDomino);
						selectedDomino = null;
						hasDiscarded = true;
					} else if (hit.tag == "UsableDomino") {
						selectedDomino = hit.gameObject;
					} else if (hit.tag == "FreeBoard" && hasPlayed == true) {
						GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetAlreadyPlayedText ();
					} else if (hit.name == "Trash" && hasPlayed == false) {
						GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetNotPlayedText ();
					}
				} else {
					GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetSelectDominoText ();
				}
			}
		}
	}

	private void UseBonusDominos() {
		switch (selectedDomino.GetComponent<Domino>().GetDominoType()) {
		case DominoType.OneLess :
			return;
		case DominoType.TwoLess :
			return;
		case DominoType.ThreeLess :
			return;
		case DominoType.Trap :
			return;
		case DominoType.Radar :
			return;
		case DominoType.ChoosenOne :
			return;
		case DominoType.Log :
			return;
		case DominoType.Color :
			return;
		case DominoType.Swap :
			return;
		case DominoType.Graviton :
			return;
		case DominoType.Megaton :
			return;
		case DominoType.Glue :
			return;
		case DominoType.Nugget :
			return;
		case DominoType.Blur :
			return;
		case DominoType.Switch :
			return;
		case DominoType.Invert :
			return;
		case DominoType.OneMore :
			return;
		case DominoType.TwoMore :
			return;
		case DominoType.ThreeMore :
			return;
		case DominoType.Draw :
			return;
		case DominoType.Jump:
			return;
		default :
			return;
		}
	}

	private void UseXLessBonus(int l) {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit != null) {
				
			}
		}
	}

	private void ChangeTurn() {
		DominoColor color = map.IsGameFinished ();
		if (color == DominoColor.White) {
			Debug.Log ("White Win"); // TODO STUFF FOR WIN
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetEndText();
			AppSupervisor.UnlockNewLevel ();
			AppSupervisor.UnlockNewStory ();
			SceneManager.LoadScene ("Menu");
		} else if (color == DominoColor.Black) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetGameOverText();
			Debug.Log ("Black Win"); // TODO STUFF FOR LOSE
			SceneManager.LoadScene ("End");
		}
		if (whoIsPlaying == PlayerType.Real) {
			hasPlayed = false;
			hasDiscarded = false;
			hasPlayedBonus = false;
			Draw (handIA, PlayerType.Computer);
			whoIsPlaying = PlayerType.Computer;
		} else if (whoIsPlaying == PlayerType.Computer) {
			Draw (handPlayer, PlayerType.Real);
			whoIsPlaying = PlayerType.Real;
		}
	}

	private void SelectWhoStart() {
		int random = Random.Range (0, 99);
		if (random < 50) {
			whoIsPlaying = PlayerType.Real;
		} else {
			whoIsPlaying = PlayerType.Computer;
		}
	}


	private void Draw(GameObject[] hand, PlayerType type) {
		bool hasBonus = false;
		for (int i = 0; i < 3; i++) {
			if (hand[i] != null && hand[i].GetComponent<Domino>().GetDominoType() != DominoType.Simple) {
				hasBonus = true;
			}
		}
		for (int i = 0; i < 3; i++) {
			if (hand [i] == null) {
				int random = Random.Range (0, dominos.Count + bonusDominos.Count);
				if (random < dominos.Count || hasBonus == true) {
					hand [i] = Instantiate (Resources.Load<GameObject> ("Prefab/Domino_numerote_blanc"));
					hand [i].GetComponent<Domino> ().SetDominoType (DominoType.Simple);
					hand [i].GetComponent<Domino> ().SetDominoFaces (dominos [dominos.Count - 1]);
					dominos.RemoveAt (dominos.Count - 1);
				} else if (random >= dominos.Count) {
					hand [i] = bonusDominos [bonusDominos.Count - 1];
					bonusDominos.RemoveAt (bonusDominos.Count - 1);
					hasBonus = true;
				}
			}
		}
		if (type == PlayerType.Real) {
			for (int i = 0; i < hand.Length; i++) {
				hand [i].GetComponent<Domino> ().SetDominoColor (DominoColor.White);
				hand [i].transform.position = handContainer [i].transform.position;
				if (hand [i].GetComponent<Domino> ().GetDominoType () == DominoType.Simple) {
					Domino.CreateDominoTexture (hand [i]);
				} else {
					Texture2D t = Resources.Load<Texture2D> (Domino.GetWhiteBonusTextureName(hand [i].GetComponent<Domino> ().GetDominoType()));
					Sprite s = Sprite.Create (t, new Rect (0, 0, t.width, t.height), new Vector2 (0.5f, 0.5f));
					hand[i].GetComponent<SpriteRenderer> ().sprite = s;
				}
			}
		} else if (type == PlayerType.Computer) {
			for (int i = 0; i < hand.Length; i++) {
				hand [i].GetComponent<Domino> ().SetDominoColor(DominoColor.Black);
				if (hand[i].GetComponent<Domino>().GetDominoType() != DominoType.Simple) {
					Texture2D t = Resources.Load<Texture2D> (Domino.GetBlackBonusTextureName(hand [i].GetComponent<Domino> ().GetDominoType()));
					Sprite s = Sprite.Create (t, new Rect (0, 0, t.width, t.height), new Vector2 (0.5f, 0.5f));
					hand[i].GetComponent<SpriteRenderer> ().sprite = s;
				}
			}
		}
	}
		
	private void InitBonusDominos() {
		bonusDominos = new List<GameObject> ();
		for (int i = 0; i < 21; i++) {
			bonusDominos.Add (Instantiate (Resources.Load<GameObject> ("Prefab/Domino_numerote_blanc")));
		}
		bonusDominos [0].GetComponent<Domino> ().CreateDomino (DominoType.OneLess);
		bonusDominos [1].GetComponent<Domino> ().CreateDomino (DominoType.TwoLess);
		bonusDominos [2].GetComponent<Domino> ().CreateDomino (DominoType.ThreeLess);
		bonusDominos [3].GetComponent<Domino> ().CreateDomino (DominoType.Trap);
		bonusDominos [4].GetComponent<Domino> ().CreateDomino (DominoType.Radar);
		bonusDominos [5].GetComponent<Domino> ().CreateDomino (DominoType.ChoosenOne);
		bonusDominos [6].GetComponent<Domino> ().CreateDomino (DominoType.Log);
		bonusDominos [7].GetComponent<Domino> ().CreateDomino (DominoType.Color);
		bonusDominos [8].GetComponent<Domino> ().CreateDomino (DominoType.Swap);
		bonusDominos [9].GetComponent<Domino> ().CreateDomino (DominoType.Graviton);
		bonusDominos [10].GetComponent<Domino> ().CreateDomino (DominoType.Megaton);
		bonusDominos [11].GetComponent<Domino> ().CreateDomino (DominoType.Glue);
		bonusDominos [12].GetComponent<Domino> ().CreateDomino (DominoType.Nugget);
		bonusDominos [13].GetComponent<Domino> ().CreateDomino (DominoType.Blur);
		bonusDominos [14].GetComponent<Domino> ().CreateDomino (DominoType.Switch);
		bonusDominos [15].GetComponent<Domino> ().CreateDomino (DominoType.Invert);
		bonusDominos [16].GetComponent<Domino> ().CreateDomino (DominoType.OneMore);
		bonusDominos [17].GetComponent<Domino> ().CreateDomino (DominoType.TwoMore);
		bonusDominos [18].GetComponent<Domino> ().CreateDomino (DominoType.ThreeMore);
		bonusDominos [19].GetComponent<Domino> ().CreateDomino (DominoType.Draw);
		bonusDominos [20].GetComponent<Domino> ().CreateDomino (DominoType.Jump);
		ShuffleBonusDominos ();
	}

	private void InitDominos() {
		// Liste d'initialisation de tout les dominos
		dominos = new List<DominoFaces> ();
		// 1 x 7
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.One, DominoValues.One, DominoValues.One, DominoValues.Two, DominoValues.One)); // 1
		// 1 x 35
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Six, DominoValues.Five, DominoValues.Six, DominoValues.Six, DominoValues.Six)); // 2
		// 2 x 9
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.One, DominoValues.One, DominoValues.Four, DominoValues.One, DominoValues.One)); // 3
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.One, DominoValues.One, DominoValues.One, DominoValues.Two, DominoValues.Two)); // 4
		// 2 x 33
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Three, DominoValues.Six, DominoValues.Six, DominoValues.Six, DominoValues.Six)); // 5
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Six, DominoValues.Four, DominoValues.Six, DominoValues.Six, DominoValues.Five)); // 6
		// 4 x 11
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.One, DominoValues.One, DominoValues.Four, DominoValues.Two, DominoValues.One)); // 7
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Two, DominoValues.Three, DominoValues.One, DominoValues.Two, DominoValues.Two)); // 8
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Five, DominoValues.One, DominoValues.Two, DominoValues.One, DominoValues.Two)); // 9
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.One, DominoValues.One, DominoValues.One, DominoValues.Three, DominoValues.Four)); // 10
		// 4 x 31
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Five)); // 11
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Six, DominoValues.Five, DominoValues.Four, DominoValues.Six, DominoValues.Six)); // 12
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Three, DominoValues.Six, DominoValues.Five, DominoValues.Six, DominoValues.Five)); // 13
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Six, DominoValues.Five, DominoValues.Four)); // 14
		// 7 x 13
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Two, DominoValues.One, DominoValues.Four, DominoValues.Three, DominoValues.Two)); // 15
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Five, DominoValues.Two, DominoValues.One, DominoValues.One, DominoValues.One)); // 16
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Two, DominoValues.Three, DominoValues.Two, DominoValues.Two, DominoValues.Two)); // 17
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.One, DominoValues.One, DominoValues.Four, DominoValues.Three, DominoValues.Three)); // 18
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Four, DominoValues.Two, DominoValues.Three, DominoValues.One, DominoValues.One)); // 19
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Three, DominoValues.One, DominoValues.One, DominoValues.Five, DominoValues.One)); // 20
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.One, DominoValues.Four, DominoValues.Three, DominoValues.One, DominoValues.Three)); // 21
		// 7 x 29
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Four, DominoValues.Five, DominoValues.Five)); // 22
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Four, DominoValues.Four, DominoValues.Five, DominoValues.Five, DominoValues.Six)); // 23
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Four, DominoValues.Five, DominoValues.Six, DominoValues.Four, DominoValues.Four)); // 24
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Six, DominoValues.Five, DominoValues.Four, DominoValues.Five, DominoValues.Five)); // 25
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Four, DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Four)); // 26
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Four, DominoValues.Five, DominoValues.Five, DominoValues.Six, DominoValues.Five)); // 27
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Five, DominoValues.Five, DominoValues.Six, DominoValues.Four, DominoValues.Five)); // 28
		// 10 x 15
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Two, DominoValues.Two, DominoValues.Four, DominoValues.Three, DominoValues.Three)); // 29
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Two, DominoValues.One, DominoValues.Four, DominoValues.Four, DominoValues.One)); // 30
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Two, DominoValues.Two, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 31
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Two, DominoValues.Two, DominoValues.Three, DominoValues.Three, DominoValues.Two)); // 32
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Two, DominoValues.Two, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 33
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Three, DominoValues.Two, DominoValues.Three, DominoValues.Two, DominoValues.One)); // 34
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Four, DominoValues.Three, DominoValues.One, DominoValues.Four, DominoValues.Two)); // 35
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Three, DominoValues.Two, DominoValues.Two, DominoValues.Two, DominoValues.Three)); // 36
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Three, DominoValues.Four, DominoValues.Three, DominoValues.One, DominoValues.Two)); // 37
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Two, DominoValues.Two, DominoValues.Three, DominoValues.One, DominoValues.Two)); // 38
		// 10 x 27
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Three, DominoValues.Five, DominoValues.Six, DominoValues.Four, DominoValues.Five)); // 39
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Two)); // 40
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Five, DominoValues.Five, DominoValues.Four, DominoValues.Five, DominoValues.Five)); // 41
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Four, DominoValues.Six)); // 42
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Four, DominoValues.Four)); // 43
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Six, DominoValues.Four)); // 44
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Four, DominoValues.Four)); // 45
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Six, DominoValues.Four, DominoValues.Five, DominoValues.One)); // 46
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Three, DominoValues.Five)); // 47
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Six, DominoValues.Four)); // 48
		// 12 x 17
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Five, DominoValues.Four, DominoValues.Three, DominoValues.Two, DominoValues.One)); // 49
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Two, DominoValues.Two, DominoValues.Two, DominoValues.Three, DominoValues.Two)); // 50
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Two, DominoValues.Three, DominoValues.Three, DominoValues.Four, DominoValues.One)); // 51
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Four, DominoValues.Four)); // 52
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Three, DominoValues.Two, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 53
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Two, DominoValues.Two, DominoValues.Four, DominoValues.Four, DominoValues.One)); // 54
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Three, DominoValues.Two, DominoValues.Two, DominoValues.Five, DominoValues.Four)); // 55
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.One, DominoValues.Two, DominoValues.Four, DominoValues.Three, DominoValues.One)); // 56
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Two, DominoValues.One, DominoValues.Three, DominoValues.Four, DominoValues.Three)); // 57
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Two, DominoValues.Three, DominoValues.Four, DominoValues.Three, DominoValues.Two)); // 58
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Five, DominoValues.Two, DominoValues.Two, DominoValues.Four, DominoValues.Three)); // 59
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.One, DominoValues.Five, DominoValues.Three, DominoValues.One, DominoValues.Four)); // 60
		// 12 x 25
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 61
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Six, DominoValues.Two, DominoValues.Three, DominoValues.Four, DominoValues.Six)); // 62
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.One, DominoValues.Four, DominoValues.Four, DominoValues.Five, DominoValues.Six)); // 63
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Five, DominoValues.Five)); // 64
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Three, DominoValues.Five, DominoValues.Five, DominoValues.Six, DominoValues.Four)); // 65
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Two, DominoValues.Four, DominoValues.Five, DominoValues.Five, DominoValues.Four)); // 66
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Six, DominoValues.Five, DominoValues.Three, DominoValues.Four, DominoValues.Three)); // 67
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Six, DominoValues.Two, DominoValues.Six, DominoValues.Four, DominoValues.Five)); // 68
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Four, DominoValues.Four, DominoValues.Five, DominoValues.Four, DominoValues.Four)); // 69
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Four, DominoValues.Six, DominoValues.Three, DominoValues.Three, DominoValues.Six)); // 70
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Four, DominoValues.Four)); // 71
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Four, DominoValues.Four, DominoValues.Four)); // 72
		// 13 x 19
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Three, DominoValues.One, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 73
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Three, DominoValues.Two, DominoValues.Three, DominoValues.Five, DominoValues.Four)); // 74
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Four, DominoValues.One, DominoValues.Five, DominoValues.Four, DominoValues.Two)); // 75
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Five, DominoValues.Two, DominoValues.Two, DominoValues.Four, DominoValues.Two)); // 76
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Three, DominoValues.One, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 77
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Four, DominoValues.Four, DominoValues.One, DominoValues.Four, DominoValues.Two)); // 78
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Six, DominoValues.Three, DominoValues.Two, DominoValues.Four, DominoValues.Two)); // 79
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Three, DominoValues.One, DominoValues.Five, DominoValues.Five, DominoValues.Two)); // 80
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Three, DominoValues.Five, DominoValues.Two, DominoValues.Four, DominoValues.Four)); // 81
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Three, DominoValues.Three, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 82
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Two, DominoValues.Two, DominoValues.Five, DominoValues.Four, DominoValues.Two)); // 83
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.One, DominoValues.One, DominoValues.Two)); // 84
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Three, DominoValues.Three, DominoValues.Three, DominoValues.Three, DominoValues.Four)); // 85
		// 13 x 23
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Five, DominoValues.Four, DominoValues.Two, DominoValues.One, DominoValues.Five)); // 86
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Three, DominoValues.Four, DominoValues.Three, DominoValues.Four, DominoValues.Five)); // 87
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Four, DominoValues.Four, DominoValues.Three, DominoValues.Four, DominoValues.Six)); // 88
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Five, DominoValues.One, DominoValues.Five, DominoValues.Five, DominoValues.Two)); // 89
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Six, DominoValues.Four, DominoValues.One, DominoValues.Four, DominoValues.Two)); // 90
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Three, DominoValues.Five, DominoValues.Five, DominoValues.Two, DominoValues.Five)); // 91
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Four, DominoValues.Three, DominoValues.Four, DominoValues.Four, DominoValues.Four)); // 92
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Two, DominoValues.One, DominoValues.Six, DominoValues.Six, DominoValues.Six)); // 93
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Four, DominoValues.Five, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 94
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Two, DominoValues.Five, DominoValues.Five, DominoValues.Five, DominoValues.Four)); // 95
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Three, DominoValues.Six, DominoValues.Three, DominoValues.Four, DominoValues.Three)); // 96
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Four, DominoValues.Five, DominoValues.Four, DominoValues.Four, DominoValues.Four)); // 97
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Four, DominoValues.Five, DominoValues.Five, DominoValues.Three, DominoValues.Three)); // 98
		// 14 x 21
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Three, DominoValues.One, DominoValues.Five, DominoValues.Four, DominoValues.Two)); // 99
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Three, DominoValues.Five, DominoValues.Six, DominoValues.Four, DominoValues.Two)); // 100
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Three, DominoValues.Three, DominoValues.Three, DominoValues.Five, DominoValues.Five)); // 101
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Four, DominoValues.Four, DominoValues.One, DominoValues.Four, DominoValues.Four)); // 102
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Three, DominoValues.Three, DominoValues.Three, DominoValues.Three, DominoValues.Six)); // 103
		dominos .Add (new DominoFaces(DominoValues.Six, DominoValues.Three, DominoValues.One, DominoValues.Three, DominoValues.Four, DominoValues.Two)); // 104
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Two, DominoValues.Five, DominoValues.One, DominoValues.Five, DominoValues.Three)); // 105
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Two, DominoValues.Five, DominoValues.Five, DominoValues.One, DominoValues.Four)); // 106
		dominos .Add (new DominoFaces(DominoValues.Two, DominoValues.Two, DominoValues.Five, DominoValues.Five, DominoValues.Four, DominoValues.Three)); // 107
		dominos .Add (new DominoFaces(DominoValues.Three, DominoValues.Four, DominoValues.Five, DominoValues.Four, DominoValues.Three, DominoValues.Two)); // 108
		dominos .Add (new DominoFaces(DominoValues.One, DominoValues.Five, DominoValues.Four, DominoValues.Six, DominoValues.Three, DominoValues.Two)); // 109
		dominos .Add (new DominoFaces(DominoValues.Five, DominoValues.Three, DominoValues.One, DominoValues.Five, DominoValues.Five, DominoValues.Two)); // 110
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Three, DominoValues.Three, DominoValues.Four, DominoValues.Four, DominoValues.Three)); // 111
		dominos .Add (new DominoFaces(DominoValues.Four, DominoValues.Three, DominoValues.Five, DominoValues.Three, DominoValues.Two, DominoValues.Four)); // 112

		ShuffleDominos ();
	}

	private void ShuffleDominos() {
		System.Random rng = new System.Random();
		int n = dominos.Count;
		while (n > 1) {
			n--;  
			int k = rng.Next(n + 1);
			DominoFaces value = dominos[k];
			dominos[k] = dominos[n];
			dominos[n] = value;
		}
	}

	private void ShuffleBonusDominos() {
		System.Random rng = new System.Random();
		int n = bonusDominos.Count;
		while (n > 1) {
			n--;  
			int k = rng.Next(n + 1);
			GameObject value = bonusDominos[k];
			bonusDominos[k] = bonusDominos[n];
			bonusDominos[n] = value;
		}
	}

	public static void ReturnClose(Coordinate position, DominoColor color) {
		int r = 0;
		int x = position.GetX ();
		int y = position.GetY ();
		GameObject[][] dominoMap = map.GetMap ();
		Domino current = dominoMap [x] [y].GetComponent<Domino> ();

		if (x > 0 && y > 0 && y - 1 < dominoMap[x - 1].Length
			&& dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('D')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('A'))) {
				dominoMap[x - 1][y - 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x - 1][y - 1]);
				enemy.SupToRange (dominoMap [x - 1] [y - 1]);
				enemy.AddToRange (dominoMap [x - 1] [y - 1]);
				r++;
			}
		}
		if (x > 0 && y < dominoMap[x - 1].Length
			&& dominoMap[x - 1][y].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x - 1][y].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x - 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('E')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('B'))) {
				dominoMap[x - 1][y].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x - 1][y]);
				enemy.SupToRange (dominoMap [x - 1] [y]);
				enemy.AddToRange (dominoMap [x - 1] [y]);
				r++;
			}
		}
		if (y + 1 < dominoMap[x].Length
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('F')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('C'))) {
				dominoMap[x][y + 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x][y + 1]);
				enemy.SupToRange (dominoMap [x] [y + 1]);
				enemy.AddToRange (dominoMap [x] [y + 1]);
				r++;
			}
		}
		if (x + 1 < dominoMap.Length && y + 1 < dominoMap[x + 1].Length
			&& dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('A')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('D'))) {
				dominoMap[x + 1][y + 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x + 1][y + 1]);
				enemy.SupToRange (dominoMap [x + 1] [y + 1]);
				enemy.AddToRange (dominoMap [x + 1] [y + 1]);
				r++;
			}
		}
		if (x + 1 < dominoMap.Length && y < dominoMap [x + 1].Length
			&& dominoMap[x + 1][y].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x + 1][y].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x + 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('B')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('E'))) {
				dominoMap[x + 1][y].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x + 1][y]);
				enemy.SupToRange (dominoMap [x + 1] [y]);
				enemy.AddToRange (dominoMap [x + 1] [y]);
				r++;
			}
		}
		if (y > 0
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('C')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('F'))) {
				dominoMap[x][y - 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x][y - 1]);
				enemy.SupToRange (dominoMap [x] [y - 1]);
				enemy.AddToRange (dominoMap [x] [y - 1]);
				r++;
			}
		}
		if (r > 0 && whoIsPlaying == PlayerType.Computer) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetInGameWinningText ();
			// TODO METTRE LA PHRASE DU TUEURs
		} else if (r > 0 && whoIsPlaying == PlayerType.Real) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetInGameIsLosingText();
			// TODO METTRE LA PHRASE DU JOUEUR
		}
	}

	public static void SymetricalSum(Coordinate position, DominoColor color) {
		int r = 0;
		int x = position.GetX ();
		int y = position.GetY ();
		GameObject[][] dominoMap = map.GetMap ();
		Domino current = dominoMap [x] [y].GetComponent<Domino> ();

		if ((x > 0) && (y > 0) && (x + 1 < dominoMap.Length)
			&& (y - 1 < dominoMap[x - 1].Length) && (y + 1 < dominoMap[x + 1].Length)
			&& dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('D')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('A')) == Domino.DominoValueToInt(dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('A')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('D'))) {
				dominoMap[x - 1][y - 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x - 1][y - 1]);
				enemy.SupToRange (dominoMap [x - 1] [y - 1]);
				enemy.AddToRange (dominoMap [x - 1] [y - 1]);
				dominoMap[x + 1][y + 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x + 1][y + 1]);
				enemy.SupToRange (dominoMap [x + 1] [y + 1]);
				enemy.AddToRange (dominoMap [x + 1] [y + 1]);
				r += 2;
			}
		}
		if ((x > 0) && (y < dominoMap[x - 1].Length) && (x + 1 < dominoMap.Length) && (y < dominoMap [x + 1].Length)
			&& dominoMap[x - 1][y].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x - 1][y].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x + 1][y].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x + 1][y].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x - 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('E')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('B')) == Domino.DominoValueToInt(dominoMap[x + 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('B')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('E'))) {
				dominoMap[x - 1][y].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x - 1][y]);
				enemy.SupToRange (dominoMap [x - 1] [y]);
				enemy.AddToRange (dominoMap [x - 1] [y]);
				dominoMap[x + 1][y].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x + 1][y]);
				enemy.SupToRange (dominoMap [x + 1] [y]);
				enemy.AddToRange (dominoMap [x + 1] [y]);
				r += 2;
			}
		}
		if ((y > 0) && (y + 1 < dominoMap[x].Length)
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('C')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('F')) == Domino.DominoValueToInt(dominoMap[x][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('F')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('C'))) {
				dominoMap[x][y - 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x][y - 1]);
				enemy.SupToRange (dominoMap [x] [y - 1]);
				enemy.AddToRange (dominoMap [x] [y - 1]);
				dominoMap[x][y + 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x][y + 1]);
				enemy.SupToRange (dominoMap [x] [y + 1]);
				enemy.AddToRange (dominoMap [x] [y + 1]);
				r += 2;
			}
		}
		if (r > 0 && whoIsPlaying == PlayerType.Computer) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetInGameWinningText ();
			// METTRE LA PHRASE DU TUEURs
		} else if (r > 0 && whoIsPlaying == PlayerType.Real) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetInGameIsLosingText();
			// METTRE LA PHRASE DU JOUEUR
		}
	}

	public static bool CheckReturnClose(Domino dominoToReturn, Domino dominoToUse, int i) {
		DominoFaces faceToUse = dominoToUse.GetDominoFaces ();
		DominoFaces faceToReturn = dominoToReturn.GetDominoFaces ();

		switch (i) {
		case 0:
			if (Domino.DominoValueToInt(faceToUse.GetFace ('A')) > Domino.DominoValueToInt(faceToReturn.GetFace ('D')))
				return true;
			break;
		case 1:
			if (Domino.DominoValueToInt(faceToUse.GetFace ('B')) > Domino.DominoValueToInt(faceToReturn.GetFace ('E')))
				return true;
			break;
		case 2:
			if (Domino.DominoValueToInt(faceToUse.GetFace ('F')) > Domino.DominoValueToInt(faceToReturn.GetFace ('C')))
				return true;
			break;
		case 3:
			if (Domino.DominoValueToInt(faceToUse.GetFace ('C')) > Domino.DominoValueToInt(faceToReturn.GetFace ('F')))
				return true;
			break;
		case 4:
			if (Domino.DominoValueToInt(faceToUse.GetFace ('E')) > Domino.DominoValueToInt(faceToReturn.GetFace ('B')))
				return true;
			break;
		case 5:
			if (Domino.DominoValueToInt(faceToUse.GetFace ('D')) > Domino.DominoValueToInt(faceToReturn.GetFace ('A')))
				return true;
			break;
		default:
			return false;
		}
		return false;
	}

	public static bool CheckSymetricalSum(Domino dominoToUse, Domino handDomino) {
		GameObject[][] dominoMap = map.GetMap ();
		DominoColor color = dominoToUse.GetDominoColor ();
		int x = dominoToUse.GetPosition().GetX();
		int y = dominoToUse.GetPosition().GetY();

		if ((x > 0) && (y > 0) && (x + 1 < dominoMap.Length)
			&& (y - 1 < dominoMap[x - 1].Length) && (y + 1 < dominoMap[x + 1].Length)
			&& dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('D')) + Domino.DominoValueToInt(handDomino.GetDominoFaces().GetFace('A')) == Domino.DominoValueToInt(dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('A')) + Domino.DominoValueToInt(handDomino.GetDominoFaces().GetFace('D'))) {
				return true;
			}
		}
		if ((x > 0) && (y < dominoMap[x - 1].Length) && (x + 1 < dominoMap.Length) && (y < dominoMap [x + 1].Length)
			&& dominoMap[x - 1][y].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x - 1][y].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x + 1][y].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x + 1][y].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x - 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('E')) + Domino.DominoValueToInt(handDomino.GetDominoFaces().GetFace('B')) == Domino.DominoValueToInt(dominoMap[x + 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('B')) + Domino.DominoValueToInt(handDomino.GetDominoFaces().GetFace('E'))) {
				return true;
			}
		}
		if ((y > 0) && (y + 1 < dominoMap[x].Length)
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('C')) + Domino.DominoValueToInt(handDomino.GetDominoFaces().GetFace('F')) == Domino.DominoValueToInt(dominoMap[x][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('F')) + Domino.DominoValueToInt(handDomino.GetDominoFaces().GetFace('C'))) {
				return true;
			}
		}
		return false;
	}

	public static bool CheckByRange(Domino dominoToCheck, Domino dominoToUse) {
		int range = Domino.DominoValueToInt (dominoToCheck.GetRange(dominoToUse.GetDominoColor()));
		int rangeOK = 0;
		Coordinate position = dominoToCheck.GetPosition ();
		int x = position.GetX ();
		int y = position.GetY ();
		GameObject[][] dominoMap = map.GetMap ();

		for (int i = 0; i < 6; i++) {
			if (i == 0) {
				if (x > 0 && y > 0 && (y - 1 < dominoMap[x - 1].Length) &&
					(dominoMap [x - 1] [y - 1].GetComponent<Domino> ().GetDominoType () == DominoType.Simple) &&
					(Domino.GetOppositeColor (dominoMap [x - 1] [y - 1].GetComponent<Domino> ().GetDominoColor ()) == dominoToUse.GetDominoColor ())) {
					if (CheckReturnClose (dominoMap [x - 1] [y - 1].GetComponent<Domino> (), dominoToUse, i)) {
						rangeOK++;
					}
				}
			}
			if (i == 1) {
				if ((x > 0) && (y < dominoMap[x - 1].Length) &&
					(dominoMap [x - 1] [y].GetComponent<Domino> ().GetDominoType () == DominoType.Simple) &&
					(Domino.GetOppositeColor (dominoMap [x - 1] [y].GetComponent<Domino> ().GetDominoColor ()) == dominoToUse.GetDominoColor ())) {
					if (CheckReturnClose (dominoMap [x - 1] [y].GetComponent<Domino> (), dominoToUse, i)) {
						rangeOK++;
					}
				}
			}
			if (i == 2) {
				if ((y > 0) &&
					(dominoMap [x] [y - 1].GetComponent<Domino> ().GetDominoType () == DominoType.Simple) &&
					(Domino.GetOppositeColor (dominoMap [x] [y - 1].GetComponent<Domino> ().GetDominoColor ()) == dominoToUse.GetDominoColor ())) {
					if (CheckReturnClose (dominoMap [x] [y - 1].GetComponent<Domino> (), dominoToUse, i)) {
						rangeOK++;
					}
				}
			}
			if (i == 3) {
				if ((y + 1 < dominoMap [x].Length) &&
					(dominoMap [x] [y + 1].GetComponent<Domino> ().GetDominoType () == DominoType.Simple) &&
					(Domino.GetOppositeColor (dominoMap [x] [y + 1].GetComponent<Domino> ().GetDominoColor ()) == dominoToUse.GetDominoColor ())) {
					if (CheckReturnClose (dominoMap [x] [y + 1].GetComponent<Domino> (), dominoToUse, i)) {
						rangeOK++;
					}
				}
			}
			if (i == 4) {
				if ((x + 1 < dominoMap.Length) && (y < dominoMap [x + 1].Length) &&
					(dominoMap [x + 1] [y].GetComponent<Domino> ().GetDominoType () == DominoType.Simple) &&
					(Domino.GetOppositeColor (dominoMap [x + 1] [y].GetComponent<Domino> ().GetDominoColor ()) == dominoToUse.GetDominoColor ())) {
					if (CheckReturnClose (dominoMap [x + 1] [y].GetComponent<Domino> (), dominoToUse, i)) {
						rangeOK++;
					}
				}
			}
			if (i == 5) {
				if ((x + 1 < dominoMap.Length) && (y + 1 < dominoMap[x + 1].Length) &&
					(dominoMap [x + 1] [y + 1].GetComponent<Domino> ().GetDominoType () == DominoType.Simple) &&
					(Domino.GetOppositeColor (dominoMap [x + 1] [y + 1].GetComponent<Domino> ().GetDominoColor ()) == dominoToUse.GetDominoColor ())) {
					if (CheckReturnClose (dominoMap [x + 1] [y + 1].GetComponent<Domino> (), dominoToUse, i)) {
						rangeOK++;
					}
				}
			}
		}

		if (rangeOK == range) {
			return true;
		} else {
			return false;
		}
	}
}