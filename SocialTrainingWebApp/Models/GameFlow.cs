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
            _session = session;
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
                    while (sameGenderRemainingUnguessedEmployees.Count != 0 && _unsortedGuessingOptions.Count < 3)
                    {
                        _indexOfEmployeeForOptions = _rndGenerator.Next(sameGenderRemainingUnguessedEmployees.Count);
                        Employee extraEmployeeOption = sameGenderRemainingUnguessedEmployees[_indexOfEmployeeForOptions];
                        _unsortedGuessingOptions.Add(extraEmployeeOption);
                        sameGenderRemainingUnguessedEmployees.Remove(extraEmployeeOption);
                    }
                    if (_unsortedGuessingOptions.Count != 3)
                    {
                        _indexOfEmployeeForOptions = _rndGenerator.Next(sameGenderAlreadyGuessedEmployees.Count);
                        _unsortedGuessingOptions.Add(sameGenderAlreadyGuessedEmployees[_indexOfEmployeeForOptions]);
                    }
                }
                else
                //if there aren't any people of the same gender left unguessed
                {
                    GetNeededOptionsFromAlreadyGuessed(sameGenderAlreadyGuessedEmployees);
                }
            }
            else //if the removed for guessing person was the last in the unguessedList (final guess)
            {
                GetNeededOptionsFromAlreadyGuessed(sameGenderAlreadyGuessedEmployees);
            }
            SetupNextMove(employeeToGuess);

        }

        public void GetNeededOptionsFromAlreadyGuessed(List<Employee> sameGenderAlreadyGuessedEmployees)
        {
            while (_unsortedGuessingOptions.Count < 3)
            {
                List<Employee> guessedEmployeeBuffer = new List<Employee>();
                guessedEmployeeBuffer.AddRange(sameGenderAlreadyGuessedEmployees);
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
            List<Employee> blankListForInitializingGame = new List<Employee>();
            List<Employee> allEmployeesInDB = new List<Employee>();
            using (var db = new AppDbContext())
            {
                allEmployeesInDB.AddRange(db.Employee);
            }
            List<Employee> unrandomizedEmployeesForGuessing = new List<Employee>();
            _indexOfEmployeeForOptions = _rndGenerator.Next(allEmployeesInDB.Count);
            Employee employeeToGuess = allEmployeesInDB[_indexOfEmployeeForOptions];
            allEmployeesInDB.RemoveAt(_indexOfEmployeeForOptions);
            unrandomizedEmployeesForGuessing.Add(employeeToGuess);
            string employeeToGuessGender = employeeToGuess.Gender;
            List<Employee> employeeOptionsOfTheSameGender = new List<Employee>();
            employeeOptionsOfTheSameGender.AddRange(allEmployeesInDB.
                Where(employeeElement => employeeElement.Gender == employeeToGuessGender).
                Except(unrandomizedEmployeesForGuessing));
            for (int i = 0; i < 2; i++)
            {
                _indexOfEmployeeForOptions = _rndGenerator.Next(employeeOptionsOfTheSameGender.Count);
                unrandomizedEmployeesForGuessing.Add(employeeOptionsOfTheSameGender[_indexOfEmployeeForOptions]);
                employeeOptionsOfTheSameGender.RemoveAt(_indexOfEmployeeForOptions);
            }
            _session["currentGameStatus"] = new Game
            {
                GameId = ++_indexOfLastGame,
                EmployeePK = _employeesWithRespectiveEmail.First().EmployeePK,
                UnguessedEmployees = JsonConvert.SerializeObject(allEmployeesInDB),
                GuessedEmployees = JsonConvert.SerializeObject(new List<Employee>() { employeeToGuess }),
                PointsSoFar = 0
            };
            List<Employee> randomizedEmployeeOptions = new List<Employee>();
            while (unrandomizedEmployeesForGuessing.Count != 0)
            {
                _indexOfEmployeeForOptions = _rndGenerator.Next(unrandomizedEmployeesForGuessing.Count);
                randomizedEmployeeOptions.Add(unrandomizedEmployeesForGuessing[_indexOfEmployeeForOptions]);
                unrandomizedEmployeesForGuessing.RemoveAt(_indexOfEmployeeForOptions);
            }
            _session["chosenEmployeeModel"] = new ChosenEmployees(employeeToGuess, randomizedEmployeeOptions);
        }



    }
}
