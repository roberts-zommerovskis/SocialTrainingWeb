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
        public List<int> _chosenEmployeeNumbers;
        public List<Employee> _allEmployees;
        public int _points;
        public string _chosenEmployeeImageId;

        public ChosenEmployees(int points)
        {
            _points = points;
            _employeeTriad = new List<Employee>();
            _chosenEmployeeNumbers = new List<int>();
            _allEmployees = DTO.GetEmployees();
        }

        public string GetOtherOptionEmployeeIDs(int chosenOption, List<Employee> employeeTriad, string imageName)
        {
            string result = chosenOption.ToString() + "!" + string.Join("!", employeeTriad.Where(x => x.ImportId != chosenOption).Select(x => x.ImportId).ToList<int>()) + "!" + _points.ToString() + "!" + imageName;
            return result;
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

        //public string ChooseIframeImage()
        //{
        //    Random rnd = new Random();
        //    int chosenTriadEmployee = rnd.Next(2);
        //    string chosenEmployeeImageId = $"{_employeeTriad[chosenTriadEmployee].ImportId.ToString()}.png";
        //    return chosenEmployeeImageId;
        //}

        public void ChooseIframeImage()
        {
            Random rnd = new Random();
            int chosenTriadEmployee = rnd.Next(2);
            _chosenEmployeeImageId = $"{_employeeTriad[chosenTriadEmployee].ImportId.ToString()}.png";
        }
    }
}
