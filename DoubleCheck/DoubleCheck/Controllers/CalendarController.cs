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
            if (Session["UserID"] != null)
            {
                int userID = Int32.Parse((string)Session["UserID"]);
                var assignments = db.Assignments1.Where(u => u.U_Id == userID);
                return View();
            }

            // Really need to return them to the login screen or something...
            return View();
           
        }
    }
}