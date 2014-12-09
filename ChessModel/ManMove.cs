using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessModel
{
    public class RegularMove
    {
        protected int x1;
        protected int y1;
        protected int x2;
        protected int y2;
        protected Man removedMan;

        public RegularMove(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        public virtual void Do(Board board)
        {
            removedMan = board.Cell(x2, y2);
            board.MoveMan(x1, y1, x2, y2);
        }

        public virtual void Undo(Board board)
        {
            board.MoveMan(x2, y2, x1, y1);
            board.SetMan(x2, y2, removedMan);
        }

        public override string ToString()
        {
            return Board.FieldName(x1, y1) + "-" + Board.FieldName(x2, y2);
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
            pawn = board.Cell(x2, y2);
            board.SetMan(x2, y2, promouted);
        }

        public override void Undo(Board board)
        {
            base.Undo(board);
            board.SetMan(x2, y2, pawn);
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
            board.MoveMan(x2, y1, x2, y2);
            base.Do(board);
        }

        public override void Undo(Board board)
        {
            base.Undo(board);
            board.MoveMan(x2, y2, x2, y1);
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
