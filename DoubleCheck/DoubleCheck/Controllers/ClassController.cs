using DoubleCheck.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace DoubleCheck.Controllers
{
    public class ClassController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: Class
        public ActionResult Index()
        {
            User user;

            if (Session["UserID"] != null)
            {
                user = db.Users.Find(Int32.Parse((string)Session["UserID"]));
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user.Classes);
        }

        // GET: Class/Create
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

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
                var startDate = DateTime.Parse(dateTimeStart);
                var endDate = DateTime.Parse(dateTimeEnd);
                var newClass = new Class { Building = collection["Building"],
                                           Name = collection["Name"],
                                           U_Id = Int32.Parse((string)Session["UserID"]),
                                           Room_Num = Int32.Parse(collection["Room_Num"]),
                                           Start_Date = startDate,
                                           End_Date = endDate};
                var lastInsertedClass = db.Classes.Add(newClass);
                TimePeriodParsing(lastInsertedClass, collection);
                db.SaveChanges();
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
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var editClass = db.Classes.Where(c => c.C_Id == id).FirstOrDefault();
            return View(editClass);
        }

        // POST: Class/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var editClass = db.Classes.Where(c => c.C_Id == id).FirstOrDefault();
                db.Entry(editClass).State = EntityState.Modified;
                editClass.Name = collection["Name"];
                editClass.Building = collection["Building"];
                editClass.Room_Num = Int32.Parse(collection["Room_Num"]);
                editClass.Start_Date = DateTime.Parse(collection["Start_Date"]);
                editClass.End_Date = DateTime.Parse(collection["End_Date"]);
                TimePeriodRemoval(editClass);
                TimePeriodParsing(editClass, collection);
                db.SaveChanges();
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
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentClass = db.Classes.Where(c => c.C_Id == id).First();
            string timePeriods = "";
            var periods = currentClass.Time_Periods;
            foreach (var period in periods) {
                string currentPeriod = "";
                string day;
                switch (period.Days)
                {
                    case "0101010":
                        day = "MWF: ";
                        break;
                    case "0010100":
                        day = "TTH: ";
                        break;
                    case "0101000":
                        day = "NW";
                        break;
                    case "1000000":
                        day = "Sun: ";
                        break;
                    case "0100000":
                        day = "Mon: ";
                        break;
                    case "0010000":
                        day = "Tues: ";
                        break;
                    case "0001000":
                        day = "Wed: ";
                        break;
                    case "0000100":
                        day = "Thurs: ";
                        break;
                    case "0000010":
                        day = "Fri: ";
                        break;
                    case "0000001":
                        day = "Sat: ";
                        break;
                    default:
                        day = "";
                        break;
                }
                var startTime = period.Start_Time.Hours > 12
                    ? (period.Start_Time.Hours - 12).ToString() + ":" + period.Start_Time.Minutes.ToString("00") + " PM"
                    : period.Start_Time.ToString(@"h\:mm") + " AM";
                var endTime = period.End_Time.Hours > 12
                    ? (period.End_Time.Hours - 12).ToString() + ":" + period.End_Time.Minutes.ToString("00") + " PM"
                    : period.End_Time.ToString(@"h\:mm") + " AM";
                currentPeriod = day != ""
                    ? day + startTime +
                    " - " + endTime + ";" + Environment.NewLine
                    : "";
                timePeriods += currentPeriod;
            }
            ViewBag.TimePeriods = timePeriods;
            return View(currentClass);
        }

        // POST: Class/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var classToDelete = db.Classes.Where(c => c.C_Id == id).FirstOrDefault();
            try
            {
                TimePeriodRemoval(classToDelete);
                db.Classes.Remove(classToDelete);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Delete", id);
            }
        }

        public void TimePeriodRemoval(Class modifyClass)
        {
            foreach (var period in modifyClass.Time_Periods.ToList())
            {
                modifyClass.Time_Periods.Remove(period);
            }
        }

        public void TimePeriodParsing(Class modifyClass, FormCollection collection)
        {
            int i = 0;
            bool finished = false;
            while (!finished)
            {
                var day = i != 0 ? collection["days" + i.ToString()] : collection["days"];
                String dbDay;
                switch (day)
                {
                    case "MWF":
                        dbDay = "0101010";
                        break;
                    case "TTH":
                        dbDay = "0010100";
                        break;
                    case "MW":
                        dbDay = "0101000";
                        break;
                    case "M":
                        dbDay = "0100000";
                        break;
                    case "T":
                        dbDay = "0010000";
                        break;
                    case "W":
                        dbDay = "0001000";
                        break;
                    case "TH":
                        dbDay = "0000100";
                        break;
                    case "F":
                        dbDay = "0000010";
                        break;
                    case "Sat":
                        dbDay = "0000001";
                        break;
                    case "Sun":
                        dbDay = "1000000";
                        break;
                    default:
                    case null:
                        dbDay = "";
                        break;
                }
                var startTime = i != 0 ? collection["start_time" + i.ToString()] : collection["start_time"];
                var endTime = i != 0 ? collection["end_time" + i.ToString()] : collection["end_time"];
                if (day != null && startTime != null && endTime != null)
                {
                    if (dbDay != "" && startTime != "" && endTime != "")
                    {
                        TimeSpan startSpan;
                        TimeSpan endSpan;
                        try
                        {
                            startSpan = TimeSpan.Parse(startTime);
                            endSpan = TimeSpan.Parse(endTime);
                        }
                        catch (FormatException e)
                        {
                            var startAMPM = startTime.Split(' ');
                            var endAMPM = endTime.Split(' ');
                            if (startAMPM.Count() > 1)
                            {
                                startTime = startAMPM[1] == "PM" && Int32.Parse(startAMPM[0].Split(':')[0]) != 12
                                    ? (Int32.Parse(startAMPM[0].Split(':')[0]) + 12).ToString() + ":" + startAMPM[0].Split(':')[1]
                                    : Int32.Parse(startAMPM[0].Split(':')[0]) == 12 && startAMPM[1] == "AM"
                                    ? "00:" + startAMPM[0].Split(':')[1] : startAMPM[0];
                            }
                            if (endAMPM.Count() > 1)
                            {
                                endTime = endAMPM[1] == "PM" && Int32.Parse(endAMPM[0].Split(':')[0]) != 12
                                    ? (Int32.Parse(endAMPM[0].Split(':')[0]) + 12).ToString() + ":" + endAMPM[0].Split(':')[1]
                                    : Int32.Parse(endAMPM[0].Split(':')[0]) == 12 && endAMPM[1] == "AM"
                                    ? "00:" + endAMPM[0].Split(':')[1] : endAMPM[0];
                            }
                        }
                        startSpan = TimeSpan.Parse(startTime);
                        endSpan = TimeSpan.Parse(endTime);
                        var existingPeriod = db.Time_Periods.Where(t => t.Start_Time == startSpan
                        && t.End_Time == endSpan && t.Days == dbDay);
                        if (existingPeriod.Count() > 0)
                        {
                            modifyClass.Time_Periods.Add(existingPeriod.First());
                        }
                        else
                        {
                            var timePeriod = new Time_Periods
                            {
                                Start_Time = startSpan,
                                End_Time = endSpan,
                                Days = dbDay
                            };
                            var lastInsertedTimePeriod = db.Time_Periods.Add(timePeriod);
                            modifyClass.Time_Periods.Add(lastInsertedTimePeriod);
                        }
                    }
                    i++;
                }
                else
                {
                    finished = true;
                }
            }
            db.SaveChanges();
        }
    }
}
