using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace DoubleCheck.Controllers
{
    public class HomeController : Controller
    {
        //
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