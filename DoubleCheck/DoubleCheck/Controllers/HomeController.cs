﻿using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using DoubleCheck.Models;

namespace DoubleCheck.Controllers
{
    using System.Linq;

    public class HomeController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: /Home/
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                var user = db.Users.Where(u => u.Id == (int) Session["UserID"]).FirstOrDefault();
                // check if user has taken cloze exam; if not, check if we've already asked them to take it this session
                if (user.Cloze_Score == null && Session["ClozeCheckComplete"] != null && (bool)Session["ClozeCheckComplete"] != true)
                {
                    return RedirectToAction("Index", "Cloze");
                }
                // else:
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}
