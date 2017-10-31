using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour {
	
	Map			m;
	GameObject	hex;
	Domino		domino;

	void Start () {
		m = gameObject.GetComponent<Map> ();
	}

	public Domino CheckWhereToPlay(GameObject[] handIA) {
		Coordinate XY = new Coordinate (0, 0);
		GameObject[][] map = m.GetMap ();
		int[] dominoToUseIndex = DominoToUseIndex (handIA);
		int rand1 = UnityEngine.Random.Range (0, map.Length);
		int rand2 = UnityEngine.Random.Range (0, map [rand1].Length);

		for (int rangetoUse = m.GetHigherNB () + 1; rangetoUse >= 0; rangetoUse--) {
			for (int k = 2; k >= 0; k--) {
				handIA [dominoToUseIndex [k]].GetComponent<Domino> ().SetPositionXY (XY);
				for (int i = 0; i < map.Length; i++) {
					for (int j = 0; j < map [i].Length; j++) {
						domino = m.GetDomino (i, j).GetComponent<Domino> ();
						if ((domino.GetDominoType () == DominoType.Blank) && (domino.GetRange (DominoColor.Black) != DominoValues.None)) {
							if (Domino.DominoValueToInt (domino.GetRange (DominoColor.Black)) >= 2) {
								if (Player.CheckSymetricalSum (domino, handIA [dominoToUseIndex [k]].GetComponent<Domino> ())) {
									XY.SetX (i);
									XY.SetY (j);
									handIA [dominoToUseIndex [k]].GetComponent<Domino> ().SetPositionXY (XY);
									return handIA [dominoToUseIndex [k]].GetComponent<Domino> ();
								}
							}
							if (Domino.DominoValueToInt (domino.GetRange (DominoColor.Black)) == rangetoUse) {
								if (Player.CheckByRange (domino, handIA [dominoToUseIndex [k]].GetComponent<Domino> ())) {
									XY.SetX (i);
									XY.SetY (j);
									handIA [dominoToUseIndex [k]].GetComponent<Domino> ().SetPositionXY (XY);
									return handIA [dominoToUseIndex [k]].GetComponent<Domino> ();
								}
							}
						}
					}
				}
			}
		}
		domino = m.GetDomino (rand1, rand2).GetComponent<Domino> ();
		while (domino.GetDominoType () != DominoType.Blank) {
			rand1 = (int)UnityEngine.Random.Range (0, (float)map.Length);
			rand2 = (int)UnityEngine.Random.Range (0, (float)map [rand1].Length);
			domino = m.GetDomino (rand1, rand2).GetComponent<Domino> ();
		}
		XY.SetX (rand1);
		XY.SetY (rand2);
		handIA [dominoToUseIndex [2]].GetComponent<Domino> ().SetPositionXY (XY);
		return handIA [dominoToUseIndex [2]].GetComponent<Domino> ();
	}

	public int[] DominoToUseIndex(GameObject[] handIA) {
		int[]	totalFaces = {0, 0, 0};
		int[]	tempTotalFaces = {0, 0, 0};
		int[]	dominoIndex = new int[3];

		for (int i = 0; i < handIA.Length; i++) {
			totalFaces [i] = handIA [i].GetComponent<Domino> ().GetTotalFaces ();
			if (i == 0) {
				dominoIndex [i] = i;
				tempTotalFaces [i] = totalFaces [i];
			}
			if (i == 1) {
				if (totalFaces [i] >= tempTotalFaces [i - 1]) {
					dominoIndex [i] = i;
					tempTotalFaces [i] = totalFaces [i];
				} else {
					dominoIndex [i] = dominoIndex [i - 1];
					dominoIndex [i - 1] = i;
					tempTotalFaces [i] = tempTotalFaces [i - 1];
					tempTotalFaces [i - 1] = totalFaces [i];
				}
			}
			if (i == 2) {
				if ((totalFaces [i] >= tempTotalFaces [i - 2]) && (totalFaces [i] >= tempTotalFaces [i - 1])) {
					dominoIndex [i] = i;
					tempTotalFaces [i] = totalFaces [i];
				} else if ((totalFaces [i] >= tempTotalFaces [i - 2]) && (totalFaces [i] <= tempTotalFaces [i - 1])) {
					dominoIndex [i] = dominoIndex [i - 1];
					dominoIndex [i - 1] = i;
					tempTotalFaces [i] = tempTotalFaces [i - 1];
					tempTotalFaces [i - 1] = totalFaces [i];
				} else {
					dominoIndex [i] = dominoIndex [i - 1];
					dominoIndex [i - 1] = dominoIndex [i - 2];
					dominoIndex [i - 2] = i;
					tempTotalFaces [i] = tempTotalFaces [i - 1];
					tempTotalFaces [i - 1] = tempTotalFaces [i - 2];
					tempTotalFaces [i - 2] = totalFaces [i];
				}
			}
		}
		return dominoIndex;
	}

	public void DominoToTrash(GameObject[] handIA) {
		int[]	totalFaces = {0, 0, 0};
		int		dominoIndex;
		int		lowerTotal;

		dominoIndex = 2;
		lowerTotal = 35;
		for (int i = 0; i < handIA.Length; i++) {
			if (handIA[i] != null) {
				totalFaces[i] = handIA [i].GetComponent<Domino>().GetTotalFaces ();
				if (totalFaces [i] < lowerTotal) {
					lowerTotal = totalFaces [i];
					dominoIndex = i;
				}
			}
		}
		Destroy (handIA [dominoIndex]);
		handIA [dominoIndex] = null;
	}

	public GameObject[] PutDominos(Domino useDomino, GameObject[] handIA) {
		hex = m.GetDomino(useDomino.GetPosition().GetX(), useDomino.GetPosition().GetY());
		domino = hex.GetComponent<Domino> ();

		for (int i = 0; i < handIA.Length; i++) {
			if (Domino.CompareDominoFaces (useDomino.GetDominoFaces(), handIA [i].GetComponent<Domino> ().GetDominoFaces ())) {
				if (domino.GetDominoType () == DominoType.Blank) {
					hex.tag = "PlayedDomino";
					domino.SetDominoType (useDomino.GetDominoType ());
					domino.SetDominoColor (useDomino.GetDominoColor ());
					domino.SetDominoFaces (useDomino.GetDominoFaces ());
					Domino.CreateDominoTexture (hex);
					AddToRange (hex);
					Destroy (handIA [i]);
					handIA [i] = null;
					Player.SymetricalSum (domino.GetPosition (), domino.GetDominoColor ());
					Player.ReturnClose (domino.GetPosition (), domino.GetDominoColor ());
				}
			}
		}
		return handIA;
	}

	public void AddToRange(GameObject lastDomino)
	{
		DominoColor color = lastDomino.GetComponent<Domino> ().GetDominoColor ();
		DominoColor rangeColor;
		int posX = lastDomino.GetComponent<Domino> ().GetPosition ().GetX();
		int posY = lastDomino.GetComponent<Domino> ().GetPosition ().GetY();

		DominoValues newValue;
		int a;
		int b;

		if (color == DominoColor.Black) {
			rangeColor = DominoColor.White;
		} else {
			rangeColor = DominoColor.Black;
		}

		for (int i = 0; i < 6; i++) {
			a = 0;
			b = 0;
			if (i == 0) {
				a++;
				b++;
			}
			if (i == 1) {
				a++;
			}
			if (i == 2) {
				b++;
			}
			if (i == 3) {
				b--;
			}
			if (i == 4) {
				a--;
			}
			if (i == 5) {
				a--;
				b--;
			}
			GameObject GameDomino = m.GetDomino ((posX - a), (posY - b));
			if (GameDomino != null) {
				Domino newDomino = GameDomino.GetComponent<Domino> ();
				if ((newDomino.GetDominoType () != DominoType.Invisible) &&
					(newDomino.GetDominoType () != DominoType.Simple)) {
					DominoValues oldValue = newDomino.GetRange (rangeColor);
					switch (oldValue) {
					case DominoValues.None:
						newValue = DominoValues.One;
						break;
					case DominoValues.One:
						newValue = DominoValues.Two;
						break;
					case DominoValues.Two:
						newValue = DominoValues.Three;
						break;
					case DominoValues.Three:
						newValue = DominoValues.Four;
						break;
					case DominoValues.Four:
						newValue = DominoValues.Five;
						break;
					case DominoValues.Five:
						newValue = DominoValues.Six;
						break;
					default:
						newValue = oldValue;
						break;
					}
					newDomino.SetRange (newValue, rangeColor);
				}
			}
		}
		m.SearchAndSetHigherNB (rangeColor);
	}

	public void SupToRange(GameObject lastDomino) {
		DominoColor color = lastDomino.GetComponent<Domino> ().GetDominoColor ();
		int posX = (int)lastDomino.GetComponent<Domino> ().GetPosition ().GetX();
		int posY = (int)lastDomino.GetComponent<Domino> ().GetPosition ().GetY();

		DominoValues newValue;
		int a;
		int b;
		for (int i = 0; i < 6; i++) {
			a = 0;
			b = 0;
			if (i == 0) {
				a++;
				b++;
			}
			if (i == 1) {
				a++;
			}
			if (i == 2) {
				b++;
			}
			if (i == 3) {
				b--;
			}
			if (i == 4) {
				a--;
			}
			if (i == 5) {
				a--;
				b--;
			}
			GameObject GameDomino = m.GetDomino ((posX - a), (posY - b));
			if (GameDomino != null) {
				Domino newDomino = GameDomino.GetComponent<Domino> ();
				if (newDomino.GetDominoType () != DominoType.Invisible) {
					DominoValues oldValue = newDomino.GetRange (color);
					switch (oldValue) {
					case DominoValues.Six:
						newValue = DominoValues.Five;
						break;
					case DominoValues.Five:
						newValue = DominoValues.Four;
						break;
					case DominoValues.Four:
						newValue = DominoValues.Three;
						break;
					case DominoValues.Three:
						newValue = DominoValues.Two;
						break;
					case DominoValues.Two:
						newValue = DominoValues.One;
						break;
					case DominoValues.One:
						newValue = DominoValues.None;
						break;
					default:
						newValue = oldValue;
						break;
					}
					newDomino.SetRange (newValue, color);
				}
			}
		}
	}
}
