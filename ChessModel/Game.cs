using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessModel
{
    class Game
    {
        public Game()
        {
            Board.InitialSetup();
        }

        private Board board = new Board();
        public Board Board { get { return board; } }
    }
}
