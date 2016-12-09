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
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Session["ClozeCheckComplete"] = true;
            return View();
        }

        // GET: /Cloze/Account
        public ActionResult Assess()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        
        public ActionResult StopAsking()
        {
            int userID = Int32.Parse((string)Session["UserID"]);
            var user = db.Users.Where(u => u.Id == userID).FirstOrDefault();
            // Assigning the user a score that gives them an average reading level
            user.Cloze_Score = 85;
            db.SaveChanges();
            return RedirectToAction("Index", "Assignments");
        }

        public ActionResult Grade()
        {
            int userID = Int32.Parse((string)Session["UserID"]);
            List<String> answers = new List<string>();
            List<string> correctAnswers = new List<string> { "D", "E", "D", "E", "D", "C", "D", "A", "C", "C"};
            for (int i = 0; i < 10; i++)
            {
                if (i < 5)
                    answers.Add(Request.Params["paragraph-" + (i + 1).ToString()+"-answers"]);
                if (i >= 5)
                    answers.Add(Request.Params["paragraph1-" + (i - 4).ToString() + "-answers"]);
            }
            decimal score = 0;
            for (int i = 0; i < correctAnswers.Count; i++) {
                if (answers[i] == correctAnswers[i])
                {
                    score++;
                }
            }
            decimal grade = score / 10;
            var user = db.Users.Where(u => u.Id == userID).FirstOrDefault();
            user.Cloze_Score = grade * 100;
            db.SaveChanges();
            return RedirectToAction("Index", "Assignments");
        }
    }
}