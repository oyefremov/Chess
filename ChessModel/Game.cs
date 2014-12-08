using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessModel
{
    public class Game
    {
        public Game()
        {
            Board.InitialSetup();
            CalculateTurns(ManColor.White);
        }

        private Board board = new Board();
        public Board Board { get { return board; } }

        public int Id { get; set; }

        private List<String> availableMoves = new List<String>();
        public IEnumerable<String> AvailableMoves { get { return availableMoves; } }
        void CalculateTurns(ManColor side)
        {
            for (int y=0; y<8; ++y)
            for (int x=0; x<8; ++x)
            {
                var man = Board.Cell(x, y);
                if (man == null || man.Color != side)
                    continue;
                StringBuilder moveToFields = new StringBuilder();
                foreach(var turn in man.Turns(Board, x, y))
                {
                    moveToFields.Append(Board.FieldName(turn.x, turn.y));
                }
                man.MoveToFields = moveToFields.ToString();
            }
        }
    }
}
