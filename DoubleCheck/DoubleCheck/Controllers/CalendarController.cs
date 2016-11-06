﻿using DoubleCheck.Models;
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
        public int id { get; set; }
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
                CalendarModel data;

                foreach (Assignment a in assignments)
                {
                    data = new CalendarModel(a.Name);
                    data.id = a.A_Id;
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
                    

                    var currentClassTimePeriods = c.Time_Periods; 
                    foreach (Time_Periods tp in currentClassTimePeriods)
                    {
                        
                        DateTime curDate = c.Start_Date;
                        // Loop through the days array and when there is a class on that day
                        // as indicated by a '1', then go ahead and create the event repeating event in the
                        // date range.
                        for (int i = 0; i < tp.Days.Count(); i++)
                        {
                            curDate = c.Start_Date;
                            if (tp.Days[i] == '1')
                            {
                                
                                // Find the first date in the date range that is on the given day of the week
                                // Casting the DayOfWeek to and int returns a value between 0 and 6
                                while ((int) curDate.DayOfWeek != i)
                                {
                                    curDate = curDate.AddDays(1);
                                }
                                while (curDate < c.End_Date)
                                {

                                    data = new CalendarModel(c.Name);
                                    // Enter the start and end time for the class for this time period
                                    //data.startTime.AddTicks(tp.Start_Time.Ticks);
                                    //data.endTime.AddTicks(tp.End_Time.Ticks);
                                    data.startTime = curDate;
                                    data.startTime = data.startTime.AddHours(tp.Start_Time.Hours);
                                    data.startTime = data.startTime.AddMinutes(tp.Start_Time.Minutes);
                                    data.startTime = data.startTime.AddSeconds(tp.Start_Time.Seconds);

                                    data.endTime = curDate;
                                    data.endTime = data.endTime.AddHours(tp.End_Time.Hours);
                                    data.endTime = data.endTime.AddMinutes(tp.End_Time.Minutes);
                                    data.endTime = data.endTime.AddSeconds(tp.End_Time.Seconds);

                                    calendarData.Add(data);

                                    curDate = curDate.AddDays(7);
                                }
                            }
                        }
                        //data.startTime = data.startTime.AddHours(tp.Start_Time.Hours);
                        //data.startTime = data.startTime.AddMinutes(tp.Start_Time.Minutes);
                        //data.startTime = data.startTime.AddSeconds(tp.Start_Time.Seconds);
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
                    
                }
                

                return View(calendarData);
            }
            // TODO: need to return a not logged in page or something here.
            return View();
           
        }
    }
}