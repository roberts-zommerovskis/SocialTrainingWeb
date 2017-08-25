using System.Data.Entity.Core.Objects.DataClasses;
using Google.GData.Extensions.Apps;
using System;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Collections.Specialized;
using SocialTrainingWebApp.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SocialTrainingWebApp.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index(string buttonid)
        {
            bool lastGameUnfinished = false;

            string currentUser = User.Identity.GetUserName();

            Game lastPlayedGameOfEmployee = null;
            List<Employee> unguessedEmployeesOfLastGame = null;
            List<Employee> guessedEmployeesOfLastGame = null;
            if (Session["justLoggedIn"] == null)
            {
                Session["justLoggedIn"] = false;
                GoogleSheetConnector.ImportDataIntoDB();
            }


            if (Session["answerSubmitted"] != null)
            {
                if ((bool)Session["answerSubmitted"] == true)
                {
                    Session["answerSubmitted"] = false;
                    Game currentGameToAdd = (Game)Session["currentGameStatus"];
                    if ((bool)Session["answeredCorrectly"])
                    {
                        Session["answeredCorrectly"] = false;
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
                        return View("Congratulations", new SessionSummaryModel(JsonConvert.DeserializeObject<List<Employee>>(currentGameToAdd.GuessedEmployees).Count, currentGameToAdd.PointsSoFar));
                    }
                }
            }
            using (var db = new AppDbContext())
            {
                List<Employee> employeesWithRespectiveEmail = db.Employee.Where(employees => employees.Email == currentUser).ToList();
                if (employeesWithRespectiveEmail.Count > 0)
                //if there is such an employee in the DB
                {
                    long indexOfLastGame;
                    List<Game> currentUsersGames = employeesWithRespectiveEmail.First().Games.ToList();
                    if (currentUsersGames == null || currentUsersGames.Count == 0) //TODO: remove null stuff
                    //if User hasn't gamed at all
                    {
                        if (db.Game.ToList() == null || db.Game.ToList().Count == 0) //TODO: remove null stuff
                        //if nobody has gamed at all
                        {
                            indexOfLastGame = 0;
                        }
                        else
                        //if somebody has gamed (doesn't have to be the specific person)
                        {
                            indexOfLastGame = db.Game.ToList()
                            .Select(gameElements => gameElements.GameId).Distinct().Max();
                        }
                        lastGameUnfinished = false;
                    }


                    else
                    //if the specific user has gamed
                    {
                        indexOfLastGame = currentUsersGames
                            .Select(gameElement => gameElement.GameId).ToList().Max();
                        lastPlayedGameOfEmployee = employeesWithRespectiveEmail.First().Games.Where(gameElements => gameElements.GameId == indexOfLastGame).Last();
                        unguessedEmployeesOfLastGame = JsonConvert.DeserializeObject<List<Employee>>(lastPlayedGameOfEmployee.UnguessedEmployees);
                        guessedEmployeesOfLastGame = JsonConvert.DeserializeObject<List<Employee>>(lastPlayedGameOfEmployee.GuessedEmployees);
                        lastGameUnfinished = true;
                        if (unguessedEmployeesOfLastGame == null || unguessedEmployeesOfLastGame.Count == 0)
                        {
                            lastGameUnfinished = false;
                            indexOfLastGame = db.Game.ToList()
                            .Select(gameElements => gameElements.GameId).Distinct().Max();
                        }
                    }
                    if (lastGameUnfinished)
                    {   //first get the guys you are gonna show, then add 1 to guessedList, then serialize it
                        //into a DB record
                        Random rndGenerator = new Random();
                        int indexOfEmployeeForOptions = rndGenerator.Next(unguessedEmployeesOfLastGame.Count);
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
                                List<Employee> guessedEmployeeBuffer = new List<Employee>();
                                guessedEmployeeBuffer.AddRange(sameGenderAlreadyGuessedEmployees);
                                while (unsortedGuessingOptions.Count < 3)
                                {
                                    indexOfEmployeeForOptions = rndGenerator.Next(guessedEmployeeBuffer.Count);
                                    unsortedGuessingOptions.Add(guessedEmployeeBuffer[indexOfEmployeeForOptions]);
                                    guessedEmployeeBuffer.Remove(guessedEmployeeBuffer[indexOfEmployeeForOptions]);
                                }
                            }
                        }
                        else //if the removed for guessing person was the last in the unguessedList (final guess)
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

                        Session["currentGameStatus"] = new Game
                        {
                            GameId = indexOfLastGame,
                            EmployeePK = employeesWithRespectiveEmail.First().EmployeePK,
                            UnguessedEmployees = JsonConvert.SerializeObject(unguessedEmployeesOfLastGame),//, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.Objects }),
                            GuessedEmployees = JsonConvert.SerializeObject(guessedEmployeesOfLastGame),// , Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.Objects }),
                            PointsSoFar = lastPlayedGameOfEmployee.PointsSoFar
                        };
                        List<Employee> employeesRandomizedForGuessing = new List<Employee>();
                        while (unsortedGuessingOptions.Count != 0)
                        {
                            indexOfEmployeeForOptions = rndGenerator.Next(unsortedGuessingOptions.Count);
                            employeesRandomizedForGuessing.Add(unsortedGuessingOptions[indexOfEmployeeForOptions]);
                            unsortedGuessingOptions.RemoveAt(indexOfEmployeeForOptions);
                        }
                        Session["chosenEmployeeModel"] = new ChosenEmployees(employeeToGuess, employeesRandomizedForGuessing);
                        return View((ChosenEmployees)Session["chosenEmployeeModel"]);
                    }
                    else //if a new game has to be started
                    {
                        List<Employee> blankListForInitializingNewGame = new List<Employee>();
                        List<Employee> allEmployeesInDB = new List<Employee>();
                        allEmployeesInDB.AddRange(db.Employee.ToList());
                        List<Employee> unrandomizedEmployeesForGuessing = new List<Employee>();
                        Random rndGenerator = new Random();
                        int indexOfEmployeeOption = rndGenerator.Next(allEmployeesInDB.Count);
                        Employee employeeToGuess = allEmployeesInDB[indexOfEmployeeOption];
                        allEmployeesInDB.RemoveAt(indexOfEmployeeOption);
                        unrandomizedEmployeesForGuessing.Add(employeeToGuess);
                        string employeeToGuessGender = employeeToGuess.Gender;
                        List<Employee> employeeOptionsOfTheSameGender = new List<Employee>();
                        employeeOptionsOfTheSameGender.AddRange(allEmployeesInDB.
                            Where(employeeElement => employeeElement.Gender == employeeToGuessGender).
                            Except(unrandomizedEmployeesForGuessing));
                        for (int i = 0; i < 2; i++)
                        {
                            indexOfEmployeeOption = rndGenerator.Next(employeeOptionsOfTheSameGender.Count);
                            unrandomizedEmployeesForGuessing.Add(employeeOptionsOfTheSameGender[indexOfEmployeeOption]);
                            employeeOptionsOfTheSameGender.RemoveAt(indexOfEmployeeOption);
                        }
                        Session["currentGameStatus"] = new Game
                        {
                            GameId = ++indexOfLastGame,
                            EmployeePK = employeesWithRespectiveEmail.First().EmployeePK,
                            UnguessedEmployees = JsonConvert.SerializeObject(allEmployeesInDB),
                            GuessedEmployees = JsonConvert.SerializeObject(new List<Employee>() { employeeToGuess }),
                            PointsSoFar = 0
                        };
                        List<Employee> randomizedEmployeeOptions = new List<Employee>();
                        while (unrandomizedEmployeesForGuessing.Count != 0)
                        {
                            indexOfEmployeeOption = rndGenerator.Next(unrandomizedEmployeesForGuessing.Count);
                            randomizedEmployeeOptions.Add(unrandomizedEmployeesForGuessing[indexOfEmployeeOption]);
                            unrandomizedEmployeesForGuessing.RemoveAt(indexOfEmployeeOption);
                        }
                        Session["chosenEmployeeModel"] = new ChosenEmployees(employeeToGuess, randomizedEmployeeOptions);
                        return View((ChosenEmployees)Session["chosenEmployeeModel"]);
                    }
                }
                else
                {
                    //TODO: Cover case where the logged in user is not one of the employees
                    return null;
                }
            }
        }

        public JsonResult CheckAnswer(string ID)
        {
            Session["answerSubmitted"] = true;
            if (int.Parse(ID) == ((ChosenEmployees)Session["chosenEmployeeModel"])._chosenTriadEmployee)
            {
                Session["answeredCorrectly"] = true;
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Session["answeredCorrectly"] = false;
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult PlayAgain(bool PlayAgain)
        {
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReturnCorrectAnswerNumber()
        {
            return Json(new { Answer = ((ChosenEmployees)Session["chosenEmployeeModel"])._chosenTriadEmployee }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Congratulations()
        {
            return View();
        }
    }
}