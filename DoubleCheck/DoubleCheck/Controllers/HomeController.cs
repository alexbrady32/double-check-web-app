using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DoubleCheck.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            ViewResult indexView = View("/Views/Home/Index.cshtml");
            
            return indexView;
        }

        public void Login()
        {
            
        }

        public void Create()
        {

        }
	}
}