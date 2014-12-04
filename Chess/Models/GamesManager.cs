using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
    public class GamesManager
    {
        static GamesManager instance = new GamesManager();
        public static GamesManager Instance { get { return Instance; } }

        Dictionary<int, ChessModel.Game> games = new Dictionary<int,ChessModel.Game>();
        public 
    }
}