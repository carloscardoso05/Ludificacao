using System.Collections.Generic;
using UnityEngine;
public class Tile : MonoBehaviour {
    public bool isStar;
    public GameColor entranceTo;
    public GameColor color;
    public List<Piece> players = new();
}