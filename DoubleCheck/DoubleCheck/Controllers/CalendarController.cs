using DoubleCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DoubleCheck.Controllers
{
    public class CalendarModel
    {
        public CalendarModel(string title)
        {
            this.title = title;
        }
        public string title { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public List<int> days { get; set; }
        //public string JsonData { get; set; }
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
                // TODO: Add in where clause for both assignments and classes
                //var assignments = db.Assignments1.Where(u => u.U_Id == userID);
                var assignments = db.Assignments;
                var classes = db.Classes;
                var timePeriods = db.Time_Periods;

                var calendarData = new List<CalendarModel>();

                foreach(Assignment a in assignments)
                {
                    CalendarModel data = new CalendarModel(a.Name);
                    data.startTime = a.Due_Date;
                    // Adds 30 minutes to the due date for calendar display purposes
                    // The calendar data needs an end time so it will display properly
                    data.endTime = a.Due_Date.AddMinutes(30);
                    //var serializer = new JavaScriptSerializer();
                    // TODO: Do we really need the json data field in the json data?
                    //data.JsonData = serializer.Serialize(data);
                    calendarData.Add(data);
                }

                
                foreach (Class c in classes)
                {
                    CalendarModel data = new CalendarModel(c.Name);
                    
                    var currentClassTimePeriods = c.Time_Periods; 
                    foreach (Time_Periods tp in currentClassTimePeriods)
                    {
                        data.startTime = data.startTime.AddHours(tp.Start_Time.Hours);
                        data.startTime = data.startTime.AddMinutes(tp.Start_Time.Minutes);
                        data.startTime = data.startTime.AddSeconds(tp.Start_Time.Seconds);
                    }
                    
                    /*
                    data.startTime = new DateTime(2016, 10, 23); // TODO: Get actual date data
                    data.startTime = data.startTime.AddHours(c.Start_Time.Hours);
                    data.startTime = data.startTime.AddMinutes(c.Start_Time.Minutes);
                    data.startTime = data.startTime.AddSeconds(c.Start_Time.Seconds);

                    data.endTime = new DateTime(2016, 10, 23); // TODO: Get actual date data
                    data.endTime = data.endTime.AddHours(c.End_Time.Hours);
                    data.endTime = data.endTime.AddMinutes(c.End_Time.Minutes);
                    data.endTime = data.endTime.AddSeconds(c.End_Time.Seconds);
                    */
                    calendarData.Add(data);
                }
                

                return View(calendarData);
            }
            // TODO: need to return a not logged in page or something here.
            return View();
        }
    }
}