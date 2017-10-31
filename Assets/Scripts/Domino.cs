using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DominoValues {
	None = 0,
	One = 1,
	Two = 2,
	Three = 3,
	Four = 4,
	Five = 5,
	Six = 6
}

public enum DominoColor {
	White,
	Black,
	Grey,
	None,
	Bonus
}

public enum DominoType {
	NotBonus,
	BonusInUse,
	Invisible,
	Empty,
	Blank, // TYPE FOR BLANK CASE ON BOARD
	Simple,
	OneLess,
	OneMore,
	TwoLess,
	TwoMore,
	ThreeLess,
	ThreeMore,
	Trap,
	Radar,
	ChoosenOne,
	Log,
	Color,
	Swap,
	Graviton,
	Megaton,
	Glue,
	Nugget,
	Blur,
	Switch,
	Invert,
	Draw,
	Jump
}

public struct DominoFaces {
	DominoValues	faceA;
	DominoValues	faceB;
	DominoValues	faceC;
	DominoValues	faceD;
	DominoValues	faceE;
	DominoValues	faceF;


	public DominoValues GetFace(char c) {
		switch (c) {
		case 'A':
			return faceA;
		case 'B':
			return faceB;
		case 'C':
			return faceC;
		case 'D':
			return faceD;
		case 'E':
			return faceE;
		case 'F':
			return faceF;
		default:
			return DominoValues.None;
		}
	}

	public void SetFace(char c, DominoValues v) {
		switch (c) {
		case 'A':
			faceA = v;
			break;
		case 'B':
			faceB = v;
			break;
		case 'C':
			faceC = v;
			break;
		case 'D':
			faceD = v;
			break;
		case 'E':
			faceE = v;
			break;
		case 'F':
			faceF = v;
			break;
		default:
			Debug.Log("Something went wrong on Domino.cs 92");
			break;
		}
	}

	public DominoFaces(DominoValues _faceA, DominoValues _faceB, DominoValues _faceC, DominoValues _faceD, DominoValues _faceE, DominoValues _faceF) {
		faceA = _faceA;
		faceB = _faceB;
		faceC = _faceC;
		faceD = _faceD;
		faceE = _faceE;
		faceF = _faceF;
	}
}

public struct Coordinate {
	private int x;
	private int y;

	public Coordinate(int _x, int _y) {
		x = _x;
		y = _y;
	}

	public int GetX() {
		return x;
	}

	public void SetX(int _x) {
		x = _x;
	}

	public int GetY() {
		return y;
	}

	public void SetY(int _y) {
		y = _y;
	}
}

public class Domino : MonoBehaviour {

	private DominoFaces		dominoFaces;
	private DominoType		dominoType;
	private DominoColor		color;
	private string			textureName;
	private Coordinate		position;
	private DominoValues	rangeB;
	private DominoValues	rangeN;
	private int				totalFaces;
	private bool			isGlued;

	public Coordinate GetPosition() {
		return position;
	}

	public void SetPositionXY(Coordinate _position) {
		position = _position;	
	}

	public bool GetIsGlued() {
		return isGlued;
	}

	public void SetIsGlued(bool g) {
		isGlued = g;
	}

	public DominoValues GetRange(DominoColor color)
	{
		if (color == DominoColor.Black)
			return rangeB;
		else
			return rangeN;
	}

	public bool BetterRange(DominoValues oldRange, DominoValues newRange)
	{
		if (oldRange == DominoValues.None)
			return true;
		else if ((oldRange == DominoValues.One) && (newRange != DominoValues.One))
			return true;
		else if ((oldRange == DominoValues.Two) && (newRange != DominoValues.One) && (newRange != oldRange))
			return true;
		else if ((oldRange == DominoValues.Three) && ((newRange != DominoValues.One) && (newRange != DominoValues.Two)) && (newRange != oldRange))
			return true;
		else if ((oldRange == DominoValues.Four) && ((newRange != DominoValues.One) && (newRange != DominoValues.Two) && (newRange != DominoValues.Three)) && (newRange != oldRange))
			return true;
		else if ((oldRange == DominoValues.Five) && (newRange == DominoValues.Six) && (newRange != oldRange))
			return true;
		else
			return false;
	}

	public void SetRange(DominoValues i, DominoColor color)
	{
		if (color == DominoColor.Black)
			rangeB = i;
		else
			rangeN = i;
	}

