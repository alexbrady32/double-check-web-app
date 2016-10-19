using DoubleCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoubleCheck.Controllers
{
    class CalendarModel
    {

    }

    public class CalendarController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: Calendar
        public ActionResult Calendar()
        {
            int userID = Int32.Parse((string)Session["UserID"]);
            var assignments = db.Assignments.Where(u => u.U_Id == userID);
            return View();
        }
    }
}