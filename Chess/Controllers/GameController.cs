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

        public ActionResult Show(int id, int move = -1, string sender="")
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

        public ActionResult NewGame()
        {
            if (!User.Identity.IsAuthenticated)
                return Content("User must be authenticated");
            var game = GamesManager.Instance.CreateGame();
            game.WhitePlayer = User.Identity.Name;
            return RedirectToAction("", new { id = game.Id });
        }

        public ActionResult Join(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return Content("User must be authenticated");
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game with id " + id + " not available");
            if (User.Identity.Name != game.WhitePlayer)
                game.BlackPlayer = User.Identity.Name;
            return RedirectToAction("", new { id = game.Id });
        }

        public ActionResult MakeMove(int id, string move)
        {
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game with id " + id + " not available");

            if (game.CurrentTurnSide == ChessModel.ManColor.White)
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