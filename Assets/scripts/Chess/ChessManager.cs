using System;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour {
    public uint turn = 0;

    public Dictionary<PieceType, Vector2> PixelOffset = new Dictionary<PieceType, Vector2>();
    
    private void OnAwake() {
        PixelOffset.Add(PieceType.Pawn, new Vector2(.2f,.31f));
        PixelOffset.Add(PieceType.Knight, new Vector2(.2f, .31f));
        PixelOffset.Add(PieceType.Bishop, new Vector2(.2f, .31f));
        PixelOffset.Add(PieceType.Rook, new Vector2(.2f, .31f));
        PixelOffset.Add(PieceType.Queen, new Vector2(.2f, .31f));
        PixelOffset.Add(PieceType.King, new Vector2(.2f, .31f));
    }

}
[Serializable]
public enum PieceType {
    Pawn = 0,
    Knight = 1,
    Bishop = 2,
    Rook = 3,
    Queen = 4,
    King = 5
}
public enum PieceColor {
    White = 0,
    Black = 1
}
