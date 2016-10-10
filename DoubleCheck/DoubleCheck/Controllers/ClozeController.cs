using DoubleCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoubleCheck.Controllers
{
    public class ClozeController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: Cloze
        public ActionResult Index()
        {
            Session["ClozeCheckComplete"] = true;
            return View();
        }

        public ActionResult Assess()
        {
            return View();
        }

        public ActionResult StopAsking()
        {
            var user = db.Users.Where(u => u.Id == (int)Session["UserID"]).FirstOrDefault();
            // Assigning the user a score that gives them an average reading level
            user.Cloze_Score = 85;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}