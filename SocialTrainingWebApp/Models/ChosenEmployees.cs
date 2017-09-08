using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialTrainingWebApp.Models
{
    public class ChosenEmployees
    {
        public List<Employee> _employeeTriad;
        public string _chosenEmployeeImageId;
        public int _chosenTriadEmployee;

        public ChosenEmployees(Employee employeeToGuess, List<Employee> allEmployeeGuessingOptions)
        {
            _employeeTriad = allEmployeeGuessingOptions;
            //_chosenEmployeeImageId = $"{ employeeToGuess.ImportId}.png";
            //TODO: uncomment and update mock images for real data
            if (employeeToGuess.Gender == "Female")
            {
                _chosenEmployeeImageId = "1020.png";
            }
            else
            {
                _chosenEmployeeImageId = "1001.png";
            }
            _chosenTriadEmployee = allEmployeeGuessingOptions.IndexOf(employeeToGuess);
        }
    }
}

