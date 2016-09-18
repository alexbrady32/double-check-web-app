using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoubleCheck.Models;

namespace DoubleCheck.Controllers
{
    public class HomeController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: Home
        public ActionResult index()
        {
            return View();
        }

    }
}
