using System.Collections.Specialized;
using SocialTrainingWebApp.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SocialTrainingWebApp.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index(string buttonid)
        {
            DataManager dataManager = new DataManager();
            dataManager.Setup(this.Session);
            int points = 0;
            ChosenEmployees chosenEmployees = null;
            List<EmployeeWrapper> allEmployees = new List<EmployeeWrapper>();
            if ((bool)Session["buttonPressed"] && buttonid != null)
            {
                dataManager.CalculatePoints(this.Session, buttonid, ref allEmployees, ref points);
            }
            else if (buttonid == null && (bool)Session["justLoggedIn"])
            {
                dataManager.HandleNewlyLoggedIn(this.Session, ref points);
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
                dataManager.PrepareGuessingOptions(this.Session, ref chosenEmployees, ref allEmployees);
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
    }
}