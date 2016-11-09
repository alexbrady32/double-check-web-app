using DoubleCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoubleCheck.Controllers
{
    public class ClassController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: Class
        public ActionResult Index()
        {
            User user = db.Users.Find(Int32.Parse((string)Session["UserID"]));
            return View(user.Classes);
        }

        // GET: Class/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Class/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Class/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // Assemble the start date and end dates
                var dateTimeStart = collection["Start_Date"];
                var dateTimeEnd = collection["End_Date"];
                var startItems = dateTimeStart.Split('/');
                var endItems = dateTimeEnd.Split('/');
                var startDate = new DateTime(Int32.Parse(startItems[2]), 
                    Int32.Parse(startItems[0]), Int32.Parse(startItems[1]));
                var endDate = new DateTime(Int32.Parse(endItems[2]),
                    Int32.Parse(endItems[0]), Int32.Parse(endItems[1]));

                var newClass = new Class { Building = collection["Building"],
                                           Name = collection["Name"],
                                           U_Id = Int32.Parse((string)Session["UserID"]),
                                           Room_Num = Int32.Parse(collection["Room_Num"]),
                                           Start_Date = startDate,
                                           End_Date = endDate};
                db.Classes.Add(newClass);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Class/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Class/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Class/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Class/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
