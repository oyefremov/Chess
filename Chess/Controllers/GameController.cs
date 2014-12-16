using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chess.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/
        public ActionResult Index(int id)
        {
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game with id " + id + " not available");
            return View(game);
        }

        public ActionResult Show(int id, int move)
        {
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game with id " + id + " not available");
            if (game.MovesCount <= move)
            {
                return HttpNotFound();
            }
            return PartialView("game", game);
        }

        public ActionResult GameList()
        {
            return PartialView("games", GamesManager.Instance.GetGames());
        }

        private ActionResult NewGameImpl(bool darkChessRule = false, bool solo = false, bool checkRule = true, bool checkmateRule = true)
        {
            if (!User.Identity.IsAuthenticated)
                return Content("User must be authenticated");
            var game = GamesManager.Instance.CreateGame(darkChessRule, checkRule, checkmateRule);
            game.WhitePlayer = User.Identity.Name;
            if (solo)
                game.BlackPlayer = game.WhitePlayer;
            return RedirectToAction("", new { id = game.Id });
        }

        public ActionResult NewSoloGame()
        {
            return NewGameImpl(solo: true);
        }

        public ActionResult NewGame()
        {
            return NewGameImpl();
        }

        public ActionResult NewDarkChessGame()
        {
            return NewGameImpl(darkChessRule: true, checkRule: false, checkmateRule: false);
        }

        public ActionResult NewDarkChessCheckGame()
        {
            return NewGameImpl(darkChessRule: true, checkRule: true, checkmateRule: false);
        }

        public ActionResult NewDarkChessCheckmateGame()
        {
            return NewGameImpl(darkChessRule: true, checkRule: true, checkmateRule: true);
        }

        public ActionResult Join(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return Content("User must be authenticated");
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game with id " + id + " not available");
            lock (game)
            {
                if (game.BlackPlayer != null)
                    return Content("Game already filled");
                if (User.Identity.Name != game.WhitePlayer && game.BlackPlayer == null)
                    game.BlackPlayer = User.Identity.Name;
            }
            return RedirectToAction("", new { id = game.Id });            
        }
        
        public ActionResult SetTimeControl(int id, string param)
        {
            if (!User.Identity.IsAuthenticated)
                return Content("User must be authenticated");
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game with id " + id + " not available");
            lock (game)
            {
                var res = param.Split(',');
                int baseTime, moveTime;
                if (res.Length != 2 || !int.TryParse(res[0], out baseTime) || !int.TryParse(res[1], out moveTime))
                {
                    return Content("Invalid param=" + param);
                }
                if (User.Identity.Name != game.WhitePlayer && User.Identity.Name != game.BlackPlayer)
                {
                    return Content("Can not change time control for game " + id);
                }
                if (baseTime == 0 && moveTime == 0)
                {
                    game.TimeControl = false;
                }
                else
                {
                    game.TimeControl = true;
                    game.MoveTime = new TimeSpan(0, 0, moveTime);
                    game.BaseTime = new TimeSpan(0, 0, baseTime);
                    game.WhitePlayerTime = game.BaseTime;
                    game.BlackPlayerTime = game.BaseTime;
                }
            }
            return RedirectToAction("", new { id = game.Id });
        }
        public ActionResult MakeMove(int id, string move)
        {
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game with id " + id + " not available");

            if (game.CurrentSide == ChessModel.ManColor.White)
            {
                if (User.Identity.Name != game.WhitePlayer)
                {
                    return Content("Only " + game.WhitePlayer + " can play for white");
                }
            }
            else
            {
                if (game.BlackPlayer == null)
                {
                    if (User.Identity.Name != game.WhitePlayer)
                    {
                        return Content("Please, join the game before playing it.");
                    }
                    game.BlackPlayer = User.Identity.Name;
                }
                else if (User.Identity.Name != game.BlackPlayer)
                {
                    return Content("Only " + game.BlackPlayer + " can play for black");
                }
            }

            try {
                game.MakeMove(move);
            }
            catch (Exception e)
            {
                return Content("Invalid move " + move + " : " + e.Message);
            }
            return PartialView("game", game);
//            return RedirectToAction("", new { id = game.Id });
        }
    }
}