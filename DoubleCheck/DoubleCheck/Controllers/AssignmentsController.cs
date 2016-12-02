﻿using DoubleCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace DoubleCheck.Controllers
{
    public class AssignmentsController : Controller
    {
        private doublecheckdbEntities db = new doublecheckdbEntities();

        // GET: Assignments
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            int userID;

            if (Session["UserID"] != null)
            {
                userID = int.Parse((string)Session["UserID"]);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            var stringsToUse = Utilities.Utilities.calculateWeeklyTotal(userID);
            var message = "";
            var string3Join = "";
            var string4Join = "";
            if (stringsToUse[1] != "")
            {
                message += "You need to spend "
                    + stringsToUse[1] + " on your assignments this week in order to finish them in time. That includes ";
                if (stringsToUse[2] != "")
                {
                    message += stringsToUse[2] + " on assignments due this week";
                    string3Join = stringsToUse[4] != "" ? ", " : " and ";
                    string4Join = " and ";
                }
                if (stringsToUse[3] != "")
                {
                    message += string3Join + stringsToUse[3] + "on assignments due in the coming weeks";
                    string4Join = " and ";
                }
                if (stringsToUse[4] != "")
                {
                    message += string4Join + stringsToUse[4] + " on past due assignments.";
                }
                else
                {
                    message += ".";
                }
                
            }
            ViewBag.Message = message;

            List<Assignment> assignments = db.Assignments.Where(model => model.U_Id.Equals(userID)).ToList();
           
            return View(assignments);
        }

        public ActionResult Create()
        {
            var user = Int32.Parse((string)Session["UserID"]);
            var usersClasses = db.Classes.Where(c => c.U_Id == user);
            ViewBag.AppDataClassList = new SelectList(usersClasses, "C_Id", "Name");
            ViewBag.AppDataTypes = new SelectList(db.Asgmt_Type, "Id", "Name");
            // For determining the estimated time to complete for reading assignments
            var userScore = db.Users.Where(u => u.Id == user).FirstOrDefault().Cloze_Score;
            if (userScore < 80)
            {
                ViewBag.ClozeMultiplier = 0.8;
            }
            else if (userScore >= 90)
            {
                ViewBag.ClozeMultiplier = 1.2;
            }
            else
            {
                ViewBag.ClozeMultiplier = 1.0;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(Assignment NewAssignment)
        {
          if(ModelState.IsValid)
            {
           
                NewAssignment.U_Id = Int32.Parse(Session["UserID"].ToString());
                NewAssignment.Status = 1;
                NewAssignment.Completion = 0;

                db.Assignments.Add(NewAssignment);
                db.SaveChanges();

                return RedirectToAction("List");
            }

            return View(NewAssignment);
        }

        public ActionResult Edit(int id)
        {
            var user = Int32.Parse((string)Session["UserID"]);
            var usersClasses = db.Classes.Where(c => c.U_Id == user);
            ViewBag.AppDataClassList = new SelectList(usersClasses, "C_Id", "Name");
            ViewBag.AppDataTypes = new SelectList(db.Asgmt_Type, "Id", "Name");

            Assignment assignment = db.Assignments.Find(id);
            if(assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        [HttpPost]
        public ActionResult Edit(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                Assignment a = db.Assignments.Find(assignment.A_Id);
                a.Name = assignment.Name;
                a.Class = assignment.Class;
                a.T_Id = assignment.T_Id;
                a.Due_Date = assignment.Due_Date;
                a.TTC = assignment.TTC;
                a.Description = assignment.Description;
                db.SaveChanges();

                return RedirectToAction("List");
            }

            return View(assignment);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment a = db.Assignments.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            return View(a);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assignment a = db.Assignments.Find(id);
            db.Assignments.Remove(a);
            db.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
