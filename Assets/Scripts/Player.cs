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
	private DominoType			hasSelectedBonus;
	private	bool				overlayBonus;

	private	bool				hasSelectedDomino;
	private	GameObject			selectedDomino;
	private	List<GameObject>	handContainer;

	private	static IA			enemy;

	public void Start() {
		GameObject.Find ("ButtonQuit").GetComponent<Button> ().enabled = false;
		GameObject.Find ("ButtonQuit").GetComponent<Text> ().enabled = false;
		GameObject.Find ("ButtonQuitBackground").GetComponent<Image> ().enabled = false;
		GameObject.Find ("ButtonQuitBackground").GetComponent<Button> ().enabled = false;
		hasSelectedBonus = DominoType.NotBonus;
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
			if (hasSelectedBonus != DominoType.NotBonus && hasSelectedBonus != DominoType.BonusInUse && !hasPlayedBonus) {
				UseBonusDominos ();
			}
			GetUserInputTouch ();
		} else if (whoIsPlaying == PlayerType.Computer) {
			Domino useDomino = enemy.CheckWhereToPlay(handIA);
			handIA = enemy.PutDominos (useDomino, handIA);
			enemy.DominoToTrash (handIA);
			ChangeTurn ();
		}
	}

	private void GoToQuit() {
		DominoColor color = map.IsGameFinished ();
		if (color == DominoColor.White) {
			GameObject.Find ("Killer_Text").GetComponent<Text> ().text = GameMessagesAccessor.GetEndText ();
			AppSupervisor.UnlockNewLevel ();
			AppSupervisor.UnlockNewStory ();
			SceneManager.LoadScene ("Victoire");
		} else if (color == DominoColor.Black) {
			GameObject.Find ("Killer_Text").GetComponent<Text> ().text = GameMessagesAccessor.GetGameOverText ();
			SceneManager.LoadScene ("GameOver");
		}
	}

	private void GetUserInputTouch() {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit != null) {
				if (hit.name == "HelpBonus") {
					Camera.main.transform.position = new Vector3 (-5.73f, 0f);
				} else if (hit.name == "EndTurn" && hasPlayed && hasDiscarded) {
					ChangeTurn ();
				} else if (!hasSelectedDomino) {
					if (hit.tag == "UsableDomino" && (!hasPlayed || !hasDiscarded || !hasPlayedBonus)) {
						if (!hasPlayedBonus && hit.gameObject.GetComponent<Domino>().GetDominoType() != DominoType.Simple) {
							hasSelectedBonus = hit.gameObject.GetComponent<Domino>().GetDominoType();
						}
						hasSelectedDomino = true;
						selectedDomino = hit.gameObject;
						selectedDomino.transform.localScale = selectedDomino.transform.localScale * 1.2f;
					} else if (hit.tag == "FreeBoard") {
						GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetSelectDominoText ();
					}
				} else if (hasSelectedDomino) {
					if (hit.tag == "FreeBoard" && (!hasPlayed || hasPlayedBonus)
						&& (hasSelectedBonus == DominoType.NotBonus || hasSelectedBonus == DominoType.BonusInUse)) {
						hasSelectedDomino = false;
						Domino d = hit.gameObject.GetComponent<Domino> ();
						d.SetDominoType (selectedDomino.GetComponent<Domino> ().GetDominoType ());
						d.SetDominoColor (selectedDomino.GetComponent<Domino> ().GetDominoColor ());
						if (!hasPlayed && selectedDomino.GetComponent<Domino> ().GetDominoType () == DominoType.Simple) {
							hasPlayed = true;
							hit.tag = "PlayedDomino";
							d.SetDominoFaces (selectedDomino.GetComponent<Domino> ().GetDominoFaces ());
							Domino.CreateDominoTexture (hit.gameObject);
							enemy.AddToRange (hit.gameObject);
							SymetricalSum (d.GetPosition (), d.GetDominoColor ());
							ReturnClose (d.GetPosition (), d.GetDominoColor ());
						} else if (hasPlayedBonus && hasSelectedBonus != DominoType.NotBonus) {
							hit.tag = "PlayedBonus";
							hit.gameObject.GetComponent<SpriteRenderer> ().sprite = selectedDomino.GetComponent<SpriteRenderer> ().sprite;
							hasSelectedBonus = DominoType.NotBonus;
							hasPlayedBonus = false;
							ResetOverlay ();
						}
						handPlayer [System.Array.IndexOf (handPlayer, selectedDomino)] = null;
						Destroy (selectedDomino);
						selectedDomino = null;
						if (map.IsGameFinished() != DominoColor.None) {
							ChangeTurn ();
						}
					} else if (hit.name == "Trash" && !hasDiscarded && hasPlayed && (hasSelectedBonus == DominoType.NotBonus || (hasSelectedBonus != DominoType.BonusInUse && !hasPlayedBonus))) {
						hasSelectedDomino = false;
						handPlayer[System.Array.IndexOf(handPlayer, selectedDomino)] = null;
						Destroy (selectedDomino);
						selectedDomino = null;
						hasSelectedBonus = DominoType.NotBonus;
						hasDiscarded = true;
						if (overlayBonus) {
							ResetOverlay ();
						}
					} else if (hit.tag == "UsableDomino" && hasSelectedBonus != DominoType.BonusInUse) {
						selectedDomino.transform.localScale = selectedDomino.transform.localScale / 1.2f;
						selectedDomino = hit.gameObject;
						selectedDomino.transform.localScale = selectedDomino.transform.localScale * 1.2f;
						hasSelectedBonus = DominoType.NotBonus;
						if (selectedDomino.GetComponent<Domino>().GetDominoType() != DominoType.Simple) {
							hasSelectedBonus = selectedDomino.GetComponent<Domino>().GetDominoType();
						}
					} else if (hit.tag == "FreeBoard" && hasPlayed && hasSelectedBonus == DominoType.NotBonus) {
						GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetAlreadyPlayedText ();
					} else if (hit.name == "Trash" && !hasPlayed && hasSelectedBonus == DominoType.NotBonus) {
						GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetNotPlayedText ();
					}
				} else {
					GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetSelectDominoText ();
				}
			}
		}
	}

	private void ResetOverlay() {
		overlayBonus = false;
		GameObject[][] dominoMap = map.GetMap();
		for (int i = 0; i < dominoMap.Length; i++) {
			for (int j = 0; j < dominoMap[i].Length; j++) {
				Color c = dominoMap [i] [j].GetComponent<SpriteRenderer> ().color;
				c.a = 1;
				dominoMap [i] [j].GetComponent<SpriteRenderer> ().color = c;
			}
		}
	}

	private void UseBonusDominos() {
		switch (selectedDomino.GetComponent<Domino>().GetDominoType()) {
		case DominoType.OneLess:
			UseXLessBonus (1);
			break;
		case DominoType.TwoLess:
			UseXLessBonus (2);
			break;
		case DominoType.ThreeLess:
			UseXLessBonus (3);
			break;
		case DominoType.Color:
			UseColorBonus ();
			break;
		case DominoType.Swap:
			UseSwapHandBonus ();
			break;
		case DominoType.Graviton:
			UseGravitonBonus ();
			break;
		case DominoType.Megaton:
			UseMegatonBonus ();
			break;
		case DominoType.Glue:
			UseGlueBonus ();
			break;
		case DominoType.OneMore:
			UseXMoreBonus (1);
			break;
		case DominoType.TwoMore :
			UseXMoreBonus (2);
			break;
		case DominoType.ThreeMore :
			UseXMoreBonus (3);
			break;
		default :
			return;
		}
	}

	private void UseXMoreBonus(int l) {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit.gameObject == selectedDomino) {
				selectedDomino.transform.localScale = selectedDomino.transform.localScale / 1.2f;
				selectedDomino = null;
				hasSelectedDomino = false;
				hasSelectedBonus = DominoType.NotBonus;
			} else if (hit != null && (hit.tag == "PlayedDomino" || hit.tag == "UsableDomino")
				&& hit.gameObject.GetComponent<Domino>().GetDominoColor() == DominoColor.White) {
				Domino domino = hit.gameObject.GetComponent<Domino> ();
				int a = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('A')) + l;
				int b = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('B')) + l;
				int c = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('C')) + l;
				int d = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('D')) + l;
				int e = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('E')) + l;
				int f = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('F')) + l;
				domino.SetDominoFaces(new DominoFaces(Domino.IntToDominoValue (Mathf.Clamp(a, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(b, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(c, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(d, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(e, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(f, 1, 6))));
				Domino.CreateDominoTexture (hit.gameObject);
				hasPlayedBonus = true;
				hasSelectedBonus = DominoType.BonusInUse;
				ResetOverlay ();
			}
		} else if (!overlayBonus){
			overlayBonus = true;
			GameObject[][] dominoMap = map.GetMap();
			for (int i = 0; i < dominoMap.Length; i++) {
				for (int j = 0; j < dominoMap[i].Length; j++) {
					if (dominoMap[i][j].GetComponent<Domino>().GetDominoColor() == DominoColor.Black
						&& dominoMap[i][j].GetComponent<Domino>().GetDominoType() == DominoType.Simple) {
						Color c = dominoMap [i] [j].GetComponent<SpriteRenderer> ().color;
						c.a -= 0.7f;
						dominoMap [i] [j].GetComponent<SpriteRenderer> ().color = c;
					}
				}
			}
		}
	}

	private void UseXLessBonus(int l) {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit.gameObject == selectedDomino) {
				selectedDomino.transform.localScale = selectedDomino.transform.localScale / 1.2f;
				selectedDomino = null;
				hasSelectedDomino = false;
				hasSelectedBonus = DominoType.NotBonus;
			} else if (hit != null && (hit.tag == "PlayedDomino")
				&& hit.gameObject.GetComponent<Domino>().GetDominoColor() == DominoColor.Black) {
				Domino domino = hit.gameObject.GetComponent<Domino> ();
				int a = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('A')) - l;
				int b = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('B')) - l;
				int c = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('C')) - l;
				int d = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('D')) - l;
				int e = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('E')) - l;
				int f = Domino.DominoValueToInt (domino.GetDominoFaces ().GetFace ('F')) - l;
				domino.SetDominoFaces(new DominoFaces(Domino.IntToDominoValue (Mathf.Clamp(a, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(b, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(c, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(d, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(e, 1, 6)), Domino.IntToDominoValue (Mathf.Clamp(f, 1, 6))));
				Domino.CreateDominoTexture (hit.gameObject);
				hasPlayedBonus = true;
				hasSelectedBonus = DominoType.BonusInUse;
				ResetOverlay ();
			}
		} else if (!overlayBonus) {
			overlayBonus = true;
			GameObject[][] dominoMap = map.GetMap();
			for (int i = 0; i < dominoMap.Length; i++) {
				for (int j = 0; j < dominoMap[i].Length; j++) {
					if (dominoMap[i][j].GetComponent<Domino>().GetDominoColor() == DominoColor.White) {
						Color c = dominoMap [i] [j].GetComponent<SpriteRenderer> ().color;
						c.a -= 0.3f;
						dominoMap [i] [j].GetComponent<SpriteRenderer> ().color = c;
					}
				}
			}
		}
	}

	private void UseColorBonus() {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit.gameObject == selectedDomino) {
				selectedDomino.transform.localScale = selectedDomino.transform.localScale / 1.2f;
				selectedDomino = null;
				hasSelectedDomino = false;
				hasSelectedBonus = DominoType.NotBonus;
			} else if (hit != null && hit.tag == "PlayedDomino") {
				Domino domino = hit.gameObject.GetComponent<Domino> ();
				if (domino.GetDominoColor () != DominoColor.White) {
					domino.SetDominoColor (DominoColor.White);
					Domino.CreateDominoTexture (hit.gameObject);
					hasPlayedBonus = true;
					hasSelectedBonus = DominoType.BonusInUse;
					ResetOverlay ();
				}
			}
		} else if (!overlayBonus) {
			overlayBonus = true;
			GameObject[][] dominoMap = map.GetMap();
			for (int i = 0; i < dominoMap.Length; i++) {
				for (int j = 0; j < dominoMap[i].Length; j++) {
					if (dominoMap[i][j].GetComponent<Domino>().GetDominoColor() == DominoColor.Black) {
						Color c = dominoMap [i] [j].GetComponent<SpriteRenderer> ().color;
						c.a -= 0.7f;
						dominoMap [i] [j].GetComponent<SpriteRenderer> ().color = c;
					}
				}
			}
		}
	}

	private void UseGravitonBonus() {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit.gameObject == selectedDomino) {
				selectedDomino.transform.localScale = selectedDomino.transform.localScale / 1.2f;
				selectedDomino = null;
				hasSelectedDomino = false;
				hasSelectedBonus = DominoType.NotBonus;
			} else if (hit != null && (hit.tag == "PlayedDomino" || hit.tag == "UsableDomino")) {
				Domino domino = hit.gameObject.GetComponent<Domino> ();
				DominoValues a = domino.GetDominoFaces ().GetFace ('F');
				DominoValues b = domino.GetDominoFaces ().GetFace ('E');
				DominoValues c = domino.GetDominoFaces ().GetFace ('D');
				DominoValues d = domino.GetDominoFaces ().GetFace ('C');
				DominoValues e = domino.GetDominoFaces ().GetFace ('B');
				DominoValues f = domino.GetDominoFaces ().GetFace ('A');
				domino.SetDominoFaces (new DominoFaces (a, b, c, d, e, f));
				Domino.CreateDominoTexture (hit.gameObject);
				if (hit.tag == "PlayedDomino") {
					SymetricalSum (domino.GetPosition (), domino.GetDominoColor ());
					ReturnClose (domino.GetPosition (), domino.GetDominoColor ());
				}
				hasPlayedBonus = true;
				hasSelectedBonus = DominoType.BonusInUse;
			}
		}
	}

	private void UseMegatonBonus() {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit.gameObject == selectedDomino) {
				selectedDomino.transform.localScale = selectedDomino.transform.localScale / 1.2f;
				selectedDomino = null;
				hasSelectedDomino = false;
				hasSelectedBonus = DominoType.NotBonus;
			} else if (hit != null && (hit.tag == "PlayedDomino" || hit.tag == "UsableDomino")) {
				Domino domino = hit.gameObject.GetComponent<Domino> ();
				DominoValues a = domino.GetDominoFaces ().GetFace ('C');
				DominoValues b = domino.GetDominoFaces ().GetFace ('B');
				DominoValues c = domino.GetDominoFaces ().GetFace ('A');
				DominoValues d = domino.GetDominoFaces ().GetFace ('F');
				DominoValues e = domino.GetDominoFaces ().GetFace ('E');
				DominoValues f = domino.GetDominoFaces ().GetFace ('D');
				domino.SetDominoFaces (new DominoFaces (a, b, c, d, e, f));
				Domino.CreateDominoTexture (hit.gameObject);
				if (hit.tag == "PlayedDomino") {
					SymetricalSum (domino.GetPosition (), domino.GetDominoColor ());
					ReturnClose (domino.GetPosition (), domino.GetDominoColor ());
				}
				hasPlayedBonus = true;
				hasSelectedBonus = DominoType.BonusInUse;
			}
		}
	}

	private void UseGlueBonus() {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit.gameObject == selectedDomino) {
				selectedDomino.transform.localScale = selectedDomino.transform.localScale / 1.2f;
				selectedDomino = null;
				hasSelectedDomino = false;
				hasSelectedBonus = DominoType.NotBonus;
			} else if (hit != null && (hit.tag == "PlayedDomino" || hit.tag == "UsableDomino")) {
				Domino domino = hit.gameObject.GetComponent<Domino> ();
				domino.SetIsGlued (true);
				hasPlayedBonus = true;
				hasSelectedBonus = DominoType.BonusInUse;
				ResetOverlay ();
			}
		} else if (!overlayBonus) {
			overlayBonus = true;
			GameObject[][] dominoMap = map.GetMap();
			for (int i = 0; i < dominoMap.Length; i++) {
				for (int j = 0; j < dominoMap[i].Length; j++) {
					if (dominoMap[i][j].GetComponent<Domino>().GetDominoColor() == DominoColor.Black) {
						Color c = dominoMap [i] [j].GetComponent<SpriteRenderer> ().color;
						c.a -= 0.7f;
						dominoMap [i] [j].GetComponent<SpriteRenderer> ().color = c;
					}
				}
			}
		}
	}

	private void UseSwapHandBonus() {
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			Vector2 v = new Vector2 (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).x, Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position).y);
			Collider2D hit = Physics2D.OverlapPoint (v);
			if (hit.gameObject == selectedDomino) {
				selectedDomino.transform.localScale = selectedDomino.transform.localScale / 1.2f;
				selectedDomino = null;
				hasSelectedDomino = false;
				hasSelectedBonus = DominoType.NotBonus;
			} else if (hit != null && (hit.tag == "FreeBoard")) {
				Domino domino = hit.gameObject.GetComponent<Domino> ();
				hasPlayedBonus = true;
				hasSelectedBonus = DominoType.NotBonus;
				hasSelectedDomino = false;
				domino.SetDominoType (DominoType.Swap);
				domino.SetDominoColor (DominoColor.Bonus);
				hit.tag = "PlayedBonus";
				hit.gameObject.GetComponent<SpriteRenderer> ().sprite = selectedDomino.GetComponent<SpriteRenderer> ().sprite;
				hasSelectedBonus = DominoType.NotBonus;
				hasPlayedBonus = false;
				handPlayer [System.Array.IndexOf (handPlayer, selectedDomino)] = null;
				Destroy (selectedDomino);
				selectedDomino = null;
				if (map.IsGameFinished() != DominoColor.None) {
					ChangeTurn ();
				}
				for (int i = 0; i < 3; i++) {
					GameObject tmp = handIA [i];
					handIA [i] = handPlayer [i];
					handPlayer [i] = tmp;
					if (handIA[i] != null) {
						handIA [i].transform.position = new Vector3 (-30, -30);
					}
					handPlayer [i].GetComponent<Domino> ().SetDominoColor (DominoColor.White);
					handPlayer [i].transform.position = handContainer [i].transform.position;
					if (handPlayer [i].GetComponent<Domino> ().GetDominoType () == DominoType.Simple) {
						Domino.CreateDominoTexture (handPlayer [i]);
					} else {
						Texture2D t = Resources.Load<Texture2D> (Domino.GetWhiteBonusTextureName(handPlayer [i].GetComponent<Domino> ().GetDominoType()));
						handPlayer [i].GetComponent<SpriteRenderer> ().sprite = Sprite.Create (t, new Rect (0, 0, t.width, t.height), new Vector2 (0.5f, 0.5f));
						handPlayer [i].GetComponent<Domino> ().SetDominoColor (DominoColor.Bonus);
					}
				}
			}
		}
	}

	private void ChangeTurn() {
		DominoColor color = map.IsGameFinished ();
		if (color != DominoColor.None) {
			GameObject.Find ("ButtonQuit").GetComponent<Button> ().enabled = true;
			GameObject.Find ("ButtonQuit").GetComponent<Text> ().enabled = true;
			GameObject.Find ("ButtonQuitBackground").GetComponent<Image> ().enabled = true;
			GameObject.Find ("ButtonQuitBackground").GetComponent<Button> ().enabled = true;
			GameObject.Find ("ButtonQuit").GetComponent<Button> ().onClick.AddListener (() => {
				GoToQuit ();
			});
			GameObject.Find ("ButtonQuitBackground").GetComponent<Button> ().onClick.AddListener (() => {
				GoToQuit ();
			});
		}
		if (whoIsPlaying == PlayerType.Real) {
			hasPlayed = false;
			hasDiscarded = false;
			hasPlayedBonus = false;
			Draw (handIA, PlayerType.Computer);
			Draw (handPlayer, PlayerType.Real);
			whoIsPlaying = PlayerType.Computer;
		} else if (whoIsPlaying == PlayerType.Computer) {
			Draw (handIA, PlayerType.Computer);
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
				if (random < dominos.Count || hasBonus) {
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
					hand[i].GetComponent<SpriteRenderer> ().sprite = Sprite.Create (t, new Rect (0, 0, t.width, t.height), new Vector2 (0.5f, 0.5f));
					hand [i].GetComponent<Domino> ().SetDominoColor (DominoColor.Bonus);
				}
			}
		} else if (type == PlayerType.Computer) {
			for (int i = 0; i < hand.Length; i++) {
				hand [i].GetComponent<Domino> ().SetDominoColor(DominoColor.Black);
				if (hand[i].GetComponent<Domino>().GetDominoType() != DominoType.Simple) {
					Texture2D t = Resources.Load<Texture2D> (Domino.GetBlackBonusTextureName(hand [i].GetComponent<Domino> ().GetDominoType()));
					hand[i].GetComponent<SpriteRenderer> ().sprite = Sprite.Create (t, new Rect (0, 0, t.width, t.height), new Vector2 (0.5f, 0.5f));
				}
			}
		}
	}
		
	private void InitBonusDominos() {
		bonusDominos = new List<GameObject> ();
		for (int i = 0; i < 11; i++) {
			bonusDominos.Add (Instantiate (Resources.Load<GameObject> ("Prefab/Domino_numerote_blanc")));
		}
		bonusDominos [0].GetComponent<Domino> ().CreateDomino (DominoType.OneLess);
		bonusDominos [1].GetComponent<Domino> ().CreateDomino (DominoType.TwoLess);
		bonusDominos [2].GetComponent<Domino> ().CreateDomino (DominoType.ThreeLess);
		bonusDominos [3].GetComponent<Domino> ().CreateDomino (DominoType.Color);
		bonusDominos [4].GetComponent<Domino> ().CreateDomino (DominoType.Swap);
		bonusDominos [5].GetComponent<Domino> ().CreateDomino (DominoType.Graviton);
		bonusDominos [6].GetComponent<Domino> ().CreateDomino (DominoType.Megaton);
		bonusDominos [7].GetComponent<Domino> ().CreateDomino (DominoType.Glue);
		bonusDominos [8].GetComponent<Domino> ().CreateDomino (DominoType.OneMore);
		bonusDominos [9].GetComponent<Domino> ().CreateDomino (DominoType.TwoMore);
		bonusDominos [10].GetComponent<Domino> ().CreateDomino (DominoType.ThreeMore);
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
			if (Domino.DominoValueToInt(dominoMap[x - 1][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('D')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('A'))
				&& dominoMap[x - 1][y - 1].GetComponent<Domino>().GetIsGlued() == false) {
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
			if (Domino.DominoValueToInt(dominoMap[x - 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('E')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('B'))
				&& dominoMap[x - 1][y].GetComponent<Domino>().GetIsGlued() == false) {
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
			if (Domino.DominoValueToInt(dominoMap[x][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('F')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('C'))
				&& dominoMap[x][y + 1].GetComponent<Domino>().GetIsGlued() == false) {
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
			if (Domino.DominoValueToInt(dominoMap[x + 1][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('A')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('D'))
				&& dominoMap[x + 1][y + 1].GetComponent<Domino>().GetIsGlued() == false) {
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
			if (Domino.DominoValueToInt(dominoMap[x + 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('B')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('E'))
				&& dominoMap[x + 1][y].GetComponent<Domino>().GetIsGlued() == false) {
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
			if (Domino.DominoValueToInt(dominoMap[x][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('C')) < Domino.DominoValueToInt(current.GetDominoFaces().GetFace('F'))
				&& dominoMap[x][y - 1].GetComponent<Domino>().GetIsGlued() == false) {
				dominoMap[x][y - 1].GetComponent<Domino>().SetDominoColor(current.GetDominoColor());
				Domino.CreateDominoTexture(dominoMap[x][y - 1]);
				enemy.SupToRange (dominoMap [x] [y - 1]);
				enemy.AddToRange (dominoMap [x] [y - 1]);
				r++;
			}
		}
		if (r > 0 && whoIsPlaying == PlayerType.Computer) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetInGameWinningText ();
		} else if (r > 0 && whoIsPlaying == PlayerType.Real) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetInGameIsLosingText();
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
				if (dominoMap [x - 1] [y - 1].GetComponent<Domino> ().GetIsGlued () == false) {
					dominoMap [x - 1] [y - 1].GetComponent<Domino> ().SetDominoColor (current.GetDominoColor ());
					Domino.CreateDominoTexture (dominoMap [x - 1] [y - 1]);
					enemy.SupToRange (dominoMap [x - 1] [y - 1]);
					enemy.AddToRange (dominoMap [x - 1] [y - 1]);
					r++;
				}
				if (dominoMap [x + 1] [y + 1].GetComponent<Domino> ().GetIsGlued () == false) {
					dominoMap [x + 1] [y + 1].GetComponent<Domino> ().SetDominoColor (current.GetDominoColor ());
					Domino.CreateDominoTexture (dominoMap [x + 1] [y + 1]);
					enemy.SupToRange (dominoMap [x + 1] [y + 1]);
					enemy.AddToRange (dominoMap [x + 1] [y + 1]);
					r++;
				}
			}
		}
		if ((x > 0) && (y < dominoMap[x - 1].Length) && (x + 1 < dominoMap.Length) && (y < dominoMap [x + 1].Length)
			&& dominoMap[x - 1][y].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x - 1][y].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x + 1][y].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x + 1][y].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x - 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('E')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('B')) == Domino.DominoValueToInt(dominoMap[x + 1][y].GetComponent<Domino>().GetDominoFaces().GetFace('B')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('E'))) {
				if (dominoMap [x - 1] [y].GetComponent<Domino> ().GetIsGlued () == false) {
					dominoMap [x - 1] [y].GetComponent<Domino> ().SetDominoColor (current.GetDominoColor ());
					Domino.CreateDominoTexture (dominoMap [x - 1] [y]);
					enemy.SupToRange (dominoMap [x - 1] [y]);
					enemy.AddToRange (dominoMap [x - 1] [y]);
					r++;
				}
				if (dominoMap [x + 1] [y].GetComponent<Domino> ().GetIsGlued () == false) {
					dominoMap [x + 1] [y].GetComponent<Domino> ().SetDominoColor (current.GetDominoColor ());
					Domino.CreateDominoTexture (dominoMap [x + 1] [y]);
					enemy.SupToRange (dominoMap [x + 1] [y]);
					enemy.AddToRange (dominoMap [x + 1] [y]);
					r++;
				}
			}
		}
		if ((y > 0) && (y + 1 < dominoMap[x].Length)
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoColor() != color) {
			if (Domino.DominoValueToInt(dominoMap[x][y - 1].GetComponent<Domino>().GetDominoFaces().GetFace('C')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('F')) == Domino.DominoValueToInt(dominoMap[x][y + 1].GetComponent<Domino>().GetDominoFaces().GetFace('F')) + Domino.DominoValueToInt(current.GetDominoFaces().GetFace('C'))) {
				if (dominoMap [x] [y - 1].GetComponent<Domino> ().GetIsGlued () == false) {
					dominoMap [x] [y - 1].GetComponent<Domino> ().SetDominoColor (current.GetDominoColor ());
					Domino.CreateDominoTexture (dominoMap [x] [y - 1]);
					enemy.SupToRange (dominoMap [x] [y - 1]);
					enemy.AddToRange (dominoMap [x] [y - 1]);
					r++;
				}
				if (dominoMap [x] [y + 1].GetComponent<Domino> ().GetIsGlued () == false) {
					dominoMap [x] [y + 1].GetComponent<Domino> ().SetDominoColor (current.GetDominoColor ());
					Domino.CreateDominoTexture (dominoMap [x] [y + 1]);
					enemy.SupToRange (dominoMap [x] [y + 1]);
					enemy.AddToRange (dominoMap [x] [y + 1]);
					r++;
				}
			}
		}
		if (r > 0 && whoIsPlaying == PlayerType.Computer) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetInGameWinningText ();
		} else if (r > 0 && whoIsPlaying == PlayerType.Real) {
			GameObject.Find("Killer_Text").GetComponent<Text>().text = GameMessagesAccessor.GetInGameIsLosingText();
		}
	}

	public static bool CheckReturnClose(Domino dominoToReturn, Domino dominoToUse, int i) {
		DominoFaces faceToUse = dominoToUse.GetDominoFaces ();
		DominoFaces faceToReturn = dominoToReturn.GetDominoFaces ();

		switch (i) {
		case 0:
			return (Domino.DominoValueToInt (faceToUse.GetFace ('A')) > Domino.DominoValueToInt (faceToReturn.GetFace ('D')));
		case 1:
			return (Domino.DominoValueToInt(faceToUse.GetFace ('B')) > Domino.DominoValueToInt(faceToReturn.GetFace ('E')));
		case 2:
			return (Domino.DominoValueToInt(faceToUse.GetFace ('F')) > Domino.DominoValueToInt(faceToReturn.GetFace ('C')));
		case 3:
			return (Domino.DominoValueToInt(faceToUse.GetFace ('C')) > Domino.DominoValueToInt(faceToReturn.GetFace ('F')));
		case 4:
			return (Domino.DominoValueToInt(faceToUse.GetFace ('E')) > Domino.DominoValueToInt(faceToReturn.GetFace ('B')));
		case 5:
			return (Domino.DominoValueToInt(faceToUse.GetFace ('D')) > Domino.DominoValueToInt(faceToReturn.GetFace ('A')));
		default:
			return false;
		}
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
				return (Domino.DominoValueToInt (dominoMap [x - 1] [y - 1].GetComponent<Domino> ().GetDominoFaces ().GetFace ('D')) + Domino.DominoValueToInt (handDomino.GetDominoFaces ().GetFace ('A')) == Domino.DominoValueToInt (dominoMap [x + 1] [y + 1].GetComponent<Domino> ().GetDominoFaces ().GetFace ('A')) + Domino.DominoValueToInt (handDomino.GetDominoFaces ().GetFace ('D')));
			}
		if ((x > 0) && (y < dominoMap [x - 1].Length) && (x + 1 < dominoMap.Length) && (y < dominoMap [x + 1].Length)
			&& dominoMap [x - 1] [y].GetComponent<Domino> ().GetDominoType () == DominoType.Simple
			&& dominoMap [x - 1] [y].GetComponent<Domino> ().GetDominoColor () != color
			&& dominoMap [x + 1] [y].GetComponent<Domino> ().GetDominoType () == DominoType.Simple
			&& dominoMap [x + 1] [y].GetComponent<Domino> ().GetDominoColor () != color) {
				return (Domino.DominoValueToInt (dominoMap [x - 1] [y].GetComponent<Domino> ().GetDominoFaces ().GetFace ('E')) + Domino.DominoValueToInt (handDomino.GetDominoFaces ().GetFace ('B')) == Domino.DominoValueToInt (dominoMap [x + 1] [y].GetComponent<Domino> ().GetDominoFaces ().GetFace ('B')) + Domino.DominoValueToInt (handDomino.GetDominoFaces ().GetFace ('E')));
		}
		if ((y > 0) && (y + 1 < dominoMap[x].Length)
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y - 1].GetComponent<Domino>().GetDominoColor() != color
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoType() == DominoType.Simple
			&& dominoMap[x][y + 1].GetComponent<Domino>().GetDominoColor() != color) {
				return (Domino.DominoValueToInt (dominoMap [x] [y - 1].GetComponent<Domino> ().GetDominoFaces ().GetFace ('C')) + Domino.DominoValueToInt (handDomino.GetDominoFaces ().GetFace ('F')) == Domino.DominoValueToInt (dominoMap [x] [y + 1].GetComponent<Domino> ().GetDominoFaces ().GetFace ('F')) + Domino.DominoValueToInt (handDomino.GetDominoFaces ().GetFace ('C')));
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