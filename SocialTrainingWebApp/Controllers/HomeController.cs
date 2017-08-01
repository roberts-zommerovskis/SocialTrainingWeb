using SocialTrainingWebApp.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SocialTrainingWebApp.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public List<Employee> _employeeList;
        public ActionResult Index()
        {

            GoogleSheetAuthentication.ParameterCreator();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}