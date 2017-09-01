using Newtonsoft.Json;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialTrainingWebApp.Models
{
    public class ProgressBar
    {
        public string _percentageString;
        public ProgressBar()
        {
            Game gameStatus = (Game)HttpContext.Current.Session["currentGameStatus"];
            double dividend = gameStatus.PointsSoFar;
            int divisor = JsonConvert.DeserializeObject<List<Employee>>(gameStatus.UnguessedEmployees).Count + JsonConvert.DeserializeObject<List<Employee>>(gameStatus.GuessedEmployees).Count - 1;
            double _percentage = Math.Round(((dividend / divisor) * 100), 0, MidpointRounding.ToEven);
            _percentageString = $"{ _percentage.ToString()}%";
        }
    }
}
