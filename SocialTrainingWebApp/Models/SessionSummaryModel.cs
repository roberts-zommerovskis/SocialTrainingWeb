using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialTrainingWebApp.Models
{
    public class SessionSummaryModel
    {
        public double _percentageValue;
        public string _percentageString;
        public string _congratulationsButtonText;
        public SessionSummaryModel(int originalEmployeeCountInDb, double guessedEmployees, bool gameCompleted)
        {
            _percentageValue = Math.Round(((guessedEmployees / originalEmployeeCountInDb) * 100), 0, MidpointRounding.ToEven);
            _percentageString = $"{ _percentageValue.ToString()}%";
            if (gameCompleted)
            {
                _congratulationsButtonText = "Play again!";
            }
            else
            {
                _congratulationsButtonText = "Continue game!";
            }
        }
    }
}
