using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour {
    public PieceType Type;
    public PieceColor Color;
    public uint MovesMade = 0;

    public void Move() {
        switch (Type) {
            case PieceType.Pawn:
                break;
            case PieceType.Knight:
                break;
            case PieceType.Bishop:
                break;
            case PieceType.Rook:
                break;
            case PieceType.Queen:
                break;
            case PieceType.King:
                break;
        }
    }
}
