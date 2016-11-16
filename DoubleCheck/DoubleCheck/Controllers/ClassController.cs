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
                var startItems = dateTimeStart.Split('-');
                var endItems = dateTimeEnd.Split('-');
                var startDate = new DateTime(Int32.Parse(startItems[0]), 
                    Int32.Parse(startItems[1]), Int32.Parse(startItems[2]));
                
                var endDate = new DateTime(Int32.Parse(endItems[0]),
                    Int32.Parse(endItems[1]), Int32.Parse(endItems[2]));

                var newClass = new Class { Building = collection["Building"],
                                           Name = collection["Name"],
                                           U_Id = Int32.Parse((string)Session["UserID"]),
                                           Room_Num = Int32.Parse(collection["Room_Num"]),
                                           Start_Date = startDate,
                                           End_Date = endDate};
                var lastInsertedClass = db.Classes.Add(newClass);
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
                            dbDay = "0010101";
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
                                    startTime = startAMPM[1] == "PM"
                                        ? (Int32.Parse(startAMPM[0].Split(':')[0]) + 12).ToString() + ":" + startAMPM[0].Split(':')[1]
                                        : startAMPM[0];
                                }
                                if (endAMPM.Count() > 1)
                                {
                                    endTime = endAMPM[1] == "PM"
                                        ? (Int32.Parse(endAMPM[0].Split(':')[0]) + 12).ToString() + ":" + endAMPM[0].Split(':')[1]
                                        : endAMPM[0];
                                }
                            }
                            startSpan = TimeSpan.Parse(startTime);
                            endSpan = TimeSpan.Parse(endTime);
                            var existingPeriod = db.Time_Periods.Where(t => t.Start_Time == startSpan
                            && t.End_Time == endSpan && t.Days == dbDay);
                            if (existingPeriod.Count() > 0)
                            {
                                lastInsertedClass.Time_Periods.Add(existingPeriod.First());
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
                                lastInsertedClass.Time_Periods.Add(lastInsertedTimePeriod);
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
                currentPeriod = day != ""
                    ? day + period.Start_Time.ToString(@"h\:mm") +
                    " - " + period.End_Time.ToString(@"h\:mm") + "\n"
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
