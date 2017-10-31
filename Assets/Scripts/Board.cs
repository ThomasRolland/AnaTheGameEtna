using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Board : MonoBehaviour {

	Button ButtonHome;
	private	Dictionary<string, int[][]>	maps; // Game maps
	bool isFull;


	void Start () {
		ButtonHome = GameObject.Find("ButtonHomeJeu").GetComponent<Button>();
		ButtonHome.onClick.AddListener( () => {ButtonHomeOnClickEvent();} );

		isFull = false;
		maps = new Dictionary<string, int[][]>();;

		maps["Agathe"] = new int[][] {
			new int[] { 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 0, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 0, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 0, 1 },
			new int[] { 0, 0, 0, 0, 1, 1 },
			new int[] { 0, 0, 0, 0, 0, 0, 1 }
		};
		maps["Aline"] = new int[][] {
			new int[] { 1 },
			new int[] { 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 0 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 0 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1 },
			new int[] { 0, 0, 0, 0, 1 }
		};
		maps["Anna"] = new int[][] {
			new int[] { 0, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1, 1 }
		};
		maps["Aurelie"] = new int[][] {
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 }
		};
		maps["Camille"] = new int[][] {
			new int[] { 1 },
			new int[] { 1 },
			new int[] { 1, 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 0, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 0, 1, 1 }
		};
		maps["Charlene"] = new int[][] {
			new int[] { 0, 1 },
			new int[] { 0, 1, 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 0, 1, 1, 1 },
			new int[] { 1, 1, 1, 0, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1 },
			new int[] { 0, 0, 0, 1 }
		};
		maps["Chloe"] = new int[][] {
			new int[] { 1, 0, 1 },
			new int[] { 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1 },
			new int[] { 1, 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 0, 1, 0, 1, 1 },
			new int[] { 0, 1, 0, 1, 1 },
			new int[] { 0, 0, 0, 1 }
		};
		maps["Clemence"] = new int[][] {
			new int[] { 0, 1 },
			new int[] { 1, 1, 1 },
			new int[] { 1, 1, 0, 1 },
			new int[] { 1, 0, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 0, 1, 1, 1 },
			new int[] { 0, 1, 0, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 1, 0, 1 },
			new int[] { 0, 0, 0, 0, 1, 1, 1 }
		};
		maps["Eva"] = new int[][] {
			new int[] { 1, 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1 }
		};
		maps["Louisa"] = new int[][] {
			new int[] { 1 },
			new int[] { 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 0, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 0, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 1, 1 },
			new int[] { 0, 0, 0, 0, 0, 1 }
		};
		maps["Louise"] = new int[][] {
			new int[] { 0, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 0, 1, 1, 1 },
			new int[] { 0, 1, 0, 1, 0, 1, 1 },
			new int[] { 1, 1, 0, 1, 0, 1 },
			new int[] { 0, 1, 1, 1, 0, 1 },
			new int[] { 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 1, 1 }
		};
		maps["Lucie"] = new int[][] {
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1, 1, 1 }
		};
		maps["Malika"] = new int[][] {
			new int[] { 0, 0, 1 },
			new int[] { 0, 0, 1, 1, 1 },
			new int[] { 1, 0, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1 }
		};
		maps["Manon"] = new int[][] {
			new int[] { 1 },
			new int[] { 1 },
			new int[] { 1, 0, 1, 0, 1 },
			new int[] { 1, 1, 1, 0, 1 },
			new int[] { 1, 1, 1, 1, 1, 0, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 0, 1, 1, 1, 1, 1 },
			new int[] { 1, 0, 1, 0, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 1, 0, 1 },
			new int[] { 0, 0, 0, 0, 1, 0, 1 }
		};
		maps["Margaux"] = new int[][] {
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 0, 0, 0, 1, 1 },
			new int[] { 0, 1, 1, 0, 0, 1, 1 },
			new int[] { 0, 1, 1, 1, 0, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 1, 1 }
		};
		maps["Marie"] = new int[][] {
			new int[] { 1, 0, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1 }
		};
		maps["Marine"] = new int[][] {
			new int[] { 1, 1 },
			new int[] { 1, 1, 1 },
			new int[] { 1, 1, 1, 0, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1, 1, 1 }
		};
		maps["Mathilde"] = new int[][] {
			new int[] { 1, 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 1, 0, 0, 1, 1 },
			new int[] { 1, 1, 0, 0, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1 }
		};
		maps["Nausica"] = new int[][] {
			new int[] { 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 0, 0, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 0, 1, 1 },
			new int[] { 0, 0, 0, 1, 1 },
			new int[] { 0, 0, 0, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 1, 1 }
		};
		maps["Noemie"] = new int[][] {
			new int[] { 0, 0, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 1, 1 },
			new int[] { 0, 0, 0, 1, 0, 1, 1 },
			new int[] { 0, 0, 0, 0, 0, 0, 1 }
		};
		maps["Sarah"] = new int[][] {
			new int[] { 1, 1 },
			new int[] { 0, 1, 1 },
			new int[] { 0, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 0, 1, 1 },
			new int[] { 0, 0, 0, 0, 0, 1, 1 }
		};
		maps["Sophie"] = new int[][] {
			new int[] { 1, 0, 1 },
			new int[] { 1, 1, 1, 0, 1 },
			new int[] { 0, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 1, 1, 1, 1, 1, 1 },
			new int[] { 0, 0, 1, 0, 1, 1 },
			new int[] { 0, 0, 0, 0, 1, 1 }
		};
		maps["Vanessa"] = new int[][] {
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 1, 1, 1, 1 },
			new int[] { 0, 0, 0, 1 },
			new int[] { 0, 0, 0, 1, 1 },
			new int[] { 0, 0, 0, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 1, 1, 1 },
			new int[] { 0, 0, 0, 0, 0, 1, 1 },
			new int[] { 0, 0, 0, 0, 0, 0, 1 }
		};
		CreateBoard (AppSupervisor.mapToLoad);
	}

	public void CreateBoard(string name) {
		foreach(KeyValuePair<string, int[][]> entry in maps) {
			if (entry.Key == name) {
				gameObject.GetComponent<Map> ().CreateMap (entry.Value, name);
			}
		}
	}

	public bool isBoardFull() {
		if (isFull == true) {
			return true;
		}
		return false;
	}

	void ButtonHomeOnClickEvent() {
		SceneManager.LoadScene ("Menu");
	}

}
