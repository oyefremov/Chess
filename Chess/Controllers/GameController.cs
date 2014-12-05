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
//            return PartialView("board", id);
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game with id " + id +" not available");
            return View(game);
        }

        public ActionResult NewGame()
        {
            var game = GamesManager.Instance.CreateGame();
            return RedirectToAction("", new { id = game.Id });
//            return View(game);
        }        
	}
}