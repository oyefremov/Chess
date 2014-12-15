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
        protected Man()
        {
            Moved = false;
        }

        public ManColor Color {get; set;}
        public int X { get; set; }
        public int Y { get; set; }
        public string MoveToFields { get; set; }

        public abstract string Name { get; }
        public abstract string Notation { get; }
        public abstract string WhiteCharCode { get; }
        public abstract string BlackCharCode { get; }
        public bool Moved { get; set; }
        public abstract ManType ManType { get; }
        override public String ToString()
        {
            return "" + Color + " " + Name;
        }

        public abstract IEnumerable<RegularMove> Moves(Board board, int x, int y);
        public virtual IEnumerable<RegularMove> ScanMoves(Board board, int x, int y) { return Moves(board, x, y); }

        protected IEnumerable<RegularMove> TurnsHelper2(Board Board, int manX, int manY, int[] dirX, int[] dirY) 
        {
            if (dirX.Length != dirY.Length)
                throw new ArgumentException("dirX.Length != dirY.Length");
            for (int i = 0; i < dirX.Length; ++i)
            {
                int dx = dirX[i];
                int dy = dirY[i];
                int x = manX;
                int y = manY;
                for (int j = 0; j < 7; ++j)
                {
                    x += dx;
                    y += dy;
                    if (Board.IsEmpty(x, y))
                    {
                        yield return new RegularMove(manX, manY, x, y);
                        continue;
                    }
                    if (Board.IsColorNot(Color, x, y))
                    {
                        yield return new RegularMove(manX, manY, x, y);
                    }
                    break;
                }
            }
        }
        public static int[] allDx = new int[] { 1, -1, 0, 0, 1, -1, -1, 1 };
        public static int[] allDy = new int[] { 0, 0, 1, -1, 1, 1, -1, -1 };
    }

    public class Pawn : Man
    {
        public Pawn(ManColor color) { Color = color; }
        public override string Name { get { return "Pawn"; } }
        public override string Notation { get { return ""; } }
        public override string WhiteCharCode { get { return "\u2659"; } } // U+2659 White Chess Pawn (HTML &#9817;)
        public override string BlackCharCode { get { return "\u265F"; } } // U+265F Black Chess Pawn (HTML &#9823;)
        public override ManType ManType { get { return ManType.Pawn; } }

        public override IEnumerable<RegularMove> Moves(Board Board, int x, int y)
        {
            int dy = Color == ManColor.White ? 1 : -1;
            var promotion = y + dy == 0 || y + dy == 7;
            if (Board.IsEmpty(x, y + dy))
            {
                if (promotion)
                    yield return new PawnPromoution(x, y, x, y + dy, new Queen(Color));
                else 
                    yield return new RegularMove(x, y, x, y + dy);
                if (y == 1 && Color == ManColor.White && Board.IsEmpty(x, 3))
                    yield return new PawnLongMove(x, 1, 3);
                else if (y == 6 && Color == ManColor.Black && Board.IsEmpty(x, 4))
                    yield return new PawnLongMove(x, 6, 4);
            }
            if (Board.IsColorNot(Color, x - 1, y + dy))
            {
                if (promotion)
                    yield return new PawnPromoution(x, y, x - 1, y + dy, new Queen(Color));
                else
                    yield return new RegularMove(x, y, x - 1, y + dy);
            }
            if (Board.IsColorNot(Color, x + 1, y + dy))
            {
                if (promotion)
                    yield return new PawnPromoution(x, y, x + 1, y + dy, new Queen(Color));
                else 
                    yield return new RegularMove(x, y, x + 1, y + dy);
            }

            // take on two square pawn move
            var lastMove = Board.LastMove;
            if (lastMove.IsPawnLongMove && lastMove.Y2 == y && Math.Abs(lastMove.X2 - x) == 1)
            {
                yield return new PawnSpeciasTake(x, y, lastMove.X2, y + dy);
            }
        }

        public override IEnumerable<RegularMove> ScanMoves(Board board, int x, int y) 
        {
            int dy = Color == ManColor.White ? 1 : -1;
            if (board.IsEmpty(x, y + dy))
            {
                yield return new RegularMove(x, y, x, y + dy);
                if (y == 1 && Color == ManColor.White && board.IsEmpty(x, 3))
                    yield return new RegularMove(x, 1, x, 3);
                else if (y == 6 && Color == ManColor.Black && board.IsEmpty(x, 4))
                    yield return new RegularMove(x, 6, x, 4);
            }
            if (Board.CheckRange(x - 1))
            {
                yield return new RegularMove(x, y, x - 1, y + dy);
            }
            if (Board.CheckRange(x + 1))
            {
                yield return new RegularMove(x, y, x + 1, y + dy);
            }

            // take on two square pawn move
            var lastMove = board.LastMove;
            if (lastMove.IsPawnLongMove && lastMove.Y2 == y && Math.Abs(lastMove.X2 - x) == 1)
            {
                yield return new RegularMove(x, y, lastMove.X2, y);
            }
        }
    }

    public class Rock : Man
    {
        public static int[] dx = new int[] { 1, -1, 0, 0 };
        public static int[] dy = new int[] { 0, 0, 1, -1 };
        public Rock(ManColor color) { Color = color; }
        public override string Name { get { return "Rock"; } }
        public override string Notation { get { return "R"; } }
        public override string WhiteCharCode { get { return "\u2656"; } } // U+2656 White Chess Rook (HTML &#9814;)
        public override string BlackCharCode { get { return "\u265C"; } } // U+265C Black Chess Rook (HTML &#9820;)
        public override ManType ManType { get { return ManType.Rock; } }
        public override IEnumerable<RegularMove> Moves(Board board, int x, int y) { return TurnsHelper2(board, x, y, dx, dy); }
    }

    public class Knight : Man
    {
        public static int[] dx = new int[] {1, 1, -1, -1, 2, 2, -2, -2 };
        public static int[] dy = new int[] {2, -2, 2, -2, 1, -1, 1, -1 };
        public Knight(ManColor color) { Color = color; }
        public override string Name { get { return "Knight"; } }
        public override string Notation { get { return "N"; } }
        public override string WhiteCharCode { get { return "\u2658"; } } // U+2658 White Chess Knight (HTML &#9816;)
        public override string BlackCharCode { get { return "\u265E"; } } // U+265E Black Chess Knight (HTML &#9822;)
        public override ManType ManType { get { return ManType.Knight; } }
        public override IEnumerable<RegularMove> Moves(Board Board, int x, int y)
        {
            for (int i = 0; i < 8; ++i)
            {
                if (Board.IsEmptyOrNotColor(Color, x + dx[i], y + dy[i]))
                    yield return new RegularMove(x, y, x + dx[i], y + dy[i]);
            }
        }
    }

    public class Bishop : Man
    {
        public static int[] dx = new int[] { 1, -1, -1, 1 };
        public static int[] dy = new int[] { 1, 1, -1, -1 };
        public Bishop(ManColor color) { Color = color; }
        public override string Name { get { return "Bishop"; } }
        public override string Notation { get { return "B"; } }
        public override string WhiteCharCode { get { return "\u2657"; } } // U+2657 White Chess Bishop (HTML &#9815;)
        public override string BlackCharCode { get { return "\u265D"; } } // U+265D Black Chess Bishop (HTML &#9821;)
        public override ManType ManType { get { return ManType.Bishop; } }
        public override IEnumerable<RegularMove> Moves(Board board, int x, int y) { return TurnsHelper2(board, x, y, dx, dy); }
    }

    public class King : Man
    {
        public King(ManColor color) { Color = color; }
        public override string Name { get { return "King"; } }
        public override string Notation { get { return "K"; } }
        public override string WhiteCharCode { get { return "\u2654"; } } // U+2654 White Chess King (HTML &#9812;)
        public override string BlackCharCode { get { return "\u265A"; } } // U+265A Black Chess King (HTML &#9818;)
        public override ManType ManType { get { return ManType.King; } }
        public override IEnumerable<RegularMove> Moves(Board Board, int manX, int manY)
        {
            if (allDx.Length != allDy.Length)
                throw new ArgumentException("allDx.Length != allDy.Length");
            for (int i = 0; i < allDx.Length; ++i)
            {
                int x = manX + allDx[i];
                int y = manY + allDy[i];
                if (Board.IsEmptyOrNotColor(Color, x, y))
                {
                    yield return new RegularMove(manX, manY, x, y);
                }
            }
            // Castling
            if (!Moved && !Board.IsCheck)
            {
                var rockA = Board.Cell(0, Y);
                if (rockA != null && !rockA.Moved &&
                    Board.IsEmpty(1, Y) &&
                    Board.IsEmpty(2, Y) &&
                    Board.IsEmpty(3, Y) &&
                    (!Board.CheckmateRule || !Board.IsCheckAt(Color, 3, Y)))
                {
                    yield return new Castling(0, Y);
                }
                var rockH = Board.Cell(7, Y);
                if (rockH != null && !rockH.Moved &&
                    Board.IsEmpty(5, Y) &&
                    Board.IsEmpty(6, Y) &&
                    (!Board.CheckmateRule || !Board.IsCheckAt(Color, 5, Y)))
                {
                    yield return new Castling(7, Y);
                }
            }
        }
    }

    public class Queen : Man
    {
        public Queen(ManColor color) { Color = color; }
        public override string Name { get { return "Queen"; } }
        public override string Notation { get { return "Q"; } }
        public override string WhiteCharCode { get { return "\u2655"; } } // U+2655 White Chess Queen (HTML &#9813;)
        public override string BlackCharCode { get { return "\u265B"; } } // U+265B Black Chess Queen (HTML &#9819;)
        public override ManType ManType { get { return ManType.Queen; } }
        public override IEnumerable<RegularMove> Moves(Board board, int x, int y) { return TurnsHelper2(board, x, y, allDx, allDy); }
    }
}
