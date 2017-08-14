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
            List<EmployeeWrapper> remainingEmployees = new List<EmployeeWrapper>();
            _allEmployees = new List<EmployeeWrapper>();
            if (!unguessedEmployees.Any())
            {
                _allEmployees = GoogleSheetConnector.AccessData();
                //.Where(x => x.ImportId == 1007).ToList<Employee>();
                //.Where(x => x.employee.ImportId < 1007).ToList(); //for testing purposes
                //.Where(x => x.employee.ImportId < 1012).ToList(); //for testing purposes
            }
            else
            {
                _allEmployees.AddRange(unguessedEmployees);
            }
            _employeeTriad = new List<EmployeeWrapper>();
            PickEmployeeForGuessing(_allEmployees);

        }

        public void PickEmployeeForGuessing(List<EmployeeWrapper> allEmployees)
        {
            List<EmployeeWrapper> unrandomisedChoiceList = new List<EmployeeWrapper>();
            int unguessedEmployeeCount = allEmployees.Count;
            Random rndGenerator = new Random();
            int indexOfEmployeeToGuess = rndGenerator.Next(unguessedEmployeeCount);
            EmployeeWrapper employeeToGuess = allEmployees[indexOfEmployeeToGuess];
            _chosenEmployeeImageId = $"{employeeToGuess.employee.ImportId.ToString()}.png";
            unrandomisedChoiceList.Add(employeeToGuess);
            string employeeToGuessSex = employeeToGuess.employee.Sex;
            int counter = 0;
            if (allEmployees.Except(new List<EmployeeWrapper> { employeeToGuess })
                .Where(wrapperElement => wrapperElement.employee.Sex == employeeToGuessSex)
                .ToList().Count > 0)
            {
                List<EmployeeWrapper> restOfValidUnguessedEmployees = allEmployees.Except(new List<EmployeeWrapper> { employeeToGuess })
                .Where(wrapperElement => wrapperElement.employee.Sex == employeeToGuessSex)
                .ToList();
                int restOfValidUnguessedEmployeesCount;
                int indexOfNextValidUnguessedEmployee;
                do
                {
                    restOfValidUnguessedEmployeesCount = restOfValidUnguessedEmployees.Count;
                    indexOfNextValidUnguessedEmployee = rndGenerator.Next(restOfValidUnguessedEmployeesCount);
                    unrandomisedChoiceList.Add(restOfValidUnguessedEmployees[indexOfNextValidUnguessedEmployee]);
                    restOfValidUnguessedEmployees.RemoveAt(indexOfNextValidUnguessedEmployee);
                    counter++;
                } while (restOfValidUnguessedEmployees.Count != 0 && counter < 2);

            }
            if (unrandomisedChoiceList.Count < 3)
            {
                int missingNumberOfChoices = 3 - unrandomisedChoiceList.Count;

                using (var db = new AppDbContext())
                {
                    int employeeCountInDb = db.Employee.Count();
                    int indexOfExtraChoice;
                    for (int i = 0; i < missingNumberOfChoices; i++)
                    {
                        do
                        {
                            indexOfExtraChoice = rndGenerator.Next(employeeCountInDb);
                        } while (unrandomisedChoiceList
                        .Select(wrapperElement => wrapperElement.employee)
                        .ToList().Contains(db.Employee.ToList()[indexOfExtraChoice])
                        ||
                        (db.Employee.ToList()[indexOfExtraChoice].Sex != employeeToGuessSex));
                        unrandomisedChoiceList
                            .Add(new EmployeeWrapper
                            { employee = db.Employee.ToList()[indexOfExtraChoice], isUnguessed = false });
                    }
                }
            }
            int indexOfRandomisedOption;
            counter = 0;
            do
            {
                indexOfRandomisedOption = rndGenerator.Next(unrandomisedChoiceList.Count);
                EmployeeWrapper randomisedEmployee = unrandomisedChoiceList[indexOfRandomisedOption];
                if (randomisedEmployee.Equals(employeeToGuess))
                {
                    _chosenTriadEmployee = counter;
                }
                _employeeTriad.Add(randomisedEmployee);
                unrandomisedChoiceList.RemoveAt(indexOfRandomisedOption);
                counter++;
            } while (unrandomisedChoiceList.Count != 0);
        }
    }
}
