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
            ViewResult indexView = View("/Views/Home/Index.cshtml");

            return indexView;
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            ViewResult assignmentsView = View("/Views/ViewAssignments.cshtml");
            var matchingUser = (from person in db.Users
                                where person.Username == username && person.Password == password
                                select person).FirstOrDefault();
            assignmentsView.ViewBag.UserId = matchingUser.Id;
            assignmentsView.ViewBag.UserFName = matchingUser.firstName;
            assignmentsView.ViewBag.UserLName = matchingUser.lastName;
            return assignmentsView;
        }

        public ActionResult CreateUser()
        {
            return View();
        }

    }
}
