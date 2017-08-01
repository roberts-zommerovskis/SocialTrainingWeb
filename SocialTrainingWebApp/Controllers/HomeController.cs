using SocialTrainingWebApp.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SocialTrainingWebApp.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public List<Employees> _employeeList;
        public ActionResult Index()
        {
            //var webClient = new WebClient();
            //string readHtml = webClient.DownloadString(@"https://spreadsheets.google.com/feeds/list/1-7koO8ufsz8yCQLu4eFnHLd1Qy5-trPYIgGKX7eE3Lo/1/public/values?alt=json");
            //var a = new JavaScriptSerializer();
            //Dictionary<string, object> results = a.Deserialize<Dictionary<string, object>>(readHtml);
            //var title = results["title"].ToString();
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