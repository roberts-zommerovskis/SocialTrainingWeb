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
        public SessionSummaryModel(int originalEmployeeCountInDb, double guessedEmployees)
        {
            //originalEmployeeCountInDb = 7; for testing purposes
            _percentageValue = Math.Round(((guessedEmployees / originalEmployeeCountInDb) * 100), 0, MidpointRounding.ToEven);
            _percentageString = $"{ _percentageValue.ToString()}%";
        }
    }
}