	public int GetTotalFaces() {
		totalFaces = 0;
		totalFaces += DominoValueToInt(dominoFaces.GetFace ('A'));
		totalFaces += DominoValueToInt(dominoFaces.GetFace ('B'));
		totalFaces += DominoValueToInt(dominoFaces.GetFace ('C'));
		totalFaces += DominoValueToInt(dominoFaces.GetFace ('D'));
		totalFaces += DominoValueToInt(dominoFaces.GetFace ('E'));
		totalFaces += DominoValueToInt(dominoFaces.GetFace ('F'));
		return totalFaces;
	}

	public void CreateDomino(Coordinate _position) {
		dominoFaces = new DominoFaces (DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None);
		dominoType = DominoType.Invisible;
		color = DominoColor.None;
		textureName = "None";
		position = _position;
		rangeB = DominoValues.None;
		rangeN = DominoValues.None;
		isGlued = false;
	}

	public void CreateDomino(DominoType _type) {
		dominoFaces = new DominoFaces (DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None);
		color = DominoColor.None;
		textureName = "None";
		dominoType = _type;
		rangeB = DominoValues.None;
		rangeN = DominoValues.None;
		position = new Coordinate ();
		isGlued = false;
	}

	public void CreateDomino(DominoType t, Coordinate _position) {
		if (t == DominoType.Blank) {
			dominoFaces = new DominoFaces (DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None, DominoValues.None);
			dominoType = DominoType.Blank;
			color = DominoColor.Grey;
			textureName = "Images/Dominos/Blanc/Domino_Vierge";
			position = _position;
			rangeB = DominoValues.None;
			rangeN = DominoValues.None;
			isGlued = false;
		}
	}

	public void CreateDomino(DominoValues _faceA, DominoValues _faceB, DominoValues _faceC, DominoValues _faceD, DominoValues _faceE, DominoValues _faceF, DominoType _type) {
		dominoFaces = new DominoFaces (_faceA, _faceB, _faceC, _faceD, _faceE, _faceF);
		dominoType = _type;
		color = DominoColor.Grey;
		textureName = "Images/Dominos/Blanc/Domino_Numerote";
		rangeB = DominoValues.None;
		rangeN = DominoValues.None;
		isGlued = false;
	}

	public string GetTexture () {
		return textureName;
	}

	public DominoFaces GetDominoFaces() {
		return dominoFaces;
	}

	public void SetDominoFaces(DominoFaces d) {
		dominoFaces = d;
	}

	public DominoType GetDominoType() {
		return dominoType;
	}

	public void SetDominoType(DominoType t) {
		dominoType = t;
	}

	public DominoColor GetDominoColor() {
		return color;
	}

	public void SetDominoColor(DominoColor c) {
		color = c;
	}

	public static int DominoValueToInt(DominoValues d) {
		switch (d) {
		case DominoValues.None:
			return 0;
		case DominoValues.One:
			return 1;
		case DominoValues.Two:
			return 2;
		case DominoValues.Three:
			return 3;
		case DominoValues.Four:
			return 4;
		case DominoValues.Five:
			return 5;
		case DominoValues.Six:
			return 6;
		default:
			return -1;
		}
	}

	public static DominoValues IntToDominoValue(int i) {
		switch (i) {
		case 0 :
			return DominoValues.None;
		case 1:
			return  DominoValues.One;
		case 2:
			return DominoValues.Two;
		case 3:
			return DominoValues.Three;
		case 4:
			return DominoValues.Four;
		case 5:
			return DominoValues.Five;
		case 6:
			return DominoValues.Six;
		default:
			return DominoValues.None;
		}
	}

	public static DominoColor GetOppositeColor(DominoColor c) {
		if (c == DominoColor.Black) {
			return DominoColor.White;
		} else if (c == DominoColor.White) {
			return DominoColor.Black;
		}
		return DominoColor.Grey;
	}

