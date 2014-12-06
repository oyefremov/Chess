using System;
using System.Collections.Generic;

namespace ChessModel
{
    public enum ManColor
    {
        Black, White
    }
    public enum ManType
    {
        King, Queen, Rock, Knight, Bishop, Pawn
    }

    public abstract class Man
    {
        public ManColor Color {get; set;}
        public int X { get; set; }
        public int Y { get; set; }

        public abstract string Name { get; }
        public abstract string WhiteCharCode { get; }
        public abstract string BlackCharCode { get; }
        public abstract ManType ManType { get; }
        override public String ToString()
        {
            return "" + Color + " " + Name;
        }
    }

    public class Pawn : Man
    {
        public Pawn(ManColor color) { Color = color; }
        public override string Name { get { return "Pawn"; } }
        public override string WhiteCharCode { get { return "\u2659"; } } // U+2659 White Chess Pawn (HTML &#9817;)
        public override string BlackCharCode { get { return "\u265F"; } } // U+265F Black Chess Pawn (HTML &#9823;)
        public override ManType ManType { get { return ManType.Pawn; } }
    }

    public class Rock : Man
    {
        public Rock(ManColor color) { Color = color; }
        public override string Name { get { return "Rock"; } }
        public override string WhiteCharCode { get { return "\u2656"; } } // U+2656 White Chess Rook (HTML &#9814;)
        public override string BlackCharCode { get { return "\u265C"; } } // U+265C Black Chess Rook (HTML &#9820;)
        public override ManType ManType { get { return ManType.Rock; } }
    }

    public class Knight : Man
    {
        public Knight(ManColor color) { Color = color; }
        public override string Name { get { return "Knight"; } }
        public override string WhiteCharCode { get { return "\u2658"; } } // U+2658 White Chess Knight (HTML &#9816;)
        public override string BlackCharCode { get { return "\u265E"; } } // U+265E Black Chess Knight (HTML &#9822;)
        public override ManType ManType { get { return ManType.Knight; } }
    }

    public class Bishop : Man
    {
        public Bishop(ManColor color) { Color = color; }
        public override string Name { get { return "Bishop"; } }
        public override string WhiteCharCode { get { return "\u2657"; } } // U+2657 White Chess Bishop (HTML &#9815;)
        public override string BlackCharCode { get { return "\u265D"; } } // U+265D Black Chess Bishop (HTML &#9821;)
        public override ManType ManType { get { return ManType.Bishop; } }
    }

    public class King : Man
    {
        public King(ManColor color) { Color = color; }
        public override string Name { get { return "King"; } }
        public override string WhiteCharCode { get { return "\u2654"; } } // U+2654 White Chess King (HTML &#9812;)
        public override string BlackCharCode { get { return "\u265A"; } } // U+265A Black Chess King (HTML &#9818;)
        public override ManType ManType { get { return ManType.King; } }
    }

    public class Queen : Man
    {
        public Queen(ManColor color) { Color = color; }
        public override string Name { get { return "Queen"; } }
        public override string WhiteCharCode { get { return "\u2655"; } } // U+2655 White Chess Queen (HTML &#9813;)
        public override string BlackCharCode { get { return "\u265B"; } } // U+265B Black Chess Queen (HTML &#9819;)
        public override ManType ManType { get { return ManType.Queen; } }
    }
}
