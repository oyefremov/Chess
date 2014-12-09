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
        protected Man removedMan;

        public RegularMove(int x1, int y1, int x2, int y2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }

        public virtual void Do(Board board)
        {
            removedMan = board.Cell(X2, Y2);
            board.MoveMan(X1, Y1, X2, Y2);
        }

        public virtual void Undo(Board board)
        {
            board.MoveMan(X2, Y2, X1, Y1);
            board.SetMan(X2, Y2, removedMan);
        }

        public override string ToString()
        {
            return Board.FieldName(X1, Y1) + "-" + Board.FieldName(X2, Y2);
        }
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
            base.Undo(board);
            board.SetMan(X2, Y2, pawn);
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

    class Castling : RegularMove
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
    }

}
