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
        public override ManType ManType { get { return ManType.Pawn; } }
    }

    public class Rock : Man
    {
        public Rock(ManColor color) { Color = color; }
        public override string Name { get { return "Rock"; } }
        public override ManType ManType { get { return ManType.Rock; } }
    }

    public class Knight : Man
    {
        public Knight(ManColor color) { Color = color; }
        public override string Name { get { return "Knight"; } }
        public override ManType ManType { get { return ManType.Knight; } }
    }

    public class Bishop : Man
    {
        public Bishop(ManColor color) { Color = color; }
        public override string Name { get { return "Bishop"; } }
        public override ManType ManType { get { return ManType.Bishop; } }
    }

    public class King : Man
    {
        public King(ManColor color) { Color = color; }
        public override string Name { get { return "King"; } }
        public override ManType ManType { get { return ManType.King; } }
    }

    public class Queen : Man
    {
        public Queen(ManColor color) { Color = color; }
        public override string Name { get { return "Queen"; } }
        public override ManType ManType { get { return ManType.Queen; } }
    }
}
