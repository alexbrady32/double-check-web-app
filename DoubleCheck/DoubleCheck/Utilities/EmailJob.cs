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
                var stringsToUse = Utilities.calculateWeeklyTotal(user.Id);
                if (stringsToUse[1] != "")
                {
                    string emailBody = "Hello, " + user.firstName.ToString() + "!\n\n"
                            + "Don't get behind in your schoolwork! You have " + stringsToUse[0] +
                            " assignments due this week. You have estimated that this will take you approximately " +
                            stringsToUse[1] + " to complete these assignments. " +
                            "Log in to DoubleCheck to get started. \n\n" +
                            "Sincerely, \n\n" + "Your friends at DoubleCheck";
                    SendEmailMessage(user.Email, emailBody);
                }
            }
        }

        public void SendEmailMessage(string recipient, string body)
        {
            using (var message = new MailMessage("doublechecksau@gmail.com", recipient))
            {
                message.Subject = "Assignment Reminder";
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