	public static void CreateDominoTexture(GameObject g) {
		Domino d = g.GetComponent<Domino> ();
		if (d.GetDominoType() == DominoType.Simple && d.GetDominoColor() == DominoColor.White) {
			Texture2D t = Resources.Load<Texture2D> ("Images/Dominos/Blanc/Domino_Vierge");
			g.GetComponent<SpriteRenderer> ().sprite = Sprite.Create (t, new Rect (0, 0, t.width, t.height), new Vector2 (0.5f, 0.5f));
			SpriteRenderer[] childrenRenderer = g.GetComponentsInChildren<SpriteRenderer> ();
			for (int i = 0; i < childrenRenderer.Length; i++) {
				Texture2D ct = null;
				switch (i) {
				case 1:
					ct = Resources.Load<Texture2D> (GetWhiteTextureName (d.GetDominoFaces ().GetFace ('A')));
					break;
				case 2:
					ct = Resources.Load<Texture2D> (GetWhiteTextureName (d.GetDominoFaces ().GetFace ('B')));
					break;
				case 3:
					ct = Resources.Load<Texture2D> (GetWhiteTextureName (d.GetDominoFaces ().GetFace ('C')));
					break;
				case 4:
					ct = Resources.Load<Texture2D> (GetWhiteTextureName (d.GetDominoFaces ().GetFace ('D')));
					break;
				case 5:
					ct = Resources.Load<Texture2D> (GetWhiteTextureName (d.GetDominoFaces ().GetFace ('E')));
					break;
				case 6:
					ct = Resources.Load<Texture2D> (GetWhiteTextureName (d.GetDominoFaces ().GetFace ('F')));
					break;
				}
				if (ct != null) {
					Sprite cs = Sprite.Create (ct, new Rect (0, 0, ct.width, ct.height), new Vector2 (0.5f, 0.5f));
					childrenRenderer [i].GetComponent<SpriteRenderer> ().sprite = cs;
				}
			}
		}
		if (d.GetDominoType() == DominoType.Simple && d.GetDominoColor() == DominoColor.Black) {
			Texture2D t = Resources.Load<Texture2D> ("Images/Dominos/Noir/Domino_Vierge_N");
			g.GetComponent<SpriteRenderer> ().sprite = Sprite.Create (t, new Rect (0, 0, t.width, t.height), new Vector2 (0.5f, 0.5f));
			SpriteRenderer[] childrenRenderer = g.GetComponentsInChildren<SpriteRenderer> ();
			for (int i = 0; i < childrenRenderer.Length; i++) {
				Texture2D ct = null;
				switch (i) {
				case 1:
					ct = Resources.Load<Texture2D> (GetBlackTextureName (d.GetDominoFaces ().GetFace ('A')));
					break;
				case 2:
					ct = Resources.Load<Texture2D> (GetBlackTextureName (d.GetDominoFaces ().GetFace ('B')));
					break;
				case 3:
					ct = Resources.Load<Texture2D> (GetBlackTextureName (d.GetDominoFaces ().GetFace ('C')));
					break;
				case 4:
					ct = Resources.Load<Texture2D> (GetBlackTextureName (d.GetDominoFaces ().GetFace ('D')));
					break;
				case 5:
					ct = Resources.Load<Texture2D> (GetBlackTextureName (d.GetDominoFaces ().GetFace ('E')));
					break;
				case 6:
					ct = Resources.Load<Texture2D> (GetBlackTextureName (d.GetDominoFaces ().GetFace ('F')));
					break;
				}
				if (ct != null) {
					Sprite cs = Sprite.Create (ct, new Rect (0, 0, ct.width, ct.height), new Vector2 (0.5f, 0.5f));
					childrenRenderer [i].GetComponent<SpriteRenderer> ().sprite = cs;
				}
			}
		}
	}

	private static string GetBlackTextureName(DominoValues v) {
		switch (v) {
		case DominoValues.One :
			return ("Images/Dominos/Numbers/1_Blanc");
		case DominoValues.Two :
			return ("Images/Dominos/Numbers/2_Blanc");
		case DominoValues.Three :
			return ("Images/Dominos/Numbers/3_Blanc");
		case DominoValues.Four :
			return ("Images/Dominos/Numbers/4_Blanc");
		case DominoValues.Five :
			return ("Images/Dominos/Numbers/5_Blanc");
		case DominoValues.Six :
			return ("Images/Dominos/Numbers/6_Blanc");
		default :
			Debug.Log ("Something went wrong on player.cs 122");
			return ("None");
		}
	}

	private static string GetWhiteTextureName(DominoValues v) {
		switch (v) {
		case DominoValues.One :
			return ("Images/Dominos/Numbers/1_Noir");
		case DominoValues.Two :
			return ("Images/Dominos/Numbers/2_Noir");
		case DominoValues.Three :
			return ("Images/Dominos/Numbers/3_Noir");
		case DominoValues.Four :
			return ("Images/Dominos/Numbers/4_Noir");
		case DominoValues.Five :
			return ("Images/Dominos/Numbers/5_Noir");
		case DominoValues.Six :
			return ("Images/Dominos/Numbers/6_Noir");
		default :
			Debug.Log ("Something went wrong on player.cs 142");
			return ("None");
		}
	}

