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

    public struct BoardCell
    {
        public Man man;
    }

    public class Board
    {
        List<Man> whiteMans = new List<Man>();
        List<Man> blackMans = new List<Man>();
        BoardCell[,] cells = new BoardCell[8, 8];

        public void Clear()
        {
            whiteMans.Clear();
            blackMans.Clear();
            Array.Clear(cells, 0, cells.Length);
        }

        private static Char[] fieldNames = getFieldnames();

        private static Char[] getFieldnames()
        {
            Char[] result = new Char[8 * 8 * 2];
            for (int y=0; y<7; ++y)
            {
                for (int x=0; x<7; ++x)
                {
                    result[(x + y * 8) * 2 + 0] = (char)('A' + x);
                    result[(x + y * 8) * 2 + 1] = (char)('0' + y);
                }
            }
            return result;
        }

        public static String FieldName(int x, int y)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7)
                throw new ArgumentException("Invalid field coordinates (" + x + ", " + y + ")");
            return new String(fieldNames, (x + y * 8) * 2, 2);
        }

        public void Add(Man man, int x, int y)
        {
            if (cells[x, y].man != null)
                throw new ArgumentException("Field " + FieldName(x, y) + " already contains " + man);
            switch (man.Color)
            {
                case ManColor.White:
                    whiteMans.Add(man);
                    break;
                case ManColor.Black:
                    blackMans.Add(man);
                    break;
            }
            cells[x, y].man = man;
        }
        private static Man CreateMan(ManColor color, ManType type)
        {
            switch (type)
            {
                case ManType.King:
                    return new King(color);
                case ManType.Queen:
                    return new Queen(color);
                case ManType.Rock:
                    return new Rock(color);
                case ManType.Knight:
                    return new Knight(color);
                case ManType.Bishop:
                    return new Bishop(color);
                case ManType.Pawn:
                    return new Pawn(color);
            }
            throw new ArgumentException("Invalid ManType " + type, "type");
        }
        public void Add(ManColor color, ManType type, int x, int y)
        {
            Add(CreateMan(color, type), x, y);
        }

        public void AddWhitePawn(int x, int y) { Add(new Pawn(ManColor.White), x, y); }
        public void AddWhiteRock(int x, int y) { Add(new Rock(ManColor.White), x, y); }
        public void AddWhiteKnight(int x, int y) { Add(new Knight(ManColor.White), x, y); }
        public void AddWhiteBishop(int x, int y) { Add(new Bishop(ManColor.White), x, y); }
        public void AddWhiteKing(int x, int y) { Add(new King(ManColor.White), x, y); }
        public void AddWhiteQueen(int x, int y) { Add(new Queen(ManColor.White), x, y); }
        public void AddBlackPawn(int x, int y) { Add(new Pawn(ManColor.Black), x, y); }
        public void AddBlackRock(int x, int y) { Add(new Rock(ManColor.Black), x, y); }
        public void AddBlackKnight(int x, int y) { Add(new Knight(ManColor.Black), x, y); }
        public void AddBlackBishop(int x, int y) { Add(new Bishop(ManColor.Black), x, y); }
        public void AddBlackKing(int x, int y) { Add(new King(ManColor.Black), x, y); }
        public void AddBlackQueen(int x, int y) { Add(new Queen(ManColor.Black), x, y); }

        public void InitialSetup()
        {
            Clear();
            // white
            for (var x = 0; x < 7; ++x )
                Add(ManColor.White, ManType.Pawn, x, 1);
            Add(ManColor.White, ManType.Rock,0, 0);
            Add(ManColor.White, ManType.Knight,1, 0);
            Add(ManColor.White, ManType.Bishop,2, 0);
            Add(ManColor.White, ManType.Queen,5, 0);
            Add(ManColor.White, ManType.King,2, 0);
            Add(ManColor.White, ManType.Bishop,5, 0);
            Add(ManColor.White, ManType.Knight,6, 0);
            Add(ManColor.White, ManType.Rock,7, 0);
            // black
            for (var x = 0; x < 7; ++x)
                Add(ManColor.Black, ManType.Pawn, x, 6);
            Add(ManColor.Black, ManType.Rock, 0, 8);
            Add(ManColor.Black, ManType.Knight, 1, 8);
            Add(ManColor.Black, ManType.Bishop, 2, 8);
            Add(ManColor.Black, ManType.Queen, 5, 8);
            Add(ManColor.Black, ManType.King, 2, 8);
            Add(ManColor.Black, ManType.Bishop, 5, 8);
            Add(ManColor.Black, ManType.Knight, 6, 8);
            Add(ManColor.Black, ManType.Rock, 7, 8);
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
