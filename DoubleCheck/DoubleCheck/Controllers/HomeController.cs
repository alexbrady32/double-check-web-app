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
    public class HomeController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: /Home/
        public ActionResult Index()
        {
            ViewResult indexView = View("/Views/Home/Index.cshtml");

            return indexView;
        }

        public string Login()
        {
            return "Hello, " + Request["usr"];
        }

        public ActionResult Create()
        {
            return View();
        }

    }
}
