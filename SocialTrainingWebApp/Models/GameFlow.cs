using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SocialTrainingWebApp.Models
{
    public class GameFlow
    {
        long _indexOfLastGame;
        public void HandleJustLoggedInUser(HttpSessionStateBase session)
        {
            session["justLoggedIn"] = false;
            GoogleSheetConnector.ImportDataIntoDB();
        }

        public bool RegisterAnswer(HttpSessionStateBase session, out Game currentGameToAdd)
        {
            session["answerSubmitted"] = false;
            currentGameToAdd = (Game)session["currentGameStatus"];
            if ((bool)session["answeredCorrectly"])
            {
                session["answeredCorrectly"] = false;
                currentGameToAdd.PointsSoFar++;
            }
            using (var db = new AppDbContext())
            {
                db.Game.Add(currentGameToAdd);
                db.SaveChanges();
            }
            List<Employee> unguessedEmployees = JsonConvert.DeserializeObject<List<Employee>>(currentGameToAdd.UnguessedEmployees);
            if (unguessedEmployees == null || unguessedEmployees.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ChosenEmployees PlayGame(HttpSessionStateBase session, string currentUser)
        {
            List<Employee> employeesWithRespectiveEmail;
            List<Game> currentUsersGames;
            using (var db = new AppDbContext())
            {
                employeesWithRespectiveEmail = db.Employee.Where(employees => employees.Email == currentUser).ToList();
                currentUsersGames = employeesWithRespectiveEmail.First().Games.ToList();
            }
            bool lastGameUnfinished = false;
            Game lastPlayedGameOfEmployee = null;
            List<Employee> unguessedEmployeesOfLastGame = null;
            List<Employee> guessedEmployeesOfLastGame = null;
            if (employeesWithRespectiveEmail.Count > 0)
            //if the player is an Employee in the DB
            {
                GetUsersPlayedGameData(currentUsersGames, employeesWithRespectiveEmail, ref lastGameUnfinished, ref lastPlayedGameOfEmployee, ref unguessedEmployeesOfLastGame, ref guessedEmployeesOfLastGame);
                Random rndGenerator = new Random();
                int indexOfEmployeeForOptions = 0;
                if (lastGameUnfinished)
                {//first get the guys you are gonna show, then add 1 to guessedList, then serialize it
                 //into a DB record
                    ResumeFromUnfinishedGame(lastPlayedGameOfEmployee, employeesWithRespectiveEmail, session, rndGenerator, indexOfEmployeeForOptions, unguessedEmployeesOfLastGame, guessedEmployeesOfLastGame);
                }
                else
                {
                    StartNewGame(employeesWithRespectiveEmail, session, rndGenerator, indexOfEmployeeForOptions);
                }
                return (ChosenEmployees)session["chosenEmployeeModel"];
            }
            else
            //if the player is not an Employee in the DB
            {
                return null;
            }
        }

        public void GetUsersPlayedGameData(List<Game> currentUsersGames, List<Employee> employeesWithRespectiveEmail, ref bool lastGameUnfinished, ref Game lastPlayedGameOfEmployee, ref List<Employee> unguessedEmployeesOfLastGame, ref List<Employee> guessedEmployeesOfLastGame)
        {
            List<Game> allTheGamesInDB;
            using (var db = new AppDbContext())
            {
                allTheGamesInDB = db.Game.ToList();
            }
            if (currentUsersGames == null || currentUsersGames.Count == 0)
            //TODO: remove null stuff
            //if User hasn't gamed at all
            {
                if (allTheGamesInDB == null || allTheGamesInDB.Count == 0)
                //TODO: remove null stuff
                //if nobody has gamed at all
                {
                    _indexOfLastGame = 0;
                }
                else
                //if somebody has gamed (doesn't have to be the specific person)
                {
                    _indexOfLastGame = allTheGamesInDB
                    .Select(gameElements => gameElements.GameId).Distinct().Max();
                }
                lastGameUnfinished = false;
            }
            else
            //if the specific user has gamed
            {
                _indexOfLastGame = currentUsersGames
                    .Select(gameElement => gameElement.GameId).ToList().Max();
                lastPlayedGameOfEmployee = employeesWithRespectiveEmail.First().Games.Where(gameElements => gameElements.GameId == _indexOfLastGame).Last();
                unguessedEmployeesOfLastGame = JsonConvert.DeserializeObject<List<Employee>>(lastPlayedGameOfEmployee.UnguessedEmployees);
                guessedEmployeesOfLastGame = JsonConvert.DeserializeObject<List<Employee>>(lastPlayedGameOfEmployee.GuessedEmployees);
                lastGameUnfinished = true;
                if (unguessedEmployeesOfLastGame == null || unguessedEmployeesOfLastGame.Count == 0)
                {
                    lastGameUnfinished = false;
                    _indexOfLastGame = allTheGamesInDB
                    .Select(gameElements => gameElements.GameId).Distinct().Max();
                }
            }
        }

        public void ResumeFromUnfinishedGame(Game lastPlayedGameOfEmployee, List<Employee> employeesWithRespectiveEmail, HttpSessionStateBase session, Random rndGenerator, int indexOfEmployeeForOptions, List<Employee> unguessedEmployeesOfLastGame, List<Employee> guessedEmployeesOfLastGame)
        {
            indexOfEmployeeForOptions = rndGenerator.Next(unguessedEmployeesOfLastGame.Count);
            Employee employeeToGuess = unguessedEmployeesOfLastGame[indexOfEmployeeForOptions];
            List<Employee> unsortedGuessingOptions = new List<Employee>() { employeeToGuess };
            unguessedEmployeesOfLastGame.Remove(employeeToGuess);
            string employeeToGuessGender = employeeToGuess.Gender;
            List<Employee> sameGenderRemainingUnguessedEmployees =
                    unguessedEmployeesOfLastGame
                    .Where(employeeElements => employeeElements.Gender == employeeToGuessGender)
                    .ToList();
            List<Employee> sameGenderAlreadyGuessedEmployees =
                guessedEmployeesOfLastGame
                .Where(employeeElements => employeeElements.Gender == employeeToGuessGender)
                .ToList();
            guessedEmployeesOfLastGame.Add(employeeToGuess);
            if (unguessedEmployeesOfLastGame.Count > 0)
            //if there is sth in the unguessed list after taking out employee for guessing
            {
                if (sameGenderRemainingUnguessedEmployees.Count > 0)
                //if there are any people of the same gender in the unguessedList
                {
                    while (sameGenderRemainingUnguessedEmployees.Count != 0 && unsortedGuessingOptions.Count < 3)
                    {
                        indexOfEmployeeForOptions = rndGenerator.Next(sameGenderRemainingUnguessedEmployees.Count);
                        Employee extraEmployeeOption = sameGenderRemainingUnguessedEmployees[indexOfEmployeeForOptions];
                        unsortedGuessingOptions.Add(extraEmployeeOption);
                        sameGenderRemainingUnguessedEmployees.Remove(extraEmployeeOption);
                    }
                    if (unsortedGuessingOptions.Count != 3)
                    {
                        indexOfEmployeeForOptions = rndGenerator.Next(sameGenderAlreadyGuessedEmployees.Count);
                        unsortedGuessingOptions.Add(sameGenderAlreadyGuessedEmployees[indexOfEmployeeForOptions]);
                    }
                }
                else
                //if there aren't any people of the same gender left unguessed
                {
                    GetNeededOptionsFromAlreadyGuessed(unsortedGuessingOptions, sameGenderAlreadyGuessedEmployees, rndGenerator, indexOfEmployeeForOptions);
                }
            }
            else //if the removed for guessing person was the last in the unguessedList (final guess)
            {
                GetNeededOptionsFromAlreadyGuessed(unsortedGuessingOptions, sameGenderAlreadyGuessedEmployees, rndGenerator, indexOfEmployeeForOptions);
            }
            SetupNextMove(employeeToGuess, rndGenerator, indexOfEmployeeForOptions, unsortedGuessingOptions, lastPlayedGameOfEmployee, guessedEmployeesOfLastGame, unguessedEmployeesOfLastGame, employeesWithRespectiveEmail, session);

        }

        public void GetNeededOptionsFromAlreadyGuessed(List<Employee> unsortedGuessingOptions, List<Employee> sameGenderAlreadyGuessedEmployees, Random rndGenerator, int indexOfEmployeeForOptions)
        {
            while (unsortedGuessingOptions.Count < 3)
            {
                List<Employee> guessedEmployeeBuffer = new List<Employee>();
                guessedEmployeeBuffer.AddRange(sameGenderAlreadyGuessedEmployees);
                indexOfEmployeeForOptions = rndGenerator.Next(guessedEmployeeBuffer.Count);
                unsortedGuessingOptions.Add(guessedEmployeeBuffer[indexOfEmployeeForOptions]);
                guessedEmployeeBuffer.Remove(guessedEmployeeBuffer[indexOfEmployeeForOptions]);
            }
        }

        public void SetupNextMove(Employee employeeToGuess, Random rndGenerator, int indexOfEmployeeForOptions, List<Employee> unsortedGuessingOptions, Game lastPlayedGameOfEmployee, List<Employee> guessedEmployeesOfLastGame, List<Employee> unguessedEmployeesOfLastGame, List<Employee> employeesWithRespectiveEmail, HttpSessionStateBase session)
        {
            session["currentGameStatus"] = new Game
            {
                GameId = _indexOfLastGame,
                EmployeePK = employeesWithRespectiveEmail.First().EmployeePK,
                UnguessedEmployees = JsonConvert.SerializeObject(unguessedEmployeesOfLastGame),
                GuessedEmployees = JsonConvert.SerializeObject(guessedEmployeesOfLastGame),
                PointsSoFar = lastPlayedGameOfEmployee.PointsSoFar
            };
            List<Employee> employeesRandomizedForGuessing = RandomizeGuessingOptions(unsortedGuessingOptions, indexOfEmployeeForOptions, rndGenerator);
            session["chosenEmployeeModel"] = new ChosenEmployees(employeeToGuess, employeesRandomizedForGuessing);
        }

        public List<Employee> RandomizeGuessingOptions(List<Employee> unsortedGuessingOptions, int indexOfEmployeeForOptions, Random rndGenerator)
        {
            List<Employee> employeesRandomizedForGuessing = new List<Employee>();
            while (unsortedGuessingOptions.Count != 0)
            {
                indexOfEmployeeForOptions = rndGenerator.Next(unsortedGuessingOptions.Count);
                employeesRandomizedForGuessing.Add(unsortedGuessingOptions[indexOfEmployeeForOptions]);
                unsortedGuessingOptions.RemoveAt(indexOfEmployeeForOptions);
            }
            return employeesRandomizedForGuessing;
        }

        public void StartNewGame(List<Employee> employeesWithRespectiveEmail, HttpSessionStateBase session, Random rndGenerator, int indexOfEmployeeForOptions)
        {
            List<Employee> blankListForInitializingGame = new List<Employee>();
            List<Employee> allEmployeesInDB = new List<Employee>();
            using (var db = new AppDbContext())
            {
                allEmployeesInDB.AddRange(db.Employee);
            }
            List<Employee> unrandomizedEmployeesForGuessing = new List<Employee>();
            rndGenerator = new Random();
            indexOfEmployeeForOptions = rndGenerator.Next(allEmployeesInDB.Count);
            Employee employeeToGuess = allEmployeesInDB[indexOfEmployeeForOptions];
            allEmployeesInDB.RemoveAt(indexOfEmployeeForOptions);
            unrandomizedEmployeesForGuessing.Add(employeeToGuess);
            string employeeToGuessGender = employeeToGuess.Gender;
            List<Employee> employeeOptionsOfTheSameGender = new List<Employee>();
            employeeOptionsOfTheSameGender.AddRange(allEmployeesInDB.
                Where(employeeElement => employeeElement.Gender == employeeToGuessGender).
                Except(unrandomizedEmployeesForGuessing));
            for (int i = 0; i < 2; i++)
            {
                indexOfEmployeeForOptions = rndGenerator.Next(employeeOptionsOfTheSameGender.Count);
                unrandomizedEmployeesForGuessing.Add(employeeOptionsOfTheSameGender[indexOfEmployeeForOptions]);
                employeeOptionsOfTheSameGender.RemoveAt(indexOfEmployeeForOptions);
            }
            session["currentGameStatus"] = new Game
            {
                GameId = ++_indexOfLastGame,
                EmployeePK = employeesWithRespectiveEmail.First().EmployeePK,
                UnguessedEmployees = JsonConvert.SerializeObject(allEmployeesInDB),
                GuessedEmployees = JsonConvert.SerializeObject(new List<Employee>() { employeeToGuess }),
                PointsSoFar = 0
            };
            List<Employee> randomizedEmployeeOptions = new List<Employee>();
            while (unrandomizedEmployeesForGuessing.Count != 0)
            {
                indexOfEmployeeForOptions = rndGenerator.Next(unrandomizedEmployeesForGuessing.Count);
                randomizedEmployeeOptions.Add(unrandomizedEmployeesForGuessing[indexOfEmployeeForOptions]);
                unrandomizedEmployeesForGuessing.RemoveAt(indexOfEmployeeForOptions);
            }
            session["chosenEmployeeModel"] = new ChosenEmployees(employeeToGuess, randomizedEmployeeOptions);
        }



    }
}
