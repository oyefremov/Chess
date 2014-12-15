using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessModel
{
    public class Game
    {
        public static Game CreateRegularGame() { return new Game(false, true, true); }
        public static Game CreateDarkGame() { return new Game(true, false, false); }
        public static Game CreateDarkCheckGame() { return new Game(true, true, false); }
        public static Game CreateDarkCheckmateGame() { return new Game(true, true, true); }

        public Game(bool darkChess, bool check, bool checkmate)
        {
            UseTimer = false;
            DarkChess = darkChess;
            Board.CheckRule = check;
            Board.CheckmateRule = checkmate;
            WhiteWinner = false;
            BlackWinner = false;
            Board.InitialSetup();
            CurrentSide = ManColor.White;
            CalculateMoves();
        }

        private Board board = new Board();
        public Board Board { get { return board; } }

        public int Id { get; set; }

        private List<Tuple<RegularMove, RegularMove>> moves = new List<Tuple<RegularMove, RegularMove>>();
        public List<Tuple<RegularMove, RegularMove>> Moves { get { return moves; } }
        public int MovesCount { get { return moves.Count == 0 ? 0 : moves.Count * 2 - (moves[moves.Count - 1].Item2 == null ? 1 : 0); } }

        private IDictionary<String, RegularMove> availableMoves = new Dictionary<String, RegularMove>();
        public IDictionary<String, RegularMove> AvailableMoves { get { return availableMoves; } }
        void CalculateMoves()
        {
            availableMoves.Clear();

            if (WhiteWinner || BlackWinner)
            {
                Board.IsCheck = true;
                Board.SetVisibility(true);
                return;
            }

            Board.IsCheck = Board.TestForCheck(CurrentSide);
            Board.SetVisibility(!DarkChess);
            List<String> whiteMans = new List<String>();
            List<String> blackMans = new List<String>();
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
                    if (man.Color == ManColor.White)
                    {
                        whiteMans.Add(man.WhiteCharCode);
                    }
                    else
                    {
                        blackMans.Add(man.BlackCharCode);
                    }
                    foreach (var move in man.ScanMoves(Board, x, y))
                    {
                        Board.RemoveShadow(man.Color, move.X2, move.Y2);
                    }
                    if (man.Color != CurrentSide)
                    {
                        man.MoveToFields = "";
                    }
                    else
                    {
                        StringBuilder moveToFields = new StringBuilder();
                        foreach (var move in man.Moves(Board, x, y))
                        {
                            if (Board.CheckmateRule)
                            {
                                move.Do(Board);
                                if (!Board.TestForCheck(CurrentSide))
                                {
                                    moveToFields.Append(Board.FieldName(move.X2, move.Y2));
                                    availableMoves.Add(move.Key(), move);
                                }
                                move.Undo(Board);
                            }
                            else
                            {
                                moveToFields.Append(Board.FieldName(move.X2, move.Y2));
                                availableMoves.Add(move.Key(), move);
                            }
                        }
                        man.MoveToFields = moveToFields.ToString();
                    }
                }
            whiteMans.Sort();
            blackMans.Sort();
            StringBuilder list = new StringBuilder();
            foreach (var man in whiteMans)
                list.Append(man);
            Board.WhiteMans = list.ToString();

            list.Clear();
            foreach (var man in blackMans)
                list.Append(man);
            Board.BlackMans = list.ToString();
        }

        public void MakeMove(string move)
        {
            var regularMove = AvailableMoves[move];
            if (regularMove == null)
                throw new ArgumentException("Invalid move " + move);

            if (DarkChess)
            {
                Man man = Board.Cell(regularMove.X2, regularMove.Y2);
                if (man != null && man.ManType == ManType.King)
                {
                    WhiteWinner = man.Color == ManColor.Black;
                    BlackWinner = man.Color == ManColor.White;
                }
            }

            regularMove.Do(Board);
            Board.LastMove = regularMove;
            ChangeSide();
            CalculateMoves();

            regularMove.Check = Board.IsCheck && AvailableMoves.Count != 0;
            regularMove.Checkmate = Board.IsCheck && AvailableMoves.Count == 0;

            if (CurrentSide == ManColor.Black)
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
            CurrentSide = CurrentSide == ManColor.White ? ManColor.Black : ManColor.White;
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

        public ManColor CurrentSide { get; set; }

        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }
        public bool DarkChess { get; set; }

        public string Name
        {
            get
            {
                string name = DarkChess ? "Dark" : "Classic";
                if (DarkChess && Board.CheckmateRule)
                    name += " (checkmate)";
                else if (DarkChess && Board.CheckRule)
                    name += " (check)";
                if (BlackPlayer == null)
                {
                    if (WhitePlayer == null)
                    {
                        return name;
                    }
                    else
                    {
                        return WhitePlayer + "'s " + name + "game";
                    }
                }
                else
                {
                    if (WhitePlayer == null)
                    {
                        return BlackPlayer + "'s " + name + "game";
                    }
                    else
                    {
                        return WhitePlayer + " vs " + BlackPlayer + " " + name;
                    }
                }
            }
        }

        public void Save()
        {
        }

        bool WhiteWinner { get; set; }
        bool BlackWinner { get; set; }

        public bool UseTimer { get; set; }
        public int BaseTimer { get; set; }
        public int TurnTimer { get; set; }
        public int WhitePlayerTimer { get; set; }
        public int BlackPlayerTimer { get; set; }
    }
}
