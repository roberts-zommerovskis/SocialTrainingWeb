using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
        public ActionResult Index()
        {
            //int randomEmployeeNumber;
            Random rnd = new Random();
            List<Employee> employeeChoiceTriad = new List<Employee>();
            _employeeList = DTO.GetEmployees();
            List<int> rndNumberList = new List<int>();
            employeeChoiceTriad.Add(_employeeList.Where(x => x.ImportId == 1007).First());
            employeeChoiceTriad.Add(_employeeList.Where(x => x.ImportId == 1009).First());
            employeeChoiceTriad.Add(_employeeList.Where(x => x.ImportId == 1010).First());


            //for (int i = 0; i < 3; i++)
            //{
            //    do
            //    {
            //        randomEmployeeNumber = rnd.Next(_employeeList.Count);
            //    } while (rndNumberList.Contains(randomEmployeeNumber));

            //    rndNumberList.Add(randomEmployeeNumber);
            //    employeeChoiceTriad.Add(_employeeList[randomEmployeeNumber]);
            //}
            return View(employeeChoiceTriad);
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