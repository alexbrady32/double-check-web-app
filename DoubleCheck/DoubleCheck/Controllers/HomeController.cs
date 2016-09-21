using System;
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
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}
