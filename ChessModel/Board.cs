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

        public Man SetMan(int x, int y, Man man)
        {
            return cells[x, y].man = man;
        }

        public void MoveMan(int x1, int y1, int x2, int y2)
        {
            cells[x2, y2].man = cells[x1, y1].man;
            cells[x1, y1].man = null;
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

            IsCheck = false;
            IsRockA1AvailableForCastling = true;
            IsRockH1AvailableForCastling = true;
            IsRockA8AvailableForCastling = true;
            IsRockH8AvailableForCastling = true;
            LastTwoSquarepawnMoveX = -1;
            LastTwoSquarepawnMoveY = -1;
        }

        public void Move(int x1, int y1, int x2, int y2)
        {
            LastTwoSquarepawnMoveX = -1;
            LastTwoSquarepawnMoveY = -1;

            var man = cells[x1, y1].man;
            cells[x1, y1].man = null;
            if (man.ManType == ManType.Pawn)
            {
                if (y2 == 0 || y2 == 7)
                {
                    man = new Queen(man.Color);
                }
                else if (Math.Abs(y1 - y2) == 2)
                {
                    LastTwoSquarepawnMoveX = x2;
                    LastTwoSquarepawnMoveY = y2;
                }
                else if (x1 != x2 && cells[x2, y2].man == null)
                {
                    cells[x2, y1].man = null;
                }
            }
            else if (man.ManType == ManType.King)
            {
                // castling
                if (Math.Abs(x1 - x2) == 2)
                {
                    if (x2 < x1)
                    {
                        cells[3, y2].man = cells[0, y2].man;
                        cells[0, y2].man = null;
                    }
                    else
                    {
                        cells[5, y2].man = cells[7, y2].man;
                        cells[7, y2].man = null;
                    }
                }
                if (man.Color == ManColor.White)
                {
                    IsRockA1AvailableForCastling = false;
                    IsRockH1AvailableForCastling = false;
                }
                else if (man.Color == ManColor.Black)
                {
                    IsRockA8AvailableForCastling = false;
                    IsRockH8AvailableForCastling = false;
                }
            }
            if (man.ManType == ManType.Rock)
            {
                if (x1 == 0 && y1 == 0)
                    IsRockA1AvailableForCastling = false;
                else if (x1 == 7 && y1 == 0)
                    IsRockH1AvailableForCastling = false;
                if (x1 == 0 && y1 == 7)
                    IsRockA8AvailableForCastling = false;
                else if (x1 == 7 && y1 == 7)
                    IsRockH8AvailableForCastling = false;
            }
            
            cells[x2, y2].man = man;
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

        internal bool IsEmptyOrNotColor(ManColor color, int x, int y)
        {
            if (!CheckRange(x) || !CheckRange(y))
                return false;
            var man = Cell(x, y);
            return man == null || man.Color != color;
        }

        public static bool IsCheck { get; set; }

        public static bool IsRockA1AvailableForCastling { get; set; }

        public static bool IsRockH1AvailableForCastling { get; set; }

        public static bool IsRockA8AvailableForCastling { get; set; }

        public static bool IsRockH8AvailableForCastling { get; set; }

        internal static bool IsCheckAt(ManColor Color, int p1, int p2)
        {
            return false;
        }

        public static int LastTwoSquarepawnMoveX { get; set; }

        public static int LastTwoSquarepawnMoveY { get; set; }

        internal bool TestForCheck(ManColor color)
        {
            // find king
            int kingX, kingY;
            for (kingY = 0; kingY < 8; ++kingY)
            {
                for (kingX = 0; kingX < 8; ++kingX)
                {
                    var king = Cell(kingX, kingY);
                    if (king != null && king.ManType == ManType.King && king.Color == color)
                        goto kingExists;
                }
            }
            throw new InvalidOperationException("" + color + " King not found");

            kingExists:

            ManColor alienColor = color == ManColor.White ? ManColor.Black : ManColor.White;

            // knight and king attack
            for (int i=0; i<8; ++i)
            {
                if (IsManAt(alienColor, ManType.Knight, kingX + Knight.dx[i], kingY + Knight.dy[i]))
                    return true;
                if (IsManAt(alienColor, ManType.King, kingX + Man.allDx[i], kingY + Man.allDx[i]))
                    return true;
            }

            // pawn atack
            int alienPawnDir = alienColor == ManColor.White ? 1 : -1;
            if (IsManAt(alienColor, ManType.Pawn, kingX - 1, kingY - alienPawnDir) ||
                IsManAt(alienColor, ManType.Pawn, kingX + 1, kingY - alienPawnDir))
                return true;

            // bishop, rock and queen
            return TestForCheckHelper(kingX, kingY, alienColor, ManType.Bishop, Bishop.dx, Bishop.dy)
                || TestForCheckHelper(kingX, kingY, alienColor, ManType.Rock, Rock.dx, Rock.dy);
        }

        private bool TestForCheckHelper(int kingX, int kingY, ManColor alienColor, ManType manType, int[] dx, int[] dy)
        {
            if (dx.Length != dy.Length)
                throw new ArgumentException("dx.Length != dy.Length");
            for (int i=0; i<dx.Length; ++i)
            {
                int x = kingX;
                int y = kingY;
                for (int j=0; j<7; ++j)
                {
                    x += dx[i];
                    y += dy[i];
                    if (!CheckRange(x) || !CheckRange(y)) break;
                    var man = Cell(x, y);
                    if (man != null)
                    {
                        if (man.Color == alienColor && (man.ManType == manType || man.ManType == ManType.Queen))
                            return true;
                        break;
                    }                    
                }
            }
            return false;
        }

        private bool IsManAt(ManColor color, ManType manType, int x, int y)
        {
            if (!CheckRange(x) || !CheckRange(y))
                return false;
            var man = Cell(x, y);
            return man != null && man.ManType == manType && man.Color == color;
        }

        public static int GetX(ManColor color, int i)
        {
            return color == ManColor.White ? i : 7 - i;
        }

        public static int GetY(ManColor color, int i)
        {
            return color == ManColor.Black ? i : 7 - i;
        }
    }
}
