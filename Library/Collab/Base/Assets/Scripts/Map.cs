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

	private float[] CreatePosXPosY(string name)
	{
		
		switch (name) {
		case "Lucie":
			return new float[2] {-1.885f, 1.3f};
		default:
			return new float[2] {-1.885f, 2};
		}
	}

	public void CreateMap(int[][] pos, string alt_name)
	{
		
		float[] posXY = CreatePosXPosY (alt_name);
		mapName = alt_name;
		map = new GameObject[pos.Length][];
		float pos_x = posXY[0];
		float pos_y = posXY[1];
		for (int i = 0; i < pos.Length; i++) {
			map [i] = new GameObject[pos [i].Length];
			for (int j = 0; j < pos[i].Length; j++) {
				if (pos [i] [j] == 1) {
					map [i] [j] = Instantiate (Resources.Load ("Prefab/Board") as GameObject);
					map [i] [j].GetComponent<Domino>().CreateDomino (DominoType.Blank);
					map [i] [j].name = "test" + j;
					map [i] [j].transform.position = new Vector2(pos_x,pos_y);
					map [i] [j].transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				} else {
					map [i] [j] = Instantiate (Resources.Load ("Prefab/Board") as GameObject);
					map [i] [j].GetComponent<Domino>().CreateDomino ();
					map [i] [j].GetComponent<SpriteRenderer> ().sprite = null;
					map [i] [j].transform.position = new Vector2(pos_x,pos_y);
					map [i] [j].transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
					map [i] [j].tag = "Untagged";
				}
//				Debug.Log (i+":"+j+" = "+pos [i] [j]);
				//Debug.Log(map[i][j].GetDominoType());
				pos_x = pos_x + 0.625f;
				pos_y = pos_y + 0.375f;
			}
			pos_x = posXY[0];
			pos_y = posXY[1]+((-1.45f*(i+1))/2);
		}
	}

}