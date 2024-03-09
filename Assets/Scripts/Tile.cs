using System.Collections.Generic;
using UnityEngine;
public class Tile : MonoBehaviour {
    public bool isSafe;
    public bool isFinal = false;
    public GameColor entranceTo;
    public GameColor color;
    public List<Piece> players = new();
}