﻿using DoubleCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoubleCheck.Controllers
{
    public class AssignmentsController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: Assignments
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View(db.Assignments1.ToList());
        }
    }
}