	public static string GetWhiteBonusTextureName(DominoType _type) {
		switch (_type) {
		case DominoType.OneLess :
			return ("Images/Dominos/Blanc/Domino_-Un");
		case DominoType.TwoLess :
			return ("Images/Dominos/Blanc/Domino_-Deux");
		case DominoType.ThreeLess :
			return ("Images/Dominos/Blanc/Domino_-Trois");
		case DominoType.Trap :
			return ("Images/Dominos/Blanc/Domino_piege");
		case DominoType.Radar :
			return ("Images/Dominos/Blanc/Domino_radar");
		case DominoType.ChoosenOne :
			return ("Images/Dominos/Blanc/Domino_elu");
		case DominoType.Log :
			return ("Images/Dominos/Blanc/Domino_fumee");
		case DominoType.Color :
			return ("Images/Dominos/Blanc/Domino_Couleur");
		case DominoType.Swap :
			return ("Images/Dominos/Blanc/Domino_Swap");
		case DominoType.Graviton :
			return ("Images/Dominos/Blanc/Domino_Graviton");
		case DominoType.Megaton :
			return ("Images/Dominos/Blanc/Domino_Megaton");
		case DominoType.Glue :
			return ("Images/Dominos/Blanc/Domino_Glue");
		case DominoType.Nugget :
			return ("Images/Dominos/Blanc/Domino_Pepite");
		case DominoType.Blur :
			return ("Images/Dominos/Blanc/Domino_Trouble");
		case DominoType.Switch :
			return ("Images/Dominos/Blanc/Domino_Switch");
		case DominoType.Invert :
			return ("Images/Dominos/Blanc/Domino_Invert");
		case DominoType.OneMore :
			return ("Images/Dominos/Blanc/Domino_+Un");
		case DominoType.TwoMore :
			return ("Images/Dominos/Blanc/Domino_+Deux");
		case DominoType.ThreeMore :
			return ("Images/Dominos/Blanc/Domino_+Trois");
		case DominoType.Draw :
			return ("Images/Dominos/Blanc/Domino_pioche");
		case DominoType.Jump:
			return ("Images/Dominos/Blanc/Domino_Saut");
		default :
			return ("None");
		}
	}

	public static string GetBlackBonusTextureName(DominoType _type) {
		switch (_type) {
		case DominoType.OneLess :
			return ("Images/Dominos/Noir/Domino_-Un_N");
		case DominoType.TwoLess :
			return ("Images/Dominos/Noir/Domino_-Deux_N");
		case DominoType.ThreeLess :
			return ("Images/Dominos/Noir/Domino_-Trois_N");
		case DominoType.Trap :
			return ("Images/Dominos/Noir/Domino_piege_N");
		case DominoType.Radar :
			return ("Images/Dominos/Noir/Domino_radar_N");
		case DominoType.ChoosenOne :
			return ("Images/Dominos/Noir/Domino_elu_N");
		case DominoType.Log :
			return ("Images/Dominos/Noir/Domino_fumee_N");
		case DominoType.Color :
			return ("Images/Dominos/Noir/Domino_Couleur_N");
		case DominoType.Swap :
			return ("Images/Dominos/Noir/Domino_Swap_N");
		case DominoType.Graviton :
			return ("Images/Dominos/Noir/Domino_Graviton_N");
		case DominoType.Megaton :
			return ("Images/Dominos/Noir/Domino_Megaton_N");
		case DominoType.Glue :
			return ("Images/Dominos/Noir/Domino_Glue_N");
		case DominoType.Nugget :
			return ("Images/Dominos/Noir/Domino_Pepite_N");
		case DominoType.Blur :
			return ("Images/Dominos/Noir/Domino_Trouble_N");
		case DominoType.Switch :
			return ("Images/Dominos/Noir/Domino_Switch_N");
		case DominoType.Invert :
			return ("Images/Dominos/Noir/Domino_Invert_N");
		case DominoType.OneMore :
			return ("Images/Dominos/Noir/Domino_+Un_N");
		case DominoType.TwoMore :
			return ("Images/Dominos/Noir/Domino_+Deux_N");
		case DominoType.ThreeMore :
			return ("Images/Dominos/Noir/Domino_+Trois_N");
		case DominoType.Draw :
			return ("Images/Dominos/Noir/Domino_pioche_N");
		case DominoType.Jump:
			return ("Images/Dominos/Noir/Domino_Saut_N");
		default :
			return ("None");
		}
	}

	public static bool CompareDominoFaces(DominoFaces dom1, DominoFaces dom2) {
		return (dom1.GetFace ('A') == dom2.GetFace ('A') &&
		dom1.GetFace ('B') == dom2.GetFace ('B') &&
		dom1.GetFace ('C') == dom2.GetFace ('C') &&
		dom1.GetFace ('D') == dom2.GetFace ('D') &&
		dom1.GetFace ('E') == dom2.GetFace ('E') &&
		dom1.GetFace ('F') == dom2.GetFace ('F'));
	}
}
