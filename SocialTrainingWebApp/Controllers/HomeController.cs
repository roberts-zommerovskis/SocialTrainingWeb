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

            Employee employeeOfInterest;
            Game lastPlayedGameOfEmployee;
            List<Employee> unguessedEmployeesOfLastGame = null;
            List<Employee> guessedEmployeesOfLastGame = null;
            if (Session["answerSubmitted"] != null)
            {
                if ((bool)Session["answerSubmitted"] == true)
                {
                    Session["answerSubmitted"] = false;
                    Game currentGameToAdd = (Game)Session["currentGameStatus"];
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
                    GoogleSheetConnector.ImportDataIntoDB();
                    List<Game> currentUsersGames = employeesWithRespectiveEmail.First().Games.ToList();
                    if (currentUsersGames == null || currentUsersGames.Count == 0) //TODO: remove null stuff
                    //if User hasn't gamed at all
                    {
                        if (db.Game.ToList() == null || db.Game.ToList().Count == 0)
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
                        employeeOfInterest = db.Employee.Where(employeeElement => employeeElement == employeesWithRespectiveEmail.First()).First();
                        lastPlayedGameOfEmployee = employeeOfInterest.Games.Where(gameElements => gameElements.GameId == indexOfLastGame).First();
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
                        if (unguessedEmployeesOfLastGame.Count > 0)
                        //if there is sth in the unguessed list after taking out employee for guessing
                        {
                            if (sameGenderRemainingUnguessedEmployees.Count > 0)
                            //if there are any people of the same gender in the unguessedList
                            {

                                while (sameGenderRemainingUnguessedEmployees.Count != 0 || unsortedGuessingOptions.Count < 3)
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
                            GameId = indexOfLastGame++,
                            EmployeePK = employeesWithRespectiveEmail.First().EmployeePK,
                            UnguessedEmployees = JsonConvert.SerializeObject(unguessedEmployeesOfLastGame),
                            GuessedEmployees = JsonConvert.SerializeObject(guessedEmployeesOfLastGame)
                        };
                        List<Employee> employeesRandomizedForGuessing = new List<Employee>();
                        while (unsortedGuessingOptions.Count != 0)
                        {
                            indexOfEmployeeForOptions = rndGenerator.Next(unsortedGuessingOptions.Count);
                            employeesRandomizedForGuessing.Add(unsortedGuessingOptions[indexOfEmployeeForOptions]);
                            unsortedGuessingOptions.RemoveAt(indexOfEmployeeForOptions);
                        }
                        return View(new ChosenEmployees(employeeToGuess, employeesRandomizedForGuessing));
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
                            GameId = indexOfLastGame++,
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
                        return View(new ChosenEmployees(employeeToGuess, randomizedEmployeeOptions));
                    }
                }
                else
                {
                    //TODO: Cover case where the logged in user is not one of the employees
                    return null;
                }
            }









            //DataManager dataManager = new DataManager();
            //dataManager.Setup(this.Session);
            //int points = 0;
            //ChosenEmployees chosenEmployees = null;
            //List<EmployeeWrapper> allEmployees = new List<EmployeeWrapper>();
            //if ((bool)Session["buttonPressed"] && buttonid != null)
            //{
            //    dataManager.CalculatePoints(this.Session, buttonid, ref allEmployees, ref points);
            //}
            //else if (buttonid == null && (bool)Session["justLoggedIn"])
            //{
            //    dataManager.HandleNewlyLoggedIn(this.Session, ref points);
            //}
            //if (allEmployees.Count != 0 || (allEmployees.Count == 0 && points == 0))
            //{
            //    if (Session["currentDataState"] != null)
            //    {
            //        if (((List<EmployeeWrapper>)Session["currentDataState"]).Count == 0 && Session["chosenImage"] != null)
            //        {
            //            Session["roundCompleted"] = true;
            //            return View("Congratulations", new SessionSummaryModel((int)Session["employeeCount"], (int)Session["points"]));
            //        }
            //        allEmployees = (List<EmployeeWrapper>)Session["currentDataState"];
            //    }
            //    dataManager.PrepareGuessingOptions(this.Session, ref chosenEmployees, ref allEmployees);
            //    return View(chosenEmployees);
            //}
            //else
            //{
            //    Session["roundCompleted"] = true;
            //    return View("Congratulations", new SessionSummaryModel((int)Session["employeeCount"], (int)Session["points"]));
            //}
        }

        public JsonResult CheckAnswer(string ID)
        {
            Session["buttonPressed"] = true;
            if (int.Parse(ID) == (int)Session["chosenTriadEmployee"])
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult PlayAgain(bool PlayAgain)
        {
            Session["roundCompleted"] = false;
            Session["chosenImage"] = null;
            Session["points"] = 0;
            Session["justLoggedIn"] = true;
            Session["currentDataState"] = null;
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReturnCorrectAnswerNumber()
        {
            return Json(new { Answer = (int)Session["chosenTriadEmployee"] }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Congratulations()
        {
            return View();
        }
    }
}