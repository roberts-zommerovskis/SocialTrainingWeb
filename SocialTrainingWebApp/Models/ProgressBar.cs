using Newtonsoft.Json;
using System.Web;
using System;
using System.Collections.Generic;

namespace SocialTrainingWebApp.Models
{
    public class ProgressBar
    {
        public string _percentageString;
        public string _progressSoFar;
        public double _dividend;
        public int _divisor;
        public Game _gameStatus;
        public ProgressBar()
        {
            _gameStatus = (Game)HttpContext.Current.Session["currentGameStatus"];
            _dividend = _gameStatus.PointsSoFar;
            _divisor = JsonConvert.DeserializeObject<List<Employee>>(_gameStatus.UnguessedEmployees).Count + JsonConvert.DeserializeObject<List<Employee>>(_gameStatus.GuessedEmployees).Count;
            CalculatePercentages(false);
        }

        public void CalculatePercentages(bool shouldIncrease)
        {
            if (shouldIncrease)
            {
                ++_dividend;
            }
            double percentage = Math.Round(((_dividend / _divisor) * 100), 0, MidpointRounding.ToEven);
            _percentageString = $"{ percentage.ToString()}%";
            _progressSoFar = $"{_gameStatus.PointsSoFar + 1}/{JsonConvert.DeserializeObject<List<Employee>>(_gameStatus.UnguessedEmployees).Count + JsonConvert.DeserializeObject<List<Employee>>(_gameStatus.GuessedEmployees).Count}";
        }






    }
}
