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
        public bool FirstMove { get; set; }

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
            FirstMove = !MovedMan.Moved;
            MovedMan.Moved = true;
            RemovedMan = board.Cell(X2, Y2);
            board.MoveMan(X1, Y1, X2, Y2);
        }

        public virtual void Undo(Board board)
        {
            MovedMan.Moved = !FirstMove;
            board.MoveMan(X2, Y2, X1, Y1);
            board.SetMan(X2, Y2, RemovedMan);
        }

        public override string ToString()
        {
            return BaseNotation() + CheckNotation();
        }

        protected virtual string BaseNotation()
        {
            return MovedMan.Notation + Board.FieldName(X1, Y1) + (RemovedMan == null ? "-" : ":") + Board.FieldName(X2, Y2);
        }

        protected string CheckNotation()
        {
            return (Checkmate ? "#" : Check ? "+" : "");
        }

        internal string Key()
        {
            return Board.FieldName(X1, Y1) + "-" + Board.FieldName(X2, Y2);
        }

        public virtual bool IsPawnLongMove { get { return false; } }

        public virtual string Fields 
        { 
            get 
            {
                return Board.FieldName(X1, Y1) + Board.FieldName(X2, Y2); 
            } 
        }
    }

    public class NoMove : RegularMove
    {
        public NoMove() : base(0, 0, 0, 0) { }
        public override void Do(Board board) {}
        public override void Undo(Board board) {}
        public override string Fields { get { return ""; } }
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

        protected override string BaseNotation()
        {
            return base.BaseNotation() + promouted.Notation;
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

        public override string Fields { get { return base.Fields + Board.FieldName(X2, Y2); } }
    }

    class PawnLongMove : RegularMove
    {
        public PawnLongMove(int x1, int y1, int y2)
            : base(x1, y1, x1, y2)
        {
        }

        public override bool IsPawnLongMove { get { return true; } }
    }

    class Castling : RegularMove
    {
        private RegularMove rockMove;

        public Castling(int x, int y)
            : base(4, y, x == 0 ? 2 : 6, y)
        {
            rockMove = new RegularMove(x, y, x == 0 ? 3 : 5, y);
        }

        public override void Do(Board board)
        {
            base.Do(board);
            rockMove.Do(board);
        }

        public override void Undo(Board board)
        {
            rockMove.Undo(board);
            base.Undo(board);
        }

        protected override string BaseNotation()
        {
            return X2 == 2 ? "0-0-0" : "0-0";
        }

        public override string Fields { get { return base.Fields + rockMove.Fields; } }
    }

}
