using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessModel
{

    public struct BoardCell
    {
        public Man man;
    }

    public struct BoardCellPos
    {
        public BoardCellPos(int x, int y) { this.x = x; this.y = y; }
        public int x, y;
    }

    public class Board
    {
        List<Man> whiteMans = new List<Man>();
        List<Man> blackMans = new List<Man>();
        BoardCell[,] cells = new BoardCell[8, 8];

        public Man Cell(int x, int y)
        {
            return cells[x, y].man;
        }

        public void Clear()
        {
            whiteMans.Clear();
            blackMans.Clear();
            Array.Clear(cells, 0, cells.Length);
        }

        public static bool CheckRange(int x) { return x >= 0 && x < 8; }

        private static Char[] fieldNames = getFieldnames();

        private static Char[] getFieldnames()
        {
            Char[] result = new Char[8 * 8 * 2];
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    result[(x + y * 8) * 2 + 0] = (char)('A' + x);
                    result[(x + y * 8) * 2 + 1] = (char)('1' + y);
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

        public ManColor FieldColorCode(int x, int y)
        {
            if (((x + y) & 1) == 0)
                return ManColor.Black;
            else
                return ManColor.White;
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

        public void InitialSetup()
        {
            Clear();
            // white
            for (var x = 0; x < 8; ++x)
                Add(ManColor.White, ManType.Pawn, x, 1);
            Add(ManColor.White, ManType.Rock, 0, 0);
            Add(ManColor.White, ManType.Knight, 1, 0);
            Add(ManColor.White, ManType.Bishop, 2, 0);
            Add(ManColor.White, ManType.Queen, 3, 0);
            Add(ManColor.White, ManType.King, 4, 0);
            Add(ManColor.White, ManType.Bishop, 5, 0);
            Add(ManColor.White, ManType.Knight, 6, 0);
            Add(ManColor.White, ManType.Rock, 7, 0);
            // black
            for (var x = 0; x < 8; ++x)
                Add(ManColor.Black, ManType.Pawn, x, 6);
            Add(ManColor.Black, ManType.Rock, 0, 7);
            Add(ManColor.Black, ManType.Knight, 1, 7);
            Add(ManColor.Black, ManType.Bishop, 2, 7);
            Add(ManColor.Black, ManType.Queen, 3, 7);
            Add(ManColor.Black, ManType.King, 4, 7);
            Add(ManColor.Black, ManType.Bishop, 5, 7);
            Add(ManColor.Black, ManType.Knight, 6, 7);
            Add(ManColor.Black, ManType.Rock, 7, 7);
        }

        public void Move(int x1, int x2, int y1, int y2)
        {
            cells[x2, y2].man = cells[x1, y1].man;
            cells[x1, y1].man = null;
        }

        internal bool IsEmpty(int x, int y)
        {
            return CheckRange(x) && CheckRange(y) && Cell(x, y) == null;
        }

        internal bool IsColor(ManColor c, int x, int y)
        {
            if (!CheckRange(x) || !CheckRange(y))
                return false;
            var man = Cell(x, y);
            return man != null && man.Color == c;
        }

        internal bool IsColorNot(ManColor c, int x, int y)
        {
            if (!CheckRange(x) || !CheckRange(y))
                return false;
            var man = Cell(x, y);
            return man != null && man.Color != c;
        }
    }
}
