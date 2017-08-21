﻿using System;
using System.Collections.Generic;
using System.Linq;

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
                _allEmployees = GoogleSheetConnector.AccessData()
                //for debugging purposes
                .Where(x => x.employee.ImportId < 1018).ToList(); //for testing purposes
                //.Where(x => x.employee.ImportId < 1012).ToList(); //for testing purposes
                //.Where(x => x.employee.ImportId < 1015).ToList(); //for testing purposes
                //for debugging purposes

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
            GetOtherEmployeeChoices(employeeToGuess, unrandomisedChoiceList, employeeToGuessSex, allEmployees);
        }

        public void GetOtherEmployeeChoices(EmployeeWrapper employeeToGuess, List<EmployeeWrapper> unrandomisedChoiceList, string employeeToGuessSex, List<EmployeeWrapper> allEmployees)
        {
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
                Random rndGenerator = new Random();
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
                PopulateFromAlreadyGuessed(unrandomisedChoiceList, employeeToGuessSex, employeeToGuess);
            }
            else
            {
                RandomiseOptions(unrandomisedChoiceList, employeeToGuess);
            }
        }

        public void PopulateFromAlreadyGuessed(List<EmployeeWrapper> unrandomisedChoiceList, string employeeToGuessSex, EmployeeWrapper employeeToGuess)
        {
            int missingNumberOfChoices = 3 - unrandomisedChoiceList.Count;

            using (var db = new AppDbContext())
            {
                int employeeCountInDb = db.Employee.Count();
                int indexOfExtraChoice;
                Random rndGenerator = new Random();
                for (int i = 0; i < missingNumberOfChoices; i++)
                {
                    do
                    {
                        indexOfExtraChoice = rndGenerator.Next(employeeCountInDb);
                    } while (unrandomisedChoiceList.Select(wrapperElement => wrapperElement.employee.ImportId)
                    .ToList().Contains(db.Employee.ToList()[indexOfExtraChoice].ImportId)
                    ||
                    (db.Employee.ToList()[indexOfExtraChoice].Sex != employeeToGuessSex));
                    unrandomisedChoiceList
                        .Add(new EmployeeWrapper
                        { employee = db.Employee.ToList()[indexOfExtraChoice], isUnguessed = false });
                }
            }

            RandomiseOptions(unrandomisedChoiceList, employeeToGuess);
        }

        public void RandomiseOptions(List<EmployeeWrapper> unrandomisedChoiceList, EmployeeWrapper employeeToGuess)
        {
            int indexOfRandomisedOption;
            int counter = 0;
            Random rndGenerator = new Random();
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

