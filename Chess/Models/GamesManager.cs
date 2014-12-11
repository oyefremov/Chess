using ChessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chess.Models
{
    public class GamesManager
    {
        static GamesManager instance = new GamesManager();
        public static GamesManager Instance { get { return instance; } }

        IDictionary<int, Game> games = new Dictionary<int,Game>();

        public Game CreateGame()
        {
            lock (games)
            {
                var id = games.Count + 1;
                Game game = new Game();
                game.Id = id;
                games.Add(id, game);
                return game;
            }
        }

        public Game GetGame(int id)
        {
            Game result;
            games.TryGetValue(id, out result);
            return result;
        }

        public bool IsValidGameId(int id)
        {
            return games.ContainsKey(id);
        }

        public IEnumerable<KeyValuePair<int, Game>> GetGames()
        {
            return games;
        }
    }
}