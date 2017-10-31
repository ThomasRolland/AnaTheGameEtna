using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map : MonoBehaviour {

	private string			mapName;
	private GameObject[][]	map;
	private int				higherNB;

	public int GetHigherNB() {
		return higherNB;
	}

	public void SetHigherNB(int i) {
		higherNB = i;
	}

	public string GetName() {
		return mapName;
	}

	public GameObject[][] GetMap() {
		return map;
	}
		
	public  GameObject GetDomino(int a, int b) {
		if ((a < map.Length) && (a >= 0)) {
			if ((b < map[a].Length) && (b >= 0)) {
				return (map [a] [b]);	
			}
		}
		return (null);
	}

	public void SearchAndSetHigherNB(DominoColor color) {
		Domino domino;
		int rangeB;

		SetHigherNB (0);
		for (int i = 0; i < map.Length; i++) {
			for (int j = 0; j < map [i].Length; j++) {
				domino = GetDomino (i, j).GetComponent<Domino> ();
				rangeB = Domino.DominoValueToInt(domino.GetRange(color));
				if ((domino.GetDominoType () == DominoType.Blank) && (rangeB > higherNB)) {
					SetHigherNB (rangeB);
				}
			}
		}
	}

	private Vector2 CreatePosXPosY(string name)
	{
		switch (name) {
		case "Agathe":
			return new Vector2 (-1.82f, 1.6f);
		case "Aline":
			return new Vector2 (-1.2f, 2.8f);
		case "Anna":
			return new Vector2 (-1.8f, 1.3f);
		case "Aurelie":
			return new Vector2 (-1.8f, 0.6f);
		case "Camille":
			return new Vector2 (-1.8f, 2.6f);
		case "Charlene":
			return new Vector2 (-1.2f, 2.5f);
		case "Chloe":
			return new Vector2 (-1.2f, 2f);
		case "Clemence":
			return new Vector2 (-1.82f, 2.3f);
		case "Eva":
			return new Vector2 (-1.2f, 1f);
		case "Louisa":
			return new Vector2 (-1.8f, 2.65f);
		case "Louise":
			return new Vector2 (-1.8f, 2.15f);
		case "Lucie":
			return new Vector2 (-1.82f, 1f);
		case "Malika":
			return new Vector2 (-1.82f, 2f);
		case "Manon":
			return new Vector2 (-1.82f, 2.8f);
		case "Margaux":
			return new Vector2 (-1.82f, 1.35f);
		case "Marie":
			return new Vector2 (-1.82f, 1f);
		case "Marine":
			return new Vector2 (-1.82f, 1.25f);
		case "Mathilde":
			return new Vector2 (-1.2f, 1.8f);
		case "Nausica":
			return new Vector2 (-1.6f, 2.1f);
		case "Noemie":
			return new Vector2 (-1.82f, 1f);
		case "Sarah":
			return new Vector2 (-1.82f, 2.5f);
		case "Sophie":
			return new Vector2 (-1.82f, 1.1f);
		case "Vanessa":
			return new Vector2 (-1.82f, 1.25f);
		default:
			return new Vector2 (-1.885f, 2);
		}
	}

	public void CreateMap(int[][] pos, string alt_name)
	{
		Vector2 posXY = CreatePosXPosY (alt_name);
		Vector2 temp_posXY = posXY;
		mapName = alt_name;
		map = new GameObject[pos.Length][];

		for (int i = 0; i < pos.Length; i++) {
			map [i] = new GameObject[pos [i].Length];
			for (int j = 0; j < pos[i].Length; j++) {
				if (pos [i] [j] == 1) {
					map [i] [j] = Instantiate (Resources.Load ("Prefab/Board") as GameObject);
					map [i] [j].GetComponent<Domino>().CreateDomino (DominoType.Blank, new Coordinate(i, j));
					map [i] [j].name = "Dominos_" + j;
					map [i] [j].transform.position = posXY;
					map [i] [j].transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				} else {
					map [i] [j] = Instantiate (Resources.Load ("Prefab/Board") as GameObject);
					map [i] [j].GetComponent<Domino>().CreateDomino (new Coordinate(i, j));
					map [i] [j].GetComponent<SpriteRenderer> ().sprite = null;
					map [i] [j].name = "Dominos_" + j;
					map [i] [j].transform.position = posXY;
					map [i] [j].transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
					map [i] [j].tag = "Untagged";
					map [i] [j].GetComponent<PolygonCollider2D> ().enabled = false;
				}
				posXY.x += 0.625f;
				posXY.y += 0.375f;
			}
			posXY.x = temp_posXY.x;
			posXY.y = temp_posXY.y + ((-1.45f * (i + 1)) / 2);
		}
	}

	public DominoColor IsGameFinished() {
		int black = 0;
		int white = 0;
		for (int i = 0; i < map.Length; i++) {
			for (int j = 0; j < map[i].Length; j++) {
				DominoColor color = map [i] [j].GetComponent<Domino> ().GetDominoColor ();
				DominoType type = map [i] [j].GetComponent<Domino> ().GetDominoType ();
				if (color == DominoColor.Black) {
					black++;
				} else if (color == DominoColor.White) {
					white++;
				} else if (type != DominoType.Invisible && color != DominoColor.Bonus) {
					return DominoColor.None;
				}
			}
		}
		if (black > white) {
			return DominoColor.Black;
		} else {
			return DominoColor.White;
		}
	}
}