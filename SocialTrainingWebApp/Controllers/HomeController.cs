using System.Linq;
using System;
using SocialTrainingWebApp.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SocialTrainingWebApp.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public List<Employee> _employeeList;
        public List<Employee> employeeChoiceTriad;
        public ActionResult Index()
        {
            ChosenEmployees chosenEmployees = new ChosenEmployees();
            chosenEmployees.PickEmployeeOptions();
            return View(chosenEmployees);
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

        public RedirectToRouteResult UserChoice(string buttonid)
        {
            return RedirectToAction("Contact", "Home");
        }

    }
}