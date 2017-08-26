using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SocialTrainingWebApp.Models
{
    public class GameFlow
    {
        private long _indexOfLastGame;
        private HttpSessionStateBase _session;
        private Game _lastPlayedGameOfEmployee;
        private List<Employee> _employeesWithRespectiveEmail;
        private List<Employee> _unguessedEmployeesOfLastGame;
        private List<Employee> _guessedEmployeesOfLastGame;
        private List<Employee> _unsortedGuessingOptions;
        private int _indexOfEmployeeForOptions;
        private Random _rndGenerator = new Random();

        public void HandleJustLoggedInUser(HttpSessionStateBase session)
        {
            session["justLoggedIn"] = false;
            GoogleSheetConnector.ImportDataIntoDB();
        }

        public bool RegisterAnswer(HttpSessionStateBase session, out Game currentGameToAdd)
        {
            currentGameToAdd = (Game)session["currentGameStatus"];
            if ((bool)session["answeredCorrectly"])
            {
                session["answeredCorrectly"] = false;
                currentGameToAdd.PointsSoFar++;
            }
            if ((bool)session["answerSubmitted"] == true)
            {
                using (var db = new AppDbContext())
                {
                    db.Game.Add(currentGameToAdd);
                    db.SaveChanges();
                }
            }
            session["answerSubmitted"] = false;
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
            _session = session;
            _session["canProceed"] = true;
            List<Game> currentUsersGames;
            using (var db = new AppDbContext())
            {
                _employeesWithRespectiveEmail = db.Employee.Where(employees => employees.Email == currentUser).ToList();
                currentUsersGames = _employeesWithRespectiveEmail.First().Games.ToList();
            }
            bool lastGameUnfinished = false;
            if (_employeesWithRespectiveEmail.Count > 0)
            //if the player is an Employee in the DB
            {
                GetUsersPlayedGameData(currentUsersGames, ref lastGameUnfinished);
                if (lastGameUnfinished)
                {//first get the guys you are gonna show, then add 1 to guessedList, then serialize it
                 //into a DB record
                    ResumeFromUnfinishedGame();
                }
                else
                {
                    StartNewGame();
                }
                return (ChosenEmployees)_session["chosenEmployeeModel"];
            }
            else
            //if the player is not an Employee in the DB
            {
                return null;
            }
        }

        public void GetUsersPlayedGameData(List<Game> currentUsersGames, ref bool lastGameUnfinished)
        {
            List<Game> allTheGamesInDB;
            using (var db = new AppDbContext())
            {
                allTheGamesInDB = db.Game.ToList();
            }
            if (currentUsersGames.Count == 0)
            //if User hasn't gamed at all
            {
                if (allTheGamesInDB.Count == 0)
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
                _lastPlayedGameOfEmployee = _employeesWithRespectiveEmail.First().Games.Where(gameElements => gameElements.GameId == _indexOfLastGame).Last();
                _unguessedEmployeesOfLastGame = JsonConvert.DeserializeObject<List<Employee>>(_lastPlayedGameOfEmployee.UnguessedEmployees);
                _guessedEmployeesOfLastGame = JsonConvert.DeserializeObject<List<Employee>>(_lastPlayedGameOfEmployee.GuessedEmployees);
                lastGameUnfinished = true;
                if (_unguessedEmployeesOfLastGame == null || _unguessedEmployeesOfLastGame.Count == 0)
                {
                    lastGameUnfinished = false;
                    _indexOfLastGame = allTheGamesInDB
                    .Select(gameElements => gameElements.GameId).Distinct().Max();
                }
            }
        }

        public void ResumeFromUnfinishedGame()
        {
            _indexOfEmployeeForOptions = _rndGenerator.Next(_unguessedEmployeesOfLastGame.Count);
            Employee employeeToGuess = _unguessedEmployeesOfLastGame[_indexOfEmployeeForOptions];
            _unsortedGuessingOptions = new List<Employee>() { employeeToGuess };
            _unguessedEmployeesOfLastGame.Remove(employeeToGuess);
            string employeeToGuessGender = employeeToGuess.Gender;
            List<Employee> sameGenderRemainingUnguessedEmployees =
                    _unguessedEmployeesOfLastGame
                    .Where(employeeElements => employeeElements.Gender == employeeToGuessGender)
                    .ToList();
            List<Employee> sameGenderAlreadyGuessedEmployees =
                _guessedEmployeesOfLastGame
                .Where(employeeElements => employeeElements.Gender == employeeToGuessGender)
                .ToList();
            _guessedEmployeesOfLastGame.Add(employeeToGuess);
            if (_unguessedEmployeesOfLastGame.Count > 0)
            //if there is sth in the unguessed list after taking out employee for guessing
            {
                if (sameGenderRemainingUnguessedEmployees.Count > 0)
                //if there are any people of the same gender in the unguessedList
                {
                    GetNeededOptions(sameGenderRemainingUnguessedEmployees);
                }
            }
            GetNeededOptions(sameGenderAlreadyGuessedEmployees);
            SetupNextMove(employeeToGuess);
        }

        public void GetNeededOptions(List<Employee> employeeOptionSource)
        {
            while (_unsortedGuessingOptions.Count < 3 && employeeOptionSource.Count != 0)
            {
                List<Employee> guessedEmployeeBuffer = new List<Employee>();
                guessedEmployeeBuffer.AddRange(employeeOptionSource);
                _indexOfEmployeeForOptions = _rndGenerator.Next(guessedEmployeeBuffer.Count);
                _unsortedGuessingOptions.Add(guessedEmployeeBuffer[_indexOfEmployeeForOptions]);
                guessedEmployeeBuffer.Remove(guessedEmployeeBuffer[_indexOfEmployeeForOptions]);
            }
        }

        public void SetupNextMove(Employee employeeToGuess)
        {
            _session["currentGameStatus"] = new Game
            {
                GameId = _indexOfLastGame,
                EmployeePK = _employeesWithRespectiveEmail.First().EmployeePK,
                UnguessedEmployees = JsonConvert.SerializeObject(_unguessedEmployeesOfLastGame),
                GuessedEmployees = JsonConvert.SerializeObject(_guessedEmployeesOfLastGame),
                PointsSoFar = _lastPlayedGameOfEmployee.PointsSoFar
            };
            List<Employee> employeesRandomizedForGuessing = RandomizeGuessingOptions();
            _session["chosenEmployeeModel"] = new ChosenEmployees(employeeToGuess, employeesRandomizedForGuessing);
        }

        public List<Employee> RandomizeGuessingOptions()
        {
            List<Employee> employeesRandomizedForGuessing = new List<Employee>();
            while (_unsortedGuessingOptions.Count != 0)
            {
                _indexOfEmployeeForOptions = _rndGenerator.Next(_unsortedGuessingOptions.Count);
                employeesRandomizedForGuessing.Add(_unsortedGuessingOptions[_indexOfEmployeeForOptions]);
                _unsortedGuessingOptions.RemoveAt(_indexOfEmployeeForOptions);
            }
            return employeesRandomizedForGuessing;
        }

        public void StartNewGame()
        {
            List<Employee> allEmployeesInDB = new List<Employee>();
            using (var db = new AppDbContext())
            {
                allEmployeesInDB.AddRange(db.Employee);
            }
            _indexOfEmployeeForOptions = _rndGenerator.Next(allEmployeesInDB.Count);
            Employee employeeToGuess = allEmployeesInDB[_indexOfEmployeeForOptions];
            allEmployeesInDB.RemoveAt(_indexOfEmployeeForOptions);
            _unsortedGuessingOptions = new List<Employee> { employeeToGuess };
            string employeeToGuessGender = employeeToGuess.Gender;
            List<Employee> employeeOptionsOfTheSameGender = new List<Employee>();
            employeeOptionsOfTheSameGender.AddRange(allEmployeesInDB.
                Where(employeeElement => employeeElement.Gender == employeeToGuessGender).
                Except(_unsortedGuessingOptions));
            GetNeededOptions(employeeOptionsOfTheSameGender);
            _session["currentGameStatus"] = new Game
            {
                GameId = ++_indexOfLastGame,
                EmployeePK = _employeesWithRespectiveEmail.First().EmployeePK,
                UnguessedEmployees = JsonConvert.SerializeObject(allEmployeesInDB),
                GuessedEmployees = JsonConvert.SerializeObject(new List<Employee>() { employeeToGuess }),
                PointsSoFar = 0
            };
            List<Employee> employeesRandomizedForGuessing = RandomizeGuessingOptions();
            _session["chosenEmployeeModel"] = new ChosenEmployees(employeeToGuess, employeesRandomizedForGuessing);
        }



    }
}
