using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessModel
{
    enum ManColor
    {
        Black, White
    }

    public class Man
    {
        public ManColor Color {get; set;}
        public int X { get; set; }
        public int Y { get; set; }

        public virtual string Name { get {return "Unknown";} }
    }

    public struct BoardCell
    {
        Man man;
    }

    public class Board
    {
        List<Man> whiteMans = new List<Man>();
        List<Man> blackMans = new List<Man>();
        BoardCell[,] cells = new BoardCell[8, 8];
    }

    public class Pawn
    {
        public virtual string Name { get { return "Pawn"; } }
    }

    public class Rock
    {
        public virtual string Name { get { return "Rock"; } }
    }

    public class Knight
    {
        public virtual string Name { get { return "Knight"; } }
    }

    public class Bishop
    {
        public virtual string Name { get { return "Bishop"; } }
    }

    public class King
    {
        public virtual string Name { get { return "King"; } }
    }

    public class Queen
    {
        public virtual string Name { get { return "Queen"; } }
    }
}
