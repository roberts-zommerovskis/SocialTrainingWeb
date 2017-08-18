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
            List<EmployeeWrapper> allEmployees = new List<EmployeeWrapper>();
            if ((bool)Session["buttonPressed"])
            {
                Session["buttonPressed"] = false;
                if (buttonid != null)
                {
                    int buttonNumber = int.Parse(buttonid);
                    //System.Threading.Thread.Sleep(2000);
                    List<EmployeeWrapper> currentTriad = (List<EmployeeWrapper>)Session["currentEmployeeTriadChoice"];
                    allEmployees = (List<EmployeeWrapper>)Session["currentDataState"];
                    string imageEmployeeNumber = (string)Session["chosenImage"];

                    if (currentTriad[buttonNumber].employee.ImportId == int.Parse(imageEmployeeNumber.Substring(0, imageEmployeeNumber.LastIndexOf('.'))))
                    {
                        _points = (int)Session["points"];
                        _points++;
                        Session["points"] = _points;
                    }
                    else
                    {
                        _points = (int)Session["points"];
                    }
                    allEmployees.RemoveAll(wrapper => wrapper.employee.ImportId == currentTriad[(int)Session["chosenTriadEmployee"]].employee.ImportId);
                    allEmployees.RemoveAll(wrapper => wrapper.isUnguessed == false);
                }
            }
            else if (buttonid == null && (bool)Session["justLoggedIn"])
            {
                Session["justLoggedIn"] = false;
                Session["points"] = 0;
                _points = 0;
                GoogleSheetConnector.ImportDataIntoDB();
                Session["employeeCount"] = GoogleSheetConnector.GetEmployeeCount();
                //for testing purposes
                //4; 
            }

            if (allEmployees.Count != 0 || (allEmployees.Count == 0 && _points == 0))
            {
                if (Session["currentDataState"] != null)
                {
                    allEmployees = (List<EmployeeWrapper>)Session["currentDataState"];
                }
                _chosenEmployees = new ChosenEmployees(allEmployees);


                if (Session["currentDataState"] != null)
                {
                    if (((List<EmployeeWrapper>)Session["currentDataState"]).Count == 0 && Session["chosenImage"] != null)
                    {


                        Session["roundCompleted"] = true;
                        return View("Congratulations", new SessionSummaryModel((int)Session["employeeCount"], (int)Session["points"]));
                    }
                }

                Session["chosenImage"] = _chosenEmployees._chosenEmployeeImageId;
                Session["currentEmployeeTriadChoice"] = _chosenEmployees._employeeTriad;
                Session["currentDataState"] = _chosenEmployees._allEmployees;
                Session["chosenTriadEmployee"] = _chosenEmployees._chosenTriadEmployee;
                return View(_chosenEmployees);
            }
            else
            {
                Session["roundCompleted"] = true;
                return View("Congratulations", new SessionSummaryModel((int)Session["employeeCount"], (int)Session["points"]));
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
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult Congratulations()
        {
            return View();
        }


    }
}