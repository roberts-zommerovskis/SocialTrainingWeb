using SocialTrainingWebApp.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SocialTrainingWebApp.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        //public int _points;
        //public ChosenEmployees _chosenEmployees;
        public ActionResult Index(string buttonid)
        {
            int points = 0;
            ChosenEmployees chosenEmployees;
            List<EmployeeWrapper> allEmployees = new List<EmployeeWrapper>();
            Setup();
            if (Session["buttonPressed"] != null && (bool)Session["buttonPressed"] && buttonid != null)
            {
                Session["buttonPressed"] = false;
                int buttonNumber = int.Parse(buttonid);
                List<EmployeeWrapper> currentTriad = (List<EmployeeWrapper>)Session["currentEmployeeTriadChoice"];
                allEmployees = (List<EmployeeWrapper>)Session["currentDataState"];
                string imageEmployeeNumber = (string)Session["chosenImage"];
                if (Session["points"] == null)
                {
                    Session["points"] = 0;
                }
                if (currentTriad[buttonNumber].employee.ImportId == int.Parse(imageEmployeeNumber.Substring(0, imageEmployeeNumber.LastIndexOf('.'))))
                {

                    points = (int)Session["points"];
                    points++;
                    Session["points"] = points;
                }
                else
                {
                    points = (int)Session["points"];
                }
                allEmployees.RemoveAll(wrapper => wrapper.employee.ImportId == currentTriad[(int)Session["chosenTriadEmployee"]].employee.ImportId);
                allEmployees.RemoveAll(wrapper => wrapper.isUnguessed == false);
            }
            else if (buttonid == null && Session["justLoggedIn"] != null)
            {
                if ((bool)Session["justLoggedIn"])
                {
                    Session["justLoggedIn"] = false;
                    Session["points"] = 0;
                    points = 0;
                    GoogleSheetConnector.ImportDataIntoDB();
                    Session["employeeCount"] = GoogleSheetConnector.GetEmployeeCount();
                }
            }
            if (allEmployees.Count != 0 || (allEmployees.Count == 0 && points == 0))
            {
                if (Session["currentDataState"] != null)
                {
                    if (((List<EmployeeWrapper>)Session["currentDataState"]).Count == 0 && Session["chosenImage"] != null)
                    {
                        Session["roundCompleted"] = true;
                        return View("Congratulations", new SessionSummaryModel((int)Session["employeeCount"], (int)Session["points"]));
                    }
                    allEmployees = (List<EmployeeWrapper>)Session["currentDataState"];
                }
                chosenEmployees = new ChosenEmployees(allEmployees);
                Session["chosenImage"] = chosenEmployees._chosenEmployeeImageId;
                Session["currentEmployeeTriadChoice"] = chosenEmployees._employeeTriad;
                Session["currentDataState"] = chosenEmployees._allEmployees;
                Session["chosenTriadEmployee"] = chosenEmployees._chosenTriadEmployee;
                return View(chosenEmployees);
            }
            else
            {
                Session["roundCompleted"] = true;
                return View("Congratulations", new SessionSummaryModel((int)Session["employeeCount"], (int)Session["points"]));
            }
        }

        public JsonResult CheckAnswer(string ID)
        {
            Session["buttonPressed"] = true;
            if (int.Parse(ID) == (int)Session["chosenTriadEmployee"])
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult PlayAgain(bool PlayAgain)
        {
            Session["roundCompleted"] = false;
            Session["chosenImage"] = null;
            Session["points"] = 0;
            Session["justLoggedIn"] = true;
            Session["currentDataState"] = null;
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReturnCorrectAnswerNumber()
        {
            return Json(new { Answer = (int)Session["chosenTriadEmployee"] }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Congratulations()
        {
            return View();
        }

        public void Setup()
        {
            if (Session["chosenImage"] == null)
            {
                Session["chosenImage"] = 0;
            }
            if (Session["currentEmployeeTriadChoice"] == null)
            {
                Session["currentEmployeeTriadChoice"] = 0;
            }
            if (Session["chosenTriadEmployee"] == null)
            {
                Session["chosenTriadEmployee"] = 0;
            }
            if (Session["points"] == null)
            {
                Session["points"] = 0;
            }
        }


    }
}