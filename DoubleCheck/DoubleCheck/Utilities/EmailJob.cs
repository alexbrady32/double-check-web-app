using DoubleCheck.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace DoubleCheck.Utilities
{
    public class EmailJob : IJob
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();
        public void Execute(IJobExecutionContext context)
        {
            var users = db.Users.Where(u => u.canNotifyByEmail == true);
            foreach (var user in users) {
                var numberOfAssignments = user.Assignments.Count();
                var totalTTC = 0;
                foreach (var assignment in user.Assignments)
                {
                    totalTTC += assignment.TTC;
                }
                if (numberOfAssignments > 0 && totalTTC > 0)
                {
                    var hours = (totalTTC / 60) > 0 ? (totalTTC / 60).ToString() + " hours and " : "";
                    var minutes = totalTTC % 60;
                    // add link to home page in future
                    string emailBody = "Hello, " + user.firstName.ToString() + "!\n\n"
                        + "Don't get behind in your schoolwork! You have " + numberOfAssignments.ToString() +
                        " assignments due this week. You have estimated that this will take you approximately " +
                        hours + minutes.ToString() + " minutes to complete these assignments. " +
                        "Log in to DoubleCheck to get started. \n\n" +
                        "Sincerely, \n\n" + "Your friends at DoubleCheck";
                    string subject = "Assignment Reminder";
                    SendEmailMessage(user.Email, emailBody, subject);

                }
            }
        }

        static public void SendEmailMessage(string recipient, string body, string subject)
        {
            using (var message = new MailMessage("doublechecksau@gmail.com", recipient))
            {
                message.Subject = subject;
                message.Body = body;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("doublechecksau@gmail.com", "DARPdoublecheck@SAU")
                })
                {
                    client.Send(message);
                }
             }
        }


    }
}