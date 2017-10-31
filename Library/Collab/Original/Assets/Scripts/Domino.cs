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
	None
}

public enum DominoType {
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
	private int 			totalFaces;

	public Coordinate GetPosition() {
		return position;
	}

	public void SetPositionXY(Coordinate _position) {
		position = _position;	
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
		}
	}

	public void CreateDomino(DominoValues _faceA, DominoValues _faceB, DominoValues _faceC, DominoValues _faceD, DominoValues _faceE, DominoValues _faceF, DominoType _type) {
		dominoFaces = new DominoFaces (_faceA, _faceB, _faceC, _faceD, _faceE, _faceF);
		dominoType = _type;
		color = DominoColor.Grey;
		textureName = "Images/Dominos/Blanc/Domino_Numerote";
		rangeB = DominoValues.None;
		rangeN = DominoValues.None;
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

	public static DominoColor GetOppositeColor(DominoColor c) {
		if (c == DominoColor.Black) {
			return DominoColor.White;
		} else if (c == DominoColor.White) {
			return DominoColor.Black;
		}
		return DominoColor.Grey;
	}

	public static bool CompareDominoFaces(DominoFaces dom1, DominoFaces dom2) {
		if (dom1.GetFace ('A') == dom2.GetFace ('A') &&
		    dom1.GetFace ('B') == dom2.GetFace ('B') &&
		    dom1.GetFace ('C') == dom2.GetFace ('C') &&
		    dom1.GetFace ('D') == dom2.GetFace ('D') &&
		    dom1.GetFace ('E') == dom2.GetFace ('E') &&
		    dom1.GetFace ('F') == dom2.GetFace ('F'))
			return true;
		return false;
	}
}
