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
            _allEmployees = new List<EmployeeWrapper>();
            if (!unguessedEmployees.Any())
            {
                _allEmployees = GoogleSheetConnector.AccessData()
                //.Where(x => x.ImportId == 1007).ToList<Employee>();
                .Where(x => x.employee.ImportId < 1007).ToList<EmployeeWrapper>(); //for testing purposes
            }
            else if (unguessedEmployees.Where(element => element.isUnguessed == true).ToList().Count < 3)
            {
                //_allEmployees.AddRange(unguessedEmployees);
                _allEmployees = GetExtraChoicesFromGuessed(unguessedEmployees.Where(element => element.isUnguessed == true).ToList());
            }
            else
            {
                _allEmployees = unguessedEmployees;
            }
            _employeeTriad = new List<EmployeeWrapper>();

        }

        private List<EmployeeWrapper> GetExtraChoicesFromGuessed(List<EmployeeWrapper> unguessedEmployees)
        {
            int neededEmployeeCount = 3 - unguessedEmployees.Count;
            using (var db = new AppDbContext())
            {
                int extractableEmployeeNumber;
                int employeeCountInDb = db.Employee.Count();
                Random rndGenerator = new Random();
                for (int i = 0; i < neededEmployeeCount; i++)
                {
                    do
                    {
                        extractableEmployeeNumber = rndGenerator.Next(employeeCountInDb);
                    } while (unguessedEmployees.Select(wrapperElement => wrapperElement.employee.ImportId).ToList<int>().Contains(db.Employee.ToList<Employee>()[extractableEmployeeNumber].ImportId));
                    unguessedEmployees.Add(new EmployeeWrapper { employee = db.Employee.ToList<Employee>()[extractableEmployeeNumber], isUnguessed = false });
                }
            }
            return unguessedEmployees;
        }

        public void PickEmployeeOptions()
        {
            List<int> availableEmployeesForGuessing = _allEmployees
                .Where(employeeComponent => employeeComponent.isUnguessed == true)
                .Select(employeeComponent => employeeComponent.employee.ImportId).ToList<int>();
            int numberOfAvailableOptions = availableEmployeesForGuessing.Count;
            if (numberOfAvailableOptions != 0)
            {
                Random rndGenerator = new Random();
                int orderNumberOfEmployeeToGuess = rndGenerator.Next(numberOfAvailableOptions);
                int importIdOfNextToGuess = availableEmployeesForGuessing[orderNumberOfEmployeeToGuess];
                EmployeeWrapper employeeToGuess = _allEmployees.Where(employeeComponent => employeeComponent.employee.ImportId == importIdOfNextToGuess).First();
                _chosenEmployeeImageId = $"{employeeToGuess.employee.ImportId.ToString()}.png";
                availableEmployeesForGuessing.RemoveAll(element => element == importIdOfNextToGuess);
                List<EmployeeWrapper> unrandomisedChoiceList = new List<EmployeeWrapper>();
                unrandomisedChoiceList.Add(employeeToGuess);
                int incorrectChoiceOrderNumber;
                unrandomisedChoiceList.AddRange(_allEmployees.Where(element => element.employee.ImportId != employeeToGuess.employee.ImportId));
                numberOfAvailableOptions = 3;
                for (int i = 0; i < 2; i++)
                {
                    incorrectChoiceOrderNumber = rndGenerator.Next(numberOfAvailableOptions);
                    numberOfAvailableOptions--;
                    _employeeTriad.Add(unrandomisedChoiceList[incorrectChoiceOrderNumber]);
                    unrandomisedChoiceList.Remove(unrandomisedChoiceList[incorrectChoiceOrderNumber]);
                }
                _employeeTriad.Add(unrandomisedChoiceList.First());
                _chosenTriadEmployee = _employeeTriad.IndexOf(employeeToGuess);
            }
        }
    }
}
