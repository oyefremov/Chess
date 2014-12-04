using System;
using System.Collections.Generic;

namespace ChessModel
{
    enum ManColor
    {
        Black, White
    }
    enum ManType
    {
        King, Queen, Rock, Knight, Bishop, Pawn
    }

    public class Man
    {
        public ManColor Color {get; set;}
        public int X { get; set; }
        public int Y { get; set; }

        public virtual string Name { get { return "Unknown"; } }
        public virtual ManType ManType { get; }
        public String ToString()
        {
            return "" + Color + " " + Name;
        }
    }

    public class Pawn : Man
    {
        public Pawn(ManColor color) { Color = color; }
        public virtual string Name { get { return "Pawn"; } }
        public virtual ManType ManType { get { return ManType.Pawn; } }
    }

    public class Rock : Man
    {
        public Rock(ManColor color) { Color = color; }
        public virtual string Name { get { return "Rock"; } }
        public virtual ManType ManType { get { return ManType.Rock; } }
    }

    public class Knight : Man
    {
        public Knight(ManColor color) { Color = color; }
        public virtual string Name { get { return "Knight"; } }
        public virtual ManType ManType { get { return ManType.Knight; } }
    }

    public class Bishop : Man
    {
        public Bishop(ManColor color) { Color = color; }
        public virtual string Name { get { return "Bishop"; } }
        public virtual ManType ManType { get { return ManType.Bishop; } }
    }

    public class King : Man
    {
        public King(ManColor color) { Color = color; }
        public virtual string Name { get { return "King"; } }
        public virtual ManType ManType { get { return ManType.King; } }
    }

    public class Queen : Man
    {
        public Queen(ManColor color) { Color = color; }
        public virtual string Name { get { return "Queen"; } }
        public virtual ManType ManType { get { return ManType.XXXXXX; } }
    }
}
