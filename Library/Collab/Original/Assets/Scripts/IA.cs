using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour {
	
	Map m;
	GameObject hex;
	Domino domino;

	// Use this for initialization
	void Start () {
		m = gameObject.GetComponent<Map> ();
	}

	public Domino CheckWhereToPlay(GameObject[] handIA) {
		Coordinate XY = new Coordinate(0, 0);
		GameObject[][] map = m.GetMap ();
		int dominoToUseIndex;

		dominoToUseIndex = DominoToUseIndex (handIA);
		//make random for first place
		handIA [dominoToUseIndex].GetComponent<Domino> ().SetPositionXY (XY);
		for (int i = 0; i < map.Length; i++) {
			for (int j = 0; j < map [i].Length; j++) {
				domino = m.GetDomino(i, j).GetComponent<Domino> ();
				if ((domino.GetDominoType () == DominoType.Blank) && (domino.GetRange(DominoColor.Black) != DominoValues.None)) {
					if (Domino.DominoValueToInt(domino.GetRange(DominoColor.Black)) == m.GetHigherNB())
					{
						Player.CheckByRange (domino, handIA[dominoToUseIndex].GetComponent<Domino>());
						XY.SetX(i);
						XY.SetY(j);
						handIA [dominoToUseIndex].GetComponent<Domino> ().SetPositionXY (XY);
						return handIA[dominoToUseIndex].GetComponent<Domino>();
					}
				}
			}
		}
		return handIA[dominoToUseIndex].GetComponent<Domino>();
	}

	public int DominoToUseIndex(GameObject[] handIA) {
		int[] 	totalFaces = {0, 0, 0};
		int 	dominoIndex;
		int 	higherTotal;

		dominoIndex = 0;
		higherTotal = 7;
		for (int i = 0; i < handIA.Length; i++) {
			totalFaces[i] = handIA [i].GetComponent<Domino> ().GetTotalFaces ();
			if (totalFaces [i] > higherTotal) {
				higherTotal = totalFaces [i];
				dominoIndex = i;
			}
		}
		return dominoIndex;
	}

	public void DominoToTrash(GameObject[] handIA) {
		int[] 	totalFaces = {0, 0, 0};
		int 	dominoIndex;
		int 	lowerTotal;

		dominoIndex = 2;
		lowerTotal = 35;
		for (int i = 0; i < handIA.Length; i++) {
			if (handIA[i] != null)
			{
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
					domino.SetDominoType (useDomino.GetDominoType ());
					domino.SetDominoColor (useDomino.GetDominoColor ());
					domino.SetDominoFaces (useDomino.GetDominoFaces ());
					Player.CreateDominoTexture (hex);
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

		if (color == DominoColor.Black)
			rangeColor = DominoColor.White;
		else
			rangeColor = DominoColor.Black;

		for (int i = 0; i < 6; i++) {
			a = 0;
			b = 0;
			if (i == 0) {
				a++;
				b++;
			}
			if (i == 1)
				a++;
			if (i == 2)
				b++;
			if (i == 3)
				b--;
			if (i == 4)
				a--;
			if (i == 5) {
				a--;
				b--;
			}
			GameObject GameDomino = m.GetDomino ((posX - a), (posY - b));
			if (GameDomino != null)
			{
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

	public void SupToRange(GameObject lastDomino)
	{
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
			if (i == 1)
				a++;
			if (i == 2)
				b++;
			if (i == 3)
				b--;
			if (i == 4)
				a--;
			if (i == 5)
			{
				a--;
				b--;
			}
			GameObject GameDomino = m.GetDomino ((posX - a), (posY - b));
			if (GameDomino != null)
			{
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
