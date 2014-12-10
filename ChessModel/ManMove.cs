using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessModel
{
    public class RegularMove
    {
        public int X1 { get; private set; }
        public int Y1 { get; private set; }
        public int X2 { get; private set; }
        public int Y2 { get; private set; }
        public Man MovedMan { get; private set; }
        public Man RemovedMan { get; private set; }
        public bool Check { get; set; }
        public bool Checkmate { get; set; }

        public RegularMove(int x1, int y1, int x2, int y2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }

        public virtual void Do(Board board)
        {
            MovedMan = board.Cell(X1, Y1);
            RemovedMan = board.Cell(X2, Y2);
            board.MoveMan(X1, Y1, X2, Y2);
        }

        public virtual void Undo(Board board)
        {
            board.MoveMan(X2, Y2, X1, Y1);
            board.SetMan(X2, Y2, RemovedMan);
        }

        public override string ToString()
        {
            return MovedMan.Notation + Board.FieldName(X1, Y1) + (RemovedMan == null ? "-" : ":") + Board.FieldName(X2, Y2) + CheckNotation();
        }

        protected string CheckNotation()
        {
            return (Checkmate ? "#" : Check ? "+" : "");
        }

        internal string Key()
        {
            return Board.FieldName(X1, Y1) + "-" + Board.FieldName(X2, Y2);
        }

        public virtual bool IsPawnLongMove { get {return false;}}
    }

    public class NoMove : RegularMove
    {
        public NoMove() : base(0, 0, 0, 0) { }
        public override void Do(Board board) {}
        public override void Undo(Board board) {}
    }

    class PawnPromoution : RegularMove
    {
        private Man promouted;
        private Man pawn;

        public PawnPromoution(int x1, int y1, int x2, int y2, Man promouted)
            : base(x1, y1, x2, y2)
        {
            this.promouted = promouted;
        }

        public override void Do(Board board)
        {
            base.Do(board);
            pawn = board.Cell(X2, Y2);
            board.SetMan(X2, Y2, promouted);
        }

        public override void Undo(Board board)
        {
            board.SetMan(X2, Y2, pawn);
            base.Undo(board);
        }

        public override string ToString()
        {
            return base.ToString() + promouted.Notation;
        }
    }

    class PawnSpeciasTake : RegularMove
    {
        public PawnSpeciasTake(int x1, int y1, int x2, int y2)
            : base(x1, y1, x2, y2)
        {
        }

        public override void Do(Board board)
        {
            board.MoveMan(X2, Y1, X2, Y2);
            base.Do(board);
        }

        public override void Undo(Board board)
        {
            base.Undo(board);
            board.MoveMan(X2, Y2, X2, Y1);
        }
    }

    class PawnLongMove : RegularMove
    {
        public PawnLongMove(int x1, int y1, int y2)
            : base(x1, y1, x1, y2)
        {
        }

        public override bool IsPawnLongMove { get { return true; } }
    }

    class TowerMove : RegularMove
    {
        private bool tower = false;

        public TowerMove(int x1, int y1, int x2, int y2)
            : base(x1, y1, x2, y2)
        {
        }

        public override void Do(Board board)
        {
            base.Do(board);
            if (X1 == 0)
            {
                if (Y1 == 0)
                {
                    tower = board.IsTowerA1AvailableForCastling;
                    board.IsTowerA1AvailableForCastling = false;
                }
                else if (Y1 == 7)
                {
                    tower = board.IsTowerA8AvailableForCastling;
                    board.IsTowerA8AvailableForCastling = false;
                }
            }
            else if (X1 == 7)
            {
                if (Y1 == 0)
                {
                    tower = board.IsTowerH1AvailableForCastling;
                    board.IsTowerH1AvailableForCastling = false;
                }
                else if (Y1 == 7)
                {
                    tower = board.IsTowerH8AvailableForCastling;
                    board.IsTowerH8AvailableForCastling = false;
                }
            }
        }

        public override void Undo(Board board)
        {
            if (X1 == 0)
            {
                if (Y1 == 0)
                {
                    board.IsTowerA1AvailableForCastling = tower;
                }
                else if (Y1 == 7)
                {
                    board.IsTowerA8AvailableForCastling = tower;
                }
            }
            else if (X1 == 7)
            {
                if (Y1 == 0)
                {
                    board.IsTowerH1AvailableForCastling = tower;
                }
                else if (Y1 == 7)
                {
                    board.IsTowerH8AvailableForCastling = tower;
                }
            }
            base.Undo(board);
        }
    }

    class KingMove : RegularMove
    {
        private bool towerA = false;
        private bool towerH = false;

        public KingMove(int x1, int y1, int x2, int y2)
            : base(x1, y1, x2, y2)
        {
        }

        public override void Do(Board board)
        {
            base.Do(board);
            Man king = board.Cell(X2, Y2);
            if (king.Color == ManColor.White)
            {
                towerA = board.IsTowerA1AvailableForCastling;
                towerH = board.IsTowerH1AvailableForCastling;
                board.IsTowerA1AvailableForCastling = false;
                board.IsTowerH1AvailableForCastling = false;
            }
            else
            {
                towerA = board.IsTowerA8AvailableForCastling;
                towerH = board.IsTowerH8AvailableForCastling;
                board.IsTowerA8AvailableForCastling = false;
                board.IsTowerH8AvailableForCastling = false;
            }
        }

        public override void Undo(Board board)
        {
            Man king = board.Cell(X2, Y2);
            if (king.Color == ManColor.White)
            {
                board.IsTowerA1AvailableForCastling = towerA;
                board.IsTowerH1AvailableForCastling = towerH;
            }
            else
            {
                board.IsTowerA8AvailableForCastling = towerA;
                board.IsTowerH8AvailableForCastling = towerH;
            }
            base.Undo(board);
        }
    }

    class Castling : KingMove
    {
        private RegularMove towerMove;

        public Castling(int x, int y)
            : base(4, y, x == 0 ? 2 : 6, y)
        {
            towerMove = new RegularMove(x, y, x == 0 ? 3 : 5, y);
        }

        public override void Do(Board board)
        {
            base.Do(board);
            towerMove.Do(board);
        }

        public override void Undo(Board board)
        {
            towerMove.Undo(board);
            base.Undo(board);
        }

        public override string ToString()
        {
            return X2 == 2 ? "0-0-0" : "0-0" + CheckNotation();
        }
    }

}
