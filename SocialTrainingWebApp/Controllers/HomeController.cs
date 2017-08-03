using System.Web.UI.WebControls;
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
        public List<Employee> _employeeChoiceTriad;
        public List<Employee> _guessedEmployees;
        public int _points;
        public ActionResult Index(string buttonid)
        {
            if (buttonid != null)
            {
                List<string> answers = buttonid.Split('!').ToList<string>();
                string imageId = answers.Last().Substring(0, answers.Last().LastIndexOf('.'));
                if (answers.First().Equals(imageId))
                {
                    _points = int.Parse(answers[answers.Count - 2]) + 1;
                }
                else
                {
                    _points = int.Parse(answers[answers.Count - 2]);
                }

            }
            else
            {
                _points = 0;
            }
            ChosenEmployees chosenEmployees = new ChosenEmployees(_points);
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