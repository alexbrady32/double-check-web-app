using DoubleCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoubleCheck.Utilities
{
    public class Utilities
    {
        // returns strings: number of assignments, total time to spend this week, total time to spend on assignments
        // due this week, total time to spend on assignments due in the coming weeks and finally total time to spend
        // on past due assignments
        public static List<String> calculateWeeklyTotal(int userID)
        {
            var db = new doublecheckdbEntities();
            List<string> times = new List<string>();
            var user = db.Users.Where(u => u.Id == userID).First();
            var incompleteAssignments = user.Assignments.Where(a => a.Completion != (decimal)1.0);
            var numberOfAssignments = incompleteAssignments.Count();
            int totalTTC = 0;
            int dueThisWeekTTC = 0;
            int dueNextWeeksTTC = 0;
            int pastDue = 0;
            foreach (var assignment in incompleteAssignments)
            {
                // for assignments TTC to get split, assignment due date must be not this week
                // and assignment's TTC must be more than 150 minutes
                int weeks = NumberOfWeeksToCount(assignment.Due_Date, DateTime.Today);
                if (weeks == 0)
                {
                    // past due
                    totalTTC += assignment.TTC;
                    pastDue += assignment.TTC;

                }
                else if (weeks > 1 && assignment.TTC > 150)
                {
                    // time per week rounded up to whole number of minutes, converted to integer
                    int timePerWeek = Convert.ToInt32(Math.Round(assignment.TTC / (float)weeks));
                    totalTTC += timePerWeek;
                    dueNextWeeksTTC += timePerWeek;
                }
                else
                {
                    totalTTC += assignment.TTC;
                    dueThisWeekTTC += assignment.TTC;
                }
            }
            times.Add(numberOfAssignments.ToString());
            var totalhours = (totalTTC / 60) > 0
                ? (totalTTC / 60) == 1 ? "1" : (totalTTC / 60).ToString() : "0";
            var totalMinutes = (totalTTC % 60) > 0 
                ? (totalTTC % 60) == 1 ? "01" : (totalTTC % 60).ToString() : "00";
            var total = totalhours + ":" + totalMinutes;
            times.Add(total);

            var thisWeekHours = (dueThisWeekTTC / 60) > 0
                ? (dueThisWeekTTC / 60) == 1 ? "1" : (dueThisWeekTTC / 60).ToString() : "0";
            var thisWeekMinutes = (dueThisWeekTTC % 60) > 0
                ? (dueThisWeekTTC % 60) == 1 ? "01" : (dueThisWeekTTC % 60).ToString() : "00";
            var thisWeek = thisWeekHours + ":" + thisWeekMinutes;
            times.Add(thisWeek);

            var nextWeeksHours = (dueNextWeeksTTC / 60) > 0
                ? (dueNextWeeksTTC / 60) == 1 ? "1" : (dueNextWeeksTTC / 60).ToString() : "0";
            var nextWeeksMinutes = (dueNextWeeksTTC % 60) > 0 
                ? (dueNextWeeksTTC % 60) == 1 ? "01" : (dueNextWeeksTTC % 60).ToString() : "00";
            var nextWeek = nextWeeksHours + ":" + nextWeeksMinutes;
            times.Add(nextWeek);

            var pastDueHours = (pastDue / 60) > 0
                ? (pastDue / 60) == 1 ? "1" : (pastDue / 60).ToString() : "0";
            var pastDueMinutes = (pastDue % 60) > 0 
                ? (pastDue % 60) == 1 ? "01" : (pastDue % 60).ToString() : "00";
            var pastDueString = pastDueHours + ":" + pastDueMinutes;
            times.Add(pastDueString);



            return times;
        }

        private static int NumberOfWeeksToCount(DateTime dueDate, DateTime today)
        {
            if (today <= dueDate)
            {
                // From http://stackoverflow.com/questions/25795254/check-if-a-datetime-is-in-same-week-as-other-datetime
                var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
                var d1 = dueDate.Date.AddDays(-1 * (int)cal.GetDayOfWeek(dueDate));
                var d2 = today.Date.AddDays(-1 * (int)cal.GetDayOfWeek(today));

                var weeks = 1;

                while (d2 != d1)
                {
                    d2 = d2.Date.AddDays(7);
                    weeks += 1;
                }

                return weeks;
            }
            return 0;
        }

    }
}