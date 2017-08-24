using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialTrainingWebApp.Models
{
    public class DataManager
    {
        public void Setup(HttpSessionStateBase session)
        {
            if (session["chosenImage"] == null)
            {
                session["chosenImage"] = 0;
            }
            if (session["currentEmployeeTriadChoice"] == null)
            {
                session["currentEmployeeTriadChoice"] = 0;
            }
            if (session["chosenTriadEmployee"] == null)
            {
                session["chosenTriadEmployee"] = 0;
            }
            if (session["points"] == null)
            {
                session["points"] = 0;
            }
            if (session["buttonPressed"] == null)
            {
                session["buttonPressed"] = false;
            }
            if (session["justLoggedIn"] == null)
            {
                session["justLoggedIn"] = true;
            }
        }

        public void HandleNewlyLoggedIn(HttpSessionStateBase session, ref int points)
        {
            session["justLoggedIn"] = false;
            session["points"] = 0;
            points = 0;
            GoogleSheetConnector.ImportDataIntoDB();
            session["employeeCount"] = GoogleSheetConnector.GetEmployeeCount();
        }

        public void CalculatePoints(HttpSessionStateBase session, string buttonid, ref List<EmployeeWrapper> allEmployees, ref int points)
        {
            session["buttonPressed"] = false;
            int buttonNumber = int.Parse(buttonid);
            List<EmployeeWrapper> currentTriad = (List<EmployeeWrapper>)session["currentEmployeeTriadChoice"];
            allEmployees = (List<EmployeeWrapper>)session["currentDataState"];
            string imageEmployeeNumber = (string)session["chosenImage"];
            if (session["points"] == null)
            {
                session["points"] = 0;
            }
            if (currentTriad[buttonNumber].employee.ImportId == int.Parse(imageEmployeeNumber.Substring(0, imageEmployeeNumber.LastIndexOf('.'))))
            {

                points = (int)session["points"];
                points++;
                session["points"] = points;
            }
            else
            {
                points = (int)session["points"];
            }
            allEmployees.RemoveAll(wrapper => wrapper.employee.ImportId == currentTriad[(int)session["chosenTriadEmployee"]].employee.ImportId);
            allEmployees.RemoveAll(wrapper => wrapper.isUnguessed == false);
        }

        //public void PrepareGuessingOptions(HttpSessionStateBase session, ref ChosenEmployees chosenEmployees, ref List<EmployeeWrapper> allEmployees)
        //{
        //    chosenEmployees = new ChosenEmployees(allEmployees);
        //    session["chosenImage"] = chosenEmployees._chosenEmployeeImageId;
        //    session["currentEmployeeTriadChoice"] = chosenEmployees._employeeTriad;
        //    //session["currentDataState"] = chosenEmployees._allEmployees;
        //    session["chosenTriadEmployee"] = chosenEmployees._chosenTriadEmployee;
        //}
    }
}
