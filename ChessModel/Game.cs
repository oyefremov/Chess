using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessModel
{
    public class Game
    {
        public Game(bool darkChess = false)
        {
            DarkChess = darkChess;
            Board.InitialSetup();
            CurrentTurnSide = ManColor.White;
            CalculateTurns2();
        }

        private Board board = new Board();
        public Board Board { get { return board; } }

        public int Id { get; set; }

        private List<Tuple<RegularMove, RegularMove>> moves = new List<Tuple<RegularMove, RegularMove>>();
        public List<Tuple<RegularMove, RegularMove>> Moves { get { return moves; } }
        public int MovesCount { get { return moves.Count == 0 ? 0 : moves.Count * 2 - (moves[moves.Count - 1].Item2 == null ? 1 : 0); } }

        private IDictionary<String, RegularMove> availableMoves = new Dictionary<String, RegularMove>();
        public IDictionary<String, RegularMove> AvailableMoves { get { return availableMoves; } }
        void CalculateTurns2()
        {
            availableMoves.Clear();
            Board.IsCheck = Board.TestForCheck(CurrentTurnSide);
            Board.SetVisibility(!DarkChess);
            for (int y = 0; y < 8; ++y)
                for (int x = 0; x < 8; ++x)
                {
                    var man = Board.Cell(x, y);
                    if (man == null)
                        continue;
                    if (DarkChess)
                    {
                        Board.RemoveShadow(man.Color, x, y);
                    }
                    if (man.Color != CurrentTurnSide)
                    {
                        man.MoveToFields = "";
                        foreach (var turn in man.Turns2(Board, x, y))
                        {
                            Board.RemoveShadow(man.Color, turn.X2, turn.Y2);
                        }
                    }
                    else
                    {
                        StringBuilder moveToFields = new StringBuilder();
                        foreach (var turn in man.Turns2(Board, x, y))
                        {
                            Board.RemoveShadow(man.Color, turn.X2, turn.Y2);
                            turn.Do(Board);
                            if (!Board.TestForCheck(CurrentTurnSide))
                            {
                                moveToFields.Append(Board.FieldName(turn.X2, turn.Y2));
                                availableMoves.Add(turn.Key(), turn);
                            }
                            turn.Undo(Board);
                        }
                        man.MoveToFields = moveToFields.ToString();
                    }
                }
        }

        public void MakeMove(string move)
        {
            var regularMove = AvailableMoves[move];
            if (regularMove == null)
                throw new ArgumentException("Invalid move " + move);

            regularMove.Do(Board);
            Board.LastMove = regularMove;
            ChangeSide();
            CalculateTurns2();

            regularMove.Check = Board.IsCheck && AvailableMoves.Count != 0;
            regularMove.Checkmate = Board.IsCheck && AvailableMoves.Count == 0;

            if (CurrentTurnSide == ManColor.Black)
            {
                moves.Add(Tuple.Create(regularMove, (RegularMove)null));
            }
            else
            {
                var last = moves.Count - 1;
                moves[last] = Tuple.Create(moves[last].Item1, regularMove);
            }
        }

        private void ChangeSide()
        {
            CurrentTurnSide = CurrentTurnSide == ManColor.White ? ManColor.Black : ManColor.White;
        }

        private void ParseMove(string move, out int x1, out int y1, out int x2, out int y2)
        {
            if (move.Length != 5 || move[2] != '-')
                throw new ArgumentException("invalid move format, use move like E2-E4");
            x1 = ParseX(move[0]);
            y1 = ParseY(move[1]);
            x2 = ParseX(move[3]);
            y2 = ParseY(move[4]);
        }

        private int ParseX(char p)
        {
            if (p < 'A' || p > 'H')
                throw new ArgumentException("invalid letter " + p + ". Valid characters A..H.");
            return p - 'A';
        }

        private int ParseY(char p)
        {
            if (p < '1' || p > '8')
                throw new ArgumentException("invalid number " + p + ". Valid range 1..8.");
            return p - '1';
        }

        public ManColor CurrentTurnSide { get; set; }

        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }
        public bool DarkChess { get; set; }

        public string Name
        {
            get
            {
                if (BlackPlayer == null)
                {
                    if (WhitePlayer == null)
                    {
                        return "Chess game";
                    }
                    else
                    {
                        return WhitePlayer + "'s game";
                    }
                }
                else
                {
                    if (WhitePlayer == null)
                    {
                        return BlackPlayer + "'s game";
                    }
                    else
                    {
                        return WhitePlayer + " vs " + BlackPlayer;
                    }
                }
            }
        }

        public void Save()
        {
        }
    }
}
