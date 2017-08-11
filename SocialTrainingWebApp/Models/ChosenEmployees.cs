using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialTrainingWebApp.Models
{
    public class ChosenEmployees
    {
        public List<EmployeeWrapper> _employeeTriad;
        public List<EmployeeWrapper> _allEmployees;
        public string _chosenEmployeeImageId;
        public int _chosenTriadEmployee;

        public ChosenEmployees(List<EmployeeWrapper> unguessedEmployees)
        {
            if (!unguessedEmployees.Any())
            {
                _allEmployees = GoogleSheetConnector.AccessData();
                //.Where(x => x.ImportId == 1007).ToList<Employee>();
                //.Where(x => x.ImportId < 1011).ToList<Employee>(); //for testing purposes
            }
            else
            {
                _allEmployees = unguessedEmployees;
            }
            _employeeTriad = new List<EmployeeWrapper>();

        }

        public void PickEmployeeOptions()
        {
            int randomEmployeeNumber;
            int optionCount;
            Random rnd = new Random();
            List<int> randomNumberList = new List<int>();
            if (_allEmployees.Count >= 3)
            {
                optionCount = 3;
            }
            else
            {
                optionCount = _allEmployees.Count;
            }

            for (int i = 0; i < optionCount; i++)
            {
                do
                {
                    randomEmployeeNumber = rnd.Next(_allEmployees.Count);
                } while (randomNumberList.Contains(randomEmployeeNumber));
                randomNumberList.Add(randomEmployeeNumber);
                _employeeTriad.Add(_allEmployees[randomEmployeeNumber]);
            }
        }

        public void ChooseIframeImage()
        {
            Random rnd = new Random();
            if (_employeeTriad.Count > 1)
            {
                _chosenTriadEmployee = rnd.Next(_employeeTriad.Count);
            }
            else
            {
                _chosenTriadEmployee = rnd.Next(1);
            }
            _chosenEmployeeImageId = $"{_employeeTriad[_chosenTriadEmployee].employee.ImportId.ToString()}.png";
        }
    }
}
