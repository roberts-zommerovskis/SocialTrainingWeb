﻿using System.Web.UI.WebControls;
using System.Linq;
using System;
using SocialTrainingWebApp.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SocialTrainingWebApp.Controllers
{
    [RequireHttps]
    [Authorize]
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
                //System.Threading.Thread.Sleep(2000);
                List<Employee> currentTriad = (List<Employee>)Session["currentEmployeeTriadChoice"];
                allEmployees = (List<Employee>)Session["currentDataState"];
                string imageEmployeeNumber = (string)Session["chosenImage"];

                if (currentTriad[buttonNumber].ImportId == int.Parse(imageEmployeeNumber.Substring(0, imageEmployeeNumber.LastIndexOf('.'))))
                {
                    _points = (int)Session["points"];
                    _points++;
                    Session["points"] = _points;
                }
                else
                {
                    _points = (int)Session["points"];
                }
                allEmployees.RemoveAll(employee => employee.ImportId == currentTriad[buttonNumber].ImportId);
            }
            else
            {
                Session["points"] = 0;
                _points = 0;
                GoogleSheetConnector.ImportDataIntoDB();
                Session["employeeCount"] = GoogleSheetConnector.GetEmployeeCount();
            }
            if (allEmployees.Count != 0 || (allEmployees.Count == 0 && _points == 0))
            {
                _chosenEmployees = new ChosenEmployees(allEmployees);
                _chosenEmployees.PickEmployeeOptions();
                _chosenEmployees.ChooseIframeImage();
                Session["chosenImage"] = _chosenEmployees._chosenEmployeeImageId;
                Session["currentEmployeeTriadChoice"] = _chosenEmployees._employeeTriad;
                Session["currentDataState"] = _chosenEmployees._allEmployees;
                Session["chosenTriadEmployee"] = _chosenEmployees._chosenTriadEmployee;
                return View(_chosenEmployees);
            }
            else
            {
                return View("Congratulations", new SessionSummaryModel((int)Session["employeeCount"], _points));
            }
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

        public JsonResult CheckAnswer(string ID)
        {
            if (int.Parse(ID) == (int)Session["chosenTriadEmployee"])
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Congratulations()
        {
            return View();
        }


    }
}