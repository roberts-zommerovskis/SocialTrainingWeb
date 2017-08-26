using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
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
            string currentUser = User.Identity.GetUserName();
            GameFlow gameFlow = new GameFlow();
            if (Session["justLoggedIn"] == null)
            {
                gameFlow.HandleJustLoggedInUser(Session);
            }
            if (Session["answerSubmitted"] != null)
            {
                if ((bool)Session["answerSubmitted"] == true || !(bool)Session["canProceed"])
                {
                    Session["canProceed"] = false;
                    Game currentGameToAdd = new Game();
                    bool gameCompleted = gameFlow.RegisterAnswer(Session, out currentGameToAdd);
                    return View("Congratulations", new SessionSummaryModel(JsonConvert.DeserializeObject<List<Employee>>(currentGameToAdd.GuessedEmployees).Count, currentGameToAdd.PointsSoFar, gameCompleted));
                }
            }
            gameFlow.PlayGame(Session, User.Identity.GetUserName());
            return View((ChosenEmployees)Session["chosenEmployeeModel"]);
        }

        public JsonResult CheckAnswer(string ID)
        {
            Session["answerSubmitted"] = true;
            if (int.Parse(ID) == ((ChosenEmployees)Session["chosenEmployeeModel"])._chosenTriadEmployee)
            {
                Session["answeredCorrectly"] = true;
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Session["answeredCorrectly"] = false;
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult PlayAgain(bool PlayAgain)
        {
            Session["canProceed"] = true;
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReturnCorrectAnswerNumber()
        {
            return Json(new { Answer = ((ChosenEmployees)Session["chosenEmployeeModel"])._chosenTriadEmployee }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Congratulations()
        {
            return View();
        }
    }
}