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
                return Content("Game with id " + id +" not available");
            return View(game);
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

            try {
                game.MakeMove(move);
            }
            catch (Exception e)
            {
                return Content("Invalid move " + move + " : " + e.Message);
            }
            return RedirectToAction("", new { id = game.Id });
        }
    }
}