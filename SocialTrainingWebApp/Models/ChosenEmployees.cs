using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialTrainingWebApp.Models
{
    public class ChosenEmployees
    {
        public List<Employee> _employeeTriad;
        public List<Employee> _allEmployees;
        public string _chosenEmployeeImageId;
        public int _chosenTriadEmployee;

        public ChosenEmployees(List<Employee> unguessedEmployees)
        {
            if (!unguessedEmployees.Any())
            {
                _allEmployees = GoogleSheetConnector.AccessData();
            }
            else
            {
                _allEmployees = unguessedEmployees;
            }
            _employeeTriad = new List<Employee>();

        }

        public void PickEmployeeOptions()
        {
            int randomEmployeeNumber;
            Random rnd = new Random();
            List<int> randomNumberList = new List<int>();
            for (int i = 0; i < 3; i++)
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
            _chosenTriadEmployee = rnd.Next(2);
            _chosenEmployeeImageId = $"{_employeeTriad[_chosenTriadEmployee].ImportId.ToString()}.png";
        }
    }
}
