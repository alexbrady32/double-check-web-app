using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using Quartz;
using Quartz.Impl;
using DoubleCheck.Utilities;

namespace DoubleCheck
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // Scheduler for email notifications
            ScheduleStart();

        }

        protected void ScheduleStart()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();

            ITrigger trigger = TriggerBuilder.Create().StartNow().WithCalendarIntervalSchedule(x => x.WithIntervalInWeeks(1)).Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
