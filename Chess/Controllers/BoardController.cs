using Chess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chess.Controllers
{
    public class BoardController : Controller
    {
        //
        // GET: /Board/
        public ActionResult Index(int id)
        {
            var game = GamesManager.Instance.GetGame(id);
            if (game == null)
                return Content("Game not available");
            return View(game.Board);
        }
	}
}