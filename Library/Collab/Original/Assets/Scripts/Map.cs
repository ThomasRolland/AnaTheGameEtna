using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map : MonoBehaviour {

	private string mapName;
	private GameObject[][] map;

	public string GetName()
	{
		return mapName;
	}
		
	public GameObject GetDomino(int a,int b)
	{
		return (map [a] [b]);
	}

	private Vector2 CreatePosXPosY(string name)
	{
		Debug.Log (name);
		switch (name) {
		case "Agathe":
			return new Vector2 (-1.82f, 1f);
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
			return new Vector2 (-1.2f, 1.3f);
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
					map [i] [j].GetComponent<Domino>().CreateDomino (DominoType.Blank);
					map [i] [j].name = "test" + j;
					map [i] [j].transform.position = posXY;
					map [i] [j].transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				} else {
					map [i] [j] = Instantiate (Resources.Load ("Prefab/Board") as GameObject);
					map [i] [j].GetComponent<Domino>().CreateDomino ();
					map [i] [j].GetComponent<SpriteRenderer> ().sprite = null;
					map [i] [j].transform.position = posXY;
					map [i] [j].transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
					map [i] [j].tag = "Untagged";
				}
//				Debug.Log (i+":"+j+" = "+pos [i] [j]);
				//Debug.Log(map[i][j].GetDominoType());
				posXY.x += 0.625f;
				posXY.y += 0.375f;
			}
			posXY.x = temp_posXY.x;
			posXY.y = temp_posXY.y+((-1.45f*(i+1))/2);
		}
	}

}