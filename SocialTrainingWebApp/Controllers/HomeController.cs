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
        public int _points;
        public ChosenEmployees _chosenEmployees;
        public ActionResult Index(string buttonid)
        {
            List<Employee> allEmployees = new List<Employee>();
            if (buttonid != null)
            {
                int buttonNumber = int.Parse(buttonid);
                System.Threading.Thread.Sleep(4000);
                List<Employee> currentTriad = (List<Employee>)Session["currentEmployeeTriadChoice"];
                allEmployees = (List<Employee>)Session["currentDataState"];
                string imageEmployeeNumber = (string)Session["chosenImage"];

                if (currentTriad[buttonNumber - 1].ImportId == int.Parse(imageEmployeeNumber.Substring(0, imageEmployeeNumber.LastIndexOf('.'))))
                {
                    _points = (int)Session["points"];
                    _points++;
                    Session["points"] = _points;
                    allEmployees.RemoveAll(employee => employee.ImportId == currentTriad[buttonNumber - 1].ImportId);

                }
                else
                {
                    _points = (int)Session["points"];
                }

            }
            else
            {
                Session["points"] = 0;
                _points = 0;
            }
            _chosenEmployees = new ChosenEmployees(allEmployees);
            _chosenEmployees.PickEmployeeOptions();
            _chosenEmployees.ChooseIframeImage();
            Session["chosenImage"] = _chosenEmployees._chosenEmployeeImageId;
            Session["currentEmployeeTriadChoice"] = _chosenEmployees._employeeTriad;
            Session["currentDataState"] = _chosenEmployees._allEmployees;
            return View(_chosenEmployees);
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