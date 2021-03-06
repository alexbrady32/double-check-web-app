﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoubleCheck.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DoubleCheck.Controllers
{
    public class AccountController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();
        private bool invalid;
        
        // GET: Account/
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();

        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(User credentials)
        {
            if (credentials.Username != null && credentials.Password != null)
            {
                // Use the hashed password when checking the database
                string hashedPassword = CreatePasswordHash(credentials.Password);

                var user = db.Users.Where(model => model.Username.Equals(credentials.Username) 
                                                 && model.Password.Equals(hashedPassword)).FirstOrDefault();

                if (user != null)
                {
                    Session.Timeout = 60;
                    Session["UserID"] = user.Id.ToString();
                    Session["UserFirstName"] = user.firstName.ToString();
                    Session["UserLastName"] = user.lastName.ToString();
                    return RedirectToAction("Index", "Home");
                }

                TempData["Message"] = "Invalid username or password, please try again.";

            }

            return View(credentials);

        }
        
        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }


        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // GET: Account/ResetPassword
        public ActionResult ResetPassword(string code)
        {
            var user = db.Users.Where(u => u.ResetPasswordHash == code).First();
            if (DateTime.Now < user.ResetPasswordExpiration)
            {
                return View(user);
            }
            // TODO: Either Hash is expired, or a code was not in the link (verify this)
            // So we need to either give them a message, or I guess just redirect the user in the case of 
            // a code not being passed in
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,Email,Password,firstName,lastName,phone_num,canNotifyByEmail,canNotifyByText")] User user)
        {
            if (ModelState.IsValid)
            {
                // Create Password Hash and store back into the model
                user.Password = CreatePasswordHash(user.Password);

                var userCount = db.Users.Count(u => (u.Username == user.Username) || (u.Email == user.Email));
                if (userCount == 0)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    ViewBag.Error = "";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "That user already exists!";
                }  
            }
            return View(user);
        }

        // GET: Account/Edit
        public ActionResult Edit()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            User user = db.Users.Find(Int32.Parse((string)Session["UserID"]));
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Account/Edit
        // [Bind(Include = "Id,Username,Password,Email,firstName,lastName,phone_num,Cloze_Score,canNotifyByEmail,canNotifyByText")] 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(user).State = EntityState.Modified;
                    //decimal? score = db.Users.Find(user.Id).Cloze_Score;
                    if ((string)Request.Params["retypePassword"] != "")
                    {
                        if (String.Equals(CreatePasswordHash((string)Request.Params["retypePassword"]), user.Password))
                        {
                            user.Password = CreatePasswordHash((string)Request.Params["newPassword"]);
                            ViewBag.Error = "";
                        }
                        else
                        {
                            ViewBag.Error = "Incorrect retyped password!";
                            return View(user);
                        }
                    }
                    //user.Cloze_Score = score;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Account/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Login", "Account");
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Account/ForgotPassword/email
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            var result = db.Users.Where(u => u.Email == email);
            if (result.Count() == 1)
            {
                User user = result.First();
                user.ResetPasswordHash = CreatePasswordHash(DateTime.Now.ToString());
                user.ResetPasswordExpiration = DateTime.Now.AddDays(3);
                db.SaveChanges();

                // Code to get current url path taken from StackOverflow post
                // http://stackoverflow.com/questions/1214607/how-can-i-get-the-root-domain-uri-in-asp-net
                var appPath = string.Empty;
                //Getting the current context of HTTP request
                var context = System.Web.HttpContext.Current;
                //Checking the current context content
                if (context != null)
                {
                    //Formatting the fully qualified website url/name
                    appPath = string.Format("{0}://{1}{2}",
                                            context.Request.Url.Scheme,
                                            context.Request.Url.Host,
                                            context.Request.Url.Port == 80
                                                ? string.Empty
                                                : ":" + context.Request.Url.Port
                                            );
                    appPath += "/DoubleCheck" 
                    appPath += "/Account/ResetPassword";
                    // Adds the password hash as a GET parameter into the url
                    appPath += "?code=";
                    appPath += user.ResetPasswordHash;
                }
                

                string emailBody = "Hello, " + user.firstName.ToString() + "!\n\n"
                        + "Please click on the following link to reset your DoubleCheck password. \n\n" +  
                        appPath + " \n\n" +
                        "Sincerely, \n\n" + "Your friends at DoubleCheck";
                string emailSubject = "DoubleCheck - Reset Password Request";
                // TODO: Use the logged in user's email address
                Utilities.EmailJob.SendEmailMessage("alexbrady32@gmail.com", emailBody, emailSubject);
                return RedirectToAction("Login");
            }
            else
            {
                // Email either exists twice (which it shouldn't, if database implemented correctly)
                // Or, email doesn't exist. For security reasons, the user should not know whether its a valid email or not
                return RedirectToAction("Login");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(User user)
        {
            if (ModelState.IsValid)
            {
                if ((string)Request.Params["retypePassword"] != "")
                {
                    if (String.Equals((string)Request.Params["retypePassword"], user.Password))
                    {
                        db.Entry(user).State = EntityState.Modified;
                        // Create Password Hash and store back into the model
                        user.Password = CreatePasswordHash(user.Password);
                        db.SaveChanges();
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        user.Password = "";
                        ViewBag.Error = "Passwords do not match!";
                        return View(user);
                    }
                }
                ViewBag.Error = "Please retype the password.";
                return View(user);
            }
            ViewBag.Error = "An error occured. Please try again later.";
            return View(user);
        }

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        public string CreatePasswordHash(string password)
        {
            string salt = "JlfufhfiuI4284rhciwIey$f";
            return Hash(password + salt);
        }

        private static string Hash(string input)
        {
            using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                var hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                var sb = new System.Text.StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
